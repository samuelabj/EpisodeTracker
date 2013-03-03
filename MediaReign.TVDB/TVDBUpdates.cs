using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MediaReign.TVDB {
	public class TVDBSeriesUpdate {
		public DateTime Updated { get; set; }
		public int ID { get; set; }
	}

	public class TVDBEpisodeUpdate {
		public DateTime Updated { get; set; }
		public int ID { get; set; }
	}

	public class TVDBUpdates {
		public DateTime Created { get; private set; }
		public List<TVDBSeriesUpdate> Series { get; private set; }
		public List<TVDBEpisodeUpdate> Episodes { get; private set; }

		public TVDBUpdates(XDocument xml) {
			var time = long.Parse(xml.Root.Attribute("time").Value);
			Created = new DateTime(1970, 1, 1).AddSeconds(time);

			Series = xml.Root.Elements("Series")
				.Select(s => new TVDBSeriesUpdate {
					ID = s.GetInt("id").Value,
					Updated = s.GetUnixDateTime("time")
				})
				.ToList();

			Episodes = xml.Root.Elements("Episode")
				.Select(ep => new TVDBEpisodeUpdate {
					ID = ep.GetInt("id").Value,
					Updated = ep.GetUnixDateTime("time")
				})
				.ToList();
		}
	}
}
