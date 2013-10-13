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
using System.Data.Entity;
using EpisodeTracker.Core.Logging;

namespace EpisodeTracker.WPF.Views.Logs {
	/// <summary>
	/// Interaction logic for Index.xaml
	/// </summary>
	public partial class Index : Window {
		public static LogLevel[] Levels = Enum.GetValues(typeof(LogLevel)).Cast<LogLevel>().ToArray();

		public Index() {
			InitializeComponent();
		}

		public int? EpisodeID { get; set; }

		bool init = false;
		protected override void OnActivated(EventArgs e) {
			base.OnActivated(e);

			if(init) return;

			using(var db = new EpisodeTrackerDBContext()) {
				logCombo.ItemsSource = new [] { "Any" }.Union(db.Log.Select(l => l.Key).Distinct().OrderBy(k => k)).ToList();
				logCombo.SelectedIndex = 0;
			}

			levelCombo.ItemsSource = Levels;
			levelCombo.SelectedItem = LogLevel.Debug;

			fromDatePicker.SelectedDate = DateTime.Now.AddDays(-7);
			fromDatePicker.DisplayDateEnd = DateTime.Now;

			untilDatePicker.DisplayDateEnd = DateTime.Now;

			LoadAsync();

			init = true;
		}

		async void LoadAsync() {
			var key = logCombo.SelectedItem as string;
			if(key == "Any") key = null;

			var level = (LogLevel)levelCombo.SelectedItem;
			var from = fromDatePicker.SelectedDate;
			var until = untilDatePicker.SelectedDate;

			var entries = await Task.Factory.StartNew(() => {
				using(var db = new EpisodeTrackerDBContext()) {
					var log = db.Log.Where(l => !EpisodeID.HasValue || l.EpisodeID == EpisodeID)
						.OrderByDescending(l => l.Date)
						.Include(l => l.Episode)
						.Include(l => l.Episode.Series);

					if(key != null) log = log.Where(l => l.Key == key);

					var i = Array.FindIndex(Levels, l => l == level);
					var levels = Levels.Skip(i);
					log = log.Where(l => levels.Contains(l.Level));

					if(from.HasValue) log = log.Where(l => l.Date >= from);
					if(until.HasValue) {
						until = until.Value.AddDays(1).AddMilliseconds(-1);
						log = log.Where(l => l.Date <= until);
					}

					return log.ToList();
				}
			});

			logGrid.ItemsSource = entries;
		}

		void Filter_Changed(object sender, RoutedEventArgs e) {
			if(!init) return;
			LoadAsync();
		}

		private void Window_KeyUp(object sender, KeyEventArgs e) {
			if(e.Key == Key.F5) {
				LoadAsync();
			}
		}
	}
}
