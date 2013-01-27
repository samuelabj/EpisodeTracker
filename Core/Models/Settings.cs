using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EpisodeTracker.Core.Data;

namespace EpisodeTracker.Core.Models {
	public partial class Settings {
		private static object _settingsLock = new object();
		private static Settings settings = null;

		private static object _factoryLock = new object();
		private static Func<EpisodeTrackerDBContext> _factory = () => {
			return new EpisodeTrackerDBContext();
		};

		public static Settings Default {
			get {
				lock(_settingsLock) {
					if(settings == null) {
						settings = new Settings();
						settings.Load();
					}

					return settings;
				}
			}
		}

		public static void SetFactory(Func<EpisodeTrackerDBContext> factory) {
			lock(_factoryLock) {
				_factory = factory;
			}
		}

		private void Load() {
			Dictionary<string, string> settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			EpisodeTrackerDBContext db = null;

			lock(_factoryLock) {
				db = _factory();
			}

			using(db) {
				var query = from s in db.Settings
							select new {
								Name = s.Name,
								Value = s.Value
							};

				settings = query.ToDictionary(n => n.Name, p => p.Value);
			}

			this.InitChildren(this, settings);
		}

		private void SaveSetting(string path, string value, string defaultValue) {
			EpisodeTrackerDBContext db = null;
			lock(_factoryLock) {
				db = _factory();
			}

			using(db) {
				var setting = db.Settings.SingleOrDefault(a => a.Name == path);

				if((value == defaultValue || value == null) && setting != null) {
					db.Settings.Remove(setting);
				} else {
					if(setting == null) {
						setting = InsertSetting(db, path, value);
					}

					setting.Value = value;
				}

				db.SaveChanges();
			}
		}

		private Setting InsertSetting(EpisodeTrackerDBContext db, string path, string value) {
			var setting = db.Settings.SingleOrDefault(a => a.Name == path);

			if(setting == null) {
				setting = new Setting {
					Name = path,
					Value = value
				};

				db.Settings.Add(setting);
				db.SaveChanges();
			}

			return setting;
		}

		public static string FromStringArray(string[] vals) {
			if(vals == null) {
				return null;
			}

			string setting = "";

			for(int i = 0; i < vals.Length; i++) {
				if(!string.IsNullOrEmpty(vals[i])) {
					if(i > 0) {
						setting += ",";
					}

					setting += vals[i].Replace(",", "\\,");
				}
			}
			return setting;
		}

		public static string[] ToStringArray(string setting) {
			if(setting == null)
				return null;

			var vals = new List<string>(Regex.Split(setting, @"(?<!\\),"));
			for(int i = 0; i < vals.Count; i++) {
				if(string.IsNullOrEmpty(vals[i])) {
					vals.RemoveAt(i);
					i--;
				} else {
					vals[i] = vals[i].Replace("\\,", ",");
				}
			}
			return vals.ToArray();
		}
	}
}