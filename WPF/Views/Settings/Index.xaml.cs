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

namespace EpisodeTracker.WPF.Views.Settings {
	/// <summary>
	/// Interaction logic for Index.xaml
	/// </summary>
	public partial class Index : Window {
		public Index() {
			InitializeComponent();
		}

		EpisodeTracker.Core.Models.Settings settings;

		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);

			settings = EpisodeTracker.Core.Models.Settings.Default;
			librariesTextBox.Text = String.Join("\r\n", settings.Libraries);
			downloadServiceEnabled.IsChecked = settings.Download.Service.IsEnabled;
		}

		private void Save_Click(object sender, RoutedEventArgs e) {
			settings.Libraries = librariesTextBox.Text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			settings.Download.Service.IsEnabled = downloadServiceEnabled.IsChecked.GetValueOrDefault();

			var downloadService = ((MainWindow)App.Current.MainWindow).DownloadService;
			if(settings.Download.Service.IsEnabled) {
				if(!downloadService.IsRunning) downloadService.Start();
			} else {
				if(downloadService.IsRunning) downloadService.Stop();
			}

			this.Close();
		}
	}
}
