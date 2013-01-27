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

		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);

			librariesTextBox.Text = String.Join("\n", EpisodeTracker.Core.Models.Settings.Default.Libraries);
		}

		private void Save_Click(object sender, RoutedEventArgs e) {
			EpisodeTracker.Core.Models.Settings.Default.Libraries = librariesTextBox.Text.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
			this.Close();
		}
	}
}
