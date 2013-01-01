using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.ComponentModel;

namespace MediaReign.TVDB {
	internal static class TVDBExtensions {

		public static string Get(this XElement parent, string name) {
			return Get<string>(parent, name, v => v);
		}

		public static int? GetInt(this XElement parent, string name) {
			return Get<int?>(parent, name, v => int.Parse(v));
		}

		public static double GetDouble(this XElement parent, string name) {
			return Get<double>(parent, name, v => double.Parse(v));
		}

		public static DateTime? GetDateTime(this XElement parent, string name) {
			return Get<DateTime?>(parent, name, v => DateTime.Parse(v));
		}

		public static DateTime GetUnixDateTime(this XElement parent, string name) {
			var start = DateTime.Parse("1/1/1970");
			return Get<DateTime>(parent, name, v => start.AddSeconds(int.Parse(v)));
		}

		public static T Get<T>(this XElement parent, string name) where T : struct, IConvertible {
			var el = parent.Element(name);
			if(el == null || String.IsNullOrEmpty(el.Value)) return default(T);
			try {
				return (T)Enum.Parse(typeof(T), el.Value, true);
			} catch(ArgumentException e) {
				// TODO logging
				Console.WriteLine(e);
				return default(T);
			}
		}

		public static string[] Split(this XElement parent, string name) {
			var val = parent.Element(name).Value;
			return val.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
		}

		public static T Get<T>(this XElement parent, string name, Func<string, T> convert) {
			var el = parent.Element(name);
			if(el == null || String.IsNullOrWhiteSpace(el.Value)) return default(T);

			try {
				return convert(el.Value);
			} catch(FormatException e) {
				// TODO logging
				Console.WriteLine(e);
				return default(T);
			}
		}
	}
}
