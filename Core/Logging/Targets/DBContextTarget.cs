using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EpisodeTracker.Core.Data;
using NLog;
using NLog.Layouts;
using NLog.Targets;

namespace EpisodeTracker.Core.Logging.Targets {
	[Target("DBContextTarget")]
	public class DBContextTarget : Target {
		public const string EpisodeID = "EpisodeID";

		protected override void Write(LogEventInfo info) {
			try {
				using(var db = new EpisodeTrackerDBContext()) {
					var level = GetLevel(info);

					object episodeID;
					info.Properties.TryGetValue(EpisodeID, out episodeID);
					var message = String.Format(info.Message, info.Parameters);

					var entry = new LogEntry {
						Key = info.LoggerName,
						Date = info.TimeStamp,
						Level = level,
						Message = message,
						EpisodeID = episodeID as int?
					};

					db.Log.Add(entry);
					db.SaveChanges();
				}
			} catch(Exception e) {
				NLog.Common.InternalLogger.Error("Could not save log info to database: " + e);
			}
		}

		LogLevel GetLevel(LogEventInfo info) {
			switch(info.Level.Name.ToLower()) {
				case "trace": return LogLevel.Trace;
				case "debug": return LogLevel.Debug;
				case "info": return LogLevel.Info;
				case "warn": return LogLevel.Warn;
				case "error": return LogLevel.Error;
				case "fatal": return LogLevel.Fatal;
				default:
					throw new NotSupportedException("LogLevel not supported: " + info.Level.Name);
			}
		}
	}
}
