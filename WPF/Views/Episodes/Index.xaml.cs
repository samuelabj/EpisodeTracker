using System;
using System.Collections.Generic;
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
			public Episode Episode { get; set; }
			public string File { get; set; }
			public DateTime? Date { get; set; }
			public string Status { get; set; }
			public TimeSpan? Tracked { get; set; }
			public bool Watched { get; set; }

			public event PropertyChangedEventHandler PropertyChanged;
		}

		public Index() {
			InitializeComponent();
		}

		public int SeriesID { get; set; }

		protected override void OnActivated(EventArgs e) {
			base.OnActivated(e);
			ShowEpisodes();
		}

		void ShowEpisodes() {
			using(var db = new EpisodeTrackerDBContext()) {
				Title = db.Series.SingleOrDefault(s => s.ID == SeriesID).Name;

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
						Date = ep.Tracked != null ? ep.Tracked.DateWatched : default(DateTime?),
						Watched = ep.Tracked != null && ep.Tracked.DateWatched.HasValue,
						Status = ep.Tracked != null ? ep.Tracked.DateWatched.HasValue ? "Watched" : "Incomplete" : null,
						Tracked = ep.Tracked != null && ep.Tracked.TrackedFile != null ? TimeSpan.FromSeconds(ep.Tracked.TrackedFile.TrackedSeconds) : default(TimeSpan?)		
					});

				dataGrid.ItemsSource = display
					.OrderByDescending(ep => ep.Episode.Number)
					.OrderByDescending(ep => ep.Episode.Season);
			}
		}

		private void Watched_Click(object sender, RoutedEventArgs e) {
			var selected = dataGrid.SelectedItems.Cast<EpisodeInfo>();
			var selectedIDs = selected.Select(s => s.Episode.ID);

			using(var db = new EpisodeTrackerDBContext()) {
				foreach(var info in selected) {
					if(info.Watched) continue;
					db.TrackedEpisodes.Add(new TrackedEpisode {
						EpisodeID = info.Episode.ID,
						DateWatched = DateTime.Now
					});
				}

				db.SaveChanges();
			}
		}

		private void Unwatched_Click(object sender, RoutedEventArgs e) {
			var selected = dataGrid.SelectedItems.Cast<EpisodeInfo>();
			var selectedIDs = selected.Select(s => s.Episode.ID);

			using(var db = new EpisodeTrackerDBContext()) {
				
				db.SaveChanges();
			}
		}
	}
}
