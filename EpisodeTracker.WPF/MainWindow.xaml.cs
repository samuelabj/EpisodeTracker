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

		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);

			Logger = LogManager.GetLogger("EpisodeTracker");
			Monitor = GetMonitor();
			Monitor.Start();

			taskbar = new TaskbarIcon();
			taskbar.Icon = new Icon(SystemIcons.Application, 40, 40);
			taskbar.ToolTipText = "Ha ha you can't do anything yet";
			taskbar.Visibility = Visibility.Visible;
			taskbar.LeftClickCommand = new ShowSampleWindowCommand { Window = this };
			taskbar.LeftClickCommandParameter = taskbar;

			ShowWatching();

			this.Hide();
			this.StateChanged += (o, ea) => { 
				if(WindowState == System.Windows.WindowState.Minimized) this.Hide();
				ShowWatching();
			};
		}

		void ShowWatching() {
			using(var db = new EpisodeTrackerDBContext()) {
				var watching = db.TrackedEpisodes.OrderByDescending(e => e.LastTracked)
					.OrderByDescending(e => e.LastTracked)
					.ToList();

				var display = watching.Select(ep => new {
					File = String.Format(@"{0} - S{1:00}E{2:00}", ep.TrackedSeries.Name, ep.Season, ep.Number),
					Date = ep.LastTracked,
					Status = ep.ProbablyWatched ? "Probably watched" : "Partial viewing",
					Tracked = TimeSpan.FromSeconds(ep.TrackedSeconds)
				});

				var watching2 = db.TrackedOthers.OrderByDescending(e => e.LastTracked)
					.OrderByDescending(e => e.LastTracked)
					.ToList();

				display = display.Union(watching2.Select(ep => new {
					File = System.IO.Path.GetFileName(ep.FileName),
					Date = ep.LastTracked,
					Status = ep.ProbablyWatched ? "Probably watched" : "Partial viewing",
					Tracked = TimeSpan.FromSeconds(ep.TrackedSeconds)
				}));

				listView.ItemsSource = display;
				dataGrid.ItemsSource = display.OrderByDescending(ep => ep.File).OrderByDescending(ep => ep.Date);
			}
		}

		ProcessMonitor GetMonitor() {
			var mon = new ProcessMonitor(Logger);

			mon.FileAdded += (o, e) => {
				this.Dispatcher.BeginInvoke(new Action(() => {
					ShowWatching();

					var bal = new NotificationBalloon();
					bal.HeaderText = "Episode Tracker";
					bal.BodyText = "Tracking file: " + e.FriendlyName;
					taskbar.ShowCustomBalloon(bal, System.Windows.Controls.Primitives.PopupAnimation.Slide, 3000);
				}));
			};

			mon.FileRemoved += (o, e) => {
				this.Dispatcher.BeginInvoke(new Action(() => {
					ShowWatching();

					var bal = new NotificationBalloon();
					bal.HeaderText = "Episode Tracker";
					bal.BodyText = "Finished tracking: " + e.FriendlyName + (e.ProbablyWatched ? " (probably watched)" : " (not watched)");
					taskbar.ShowCustomBalloon(bal, System.Windows.Controls.Primitives.PopupAnimation.Slide, 3000);
				}));
			};

			return mon;
		}

		protected override void OnSourceInitialized(EventArgs e) {
			base.OnSourceInitialized(e);
			HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
			source.AddHook(WndProc);
		}

		private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
			// Handle messages...
			if(msg == NativeMethods.WM_SHOWME) {
				ShowMe();
			}

			return IntPtr.Zero;
		}

		private void ShowMe() {
			if(WindowState == WindowState.Minimized) {
				WindowState = WindowState.Normal;
			}
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
