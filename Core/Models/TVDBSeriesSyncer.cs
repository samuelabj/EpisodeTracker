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

		public static void Sync(int tvdbSeriesID, string name = null, bool asyncBanners = true) {
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
					series.Overview = tvdbSeries.Overview;
					series.LengthMinutes = tvdbSeries.LengthMinutes;

					series.Updated = DateTime.Now;

					foreach(var ep in tvdbSeries.Episodes) {
						SyncEpisode(ep, series);
					}

					db.SaveChanges();
				}

				// Do this after saving so we can use the ID
				if(asyncBanners) {
					Task.Factory.StartNew(() => {
						SyncBanners(series, tvdbSeries);
					});
				} else {
					SyncBanners(series, tvdbSeries);
				}

				lock(syncing) {
					syncing.Remove(tvdbSeriesID);
				}

				syncInfo.Complete = true;
			}
		}

		public static void Sync(string name, bool asyncBanners = true) {
			var request = new TVDBRequest();
			var result = request.Search(name).FirstOrDefault();
			if(result == null) return;
			Sync(result.ID, name, asyncBanners);
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

		static void SyncBanners(Series series, TVDBSeries tvdbSeries) {
			var root = @"External\Series\" + series.ID;
			if(!Directory.Exists(root)) Directory.CreateDirectory(root);

			DownloadBanner(tvdbSeries.BannerPath, root, "banner.jpg");
			DownloadBanner(tvdbSeries.FanartPath, root, "fanart.jpg");

			foreach(var ep in series.Episodes) {
				if(!ep.TVDBID.HasValue) continue;
				var tvdbEP = tvdbSeries.Episodes.SingleOrDefault(te => te.ID == ep.TVDBID.Value);
				if(tvdbEP == null) continue;
				if(!String.IsNullOrEmpty(tvdbEP.Filename)) {
					DownloadBanner(tvdbEP.Filename, root, ep.ID + ".jpg");
				}
			}
		}

		static void DownloadBanner(string banner, string directory, string fileName) {
			var path = Path.Combine(directory, fileName);
			if(File.Exists(path)) return;
			new TVDBRequest().DownloadBanner(banner, path);
		}
	}
}
