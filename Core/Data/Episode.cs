using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MediaReign.Core.TvMatchers;

namespace EpisodeTracker.Core.Data {
	public class Episode : IEquatable<TvMatch> {
		[Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		
		public int SeriesID { get; set; }
		public int Season { get; set; }
		public int Number { get; set; }
		[MaxLength(200)]
		public string Name { get; set; }
		public DateTime? Aired { get; set; }
		public string Overview { get; set; }
		public int? TVDBID { get; set; }
		public DateTime Added { get; set; }
		public DateTime Updated { get; set; }
		public int? AbsoluteNumber { get; set; }
		public double? Rating { get; set; }
		[MaxLength(2000)]
		public string FileName { get; set; }
		public bool IgnoreDownload { get; set; }

		[ForeignKey("ID")]
		public virtual Series Series { get; set; }
		[ForeignKey("EpisodeID")]
		public virtual ICollection<TrackedEpisode> Tracked { get; set; }
		[ForeignKey("EpisodeID")]
		public virtual ICollection<EpisodeDownloadLog> DownloadLog { get; set; }

		public string ToString(bool includeSeries) {
			return (includeSeries ? Series.Name + " " : null ) + String.Format("S{0:00}E{1:00} - {2} (ID:{3})", Season, Number, Name, ID);
		}

		public override string ToString() {
			return ToString(false);
		}

		public bool Equals(TvMatch other) {
			return Equals(other, false);
		}

		public bool Equals(TvMatch other, bool absolute) {
			if(absolute) {
				if(!other.Season.HasValue && other.Episode == AbsoluteNumber) return true;
			} else {
				if(other.Season == Season) {
					if(other.Episode == Number) return true;
					if(other.ToEpisode.HasValue) {
						if(Number > other.Episode && Number <= other.ToEpisode) return true;
					}
				}
			}

			return false;
		}

		public static Expression<Func<Episode, bool>> EqualsMatchExpression(TvMatch match) {
			return (Expression<Func<Episode, bool>>)(ep =>
				match.Season.HasValue
				&& ep.Season == match.Season
				&& (
					ep.Number == match.Episode
					|| match.ToEpisode.HasValue
					&& ep.Number > match.Episode
					&& ep.Number <= match.ToEpisode.Value
				)
				|| !match.Season.HasValue
				&& ep.AbsoluteNumber == match.Episode
			);
		}
	}
}
