using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EpisodeTracker.Core.Logging.Targets;
using NLog;

namespace EpisodeTracker.Core.Logging {
	public class Logger {

		static Logger() {
			NLog.Config.ConfigurationItemFactory.Default.Targets.RegisterDefinition("DBContextTarget", typeof(DBContextTarget));
		}

		public string Key { get; private set; }
		
		public void Trace(string message, params object[] arguments) { 
			Log(LogLevel.Trace, message, arguments); 
		}

		public void Debug(string message, params object[] arguments) {
			Log(LogLevel.Debug, message, arguments); 
		}

		public void Info(string message, params object[] arguments) { 
			Log(LogLevel.Info, message, arguments); 
		}

		public void Warn(string message, params object[] arguments) {
			Log(LogLevel.Warn, message, arguments); 
		}

		public void Error(string message, params object[] arguments) {
			Log(LogLevel.Error, message, arguments); 
		}

		public void Fatal(string message, params object[] arguments) {
			Log(LogLevel.Fatal, message, arguments);
		}

		public void Log(LogLevel level, string message, params object[] arguments) {
            var info = new LogEventInfo(GetLevel(level), Key, message);
			info.Parameters = arguments;

			LogManager.GetLogger(Key).Log(info);
        }

		public void Log(LogEntryBuilder entry) {
			var info = new LogEventInfo {
				LoggerName = Key,
				TimeStamp = DateTime.Now,
				Level = GetLevel(entry.Level),
				Message = entry.MessageFormat,
				Parameters = entry.Arguments
			};

			info.Properties.Add(DBContextTarget.EpisodeID, entry.EpisodeID);
			LogManager.GetLogger(Key).Log(info);
		}

		public LogEntryBuilder Build() {
			return new LogEntryBuilder(this);
		}

		private NLog.LogLevel GetLevel(LogLevel level) {
			switch(level) {
				case LogLevel.Trace: return NLog.LogLevel.Trace;
				case LogLevel.Debug: return NLog.LogLevel.Debug;
				case LogLevel.Info: return NLog.LogLevel.Info;
				case LogLevel.Warn: return NLog.LogLevel.Warn;
				case LogLevel.Error: return NLog.LogLevel.Error;
				case LogLevel.Fatal: return NLog.LogLevel.Fatal;
				default:
					throw new NotSupportedException("LogLevel not supported: " + level);
			}
		}

		static Dictionary<string, Logger> loggers = new Dictionary<string, Logger>(StringComparer.OrdinalIgnoreCase);
		public static Logger Get(string key) {
			lock(loggers) {
				Logger logger;
				if(!loggers.TryGetValue(key, out logger)) {
					logger = new Logger {
						Key = key
					};
					loggers.Add(key, logger);
				}
				return logger;
			}
		}
	}
}
