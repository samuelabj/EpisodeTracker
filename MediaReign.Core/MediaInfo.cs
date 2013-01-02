using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaReign.Core {
	public class MediaInfo : IDisposable {
		MediaInfoLib.MediaInfo info;

		public MediaInfo(string fileName) {
			info = new MediaInfoLib.MediaInfo();
			info.Open(fileName);

			var ms = info.Get(MediaInfoLib.StreamKind.General, 0, "Duration");
			Duration = TimeSpan.FromMilliseconds(long.Parse(ms));
		}

		public TimeSpan Duration { get; private set; }

		public void Dispose() {
			info.Close();
		}
	}
}
