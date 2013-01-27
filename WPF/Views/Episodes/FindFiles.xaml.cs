using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using EpisodeTracker.Core.Data;
using EpisodeTracker.Core.Models;
using EpisodeTracker.WPF.Views.Shared;
using MediaReign.TVDB;
using NLog;

namespace EpisodeTracker.WPF.Views.Episodes {
	/// <summary>
	/// Interaction logic for FindFiles.xaml
	/// </summary>
	public partial class FindFiles : Window {
		public FindFiles() {
			InitializeComponent();
		}

		class FindFilesInfo : INotifyPropertyChanged {
			public event PropertyChangedEventHandler PropertyChanged;
			public string SeriesName { get; set; }
			public List<EpisodeFileSearchResult> Results { get; set; }
			public string Status { get; set; }
			public bool Complete { get; set; }
			public bool Error { get; set; }
			public bool NotFound { get; set; }
		};

		ObservableCollection<FindFilesInfo> foundFiles;
		Logger Logger;

		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);

			Logger = LogManager.GetLogger("EpisodeTracker");
			FindFilesAsync();
		}

		async void FindFilesAsync() {
			var status = new StatusModal {
				Text = "Searching for episode files...",
				SubText = "Files found: 0",
				ShowSubText = true
			};
			grid.Children.Add(status);

			var tasks = new List<Task<List<EpisodeFileSearchResult>>>();
			var searcher = new EpisodeFileSearcher();
			var totalFound = 0;

			searcher.FilesFound += (o, e) => {
				this.Dispatcher.BeginInvoke(new Action(() => {
					Interlocked.Add(ref totalFound, e.Results);
					status.SubText = "Files found: " + totalFound;
				}));
			};

			foreach(var path in Core.Models.Settings.Default.Libraries) {
				tasks.Add(searcher.SearchAsync(path));
			}

			await Task.WhenAll(tasks);

			var groups = tasks
				.SelectMany(t => t.Result)
				.GroupBy(r => r.Match.Name, StringComparer.OrdinalIgnoreCase)
				.Select(g => new FindFilesInfo {
					SeriesName = g.Key,
					Results = g.ToList()
				});

			foundFiles = new ObservableCollection<FindFilesInfo>(groups);
			dataGrid.ItemsSource = foundFiles;

			var total = groups.Count();

			status.Text = "Processing " + foundFiles.Sum(g => g.Results.Count()) + " files...";
			status.SubText = String.Format("{0} / {1} series", 0, total);
			status.ShowProgress = true;
			
			var completed = 0;

			await Task.Factory.StartNew(() => {
				Parallel.ForEach(foundFiles, info => {
					ProcessFiles(info);
					Interlocked.Increment(ref completed);

					this.Dispatcher.BeginInvoke(new Action(() => {			
						status.SubText = String.Format("{0} / {1} series", completed, total);
						status.UpdateProgress(completed, total);
					}));
				});
			});

			grid.Children.Remove(status);
		}

		void ProcessFiles(FindFilesInfo info) {
			Series series;
			using(var db = new EpisodeTrackerDBContext()) {
				series = db.Series.SingleOrDefault(s => s.Name == info.SeriesName);

				if(series == null) {
					info.Status = "Searching for series on TVDB...";
					TVDBSearchResult tvdbResult;
					try {
						tvdbResult = TVDBSeriesSearcher.Search(info.SeriesName);
					} catch(Exception e) {
						info.Status = "Error searching TVDB for series: " + e.Message;
						info.Error = true;
						Logger.Error("Error searching for series: " + info.SeriesName + " - " + e);
						return;
					}

					if(tvdbResult == null) {
						info.Status = "Series does not exist on TVDB";
						info.NotFound = true;
						return;
					}

					series = db.Series.SingleOrDefault(s => s.TVDBID == tvdbResult.ID);

					if(series == null) {
						info.Status = "Syncing TVDB series...";
						try {
							var syncer = new TVDBSeriesSyncer {
								Name = info.SeriesName,
								TVDBID = tvdbResult.ID,
								DownloadBanners = true
							};
							syncer.BannerDownloaded += (o, e) => info.Status = String.Format("Banners downloaded: {0}/{1}", e.Complete, e.Total);
							syncer.Sync();
						} catch(Exception e) {
							info.Status = "Error syncing with TVDB: " + e.Message;
							info.Error = true;
							Logger.Error("Error syncing with TVDB: " + tvdbResult.Name + " - " + e);
							return;
						}
						series = db.Series.SingleOrDefault(s => s.TVDBID == tvdbResult.ID);
						info.Status = "Synced";
					}
				}

				info.Status = "Updating episodes with file names...";
				var episodes = series.Episodes.ToList();
				foreach(var f in info.Results) {
					var eps = episodes.Where(ep =>
						f.Match.Season.HasValue
						&& ep.Season == f.Match.Season
						&& (
							ep.Number == f.Match.Episode
							|| f.Match.ToEpisode.HasValue
							&& ep.Number >= f.Match.Episode
							&& ep.Number <= f.Match.ToEpisode
						)
					);

					eps.ToList().ForEach(ep => ep.FileName = f.FileName);
				}

				try {
					db.SaveChanges();
				} catch(Exception e) {
					info.Status = "Could not update episodes: " + e.Message;
					info.Error = true;
					Logger.Error("Error update episodes with file names: " + e.Message);
					return;
				}

				info.Status = "Complete";
				info.Complete = true;
			}
		}
	}
}
