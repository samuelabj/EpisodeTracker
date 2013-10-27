using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MediaReign.Core.TvMatchers {
	public class TvMatcher {
		const string Show = @"(?<Series>((.+?|\(\d+\))[\s_]?[\s._\-][\s_]?)+?)";
		static string[] SeasonEpisode = new[] {
			@"S(?<Season>\d+)E(?<Episode>\d+)(([\s_]?[\-_:][\s_]?)?([\s_]?E)?(?<ToEpisode>\d+))?[^\da-z]", // Show S01E01
			@"(?<Season>\d+)x(?<Episode>\d+)([\s_]?[\-_:][\s_]?(?<ToEpisode>\d+))?[^\da-z]", // Show 1x1
			@"Ep(?<Episode>\d+)(([\s_]?[\-_:][\s_]?)?([\s_]?Ep)?(?<ToEpisode>\d+))?[^\da-z]",
			//@"[^\w(]+(?<Season>\d{1})(?<Episode>\d{2})([-\s:]{1,2}(?<ToEpisode>\d+))?[^\w)]", // Show 101
			//@"[\W_]+?(?<Episode>\d{2})([-\s:]{1,2}(?<ToEpisode>\d+))?[\W_]" // Show 01
			@"(?<Episode>\d{1,3})([\s_]?[\-_:][\s_]?(?<ToEpisode>\d+))?[^\da-z]", // Show 001
		};

		Regex Separator = new Regex(@"[\s.\-_]");
		Regex Cleanup = new Regex(@"[']|(^\(.*?\))|\[.*?\]");
		Regex Exclude = new Regex(@".*sample.*");
		Regex Multipart = new Regex("^[-_EeXx]+([0-9]+)");

		LinkedList<Regex> Matches;
		string SeriesGroup = "Series";
		string SeasonGroup = "Season";
		string EpisodeGroup = "Episode";
		string ToEpisodeGroup = "ToEpisode";

		public TvMatcher() {
			Matches = new LinkedList<Regex>(
				SeasonEpisode.Select(s => new Regex(Show + s, RegexOptions.IgnoreCase)));
		}

		public TvMatch Match(string value) {
			if(Exclude.IsMatch(value)) return null;
			var clean = Cleanup.Replace(value, String.Empty);

			foreach(var matchreg in Matches) {
				var match = matchreg.Match(clean);
				if(!match.Success) continue;

				var show = match.Groups[SeriesGroup].Value.Trim();
				var season = match.Groups[SeasonGroup].Value;
				var episode = match.Groups[EpisodeGroup].Value;
				var toEpisode = match.Groups[ToEpisodeGroup].Value;

				if(String.IsNullOrWhiteSpace(show)) continue;

				show = Separator.Replace(show, " ");
				show = show.CleanSpaces(); // Remove multiple spaces

				var s = default(int?);
				var e = default(int?);
				var toE = default(int?);

				if(!String.IsNullOrWhiteSpace(season)) {
					s = int.Parse(season);
				}

				if(!String.IsNullOrWhiteSpace(episode)) {
					e = int.Parse(episode);
				}

				if(!String.IsNullOrWhiteSpace(toEpisode)) {
					toE = int.Parse(toEpisode);
				}

				if(e.HasValue) {
					return new TvMatch {
						Name = show,
						Season = s,
						Episode = e.Value,
						ToEpisode = toE
					};
				}
			}

			return null;
		}
	}
}
