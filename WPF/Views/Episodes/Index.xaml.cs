using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace EpisodeTracker.WPF.Views.Episodes {
	/// <summary>
	/// Interaction logic for Index.xaml
	/// </summary>
	public partial class Index : Window {
		public Index() {
			InitializeComponent();
		}

		public int SeriesID { get; set; }

		protected override void OnActivated(EventArgs e) {
			base.OnActivated(e);
			ShowEpisodes();
		}

		void ShowEpisodes() {
			using(var db = new EpisodeTrackerDBContext()) {
				Title = db.Series.SingleOrDefault(s => s.ID == SeriesID).Name;

				var episodes = db.Episodes
					.Where(ep => ep.SeriesID == SeriesID)
					.Select(ep => new {
						Episode = ep,
						Tracked = ep.TrackedEpisodes
							.Select(te => te.TrackedFile)
							.OrderByDescending(f => f.LastTracked)
							.FirstOrDefault()
					})
					.ToList();

				var display = episodes
					.Select(ep => new {
						Season = (int?)ep.Episode.Season,
						Episode = (int?)ep.Episode.Number,
						Name = ep.Episode.Name,
						Overview = ep.Episode.Overview,
						Aired = ep.Episode.Aired,
						File = ep.Tracked != null ? System.IO.Path.GetFileName(ep.Tracked.FileName) : null,
						Date = ep.Tracked != null ? ep.Tracked.LastTracked : default(DateTime?),
						Status = ep.Tracked != null ? ep.Tracked.ProbablyWatched ? "Probably watched" : "Partial viewing" : null,
						Tracked = ep.Tracked != null ? TimeSpan.FromSeconds(ep.Tracked.TrackedSeconds) : default(TimeSpan?)		
					});

				dataGrid.ItemsSource = display
					.OrderByDescending(ep => ep.Episode)
					.OrderByDescending(ep => ep.Season);
			}
		}
	}
}
