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

		private void InitChildren(Settings parent, Dictionary<string, string> loadFrom) {
			string val = null;

				if(loadFrom.TryGetValue("Libraries", out val)) {
					try {
						_Libraries = ToStringArray(val);
						for(int i = 0; i < _Libraries.Length; i++) { _Libraries[i] = _Libraries[i].Trim(); }
					} catch { } // ignore invalid values
				}
		}
	}
}

