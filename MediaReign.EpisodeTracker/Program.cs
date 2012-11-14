using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MediaReign.EpisodeTracker.Data;
using MediaReign.EpisodeTracker.Monitors;
using NLog;

namespace MediaReign.EpisodeTracker {
	class Program {
		static Logger Logger;
		static NotifyIcon notify;

		static void Main(string[] args) {
			try {
				Logger = LogManager.GetLogger("EpisodeTracker");
				notify = new NotifyIcon();
				notify.Text = "EpisodeTracker";
				notify.Icon = new Icon(SystemIcons.Application, 40, 40);
				notify.Visible = true;		

				Run();
			} catch(Exception e) {
				Logger.Fatal(e);
			} finally {
				if(notify != null) notify.Dispose();
			}
		}

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

			mon.FileAdded += (o, f) => {
				notify.ShowBalloonTip(3000, "EpisodeTracker", "Tracking file: " + Path.GetFileNameWithoutExtension(f), ToolTipIcon.None);
			};

			mon.FileRemoved += (o, f) => {
				notify.ShowBalloonTip(3000, "EpisodeTracker", "Finished tracking: " + Path.GetFileNameWithoutExtension(f), ToolTipIcon.None);
			};

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
						Console.WriteLine(String.Format("\tLast watched: S{1:00}E{2:00}\n\t\t{4}\n\t\t{0}\n\t\t{3}", last.LastTracked, last.Season, last.Number, TimeSpan.FromSeconds(last.TrackedSeconds), last.ProbablyWatched ? "Probably watched" : "Partial viewing"));
						if(last.ID != top.ID) Console.WriteLine(String.Format("\tLatest episode watched: S{1:00}E{2:00}\n\t\t{4}\n\t\t{0}\n\t\t{3}", top.LastTracked, top.Season, top.Number, TimeSpan.FromSeconds(top.TrackedSeconds), top.ProbablyWatched ? "Probably watched" : "Partial viewing"));
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
						Console.Write(String.Format("{1} - S{2:00}E{3:00}\n\t{5}\n\t{0}\n\t{4}\n\n", 
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
						Console.Write(String.Format("{0}\n\t{3}\n\t{1}\n\t{2}\n\n",
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
