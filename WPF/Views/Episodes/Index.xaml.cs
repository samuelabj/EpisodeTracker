﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using EpisodeTracker.Core.Data;
using EpisodeTracker.Core.Models;
using EpisodeTracker.WPF.Views.Shared;
using EpisodeTracker.Core.Logging;

namespace EpisodeTracker.WPF.Views.Episodes {
	/// <summary>
	/// Interaction logic for Index.xaml
	/// </summary>
	public partial class Index : Window {

		public class EpisodeInfo : INotifyPropertyChanged {
			public event PropertyChangedEventHandler PropertyChanged;

			public Episode Episode { get; set; }
			public TrackedEpisode Tracked { get; set; }
			public TimeSpan? TrackedTime { get; set; }
			public string BannerPath { get; set; }
		}

		ObservableCollection<EpisodeInfo> episodeInfo;
		Logger Logger;

		public Index() {
			InitializeComponent();
		}

		public int SeriesID { get; set; }

		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);

			Logger = Logger.Get("General");
			statusModal.Visibility = System.Windows.Visibility.Hidden;
		}

		protected override void OnActivated(EventArgs e) {
			base.OnActivated(e);

			if(episodeInfo == null) {
				using(var db = new EpisodeTrackerDBContext()) {
					var title = db.Series.SingleOrDefault(s => s.ID == SeriesID).Name;
					Title = title;
				}

				ShowEpisodesAsync();
			}
		}

		List<EpisodeInfo> GetEpisodes() {
			using(var db = new EpisodeTrackerDBContext()) {
				var episodes = db.Episodes
					.Where(ep => ep.SeriesID == SeriesID && ep.Season != 0)
					.Select(ep => new {
						Episode = ep,
						Tracked = ep.Tracked
							.OrderByDescending(t => t.Updated)
							.OrderByDescending(t => t.Watched)
							.FirstOrDefault()
					})
					.ToList();

				return episodes
					.Select(ep => new EpisodeInfo {
						Episode = ep.Episode,
						Tracked = ep.Tracked,
						TrackedTime = ep.Tracked != null && ep.Tracked.TrackedFile != null ? TimeSpan.FromSeconds(ep.Tracked.TrackedFile.TrackedSeconds) : default(TimeSpan?),
						BannerPath = GetBannerPath(ep.Episode)
					})
					.ToList();
			}
		}

		async void ShowEpisodesAsync() {
			statusModal.Text = "Loading...";
			statusModal.Visibility = System.Windows.Visibility.Visible;
			
			var episodes = await Task.Factory.StartNew(() => GetEpisodes());

			if(!episodes.Any(d => d.BannerPath != null)) {
				dataGrid.Columns.First().Visibility = System.Windows.Visibility.Collapsed;
			}

			EventHandler loaded = null;
			loaded = new EventHandler((o, e) => {
				dataGrid.SelectedIndex = 0;
				DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(0);
				if(row != null) {
					row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
				}

				dataGrid.LayoutUpdated -= loaded;
			});
			dataGrid.LayoutUpdated += loaded;

			dataGrid.ItemsSource = episodeInfo = new ObservableCollection<EpisodeInfo>(
				episodes
				.OrderByDescending(ep => ep.Episode.Number)
				.OrderByDescending(ep => ep.Episode.Season)
			);

			statusModal.Visibility = System.Windows.Visibility.Collapsed;
		}

		string GetBannerPath(Episode ep) {
			var path = System.IO.Path.GetFullPath(@"Resources\Series\" + ep.SeriesID + @"\" + ep.ID + ".jpg");
			if(System.IO.File.Exists(path)) return path;
			return null;
		}

		private void Watched_Click(object sender, RoutedEventArgs e) {
			var selected = dataGrid.SelectedItems.Cast<EpisodeInfo>();
			var selectedIDs = selected.Select(s => s.Episode.ID);

			using(var db = new EpisodeTrackerDBContext()) {
				foreach(var info in selected) {
					if(info.Tracked != null && info.Tracked.Watched) continue;
					
					info.Tracked = new TrackedEpisode {
						EpisodeID = info.Episode.ID,
						Added = DateTime.Now,
						Updated = DateTime.Now,
						Watched = true
					};

					db.TrackedEpisodes.Add(info.Tracked);
				}

				db.SaveChanges();
			}
		}

		private void Unwatched_Click(object sender, RoutedEventArgs e) {
			var selected = dataGrid.SelectedItems.Cast<EpisodeInfo>();
			var selectedIDs = selected.Select(s => s.Episode.ID);

			using(var db = new EpisodeTrackerDBContext()) {
				foreach(var info in selected) {
					var tracked = db.TrackedEpisodes.Where(te => te.EpisodeID == info.Episode.ID && te.Watched);
					if(!tracked.Any()) continue;

					foreach(var te in tracked) db.TrackedEpisodes.Remove(te);
					info.Tracked = null;
				}

				db.SaveChanges();
			}
		}

		private void Delete_Click(object sender, RoutedEventArgs e) {
			if(MessageBox.Show("Are you sure you want to delete these episodes?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) {
				var selected = dataGrid.SelectedItems.Cast<EpisodeInfo>();
				var selectedIDs = selected.Select(s => s.Episode.ID);

				using(var db = new EpisodeTrackerDBContext()) {
					var eps = db.Episodes.Where(ep => selectedIDs.Contains(ep.ID));
					foreach(var ep in eps) {
						db.Episodes.Remove(ep);
						episodeInfo.Remove(selected.Single(s => s.Episode.ID == ep.ID));
					}

					db.SaveChanges();
				}
			}
		}

		private void Window_KeyUp(object sender, KeyEventArgs e) {
			if(e.Key == Key.F5) {
				ShowEpisodesAsync();
			} else if(e.Key == Key.Escape) {
				this.Close();
			}
		}

		void Watch_Click(object sender, RoutedEventArgs e) {
			var info = dataGrid.SelectedItem as EpisodeInfo;

			PerformWatch(info.Episode);
		}

		void Row_KeyDown(object sender, KeyEventArgs e) {
			var row = sender as DataGridRow;
			var info = row.Item as EpisodeInfo;

			if(e.Key == Key.Enter || e.Key == Key.W) {
				PerformWatch(info.Episode);
			}
		}

		async void PerformWatch(Episode episode) {
			if(episode == null) {
				MessageBox.Show("Nothing to watch");
			}

			if(episode.FileName == null || !File.Exists(episode.FileName)) {
				// Try and find
				var status = new StatusModal {
					Text = "Searching for episode file...",
					SubText = "Parsing files: 0",
					ShowSubText = true
				};
				status.SetValue(Grid.RowProperty, 1);
				grid.Children.Add(status);

				var tasks = new List<Task<List<EpisodeFileSearchResult>>>();
				var searcher = new EpisodeFileSearcher();
				var totalFound = 0;

				searcher.FilesFound += (o, ea) => {
					this.Dispatcher.BeginInvoke(new Action(() => {
						Interlocked.Add(ref totalFound, ea.Results);
						status.SubText = "Parsing files: " + totalFound;
					}));
				};

				foreach(var path in Core.Models.Settings.Default.Libraries) {
					tasks.Add(searcher.SearchAsync(path));
				}

				try {
					await Task.WhenAll(tasks);
				} catch(ApplicationException ex) {
					Logger.Error("Error searching for file: " + ex);
					MessageBox.Show(ex.Message);
				}

				var groups = tasks
				.Where(t => !t.IsFaulted)
				.SelectMany(t => t.Result)
				.GroupBy(r => r.Match.Name, StringComparer.OrdinalIgnoreCase)
				.Select(g => new {
					SeriesName = g.Key,
					Results = g.ToList()
				})
				.OrderBy(g => g.SeriesName);

				var total = groups.Count();

				status.Text = "Checking results...";
				status.SubText = String.Format("{0} / {1} series", 0, total);
				status.ShowProgress = true;

				var completed = 0;
				EpisodeFileSearchResult result = null;

				await Task.Factory.StartNew(() => {
					var para = groups
						.AsParallel()
						.WithDegreeOfParallelism(5);

					para.ForAll(info => {
						try {
							using(var db = new EpisodeTrackerDBContext()) {
								var seriesName = info.SeriesName;

								if(db.SeriesIgnore.Any(s => s.Name == seriesName)) {
									return;
								}

								var series = db.Series.SingleOrDefault(s => s.Name == seriesName || s.Aliases.Any(a => a.Name == seriesName));
								if(series == null || series.ID != episode.SeriesID) return;

								var ep = episode;
								var r = info.Results.FirstOrDefault(f =>
									!f.Match.Season.HasValue
									&& f.Match.Episode == ep.AbsoluteNumber
									|| (
										f.Match.Season.HasValue
										&& f.Match.Season == ep.Season
										&& (
											f.Match.Episode == ep.Number
											|| f.Match.ToEpisode.HasValue
											&& f.Match.Episode <= ep.Number
											&& f.Match.ToEpisode >= ep.Number
										)
									)
								);

								if(r != null) result = r;
							}
						} finally {
							Interlocked.Increment(ref completed);

							this.Dispatcher.BeginInvoke(new Action(() => {
								status.SubText = String.Format("{0} / {1} series", completed, total);
								status.UpdateProgress(completed, total);
							}));
						}
					});
				});

				grid.Children.Remove(status);

				if(result != null) {
					using(var db = new EpisodeTrackerDBContext()) {
						var episodeDB = db.Episodes.Single(ep => ep.ID == episode.ID);
						episodeDB.FileName = result.FileName;
						episode.FileName = result.FileName;
						db.SaveChanges();
					}
				} else {
					MessageBox.Show("Could not find file");
					return;
				}
			}

			try {
				Process.Start(episode.FileName);
			} catch(Exception ex) {
				MessageBox.Show("Problem opening file: " + ex.Message);
				Logger.Error("Error opening filename: " + episode.FileName + " - " + ex);
			}
		}

		async void Download_Click(object sender, RoutedEventArgs e) {
			var epInfo = dataGrid.SelectedItem as EpisodeInfo;
			var episode = epInfo.Episode;
			var logger = Logger.Get("General");

			var downloadingModel = new StatusModal();
			grid.Children.Add(downloadingModel);
			downloadingModel.Text = "Searching...";
			logger.Info("Starting search");

			Series series;
			using(var db = new EpisodeTrackerDBContext()) {
				series = db.Series.Single(s => s.ID == SeriesID);
			}
			episode.Series = series;

			var searcher = new EpisodeTorrentSearcher();
			searcher.HD = series.DownloadHD;
			searcher.MinMB = series.DownloadMinMB;
			searcher.MaxMB = series.DownloadMaxMB;
			searcher.MinSeeds = series.DownloadMinSeeds;
			searcher.UseAbsoluteEpisodeFormat = series.DownloadUseAbsoluteEpisode;

			var results = await searcher.SearchAsync(episode);

			var display = new Func<EpisodeTorrentSearcherResult, string>(r => String.Format("{0} ({1}/{2}) {3}MB", r.Title, r.Seeds, r.Leechs, r.MB));
			var displayList = new Func<IEnumerable<EpisodeTorrentSearcherResult>, string>(list => String.Join("\n", list.Select(r => display(r))));
			logger.Info(results.Any() ? String.Format("Found results: {0}\n{1}", results.Count, displayList(results)) : "No results");

			using(var db = new EpisodeTrackerDBContext()) {
				var exclude = results.Where(r => db.EpisodeDownloadLog.Any(edl => edl.URL == r.DownloadURL.AbsolutePath)).ToArray();
				logger.Info("Excluding: {0}\n{1}", exclude.Count(), displayList(exclude));
			}

			if(results.Any()) {
				var first = results.First();
				logger.Info("Downloading: " + display(first));

				downloadingModel.Text = "Downloading...";
				downloadingModel.SubText = display(first);

				var webClient = new CustomWebClient();
				if(!Directory.Exists("torrents")) Directory.CreateDirectory("torrents");

				var fileName = @"torrents\" + first.Title + ".torrent";
				await webClient.DownloadFileTaskAsync(first.DownloadURL, fileName);

				Logger.Debug("Starting process: " + fileName);

				Process.Start(fileName);

				using(var db = new EpisodeTrackerDBContext()) {
					db.EpisodeDownloadLog.Add(new EpisodeDownloadLog {
						EpisodeID = episode.ID,
						Date = DateTime.Now,
						URL = first.DownloadURL.ToString()
					});
					db.SaveChanges();
				}

				var bal = new NotificationBalloon {
					HeaderText = "Episode Tracker",
					BodyText = "Found download: " + display(first)
				};
				((MainWindow)App.Current.MainWindow).Taskbar
					.ShowCustomBalloon(bal, PopupAnimation.Slide, 5000);
			} else {
				var bal = new NotificationBalloon {
					HeaderText = "Episode Tracker",
					BodyText = "No results found"
				};
				((MainWindow)App.Current.MainWindow).Taskbar
					.ShowCustomBalloon(bal, PopupAnimation.Slide, 5000);
			}

			grid.Children.Remove(downloadingModel);
		}

		private void Log_Click(object sender, RoutedEventArgs e) {
			var selected = dataGrid.SelectedItem as EpisodeInfo;

			var log = new Views.Logs.Index();
			log.EpisodeID = selected.Episode.ID;
			log.WindowState = System.Windows.WindowState.Maximized;
			log.ShowDialog();
		}

		class CustomWebClient : WebClient {
			protected override WebRequest GetWebRequest(Uri address) {
				HttpWebRequest request = base.GetWebRequest(address) as HttpWebRequest;
				request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
				return request;
			}
		}
	}
}
