using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hardcodet.Wpf.TaskbarNotification;
using MediaReign.EpisodeTracker.Data;
using MediaReign.EpisodeTracker.Monitors;
using NLog;

namespace MediaReign.EpisodeTracker {
	class Program {
		static Logger Logger;
		static TaskbarIcon taskbar;

		//[STAThread]
		//static void Main(string[] args) {

		//	try {
		//		Logger = LogManager.GetLogger("EpisodeTracker");

		//		taskbar = new TaskbarIcon();

		//		Run();
		//	} catch(Exception e) {
		//		Logger.Fatal(e);
		//	} finally {
		//		if(taskbar != null) taskbar.Dispose();
		//	}
		//}

		static void Run() {
			ShowWatching();

			var monitor = GetMonitor();
			monitor.Start();

			var listen = true;
			while(listen) {
				var cmd = Console.ReadLine();
				switch(cmd.ToLower()) {
					case "start":
						if(monitor != null && monitor.Running) {
							Console.WriteLine("Monitor is already running!");
						} else {
							monitor = GetMonitor();
							monitor.Start();
						}
						break;
					case "stop":
						if(monitor == null || !monitor.Running) {
							Console.WriteLine("Monitor is not running!");
						} else {
							monitor.Stop();
							monitor.Dispose();
							Console.WriteLine("Stopped");
						}
						break;
					case "exit":
						if(monitor != null) monitor.Dispose();
						listen = false;
						break;
					case "list":
						ShowWatching();
						break;
					case "list other":
						ShowWatchingOther();
						break;
					case "search":
						Console.Write("Enter the series name: ");
						var search = Console.ReadLine();
						if(String.IsNullOrWhiteSpace(search)) {
							Console.WriteLine("You must enter in something to search!");
							break;
						}
						DoSearch(search);
						break;
					default:
						Console.WriteLine("Unknown command");
						break;
				}
			}
		}

		static ProcessMonitor GetMonitor() {
			var mon = new ProcessMonitor(Logger);

			//mon.FileAdded += (o, e) => {
			//	RunInSTA(() => {
			//		var balloon = new NotificationBalloon();
			//		balloon.BodyText = "Tracking file: " + e.FriendlyName;
			//		taskbar.ShowCustomBalloon(balloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 5000);
			//	});
			//};

			//mon.FileRemoved += (o, e) => {
			//	RunInSTA(() => {
			//		var balloon = new NotificationBalloon();
			//		balloon.BodyText = "Finished tracking: " + e.FriendlyName + (e.ProbablyWatched ? " (probably watched)" : " (not watched)");
			//		taskbar.ShowCustomBalloon(balloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 5000);
			//	});
			//};

			return mon;
		}

		private static void DoSearch(string search) {
			using(var db = new EpisodeTrackerDBContext()) {
				var results = db.TrackedSeries
					.Where(s => s.Name.Contains(search))
					.OrderByDescending(s => s.TrackedEpisodes.Max(e => e.LastTracked))
					.Take(10);

				if(results.Any()) {
					Console.WriteLine(String.Format("Found {0} results: ", results.Count()));
					Console.WriteLine();
					foreach(var series in results) {
						var last = series.TrackedEpisodes.OrderByDescending(e => e.LastTracked).First();
						var top = series.TrackedEpisodes.OrderByDescending(e => e.Number).OrderByDescending(e => e.Season).First();

						Console.WriteLine(series.Name);
						Console.WriteLine(String.Format(@"	Last watched: S{1:00}E{2:00}
		{4}
		{0}
		{3}", last.LastTracked, last.Season, last.Number, TimeSpan.FromSeconds(last.TrackedSeconds), last.ProbablyWatched ? "Probably watched" : "Partial viewing"));
						if(last.ID != top.ID) Console.WriteLine(String.Format(@"	Latest episode watched: S{1:00}E{2:00}
		{4}
		{0}
		{3}", top.LastTracked, top.Season, top.Number, TimeSpan.FromSeconds(top.TrackedSeconds), top.ProbablyWatched ? "Probably watched" : "Partial viewing"));
						Console.WriteLine();
					}
					
				} else {
					Console.WriteLine("Did not find any results");
					Console.WriteLine();
				}
			}
		}

		static void ShowWatching() {
			using(var db = new EpisodeTrackerDBContext()) {
				var watching = db.TrackedSeries
					.Select(s => s.TrackedEpisodes.OrderByDescending(e => e.LastTracked).FirstOrDefault())
					.OrderByDescending(e => e.LastTracked)
					.Take(5);

				if(watching.Any()) {
					Console.WriteLine();
					foreach(var ep in watching) {
						Console.Write(String.Format(@"{1} - S{2:00}E{3:00}
	{5}
	{0}
	{4}

", 
							ep.LastTracked, 
							ep.TrackedSeries.Name, 
							ep.Season, 
							ep.Number, 
							TimeSpan.FromSeconds(ep.TrackedSeconds), 
							ep.ProbablyWatched ? "Probably watched" : "Partial viewing"));
					}
					Console.WriteLine();
				} else {
					Console.WriteLine("You don't appear to be watching any tv series");
					Console.WriteLine();
				}
			}
		}

		static void ShowWatchingOther() {
			using(var db = new EpisodeTrackerDBContext()) {
				var watching = db.TrackedOthers.OrderByDescending(o => o.LastTracked);
				if(watching.Any()) {
					Console.WriteLine();
					foreach(var other in watching) {
						Console.Write(String.Format(@"{0}
	{3}
	{1}
	{2}

",
							Path.GetFileName(other.FileName),
							other.LastTracked,
							TimeSpan.FromSeconds(other.TrackedSeconds),
							other.ProbablyWatched ? "Probably watched" : "Partial viewing"));
					}
					Console.WriteLine();
				} else {
					Console.WriteLine("No 'other' found");
				}
			}
		}
	}
}
