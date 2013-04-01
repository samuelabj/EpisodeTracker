using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EpisodeTracker.Torrents.Parsers;
using Xunit;

namespace EpisodeTracker.Tests {
	public class TorrentParserFacts {

		[Fact]
		public void Has_Multiple_Files() {
			var content = @"d8:announce36:udp://fr33dom.h33t.com:3310/announce13:announce-listll36:udp://fr33dom.h33t.com:3310/announce37:http://fr33dom.h33t.com:3310/announceee7:comment0:10:created by29:mktorrent-GUI [mktorrent 1.0]13:creation datei1362609721e4:infod5:filesld6:lengthi245e4:pathl40:Californication S06E08 Hdtv 480p-MGD.txteed6:lengthi262516420e4:pathl40:Californication S06E08 Hdtv 480p-MGD.zipeee4:name36:Californication S06E08 Hdtv 480p-MGDee";
			var torrent = TorrentParser.ParseContent(content);
			
			Assert.Equal(2, torrent.Files.Count);
			
			var file1 = torrent.Files[0];
			var file2 = torrent.Files[1];

			Assert.Equal("Californication S06E08 Hdtv 480p-MGD.txt", file1.Path);
			Assert.Equal("Californication S06E08 Hdtv 480p-MGD.zip", file2.Path);
		}

		[Fact]
		public void Has_Single_File() {
			var content = @"d8:announce38:udp://tracker.publicbt.com:80/announce13:announce-listll38:udp://tracker.publicbt.com:80/announce39:http://tracker.publicbt.com:80/announceel38:udp://bt.careland.com.cn:6969/announce39:http://bt.careland.com.cn:6969/announceel36:udp://bt01.gamebar.com:6969/announce37:http://bt01.gamebar.com:6969/announceel37:udp://repeater.xiph.org:6969/announce38:http://repeater.xiph.org:6969/announceee7:comment103:Torrent downloaded from http://torrent.cd Torrent downloaded from torrent cache at http://torcache.net/10:created by33:Torrent PHP Class - Adrien Gibrat13:creation datei1363436034e4:infod6:lengthi246830834e4:name38:Greys.Anatomy.S09E17.HDTV.x264-LOL.mp4ee";
			var torrent = TorrentParser.ParseContent(content);

			Assert.Equal(1, torrent.Files.Count);

			var file1 = torrent.Files[0];

			Assert.Equal("Greys.Anatomy.S09E17.HDTV.x264-LOL.mp4", file1.Path);
		}
	}
}
