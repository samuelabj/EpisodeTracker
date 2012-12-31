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
using MediaReign.EpisodeTracker.Data;
using MediaReign.EpisodeTracker.Monitors;
using NLog;

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
			public int Season { get; set; }
			public int Episode { get; set; }
			public DateTime Date { get; set; }
			public string Status { get; set; }
			public TimeSpan Tracked { get; set; }
		}

		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);

			Logger = LogManager.GetLogger("EpisodeTracker");
			Monitor = GetMonitor();
			Monitor.Start();

			taskbar = new TaskbarIcon();
			taskbar.Icon = new Icon(SystemIcons.Application, 40, 40);
			taskbar.ToolTipText = "Episode Tracker";
			taskbar.Visibility = Visibility.Visible;
			taskbar.LeftClickCommand = new ShowSampleWindowCommand { Window = this };
			taskbar.LeftClickCommandParameter = taskbar;

			this.Hide();
			this.StateChanged += (o, ea) => { 
				if(WindowState == System.Windows.WindowState.Minimized) this.Hide();
				RefreshLists();
			};
		}

		void ShowSeries() {
			using(var db = new EpisodeTrackerDBContext()) {
				var watching = db.Series
					.SelectMany(s => s.Episodes.Select(ep => ep.TrackedFiles.OrderByDescending(f => f.LastTracked)).FirstOrDefault())
					.ToList();

				var display = watching
					.Where(f => f.Episode != null)
					.Select(f => new SeriesInfo {
						SeriesID = f.Episode.SeriesID,
						Series = f.Episode.Series.Name,
						Season = f.Episode.Season,
						Episode = f.Episode.Number,
						Date = f.LastTracked,
						Status = f.ProbablyWatched ? "Probably watched" : "Partial viewing",
						Tracked = TimeSpan.FromSeconds(f.TrackedSeconds)
					});

				seriesGrid.ItemsSource = display
					.OrderByDescending(f => f.Date);
			}
		}

		void ShowOther() {
			using(var db = new EpisodeTrackerDBContext()) {
				var watching = db.TrackedFiles
					.ToList();

				var display = watching
					.Where(f => f.Episode == null)
					.Select(f => new {
						File = System.IO.Path.GetFileName(f.FileName),
						Date = f.LastTracked,
						Status = f.ProbablyWatched ? "Probably watched" : "Partial viewing",
						Tracked = TimeSpan.FromSeconds(f.TrackedSeconds)
					});

				otherGrid.ItemsSource = display
					.OrderByDescending(f => f.File)
					.OrderByDescending(f => f.Date);
			}
		}

		void RefreshLists() {
			ShowSeries();
			ShowOther();
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
					bal.BodyText = "Finished tracking: " + e.FriendlyName + (e.ProbablyWatched ? " (probably watched)" : " (not watched)");
					taskbar.ShowCustomBalloon(bal, System.Windows.Controls.Primitives.PopupAnimation.Slide, 3000);
				}));
			};

			return mon;
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
