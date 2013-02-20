using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using  System.Globalization;

namespace EpisodeTracker.Core.Models {
	public partial class Settings {		
	
			// Libraries
			private string[] _Libraries = null;
			private object _lockLibraries = new object();
					
			public string[] Libraries {
				get {
					lock(_lockLibraries) {
						if(_Libraries == null) return new string[0];
						return _Libraries;
					}
				}
				set {
					lock(_lockLibraries) {
						_Libraries = value;
					
						SaveSetting("Libraries", FromStringArray(value), "");
					}
				}
			}

		public DownloadSettings Download { get; set; }
		public class DownloadSettings {
			protected Settings Settings { get; private set; }
			
			public DownloadSettings() { }
			
			public DownloadSettings(Settings settings, Dictionary<string, string> loadFrom) {
				Settings = settings;
				LoadSettings(loadFrom);
			}
			
			
		public ServiceSettings Service { get; set; }
		public class ServiceSettings {
			protected Settings Settings { get; private set; }
			
			public ServiceSettings() { }
			
			public ServiceSettings(Settings settings, Dictionary<string, string> loadFrom) {
				Settings = settings;
				LoadSettings(loadFrom);
			}
			
				
			// IsEnabled
			private bool? _IsEnabled = null;
			private object _lockIsEnabled = new object();
			public const bool IsEnabledDefault = false;		
			public bool IsEnabled {
				get {
					lock(_lockIsEnabled) {
						if(!_IsEnabled.HasValue) return IsEnabledDefault;
						return _IsEnabled.Value;
					}
				}
				set {
					lock(_lockIsEnabled) {
						_IsEnabled = value;
					
						if(Settings != null) {
							Settings.SaveSetting("Download.Service.IsEnabled", value.ToString(), string.Empty);
						}
					}
				}
			}
			
			private void LoadSettings(Dictionary<string, string> loadFrom) {
							string val = null;
						
				if(loadFrom.TryGetValue("Download.Service.IsEnabled", out val)) {
					try {
						_IsEnabled = bool.Parse(val);
					} catch { } // ignore invalid values
				}
			}	
		}
			
			private void LoadSettings(Dictionary<string, string> loadFrom) {
									Service = new ServiceSettings(Settings, loadFrom);
			}	
		}

		private void InitChildren(Settings parent, Dictionary<string, string> loadFrom) {
			string val = null;

				if(loadFrom.TryGetValue("Libraries", out val)) {
					try {
						_Libraries = ToStringArray(val);
						for(int i = 0; i < _Libraries.Length; i++) { _Libraries[i] = _Libraries[i].Trim(); }
					} catch { } // ignore invalid values
				}
			Download = new DownloadSettings(parent, loadFrom);
		}
	}
}

