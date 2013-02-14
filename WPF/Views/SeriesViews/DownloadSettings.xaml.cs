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

namespace EpisodeTracker.WPF.Views.SeriesViews {
	/// <summary>
	/// Interaction logic for DownloadSettings.xaml
	/// </summary>
	public partial class DownloadSettings : Window {
		bool formSet;

		public DownloadSettings() {
			InitializeComponent();
		}

		public int SeriesID { get; set; }

		protected override void OnActivated(EventArgs e) {
			base.OnActivated(e);

			if(formSet) return;
			formSet = true;
			
			Series series;
			using(var db = new EpisodeTrackerDBContext()) {
				series = db.Series.Single(s => s.ID == SeriesID);
			}

			minSeedsTxt.Text = series.DownloadMinSeeds.GetValueOrDefault(50).ToString();
			minMBTxt.Text = series.DownloadMinMB.GetValueOrDefault(50).ToString();
			maxMBTxt.Text = series.DownloadMaxMB.GetValueOrDefault(2000).ToString();
			useAbsoluteEpisodeChk.IsChecked = series.DownloadUseAbsoluteEpisode;
			downloadHDCombo.SelectedIndex = series.DownloadHD.HasValue ? series.DownloadHD.Value ? 1 : 2 : 0;
			enableAutoDownloadChk.IsChecked = series.DownloadAutomatically;
		}

		void Save_Click(object sender, RoutedEventArgs e) {
			using(var db = new EpisodeTrackerDBContext()) {
				var series = db.Series.Single(s => s.ID == SeriesID);

				series.DownloadMinSeeds = String.IsNullOrEmpty(minSeedsTxt.Text) ? default(int?) : int.Parse(minSeedsTxt.Text);
				series.DownloadMinMB = String.IsNullOrEmpty(minMBTxt.Text) ? default(int?) : int.Parse(minMBTxt.Text);
				series.DownloadMaxMB = String.IsNullOrEmpty(maxMBTxt.Text) ? default(int?) : int.Parse(maxMBTxt.Text);
				series.DownloadUseAbsoluteEpisode = useAbsoluteEpisodeChk.IsChecked.GetValueOrDefault();
				series.DownloadHD = downloadHDCombo.SelectedIndex == 0 ? default(bool?) : downloadHDCombo.SelectedIndex == 1 ? true : false;
				series.DownloadAutomatically = enableAutoDownloadChk.IsChecked.GetValueOrDefault();

				db.SaveChanges();
			}

			this.Close();
		}
	}
}
