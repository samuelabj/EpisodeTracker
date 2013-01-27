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
using System.Threading;
using NLog;

namespace EpisodeTracker.Core.Models {
	public class TVDBSeriesSyncer {
		public delegate void BannerDownloadProgressEventHandler(TVDBSeriesSyncer sender, BannerDownloadProgressEventArgs e);
		public class BannerDownloadProgressEventArgs : EventArgs {
			public int Total { get; set; }
			public int Complete { get; set; }
		}

		static Dictionary<int, SyncInfo> syncing = new Dictionary<int, SyncInfo>();
		static object genreLock = new object();
		struct SyncInfo {
			public object Lock { get; set; }
			public bool Complete { get; set; }
		}

		int TotalBanners = 0;
		int CompleteBanners = 0;
		Logger Logger;

		public TVDBSeriesSyncer() {
			Logger = LogManager.GetLogger("EpisodeTracker");
		}

		public event BannerDownloadProgressEventHandler BannerDownloaded;
		public string Name { get; set; }
		public int TVDBID { get; set; }
		public bool DownloadBanners { get; set; }
		public bool DownloadBannersAsync { get; set; }

		public void Sync() {
			SyncInfo syncInfo;
			lock(syncing) {
				if(!syncing.TryGetValue(TVDBID, out syncInfo)) {
					syncing.Add(TVDBID, syncInfo = new SyncInfo { Lock = new object() });
				}
			}

			lock(syncInfo.Lock) {
				Sync(syncInfo);
			}
		}

		void LogLength(Series series, string prop, string val) {
			if(val == null) return;
			Logger.Debug(series.Name + " - " + prop + ": " + val.Length);
		}

		private void Sync(SyncInfo syncInfo) {
			if(syncInfo.Complete) return;

			var tvdbSeries = new TVDBRequest().Series(TVDBID, true);
			Series series = null;

			using(var db = new EpisodeTrackerDBContext()) {
				var seriesQuery = db.Series.Include(s => s.Episodes);
				series = seriesQuery.SingleOrDefault(s => s.TVDBID == tvdbSeries.ID);
				if(series == null) series = seriesQuery.SingleOrDefault(s => s.Name == tvdbSeries.Name || s.Name == Name);

				if(series == null) {
					series = new Series {
						Added = DateTime.Now
					};
					db.Series.Add(series);
				}

				series.TVDBID = tvdbSeries.ID;
				series.Name = tvdbSeries.Name;
				LogLength(series, "name", tvdbSeries.Name);
				series.AirsDay = tvdbSeries.AirsDay;
				series.AirsTime = tvdbSeries.AirsTime;
				series.Status = tvdbSeries.Status;
				series.Overview = tvdbSeries.Overview;
				LogLength(series, "overview", tvdbSeries.Overview);
				series.LengthMinutes = tvdbSeries.LengthMinutes;
				series.Rating = tvdbSeries.Rating;

				series.Genres.Clear();
				GenresSync(series, tvdbSeries.Genres);

				series.Updated = DateTime.Now;

				foreach(var ep in tvdbSeries.Episodes) {
					SyncEpisode(ep, series);
				}

				db.SaveChanges();
			}

			if(DownloadBanners || DownloadBannersAsync) {
				// Do this after saving so we can use the ID
				var syncBanners = SyncBannersAsync(series, tvdbSeries);
				if(!DownloadBannersAsync) syncBanners.Wait();
			}

			lock(syncing) {
				syncing.Remove(TVDBID);
			}

			syncInfo.Complete = true;
		}

		void GenresSync(Series series, string[] genres) {
			lock(genreLock) {
				var dbGenres = new List<Genre>();
				using(var db = new EpisodeTrackerDBContext()) {
					foreach(var tvdbGenre in genres) {
						var genre = db.Genres.SingleOrDefault(g => g.Name == tvdbGenre);
						if(genre == null) {
							genre = new Genre {
								Name = tvdbGenre
							};
							db.Genres.Add(genre);
						}
						dbGenres.Add(genre);
					}
					db.SaveChanges();
				}

				foreach(var genre in dbGenres) {
					series.Genres.Add(new SeriesGenre {
						GenreID = genre.ID
					});
				}
			}
		}

		void SyncEpisode(TVDBEpisode tvDBEpisode, Series series) {
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
			LogLength(series, "ep name", tvDBEpisode.Name);
			episode.Overview = tvDBEpisode.Overview;
			LogLength(series, "ep overview", tvDBEpisode.Overview);
			episode.Aired = tvDBEpisode.Aired <= SqlDateTime.MaxValue.Value && tvDBEpisode.Aired >= SqlDateTime.MinValue.Value ? tvDBEpisode.Aired : default(DateTime?);
			episode.AbsoluteNumber = tvDBEpisode.AbsoluteNumber;
			episode.Rating = tvDBEpisode.Rating;
			episode.Updated = DateTime.Now;
		}

		Task SyncBannersAsync(Series series, TVDBSeries tvdbSeries) {
			var root = @"Resources\Series\" + series.ID;
			if(!Directory.Exists(root)) Directory.CreateDirectory(root);

			var tasks = new List<Task>();
			tasks.Add(DownloadBanner(tvdbSeries.BannerPath, root, "banner.jpg"));
			tasks.Add(DownloadBanner(tvdbSeries.FanartPath, root, "fanart.jpg"));

			foreach(var ep in series.Episodes) {
				if(!ep.TVDBID.HasValue) continue;
				var tvdbEP = tvdbSeries.Episodes.SingleOrDefault(te => te.ID == ep.TVDBID.Value);
				if(tvdbEP == null) continue;
				tasks.Add(DownloadBanner(tvdbEP.Filename, root, ep.ID + ".jpg"));
			}

			return Task.WhenAll(tasks);
		}

		Task DownloadBanner(string banner, string directory, string fileName) {
			if(String.IsNullOrEmpty(banner)) return Task.FromResult(0);

			var path = Path.Combine(directory, fileName);
			if(File.Exists(path)) {
				if(ValidJPG(path)) return Task.FromResult(0);
				File.Delete(path);
			}

			TotalBanners++;

			return Task.Factory.StartNew(() => {
				new TVDBRequest().DownloadBanner(banner, path);
				Interlocked.Increment(ref CompleteBanners);
				if(BannerDownloaded != null) {
					BannerDownloaded(this, new BannerDownloadProgressEventArgs {
						Total = TotalBanners,
						Complete = CompleteBanners
					});
				}
			});
		}

		bool ValidJPG(string path) {
			try {
				using(var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
				using(var br = new BinaryReader(fs)) {
					byte lastByte = byte.MinValue;
					var buffer = new byte[1024];
					var read = 0;

					do {
						read = br.Read(buffer, 0, buffer.Length);
						for(var i = 0; i < read; i++) {
							var b = buffer[i];
							// EOI (end of image) marker
							if(b == 0xd9 && lastByte == 0xff) {
								return true;
							}
							lastByte = b;
						}
					} while(read == buffer.Length);
				}

				return false;
			} catch(Exception) {
				return true;
			}
		}
	}
}
