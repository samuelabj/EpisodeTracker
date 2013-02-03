using System;
using System.Collections.Concurrent;
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

		public enum SeriesFileInfoState {
			None,
			Found,
			NotFound,
			Ignored,
			Synced,
			Error
		}

		class SeriesFileInfo : INotifyPropertyChanged {
			public event PropertyChangedEventHandler PropertyChanged;
			public string SeriesName { get; set; }
			public List<EpisodeFileSearchResult> Results { get; set; }
			public string FileNames { get { return String.Join("\n", Results.Select(r => r.FileName)); } }
			public string Status { get; set; }
			public SeriesFileInfoState State { get; set; }
			public int? TVDBID { get; set; }
			public int? SeriesID { get; set; }
			public string SuggestedName { get; set; }
		};

		ObservableCollection<SeriesFileInfo> foundFiles;
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

			try {
				await Task.WhenAll(tasks);
			} catch(ApplicationException e) {
				Logger.Error("Error searching for files: " + e);
				MessageBox.Show(e.Message);
			}

			var groups = tasks
				.Where(t => !t.IsFaulted)
				.SelectMany(t => t.Result)
				.GroupBy(r => r.Match.Name, StringComparer.OrdinalIgnoreCase)
				.Select(g => new SeriesFileInfo {
					SeriesName = g.Key,
					Results = g.ToList()
				})
				.OrderBy(g => g.SeriesName);

			foundFiles = new ObservableCollection<SeriesFileInfo>(groups);
			dataGrid.ItemsSource = foundFiles;

			var total = groups.Count();

			status.Text = "Downloading series info...";
			status.SubText = String.Format("{0} / {1} series", 0, total);
			status.ShowProgress = true;
			
			var completed = 0;
			await Task.Factory.StartNew(() => {
				var para = foundFiles
					.AsParallel()
					.WithDegreeOfParallelism(5);

				para.ForAll(info => {
					FindSeries(info);
					Interlocked.Increment(ref completed);

					this.Dispatcher.BeginInvoke(new Action(() => {
						status.SubText = String.Format("{0} / {1} series", completed, total);
						status.UpdateProgress(completed, total);
					}));
				});
			});

			grid.Children.Remove(status);
		}

		void FindSeries(SeriesFileInfo info) {
			Series series;
			info.State = SeriesFileInfoState.None;
			info.TVDBID = null;
			info.SeriesID = null;

			using(var db = new EpisodeTrackerDBContext()) {
				var seriesName = info.SuggestedName ?? info.SeriesName;

				if(db.SeriesIgnore.Any(s => s.Name == seriesName)) {
					info.State = SeriesFileInfoState.Ignored;
					info.Status = "Ignored";
					return;
				}

				series = db.Series.SingleOrDefault(s => s.Name == seriesName || s.Aliases.Any(a => a.Name == seriesName));

				if(series == null || !series.TVDBID.HasValue) {
					info.Status = "Searching for series on TVDB...";
					TVDBSearchResult tvdbResult;
					try {
						tvdbResult = TVDBSeriesSearcher.Search(seriesName);
					} catch(Exception e) {
						info.Status = "Error searching TVDB for series: " + e.Message;
						info.State = SeriesFileInfoState.Error;
						Logger.Error("Error searching for series: " + seriesName + " - " + e);
						return;
					}

					if(tvdbResult == null) {
						info.Status = "Series does not exist on TVDB";
						info.State = SeriesFileInfoState.NotFound;
						return;
					}

					if(series == null) {
						series = db.Series.SingleOrDefault(s => s.Name == tvdbResult.Name || s.Aliases.Any(a => a.Name == tvdbResult.Name));
						if(series != null) info.SeriesID = series.ID;
					}

					info.SuggestedName = tvdbResult.Name;
					info.TVDBID = tvdbResult.ID;
				} else {
					info.SuggestedName = series.Name;
					info.TVDBID = series.TVDBID;
					info.SeriesID = series.ID;

					// Confirmed series, check if there's any new files
					var episodes = series.Episodes.ToList();
					List<EpisodeFileSearchResult> newFiles = new List<EpisodeFileSearchResult>();

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

						if(eps.Any(ep => ep.FileName != f.FileName)) {
							newFiles.Add(f);
						}
					}

					info.Results = newFiles;

					if(!newFiles.Any()) {
						info.State = SeriesFileInfoState.Ignored;
						return;
					}
				}

				info.Status = "Found series info: " + info.TVDBID;
				info.State = SeriesFileInfoState.Found;
			}
		}

		void SyncFiles(SeriesFileInfo info) {
			Series series;
			info.State = SeriesFileInfoState.None;

			using(var db = new EpisodeTrackerDBContext()) {
				series = db.Series.SingleOrDefault(s => s.ID == info.SeriesID || s.TVDBID == info.TVDBID);

				if(series == null || series.Updated <= DateTime.Now.AddDays(-7)) {
					info.Status = "Syncing TVDB series...";
					try {
						var syncer = new TVDBSeriesSyncer {
							Name = info.SeriesName,
							TVDBID = info.TVDBID.Value,
							DownloadBanners = true
						};
						syncer.BannerDownloaded += (o, e) => info.Status = String.Format("Banners downloaded: {0}/{1}", e.Complete, e.Total);
						syncer.Sync();
					} catch(Exception e) {
						info.Status = "Error syncing with TVDB: " + e.Message;
						info.State = SeriesFileInfoState.Error;
						Logger.Error("Error syncing with TVDB: " + info.TVDBID + " - " + e);
						return;
					}
					series = db.Series.SingleOrDefault(s => s.TVDBID == info.TVDBID);
					info.Status = "Synced";
				} else if(!series.Name.Equals(info.SeriesName, StringComparison.OrdinalIgnoreCase)) {
					// Save alias
					if(!db.SeriesAliases.Any(a => a.Name == info.SeriesName)) {
						db.SeriesAliases.Add(new SeriesAlias {
							SeriesID = series.ID,
							Name = info.SeriesName
						});
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
					info.State = SeriesFileInfoState.Error;
					Logger.Error("Error update episodes with file names: " + e.Message);
					return;
				}

				info.Status = "Synced";
				info.State = SeriesFileInfoState.Synced;
			}
		}

		private async void SuggestedName_LostFocus(object sender, RoutedEventArgs e) {
			var info = dataGrid.SelectedItem as SeriesFileInfo;
			var txtbox = sender as TextBox;
			info.SuggestedName = txtbox.Text;

			if(!info.SuggestedName.Equals(info.SeriesName)) {
				await Task.Factory.StartNew(() => FindSeries(info));
			}
		}

		private async void Sync_Click(object sender, RoutedEventArgs e) {
			var selected = dataGrid.SelectedItems
				.Cast<object>()
				.Where(o => o is SeriesFileInfo)
				.Cast<SeriesFileInfo>()
				.Where(s => s.TVDBID.HasValue)
				.ToList();

			var total = selected.Count();

			var status = new StatusModal {
				Text = "Processing " + foundFiles.Sum(g => g.Results.Count()) + " files...",
				SubText = String.Format("{0} / {1} series", 0, total),
				ShowSubText = true,
				ShowProgress = true,
			};

			grid.Children.Add(status);

			var completed = 0;
			await Task.Factory.StartNew(() => {
				var para = selected
					.AsParallel()
					.WithDegreeOfParallelism(5);

				para.ForAll(info => {
					SyncFiles(info);
					Interlocked.Increment(ref completed);

					this.Dispatcher.BeginInvoke(new Action(() => {
						status.SubText = String.Format("{0} / {1} series", completed, total);
						status.UpdateProgress(completed, total);
					}));
				});
			});

			grid.Children.Remove(status);
		}

		private void Ignore_Click(object sender, RoutedEventArgs e) {
			var selected = dataGrid.SelectedItems
				.Cast<object>()
				.Where(o => o is SeriesFileInfo)
				.Cast<SeriesFileInfo>()
				.ToList();

			using(var db = new EpisodeTrackerDBContext()) {
				foreach(var info in selected) {
					if(!db.SeriesIgnore.Any(i => i.Name == info.SeriesName)) {
						db.SeriesIgnore.Add(new SeriesIgnore {
							Name = info.SeriesName
						});
						info.Status = "Added to ignore list";
						info.State = SeriesFileInfoState.Ignored;
					}
				}
				db.SaveChanges();
			}
		}
	}
}
