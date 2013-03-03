using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using EpisodeTracker.Core.Data;
using EpisodeTracker.Core.Models;
using EpisodeTracker.WPF.Models;
using EpisodeTracker.WPF.Views.Episodes;
using EpisodeTracker.WPF.Views.Shared;
using Hardcodet.Wpf.TaskbarNotification;
using EpisodeTracker.Core.Logging;
using System.Text.RegularExpressions;
using MediaReign.TVDB;

namespace EpisodeTracker.WPF {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		class SeriesInfo : INotifyPropertyChanged {
			public event PropertyChangedEventHandler PropertyChanged;

			public Series Series { get; set; }
			public Episode LastEpisode { get; set; }
			public DateTime? LastWatched { get; set; }
			public int Total { get; set; }
			public int Watched { get; set; }
			public int Unwatched { get; set; }
			public Episode NextEpisode { get; set; }
			public DateTime? NextAirs { get; set; }
			public string BannerPath { get; set; }
			public string Genres { get; set; }
			public bool Hide { get; set; }
			public bool HasNew { get; set; }
		}

		Logger Logger;
		EpisodeProcessMonitor Monitor;
		ObservableCollection<SeriesInfo> SeriesList;

		public MainWindow() {
			InitializeComponent();
		}

		public TaskbarIcon Taskbar { get; private set; }
		public EpisodeDownloadService DownloadService { get; private set; }
		public EpisodeFileService FileService { get; private set; }

		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);

			Logger = Logger.Get("General");

			Monitor = GetMonitor();
			Monitor.Start();

			DownloadService = GetDownloadService();
			if(Settings.Default.Download.Service.IsEnabled) {
				Task.Factory.StartNew(async () => {
					await Task.Delay(TimeSpan.FromSeconds(10));
					DownloadService.Start();
				});
			}

			FileService = new EpisodeFileService();
			FileService.Start();

			statusModal.Visibility = System.Windows.Visibility.Collapsed;
			searchBox.Visibility = System.Windows.Visibility.Collapsed;
			searchBox.Text = null;

			Taskbar = new TaskbarIcon();
			Taskbar.Icon = new Icon(Application.GetResourceStream(new Uri("pack://application:,,,/EpisodeTracker;component/resources/images/app.ico")).Stream, 40, 40);
			Taskbar.ToolTipText = "Episode Tracker";
			Taskbar.Visibility = Visibility.Visible;
			Taskbar.LeftClickCommand = new ShowSampleWindowCommand { Window = this };
			Taskbar.LeftClickCommandParameter = Taskbar;

			//this.Hide();
			this.StateChanged += (o, ea) => { 
				if(WindowState == System.Windows.WindowState.Minimized) this.Hide();
				LoadListsAsync();
				CheckUpdatesAsync();
			};

			this.Closing += (o, ev) => {
				if(MessageBox.Show("Are you sure you want to exit?", "Confirm exit", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) {
					ev.Cancel = true;

					Monitor.Dispose();
					DownloadService.Dispose();
					FileService.Dispose();
				}
			};

			LoadListsAsync();
			CheckUpdatesAsync();

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

		async void CheckUpdatesAsync() {
			await Task.Factory.StartNew(() => {
				var updates = new TVDBRequest().Updates(Settings.Default.LastTVDBUpdateCheck.GetValueOrDefault());

				var episodeIDs = updates.Episodes.Select(ep => ep.ID);
				var seriesIDs = updates.Series.Select(s => s.ID);

				using(var db = new EpisodeTrackerDBContext()) {
					seriesIDs = db.Series.Where(s => seriesIDs.Contains(s.TVDBID.Value) && s.Episodes.Any(ep => episodeIDs.Contains(ep.TVDBID.Value)))
						.Select(s => s.TVDBID.Value)
						.ToList();
				}

				Parallel.ForEach(seriesIDs, id => {
					var syncer = new TVDBSeriesSyncer {
						TVDBID = id,
						DownloadBanners = true
					};

					syncer.Sync();
				});

				Settings.Default.LastTVDBUpdateCheck = DateTime.Now;
			});
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
				var para = series
					.AsParallel()
					.WithDegreeOfParallelism(5);

				para.ForAll(s => {
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
			List<Series> series;
			using(var db = new EpisodeTrackerDBContext()) {
				series = db.Series.ToList();
			}

			return series.AsParallel()
				.Select(s => {
					using(var db = new EpisodeTrackerDBContext()) {
						var episodes = db.Episodes.Where(ep => ep.SeriesID == s.ID);

						var latest = episodes
							.Where(ep => ep.Season != 0)
							.OrderBy(ep => ep.Number)
							.OrderBy(ep => ep.Season)
							.OrderByDescending(ep => ep.Tracked.FirstOrDefault().Updated)
							.FirstOrDefault();

						if(latest == null) return null;

						var next = !latest.Tracked.Any(t => t.Watched) ? latest : episodes.Where(e2 =>
								!e2.Tracked.Any(t => t.Watched)
								&& (
									e2.Season > latest.Season
									|| e2.Season == latest.Season
									&& e2.Number > latest.Number
								)
							)
							.OrderBy(e2 => e2.Number)
							.OrderBy(e2 => e2.Season)
							.FirstOrDefault();

						var total = episodes.Count(e => e.Season != 0 && e.Aired.HasValue && e.Aired <= DateTime.Now); // don't include specials
						var watched = episodes.Count(e => e.Season != 0 && e.Tracked.Any(te => te.Watched));
						var nextAirs = episodes.Where(e => e.Aired > DateTime.Now).Min(e => e.Aired);
						var hasNew = episodes.Any(e => e.Tracked.Any(t => !t.Watched) && e.DownloadLog.Any());
						var unwatched = total - watched;
						if(unwatched < 0) unwatched = 0;

						return new SeriesInfo {
							Series = s,
							LastEpisode = latest,
							LastWatched = latest.Tracked.Max(t => (DateTime?)t.Updated),
							Total = total,
							Watched = watched,
							Unwatched = unwatched,
							NextEpisode = next,
							NextAirs = nextAirs,
							BannerPath = GetBannerPath(s),
							Genres = String.Join(", ", db.SeriesGenres.Where(g => g.SeriesID == s.ID).Select(g => g.Genre.Name)),
							HasNew = hasNew
						};
					}
				})
				.Where(s => s != null)
				.ToList();
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

			EventHandler loaded = null;
			loaded = new EventHandler((o, e) => {
				seriesGrid.SelectedIndex = 0;
				DataGridRow row = (DataGridRow)seriesGrid.ItemContainerGenerator.ContainerFromIndex(0);
				if(row != null) {
					row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
				}

				seriesGrid.LayoutUpdated -= loaded;
			});
			seriesGrid.LayoutUpdated += loaded;

			seriesGrid.ItemsSource = SeriesList = new ObservableCollection<SeriesInfo>(
				series.Result.OrderByDescending(f => f.LastWatched)
			);

			otherGrid.ItemsSource = other.Result;

			statusModal.Visibility = System.Windows.Visibility.Collapsed;
		}

		void SeriesRow_DoubleClick(object sender, RoutedEventArgs e) {
			var row = sender as DataGridRow;
			var info = row.Item as SeriesInfo;

			ShowEpisodes(info);
		}

		void ShowEpisodes(SeriesInfo info) {
			var episodesView = new EpisodeTracker.WPF.Views.Episodes.Index {
				SeriesID = info.Series.ID
			};
			episodesView.WindowState = System.Windows.WindowState.Maximized;
			episodesView.ShowDialog();
		}

		void SeriesRow_KeyUp(object sender, KeyEventArgs e) {
			var row = sender as DataGridRow;
			var info = row.Item as SeriesInfo;

			if(searching) return;

			if(e.Key == Key.Delete) {
				PerformDelete(new[] { info });
			} else if(e.Key == Key.W) {
				PerformWatch(info.NextEpisode);
			} else if(e.Key == Key.L) {
				PerformWatch(info.LastEpisode);
			}
		}

		void SeriesRow_KeyDown(object sender, KeyEventArgs e) {
			var row = sender as DataGridRow;
			var info = row.Item as SeriesInfo;

			if(e.Key == Key.Enter) {
				ShowEpisodes(info);
			}
		}

		EpisodeProcessMonitor GetMonitor() {
			var mon = new EpisodeProcessMonitor();

			mon.FileAdded += (o, e) => {
				this.Dispatcher.BeginInvoke(new Action(() => {
					LoadListsAsync();

					var bal = new NotificationBalloon();
					bal.HeaderText = "Episode Tracker";
					bal.BodyText = "Tracking file: " + e.FriendlyName;
					Taskbar.ShowCustomBalloon(bal, System.Windows.Controls.Primitives.PopupAnimation.Slide, 3000);
				}));
			};

			mon.FileRemoved += (o, e) => {
				this.Dispatcher.BeginInvoke(new Action(() => {
					LoadListsAsync();

					var bal = new NotificationBalloon();
					bal.HeaderText = "Episode Tracker";
					bal.BodyText = "Finished tracking: " + e.FriendlyName + (e.Watched ? " (watched)" : " (not watched)");
					Taskbar.ShowCustomBalloon(bal, System.Windows.Controls.Primitives.PopupAnimation.Slide, 3000);
				}));
			};

			return mon;
		}

		EpisodeDownloadService GetDownloadService() {
			var service = new EpisodeDownloadService();
			service.DownloadFound += (o, e) => {
				this.Dispatcher.BeginInvoke(new Action(() => {
					RecheckHasNew(e.Result.Item1.SeriesID);

					var bal = new NotificationBalloon {
						HeaderText = "Episode Tracker",
						BodyText = "Found new download: " + e.Result.Item2.Title
					};
					Taskbar.ShowCustomBalloon(bal, System.Windows.Controls.Primitives.PopupAnimation.Slide, 3000);
				}));
			};

			return service;
		}

		void RecheckHasNew(int seriesID) {
			var seriesInfo = SeriesList.Single(s => s.Series.ID == seriesID);
			using(var db = new EpisodeTrackerDBContext()) {
				seriesInfo.HasNew = db.Episodes.Any(ep => ep.SeriesID == seriesID && !ep.Tracked.Any(t => t.Watched) && ep.DownloadLog.Any());
			}
		}

		private void UpdateAll_Click(object sender, RoutedEventArgs e) {
			UpdateSeriesAsync(true);
		}

		private void UpdateSelected_Click(object sender, RoutedEventArgs e) {
			var selected = seriesGrid.SelectedItems.Cast<SeriesInfo>().Select(s => s.Series.ID);
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

		bool searching = false;
		void Window_KeyDown(object sender, KeyEventArgs e) {
			if(Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.F) {
				searchBox.Visibility = System.Windows.Visibility.Visible;
				searching = true;
				searchBox.Focus();
			} else if(searching && e.Key == Key.Escape) {
				SeriesList.ToList().ForEach(s => s.Hide = false);
				searching = false;
				searchBox.Visibility = System.Windows.Visibility.Collapsed;
				searchBox.Text = "";
				seriesGrid.Focus();
			}
		}

		Task searchDelayTask;
		async void SearchBox_TextChanged(object sender, RoutedEventArgs e) {
			if(!searching) return;

			if(searchDelayTask != null) return;

			searchDelayTask = Task.Delay(200);
			await searchDelayTask;

			foreach(var s in SeriesList) {
				if(s.Series.Name.IndexOf(searchBox.Text, StringComparison.OrdinalIgnoreCase) > -1) {
					s.Hide = false;
				} else {
					s.Hide = true;
				}
			}

			searchDelayTask = null;
		}

		private void Delete_Click(object sender, RoutedEventArgs e) {
			var selected = seriesGrid.SelectedItems.Cast<SeriesInfo>().Select(s => SeriesList.Single(s2 => s2.Series.ID == s.Series.ID));
			PerformDelete(selected);
		}

		void PerformDelete(IEnumerable<SeriesInfo> selected) {
			if(MessageBox.Show("Are you sure you want to delete these " + selected.Count() + " series?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) {
				using(var db = new EpisodeTrackerDBContext()) {
					for(var i = 0; i < selected.Count(); i++) {
						var series = selected.ElementAt(i);
						var se = db.Series.Single(s => s.ID == series.Series.ID);

						db.Series.Remove(se);
						SeriesList.Remove(series);
					}
					db.SaveChanges();
				}
			}
		}

		private void LocateEpisodeFiles_Click(object sender, RoutedEventArgs e) {
			var findWindow = new FindFiles();
			findWindow.WindowState = System.Windows.WindowState.Maximized;
			try {
				findWindow.ShowDialog();
			} catch(Exception ex) {
				MessageBox.Show(ex.ToString());
				Logger.Error("Unhandled exception locating episode files: " + ex);
			}
		}

		private void WatchNext_Click(object sender, RoutedEventArgs e) {
			var selected = seriesGrid.SelectedItem as SeriesInfo;
			PerformWatch(selected.NextEpisode);
		}

		private void WatchLast_Click(object sender, RoutedEventArgs e) {
			var selected = seriesGrid.SelectedItem as SeriesInfo;
			PerformWatch(selected.LastEpisode);
		}

		async void PerformWatch(Episode episode) {
			if(episode == null) {
				MessageBox.Show("Nothing to watch");
			}

			if(episode.FileName == null || !File.Exists(episode.FileName)) {
				// Try and find
				var status = new StatusModal {
					Text = "Searching for episode file...",
					SubText = "Parsing files: 0",
					ShowSubText = true
				};
				status.SetValue(Grid.RowProperty, 1);
				grid.Children.Add(status);

				var tasks = new List<Task<List<EpisodeFileSearchResult>>>();
				var searcher = new EpisodeFileSearcher();
				var totalFound = 0;

				searcher.FilesFound += (o, ea) => {
					this.Dispatcher.BeginInvoke(new Action(() => {
						Interlocked.Add(ref totalFound, ea.Results);
						status.SubText = "Parsing files: " + totalFound;
					}));
				};

				foreach(var path in Core.Models.Settings.Default.Libraries) {
					tasks.Add(searcher.SearchAsync(path));
				}

				try {
					await Task.WhenAll(tasks);
				} catch(ApplicationException ex) {
					Logger.Error("Error searching for file: " + ex);
					MessageBox.Show(ex.Message);
				}

				var groups = tasks
					.Where(t => !t.IsFaulted)
					.SelectMany(t => t.Result)
					.GroupBy(r => r.Match.Name, StringComparer.OrdinalIgnoreCase)
					.Select(g => new {
						SeriesName = g.Key,
						Results = g.ToList()
					})
					.OrderBy(g => g.SeriesName);

				var total = groups.Count();

				status.Text = "Checking results...";
				status.SubText = String.Format("{0} / {1} series", 0, total);
				status.ShowProgress = true;

				var completed = 0;
				EpisodeFileSearchResult result = null;

				await Task.Factory.StartNew(() => {
					var para = groups
						.AsParallel()
						.WithDegreeOfParallelism(5);

					para.ForAll(info => {
						try {
							using(var db = new EpisodeTrackerDBContext()) {
								var seriesName = info.SeriesName;

								if(db.SeriesIgnore.Any(s => s.Name == seriesName)) {
									return;
								}

								var series = db.Series.SingleOrDefault(s => s.Name == seriesName || s.Aliases.Any(a => a.Name == seriesName));
								if(series == null || series.ID != episode.SeriesID) return;

								var ep = episode;
								var r = info.Results.FirstOrDefault(f => Episode.EqualsMatchExpression(f.Match).Compile()(ep));

								if(r != null) result = r;
							}
						} finally {
							Interlocked.Increment(ref completed);

							this.Dispatcher.BeginInvoke(new Action(() => {
								status.SubText = String.Format("{0} / {1} series", completed, total);
								status.UpdateProgress(completed, total);
							}));
						}
					});
				});

				grid.Children.Remove(status);

				if(result != null) {
					using(var db = new EpisodeTrackerDBContext()) {
						var episodeDB = db.Episodes.Single(ep => ep.ID == episode.ID);
						episodeDB.FileName = result.FileName;
						episode.FileName = result.FileName;
						db.SaveChanges();
					}
				} else {
					MessageBox.Show("Could not find file");
					return;
				}
			}

			try {
				Process.Start(episode.FileName);
			} catch(Exception ex) {
				MessageBox.Show("Problem opening file: " + ex.Message);
				Logger.Error("Error opening filename: " + episode.FileName + " - " + ex);
			}
		}

		private void Settings_Click(object sender, RoutedEventArgs e) {
			var settings = new Views.Settings.Index();
			settings.ShowDialog();
		}

		private void DownloadSettings_Click(object sender, RoutedEventArgs e) {
			var selected = seriesGrid.SelectedItem as SeriesInfo;
			var settings = new Views.SeriesViews.DownloadSettings();
			settings.SeriesID = selected.Series.ID;
			settings.ShowDialog();
		}

		private void Log_Click(object sender, RoutedEventArgs e) {
			var log = new Views.Logs.Index();
			log.WindowState = System.Windows.WindowState.Maximized;
			log.ShowDialog();
		}

		async void ForceCheckDownloads_Click(object sender, RoutedEventArgs e) {
			var checkModal = new StatusModal {
				Text = "Starting up service..."
			};
			checkModal.SetValue(Grid.RowProperty, 1);
			grid.Children.Add(checkModal);

			if(!Settings.Default.Download.Service.IsEnabled) {
				checkModal.Text = "Download service is not enabled";
				checkModal.ShowActivityIndicator = false;
				await Task.Delay(TimeSpan.FromSeconds(3));
				grid.Children.Remove(checkModal);
				return;
			}
			
			var service = ((MainWindow)App.Current.MainWindow).DownloadService;

			await Task.Factory.StartNew(() => {
				if(service.IsRunning) service.Stop();
				service.Start();
			});

			checkModal.Text = "Service will notify you of any new downloads found";
			checkModal.ShowActivityIndicator = false;
			await Task.Delay(TimeSpan.FromSeconds(3));
			grid.Children.Remove(checkModal);
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
