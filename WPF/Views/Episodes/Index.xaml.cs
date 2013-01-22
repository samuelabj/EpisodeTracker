using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using EpisodeTracker.Core.Data;

namespace EpisodeTracker.WPF.Views.Episodes {
	/// <summary>
	/// Interaction logic for Index.xaml
	/// </summary>
	public partial class Index : Window {

		public class EpisodeInfo : INotifyPropertyChanged {
			public event PropertyChangedEventHandler PropertyChanged;

			public Episode Episode { get; set; }
			public TrackedEpisode Tracked { get; set; }
			public TimeSpan? TrackedTime { get; set; }
			public string BannerPath { get; set; }
		}

		ObservableCollection<EpisodeInfo> episodeInfo;

		public Index() {
			InitializeComponent();
		}

		public int SeriesID { get; set; }

		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			statusModal.Visibility = System.Windows.Visibility.Hidden;
		}

		protected override void OnActivated(EventArgs e) {
			base.OnActivated(e);

			if(episodeInfo == null) {
				statusModal.Text = "Loading...";
				statusModal.Visibility = System.Windows.Visibility.Visible;
				Task.Factory.StartNew(() => ShowEpisodes());
			}
		}

		void ShowEpisodes() {
			using(var db = new EpisodeTrackerDBContext()) {
				var title = db.Series.SingleOrDefault(s => s.ID == SeriesID).Name;
				this.Dispatcher.BeginInvoke(new Action(() => {
					Title = title;
				}));

				var episodes = db.Episodes
					.Where(ep => ep.SeriesID == SeriesID)
					.Select(ep => new {
						Episode = ep,
						Tracked = ep.Tracked
							.Where(t => t.DateWatched.HasValue)
							.OrderByDescending(t => t.DateWatched)
							.FirstOrDefault()
					})
					.ToList();

				var display = episodes
					.Select(ep => new EpisodeInfo {
						Episode = ep.Episode,
						Tracked = ep.Tracked,
						TrackedTime = ep.Tracked != null && ep.Tracked.TrackedFile != null ? TimeSpan.FromSeconds(ep.Tracked.TrackedFile.TrackedSeconds) : default(TimeSpan?),		
						BannerPath = GetBannerPath(ep.Episode)
					})
					.ToList();

				this.Dispatcher.BeginInvoke(new Action(() => {
					if(!display.Any(d => d.BannerPath != null)) {
						dataGrid.Columns.First().Visibility = System.Windows.Visibility.Collapsed;
					}

					dataGrid.ItemsSource = episodeInfo = new ObservableCollection<EpisodeInfo>(
						display
						.OrderByDescending(ep => ep.Episode.Number)
						.OrderByDescending(ep => ep.Episode.Season)
					);

					statusModal.Visibility = System.Windows.Visibility.Collapsed;
				}));
			}
		}

		string GetBannerPath(Episode ep) {
			var path = System.IO.Path.GetFullPath(@"External\Series\" + ep.SeriesID + @"\" + ep.ID + ".jpg");
			if(System.IO.File.Exists(path)) return path;
			return null;
		}

		private void Watched_Click(object sender, RoutedEventArgs e) {
			var selected = dataGrid.SelectedItems.Cast<EpisodeInfo>();
			var selectedIDs = selected.Select(s => s.Episode.ID);

			using(var db = new EpisodeTrackerDBContext()) {
				foreach(var info in selected) {
					if(info.Tracked != null && info.Tracked.DateWatched.HasValue) continue;
					
					info.Tracked = new TrackedEpisode {
						EpisodeID = info.Episode.ID,
						DateWatched = DateTime.Now
					};

					db.TrackedEpisodes.Add(info.Tracked);
				}

				db.SaveChanges();
			}
		}

		private void Unwatched_Click(object sender, RoutedEventArgs e) {
			var selected = dataGrid.SelectedItems.Cast<EpisodeInfo>();
			var selectedIDs = selected.Select(s => s.Episode.ID);

			using(var db = new EpisodeTrackerDBContext()) {
				foreach(var info in selected) {
					var tracked = db.TrackedEpisodes.Where(te => te.EpisodeID == info.Episode.ID && te.DateWatched.HasValue);
					if(!tracked.Any()) continue;

					foreach(var te in tracked) db.TrackedEpisodes.Remove(te);
					info.Tracked = null;
				}

				db.SaveChanges();
			}
		}

		private void Delete_Click(object sender, RoutedEventArgs e) {
			if(MessageBox.Show("Are you sure you want to delete these episodes?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) {
				var selected = dataGrid.SelectedItems.Cast<EpisodeInfo>();
				var selectedIDs = selected.Select(s => s.Episode.ID);

				using(var db = new EpisodeTrackerDBContext()) {
					var eps = db.Episodes.Where(ep => selectedIDs.Contains(ep.ID));
					foreach(var ep in eps) {
						db.Episodes.Remove(ep);
						episodeInfo.Remove(selected.Single(s => s.Episode.ID == ep.ID));
					}

					db.SaveChanges();
				}
			}
		}
	}
}
