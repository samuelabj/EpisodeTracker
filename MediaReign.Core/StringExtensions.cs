using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MediaReign.Core {
	public static class StringExtensions {
		public static Regex MultipleSpaces = new Regex(@"\s{2,}");

		public static string CleanSpaces(this string source) {
			return MultipleSpaces.Replace(source, " ").Trim();
		}
	}
}
