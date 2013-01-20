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

		public static string Summarise(this string source, int length) {
			if(source.Length <= length) return source;
			var removeIndex = default(int?);

			for(var i = length - 1 - 3; i >= 0; i--) {
				var c = source[i];
				if(char.IsSeparator(c)) {
					removeIndex = i;
					break;
				}
			}

			if(!removeIndex.HasValue) {
				removeIndex = length - 3;
			}

			return source.Remove(removeIndex.Value) + "...";
		}
	}
}
