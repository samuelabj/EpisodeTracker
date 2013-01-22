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

namespace EpisodeTracker.WPF {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		TaskbarIcon taskbar;
		Logger Logger;
		ProcessMonitor Monitor;

		public MainWindow() {
			InitializeComponent();
		}

		class SeriesInfo {
			public int SeriesID { get; set; }
			public string Series { get; set; }
			public string Overview { get; set; }
			public int Season { get; set; }
			public int Episode { get; set; }
			public DateTime Date { get; set; }
			public string Status { get; set; }
			public int Total { get; set; }
			public int Watched { get; set; }
			public DateTime? NextAirs { get; set; }
			public string BannerPath { get; set; }
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
				RefreshLists();
			};

			this.Closing += (o, ev) => {
				if(MessageBox.Show("Are you sure you want to exit?", "Confirm exit", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) {
					ev.Cancel = true;
				}
			};

			Task.Factory.StartNew(() => {
				try {
					UpdateSeries(false);
				} catch(Exception ex) {
					Logger.Error("Error running UpdateSeries: " + ex);
				}
			});

			RefreshLists();

			this.WindowState = System.Windows.WindowState.Maximized;
		}

		void UpdateSeries(bool force, IEnumerable<int> seriesIDs = null) {
			var tasks = new List<Task>();
			StatusModal updateStatus = null;
			if(seriesIDs == null) seriesIDs = new int[0];

			using(var db = new EpisodeTrackerDBContext()) {
				var old = force ? DateTime.Now.AddDays(1) : DateTime.Now.AddDays(-7);
				var series = db.Series.Where(s => 
					seriesIDs.Any() 
					&& seriesIDs.Contains(s.ID)
					|| !seriesIDs.Any() && (
						!s.TVDBID.HasValue 
						|| s.Updated <= old 					
					)
				)
				.ToList();

				if(series.Any()) {
					this.Dispatcher.BeginInvoke(new Action(() => {
						updateStatus = new StatusModal();
						updateStatus.Text = "Updating Series";
						updateStatus.SubText = "0 / " + series.Count;
						updateStatus.ShowSubText = true;
						updateStatus.ShowProgress = true;
						updateStatus.SetValue(Grid.RowProperty, 1);
						grid.Children.Add(updateStatus);
					}));
				}

				int complete = 0;

				foreach(var temp in series) {
					var s = temp;
					var task = Task.Factory.StartNew(() => {
						if(!s.TVDBID.HasValue) {
							TVDBSeriesSyncer.Sync(s.Name, asyncBanners: false);
						} else {
							TVDBSeriesSyncer.Sync(s.TVDBID.Value, asyncBanners: false);
						}

						Interlocked.Increment(ref complete);
						this.Dispatcher.BeginInvoke(new Action(() => {
							updateStatus.SubText = String.Format("{0} / {1}", complete, series.Count);
							updateStatus.Progress = complete / (double)series.Count * 100;
						}));
					});
					tasks.Add(task);
				}
			}

			Task.WaitAll(tasks.ToArray());

			if(statusModal != null) {
				this.Dispatcher.BeginInvoke(new Action(() => {
					grid.Children.Remove(updateStatus);
				}));
			}
		}

		void ShowSeries() {
			using(var db = new EpisodeTrackerDBContext()) {
				var watching = db.Series
					.Select(s =>
						s.Episodes
							.SelectMany(ep => ep.Tracked)
							.Where(t => t.DateWatched.HasValue)
							.OrderByDescending(t => t.DateWatched)
							.FirstOrDefault()
					)
					.AsQueryable()
					.Include(t => t.Episode)
					.Include(t => t.Episode.Series)
					.ToList()
					.Where(t => t != null);

				var seriesInfo = db.Series.Select(s => new {
					s.ID,
					Total = s.Episodes.Count(e => e.Season != 0), // don't include specials
					Watched = s.Episodes.Count(e => e.Tracked.Any(te => te.DateWatched.HasValue)),
					NextAirs = (DateTime?)s.Episodes.Where(e => e.Aired > DateTime.Now).Min(e => e.Aired)
				})
				.ToDictionary(s => s.ID);

				var display = watching
					.Select(t => new SeriesInfo {
						SeriesID = t.Episode.SeriesID,
						Series = t.Episode.Series.Name,
						Overview = t.Episode.Series.Overview,
						Status = t.Episode.Series.Status,
						Season = t.Episode.Season,
						Episode = t.Episode.Number,
						Date = t.DateWatched.Value,
						Total = seriesInfo[t.Episode.SeriesID].Total,
						Watched = seriesInfo[t.Episode.SeriesID].Watched,
						NextAirs = seriesInfo[t.Episode.SeriesID].NextAirs,
						BannerPath = GetBannerPath(t.Episode.Series)
					});

				this.Dispatcher.BeginInvoke(new Action(() => {
					seriesGrid.ItemsSource = display
						.OrderByDescending(f => f.Date);
				}));
			}
		}

		string GetBannerPath(Series series) {
			var path = System.IO.Path.GetFullPath(@"External\Series\" + series.ID + @"\banner.jpg");
			if(File.Exists(path)) return path;
			return null;
		}

		void ShowOther() {
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

				this.Dispatcher.BeginInvoke(new Action(() => {
					otherGrid.ItemsSource = display
						.OrderByDescending(f => f.File)
						.OrderByDescending(f => f.Date);
				}));
			}
		}

		void RefreshLists() {
			statusModal.Text = "Refreshing...";
			statusModal.Visibility = System.Windows.Visibility.Visible;

			Task.Factory.StartNew(() => {
				ShowSeries();
				ShowOther();

				this.Dispatcher.BeginInvoke(new Action(() => {
					statusModal.Visibility = System.Windows.Visibility.Collapsed;
				}));
			});
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
					RefreshLists();

					var bal = new NotificationBalloon();
					bal.HeaderText = "Episode Tracker";
					bal.BodyText = "Tracking file: " + e.FriendlyName;
					taskbar.ShowCustomBalloon(bal, System.Windows.Controls.Primitives.PopupAnimation.Slide, 3000);
				}));
			};

			mon.FileRemoved += (o, e) => {
				this.Dispatcher.BeginInvoke(new Action(() => {
					RefreshLists();

					var bal = new NotificationBalloon();
					bal.HeaderText = "Episode Tracker";
					bal.BodyText = "Finished tracking: " + e.FriendlyName + (e.Watched ? " (probably watched)" : " (not watched)");
					taskbar.ShowCustomBalloon(bal, System.Windows.Controls.Primitives.PopupAnimation.Slide, 3000);
				}));
			};

			return mon;
		}

		private void UpdateAll_Click(object sender, RoutedEventArgs e) {
			Task.Factory.StartNew(() => UpdateSeries(true));
		}

		private void UpdateSelected_Click(object sender, RoutedEventArgs e) {
			var selected = seriesGrid.SelectedItems.Cast<SeriesInfo>().Select(s => s.SeriesID);
			Task.Factory.StartNew(() => UpdateSeries(true, selected));
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
