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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EpisodeTracker.WPF.Views.Shared {
	/// <summary>
	/// Interaction logic for StatusModal.xaml
	/// </summary>
	public partial class StatusModal : UserControl {
		public StatusModal() {
			InitializeComponent();
		}

		public string Text {
			get { return statusText.Text; }
			set { statusText.Text = value; }
		}

		public string SubText {
			get { return statusSubText.Text; }
			set { statusSubText.Text = value; }
		}

		public bool ShowSubText {
			get { return statusSubText.Visibility == System.Windows.Visibility.Visible; }
			set { statusSubText.Visibility = value ? Visibility.Visible : System.Windows.Visibility.Collapsed; }
		}

		public bool ShowProgress {
			get { return statusProgress.Visibility == System.Windows.Visibility.Visible; }
			set { statusProgress.Visibility = value ? Visibility.Visible : System.Windows.Visibility.Collapsed; }
		}

		public double Progress {
			get { return statusProgress.Value; }
			set { statusProgress.Value = value; }
		}

		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);

			statusSubText.Visibility = System.Windows.Visibility.Collapsed;
			statusProgress.Visibility = System.Windows.Visibility.Collapsed;
			Progress = 0;
		}
	}
}
