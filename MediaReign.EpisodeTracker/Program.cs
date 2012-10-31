using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaReign.EpisodeTracker.Data;
using MediaReign.EpisodeTracker.Monitors;
using NLog;

namespace MediaReign.EpisodeTracker {
	class Program {
		static Logger Logger;

		static void Main(string[] args) {
			try {
				Logger = LogManager.GetLogger("EpisodeTracker"); ;
				Run();
			} catch(Exception e) {
				Logger.Fatal(e);
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
							Console.WriteLine("Stopped");
						}
						break;
					case "exit":
						if(monitor != null && monitor.Running) monitor.Stop();
						listen = false;
						break;
					case "Partial viewing":
						ShowWatching();
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
						Console.WriteLine("Unknown command. Accepted commands: start, stop, watching, exit.");
						break;
				}
			}
		}

		static ProcessMonitor GetMonitor() {
			return new ProcessMonitor(Logger);
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
						Console.WriteLine(String.Format("Last watched: S{1:00}E{2:00} ({0}, tracked {3}) - {4}", last.LastTracked, last.Season, last.Number, TimeSpan.FromSeconds(last.TrackedSeconds), last.ProbablyWatched ? "Probably watched" : "Partial viewing"));
						Console.WriteLine(String.Format("Latest episode watched: S{1:00}E{2:00} ({0}, tracked {3}) - {4}", top.LastTracked, top.Season, top.Number, TimeSpan.FromSeconds(top.TrackedSeconds), top.ProbablyWatched ? "Probably watched" : "Partial viewing"));
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
					Console.WriteLine("These are the last episodes you watched by series: ");
					Console.WriteLine();
					foreach(var ep in watching) {
						Console.WriteLine(String.Format("{1} - S{2:00}E{3:00} ({0}, tracked {4}) - {5}", 
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

				var watchingOther = db.TrackedOthers.OrderByDescending(o => o.LastTracked);
				if(watchingOther.Any()) {
					Console.WriteLine("These are the files you recently watched which were not recognised as tv episodes: ");
					Console.WriteLine();
					foreach(var other in watchingOther) {
						Console.WriteLine(String.Format("{0} ({1}, tracked {2}) - {3}", 
							Path.GetFileName(other.FileName), 
							other.LastTracked, 
							TimeSpan.FromSeconds(other.TrackedSeconds), 
							other.ProbablyWatched ? "Probably watched" : "Partial viewing"));
					}
					Console.WriteLine();
				}
			}
		}
	}
}
