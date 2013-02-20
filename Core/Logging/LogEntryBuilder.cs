using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EpisodeTracker.Core.Logging.Targets;

namespace EpisodeTracker.Core.Logging {
	public class LogEntryBuilder {
		Logger _logger;

		public LogEntryBuilder(Logger logger) {
			_logger = logger;
		}

		public LogLevel Level { get; set; }
		public string MessageFormat { get; set; }
		public object[] Arguments { get; set; }
		public int? EpisodeID { get; set; }

		public LogEntryBuilder Episode(int id) {
			EpisodeID = id;
			return this;
		}

		public LogEntryBuilder Message(string message, params object[] arguments) {
			MessageFormat = message;
			Arguments = arguments;
			return this;
		}

		public void Trace() {
			Log(LogLevel.Trace);
		}

		public void Debug() {
			Log(LogLevel.Debug);
		}

		public void Info() {
			Log(LogLevel.Info);
		}

		public void Warn() {
			Log(LogLevel.Warn);
		}

		public void Error() {
			Log(LogLevel.Error);
		}

		public void Fatal() {
			Log(LogLevel.Fatal);
		}

		public void Log(LogLevel level) {
			Level = level;
			_logger.Log(this);
		}
	}
}
