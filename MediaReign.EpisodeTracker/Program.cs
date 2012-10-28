using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaReign.EpisodeTracker.Data;
using MediaReign.EpisodeTracker.Monitors;

namespace MediaReign.EpisodeTracker {
	class Program {
		static void Main(string[] args) {
			var monitor = new ProcessMonitor();
			ShowWatching();
			
			Console.Write("Enter a command: ");
			var listen = true;
			while(listen) {		
				var cmd = Console.ReadLine();
				switch(cmd.ToLower()) {
					case "start":
						if(monitor.Running) {
							Console.WriteLine("Monitor is already running!");
						} else {
							monitor.Start();
						}
						break;
					case "stop":
						if(!monitor.Running) {
							Console.WriteLine("Monitor is not running!");
						} else {
							monitor.Stop();
							Console.WriteLine("Stopped");
						}
						break;
					case "exit":
						if(monitor.Running) monitor.Stop();
						listen = false;
						break;
					case "watching":
						ShowWatching();
						break;
					default:
						Console.WriteLine("Unknown command. Accepted commands: start, stop, watching, exit.");
						break;
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
						Console.WriteLine(String.Format("{1} - S{2:00}E{3:00} ({0})", ep.LastTracked, ep.TrackedSeries.Name, ep.Season, ep.Number));
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
						Console.WriteLine(String.Format("{0} ({1})", Path.GetFileName(other.FileName), other.LastTracked));
					}
					Console.WriteLine();
				}
			}
		}
	}
}
