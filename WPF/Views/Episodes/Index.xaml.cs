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

		public int? SeriesID { get; set; }

		protected override void OnActivated(EventArgs e) {
			base.OnActivated(e);
			ShowEpisodes();
		}

		void ShowEpisodes() {
			using(var db = new EpisodeTrackerDBContext()) {
				var watching = db.TrackedFiles
					.Where(f => !SeriesID.HasValue || f.Episode.SeriesID == SeriesID)
					.OrderByDescending(e => e.LastTracked)
					.ToList();

				var display = watching
					.Where(f => f.Episode != null)
					.Select(f => new {
						File = System.IO.Path.GetFileName(f.FileName),
						Series = f.Episode.Series.Name,
						Season = (int?)f.Episode.Season,
						Episode = (int?)f.Episode.Number,
						Date = f.LastTracked,
						Status = f.ProbablyWatched ? "Probably watched" : "Partial viewing",
						Tracked = TimeSpan.FromSeconds(f.TrackedSeconds)
					});

				dataGrid.ItemsSource = display.OrderByDescending(ep => ep.File).OrderByDescending(ep => ep.Date);
			}
		}
	}
}
