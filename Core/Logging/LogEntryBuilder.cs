using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EpisodeTracker.Core.Logging.Targets;

namespace EpisodeTracker.Core.Logging {
	public class LogEntryBuilder {
		Logger _logger;

		public LogEntryBuilder(Logger logger, LogLevel level) {
			_logger = logger;
			Level = level;
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

		public void Log() {
			_logger.Log(this);
		}
	}
}
