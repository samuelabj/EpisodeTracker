using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EpisodeTracker.WPF.Views.Shared;
using Hardcodet.Wpf.TaskbarNotification;
using EpisodeTracker.Core.Data;
using EpisodeTracker.Core.Monitors;
using NLog;
using System.Data.Entity;
using System.IO;
using EpisodeTracker.Core.Models;
using System.Threading;
using EpisodeTracker.WPF.Models;
using MediaReign.Core;
using EpisodeTracker.WPF.Views.Episodes;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace EpisodeTracker.WPF {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		class SeriesInfo : INotifyPropertyChanged {
			public event PropertyChangedEventHandler PropertyChanged;

			public int SeriesID { get; set; }
			public string Series { get; set; }
			public string Overview { get; set; }
			public int Season { get; set; }
			public int Episode { get; set; }
			public DateTime? LastWatched { get; set; }
			public string Status { get; set; }
			public int Total { get; set; }
			public int Watched { get; set; }
			public Episode WatchNext { get; set; }
			public DateTime? NextAirs { get; set; }
			public string BannerPath { get; set; }
			public string Genres { get; set; }
			public double? Rating { get; set; }
		}

		TaskbarIcon taskbar;
		Logger Logger;
		ProcessMonitor Monitor;
		ObservableCollection<SeriesInfo> seriesList;

		public MainWindow() {
			InitializeComponent();
		}

		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);

			Logger = LogManager.GetLogger("EpisodeTracker");
			Monitor = GetMonitor();
			Monitor.Start();

			statusModal.Visibility = System.Windows.Visibility.Collapsed;

			taskbar = new TaskbarIcon();
			taskbar.Icon = new Icon(Application.GetResourceStream(new Uri("pack://application:,,,/EpisodeTracker;component/resources/images/app.ico")).Stream, 40, 40);
			taskbar.ToolTipText = "Episode Tracker";
			taskbar.Visibility = Visibility.Visible;
			taskbar.LeftClickCommand = new ShowSampleWindowCommand { Window = this };
			taskbar.LeftClickCommandParameter = taskbar;

			//this.Hide();
			this.StateChanged += (o, ea) => { 
				if(WindowState == System.Windows.WindowState.Minimized) this.Hide();
				LoadListsAsync();
			};

			this.Closing += (o, ev) => {
				if(MessageBox.Show("Are you sure you want to exit?", "Confirm exit", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) {
					ev.Cancel = true;
				}
			};

			UpdateSeriesAsync(false);
			LoadListsAsync();

			this.WindowState = System.Windows.WindowState.Maximized;
		}

		List<Series> GetSeriesToUpdate(bool force, IEnumerable<int> seriesIDs = null) {
			if(seriesIDs == null) seriesIDs = new int[0];

			using(var db = new EpisodeTrackerDBContext()) {
				var old = force ? DateTime.Now.AddDays(1) : DateTime.Now.AddDays(-7);
				return db.Series.Where(s =>
					seriesIDs.Any()
					&& seriesIDs.Contains(s.ID)
					|| !seriesIDs.Any() && (
						!s.TVDBID.HasValue
						|| s.Updated <= old
					)
				)
				.ToList();
			}
		}

		async void UpdateSeriesAsync(bool force, IEnumerable<int> seriesIDs = null) {
			var series = await Task.Factory.StartNew(() => GetSeriesToUpdate(force, seriesIDs));
			if(!series.Any()) return;

			var updateStatus = new StatusModal();
			updateStatus.Text = "Updating Series";
			updateStatus.SubText = series.Count == 1 ? series.First().Name : "0 / " + series.Count + " series";
			updateStatus.ShowSubText = true;
			updateStatus.ShowProgress = true;
			updateStatus.SetValue(Grid.RowProperty, 1);
			grid.Children.Add(updateStatus);

			int complete = 0;
			int completeBanners = 0;
			var totalBanners = new Dictionary<int, int>();

			await Task.Factory.StartNew(() => {
				Parallel.ForEach(series, s => {
					if(!s.TVDBID.HasValue) {
						var tvdbResult = TVDBSeriesSearcher.Search(s.Name);
						if(tvdbResult != null) {
							s.TVDBID = tvdbResult.ID;
						}
					}

					if(s.TVDBID.HasValue) {
						var syncer = new TVDBSeriesSyncer {
							Name = s.Name,
							TVDBID = s.TVDBID.Value,
							DownloadBanners = true
						};

						syncer.BannerDownloaded += (o, e) => {
							Interlocked.Increment(ref completeBanners);
							var total = 0;
							lock(totalBanners) {
								totalBanners[s.TVDBID.Value] = e.Total;
								total = totalBanners.Sum(t => t.Value);
							}

							this.Dispatcher.BeginInvoke(new Action(() => {
								if(series.Count == 1) {
									updateStatus.SubText = String.Format("{0} / {1} banners", e.Complete, e.Total);
									updateStatus.UpdateProgress(e.Complete, e.Total);
								} else {
									updateStatus.SubText = String.Format("{0} / {1} series, {2} / {3} banners", complete, series.Count, completeBanners, total);
								}
							}));
						};

						syncer.Sync();
					}

					Interlocked.Increment(ref complete);
					this.Dispatcher.BeginInvoke(new Action(() => {
						updateStatus.SubText = String.Format("{0} / {1} series, {2} banners", complete, series.Count, completeBanners);
						updateStatus.Progress = complete / (double)series.Count * 100;
					}));
				});
			});

			grid.Children.Remove(updateStatus);
		}

		List<SeriesInfo> GetSeries() {
			using(var db = new EpisodeTrackerDBContext()) {
				var watching = db.Series
					.Select(s =>
						s.Episodes
							.Where(e => e.Season != 0)
							.OrderBy(e => e.Number)
							.OrderBy(e => e.Season)
							.OrderByDescending(e => e.Tracked.FirstOrDefault().Updated)
							.FirstOrDefault()
					)
					.AsQueryable()
					.Include(e => e.Series)
					.Include(e => e.Series.Genres.Select(g => g.Genre))
					.Include(e => e.Tracked)
					.ToList()
					.Where(e => e != null);

				var seriesInfo = db.Series.Select(s => new {
					s.ID,
					Total = s.Episodes.Count(e => e.Season != 0 && e.Aired.HasValue && e.Aired <= DateTime.Now), // don't include specials
					Watched = s.Episodes.Count(e => e.Tracked.Any(te => te.Watched)),
					NextAirs = (DateTime?)s.Episodes.Where(e => e.Aired > DateTime.Now).Min(e => e.Aired) 
				})
				.ToDictionary(s => s.ID);

				var display = new List<SeriesInfo>();
				foreach(var e in watching) {
					display.Add(new SeriesInfo {
						SeriesID = e.SeriesID,
						Series = e.Series.Name,
						Overview = e.Series.Overview,
						Status = e.Series.Status,
						Season = e.Season,
						Episode = e.Number,
						LastWatched = e.Tracked.Max(t => (DateTime?)t.Updated),
						Total = seriesInfo[e.SeriesID].Total,
						Watched = seriesInfo[e.SeriesID].Watched,
						WatchNext = !e.Tracked.Any(t => t.Watched) ? e : db.Episodes.Where(e2 => 
							e2.SeriesID == e.SeriesID 
							&& !e2.Tracked.Any(t => t.Watched) 
							&& (
								e2.Season > e.Season 
								|| e2.Season == e.Season 
								&& e2.Number > e.Number
							)
						)
						.OrderBy(e2 => e2.Number)
						.OrderBy(e2 => e2.Season)
						.FirstOrDefault(),
						NextAirs = seriesInfo[e.SeriesID].NextAirs,
						BannerPath = GetBannerPath(e.Series),
						Genres = String.Join(", ", e.Series.Genres.Select(g => g.Genre.Name)),
						Rating = e.Series.Rating
					});
				}

				return display;
			}
		}

		string GetBannerPath(Series series) {
			var path = System.IO.Path.GetFullPath(@"Resources\Series\" + series.ID + @"\banner.jpg");
			if(File.Exists(path)) return path;
			return null;
		}

		List<object> GetOther() {
			using(var db = new EpisodeTrackerDBContext()) {
				var watching = db.TrackedFile
					.Where(f => !f.Episodes.Any())
					.ToList();

				var display = watching
					.Select(f => new {
						File = System.IO.Path.GetFileName(f.FileName),
						Date = f.Stop,
						Status = "Unknown",
						Tracked = TimeSpan.FromSeconds(f.TrackedSeconds)
					});

				return display
						.OrderByDescending(f => f.File)
						.OrderByDescending(f => f.Date)
						.Cast<object>()
						.ToList();
			}
		}

		async void LoadListsAsync() {
			statusModal.Text = "Loading...";
			statusModal.Visibility = System.Windows.Visibility.Visible;

			var series = Task.Factory.StartNew(() => GetSeries());
			var other = Task.Factory.StartNew(() => GetOther());

			await Task.WhenAll(series, other);

			seriesGrid.ItemsSource = seriesList = new ObservableCollection<SeriesInfo>(
				series.Result
						.OrderByDescending(f => f.LastWatched)
			);

			otherGrid.ItemsSource = other.Result;

			statusModal.Visibility = System.Windows.Visibility.Collapsed;
		}

		void SeriesRowDoubleClick(object sender, RoutedEventArgs e) {
			var row = sender as DataGridRow;
			var info = row.Item as SeriesInfo;

			var episodesView = new EpisodeTracker.WPF.Views.Episodes.Index {
				SeriesID = info.SeriesID
			};
			episodesView.WindowState = System.Windows.WindowState.Maximized;
			episodesView.ShowDialog();
		}

		ProcessMonitor GetMonitor() {
			var mon = new ProcessMonitor(Logger);

			mon.FileAdded += (o, e) => {
				this.Dispatcher.BeginInvoke(new Action(() => {
					LoadListsAsync();

					var bal = new NotificationBalloon();
					bal.HeaderText = "Episode Tracker";
					bal.BodyText = "Tracking file: " + e.FriendlyName;
					taskbar.ShowCustomBalloon(bal, System.Windows.Controls.Primitives.PopupAnimation.Slide, 3000);
				}));
			};

			mon.FileRemoved += (o, e) => {
				this.Dispatcher.BeginInvoke(new Action(() => {
					LoadListsAsync();

					var bal = new NotificationBalloon();
					bal.HeaderText = "Episode Tracker";
					bal.BodyText = "Finished tracking: " + e.FriendlyName + (e.Watched ? " (probably watched)" : " (not watched)");
					taskbar.ShowCustomBalloon(bal, System.Windows.Controls.Primitives.PopupAnimation.Slide, 3000);
				}));
			};

			return mon;
		}

		private void UpdateAll_Click(object sender, RoutedEventArgs e) {
			UpdateSeriesAsync(true);
		}

		private void UpdateSelected_Click(object sender, RoutedEventArgs e) {
			var selected = seriesGrid.SelectedItems.Cast<SeriesInfo>().Select(s => s.SeriesID);
			UpdateSeriesAsync(true, selected);
		}

		private void Refresh_Click(object sender, RoutedEventArgs e) {
			LoadListsAsync();
		}

		private void Window_KeyUp(object sender, KeyEventArgs e) {
			if(e.Key == Key.F5) {
				LoadListsAsync();
			}
		}

		private void Delete_Click(object sender, RoutedEventArgs e) {
			var selected = seriesGrid.SelectedItems.Cast<SeriesInfo>().Select(s => seriesList.Single(s2 => s2.SeriesID == s.SeriesID));
			if(MessageBox.Show("Are you sure you want to delete these " + selected.Count() + " series?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) {
				using(var db = new EpisodeTrackerDBContext()) {
					for(var i = 0; i < selected.Count(); i++) {
						var series = selected.ElementAt(i);
						var se = db.Series.Single(s => s.ID == series.SeriesID);
						
						db.Series.Remove(se);
						seriesList.Remove(series);
					}
					db.SaveChanges();
				}
			}
		}

		private void LocateEpisodeFiles_Click(object sender, RoutedEventArgs e) {
			var findWindow = new FindFiles();
			findWindow.WindowState = System.Windows.WindowState.Maximized;
			findWindow.ShowDialog();
		}

		private void WatchNext_Click(object sender, RoutedEventArgs e) {
			var selected = seriesGrid.SelectedItem as SeriesInfo;
			if(selected.WatchNext.FileName == null || !File.Exists(selected.WatchNext.FileName)) {
				MessageBox.Show("Could not find file");
				return;
			}

			try {
				Process.Start(selected.WatchNext.FileName);
			} catch(Exception ex) {
				MessageBox.Show("Problem opening file: " + ex.Message);
				Logger.Error("Error opening filename: " + selected.WatchNext.FileName + " - " + ex);
			}
		}

		private void Settings_Click(object sender, RoutedEventArgs e) {
			var settings = new Views.Settings.Index();
			settings.ShowDialog();
		}

		public class ShowSampleWindowCommand : CommandBase<ShowSampleWindowCommand> {
			public Window Window { get; set; }

			public override void Execute(object parameter) {
				Window.Show();
				Window.WindowState = WindowState.Maximized;
				CommandManager.InvalidateRequerySuggested();
			}

			public override bool CanExecute(object parameter) {
				return true;
			}
		}
	}
}
