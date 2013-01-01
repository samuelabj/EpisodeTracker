using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EpisodeTracker.Core.Data;
using MediaReign.TVDB;
using System.Data.Entity;

namespace EpisodeTracker.Core.Models {
	public class TVDBSeriesSyncer {
		static Dictionary<int, SyncInfo> syncing = new Dictionary<int, SyncInfo>();
		struct SyncInfo {
			public object Lock { get; set; }
			public bool Complete { get; set; }
		}

		public static void Sync(int tvdbSeriesID, string name = null) {
			SyncInfo syncInfo;
			lock(syncing) {			
				if(!syncing.TryGetValue(tvdbSeriesID, out syncInfo)) {
					syncing.Add(tvdbSeriesID, syncInfo = new SyncInfo { Lock = new object() });
				}
			}

			lock(syncInfo.Lock) {
				if(syncInfo.Complete) return;

				var tvdbSeries = new TVDBRequest().Series(tvdbSeriesID, true);
				Series series = null;

				using(var db = new EpisodeTrackerDBContext()) {
					var seriesQuery = db.Series.Include(s => s.Episodes);
					series = seriesQuery.SingleOrDefault(s => s.TVDBID == tvdbSeries.ID);
					if(series == null) series = seriesQuery.SingleOrDefault(s => s.Name == tvdbSeries.Name || s.Name == name);

					if(series == null) {
						series = new Series {
							Added = DateTime.Now
						};
						db.Series.Add(series);
					}

					series.TVDBID = tvdbSeries.ID;
					series.Name = tvdbSeries.Name;
					series.AirsDay = tvdbSeries.AirsDay;
					series.AirsTime = tvdbSeries.AirsTime;
					series.Status = tvdbSeries.Status;
					series.Updated = DateTime.Now;

					foreach(var ep in tvdbSeries.Episodes) {
						SyncEpisode(ep, series);
					}

					db.SaveChanges();
				}

				// Do this after saving so we can use the ID
				SyncBanners(series.ID, tvdbSeries);

				lock(syncing) {
					syncing.Remove(tvdbSeriesID);
				}

				syncInfo.Complete = true;
			}
		}

		public static void Sync(string name) {
			var request = new TVDBRequest();
			var result = request.Search(name).FirstOrDefault();
			if(result == null) return;
			Sync(result.ID, name);
		}

		static void SyncEpisode(TVDBEpisode tvDBEpisode, Series series) {
			Episode episode = null;

			episode = series.Episodes
				.SingleOrDefault(ep => ep.TVDBID == tvDBEpisode.ID);

			if(episode == null) {
				episode = series.Episodes
					.SingleOrDefault(ep =>
						ep.Number == tvDBEpisode.Number
						&& ep.Season == tvDBEpisode.Season
					);
			}

			if(episode == null) {
				episode = new Episode {
					Season = tvDBEpisode.Season,
					Number = tvDBEpisode.Number,
					Added = DateTime.Now
				};
				series.Episodes.Add(episode);
			}

			episode.TVDBID = tvDBEpisode.ID;
			episode.Name = tvDBEpisode.Name;	
			episode.Overview = tvDBEpisode.Overview;
			episode.Aired = tvDBEpisode.Aired <= SqlDateTime.MaxValue.Value && tvDBEpisode.Aired >= SqlDateTime.MinValue.Value ? tvDBEpisode.Aired : default(DateTime?);
			episode.AbsoluteNumber = tvDBEpisode.AbsoluteNumber;
			episode.Updated = DateTime.Now;
		}

		static void SyncBanners(int seriesID, TVDBSeries tvdbSeries) {
			var root = @"External\Series\" + seriesID;
			DownloadBanner(tvdbSeries.BannerPath, Path.Combine(root, "banner.jpg"));
			DownloadBanner(tvdbSeries.FanartPath, Path.Combine(root, "fanart.jpg"));
		}

		static void DownloadBanner(string banner, string fileName) {
			var dirPath = Path.GetDirectoryName(fileName);
			if(!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
			if(File.Exists(fileName)) return;
			new TVDBRequest().DownloadBanner(banner, fileName);
		}
	}
}
