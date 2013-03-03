using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace EpisodeTracker.WPF.Models.MarkupExtensions {
	public class URIToBitmapConverter : MarkupExtension, IValueConverter {
		public URIToBitmapConverter() { }

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(value == null)
				return null;

			if(!string.IsNullOrEmpty(value.ToString())) {
				BitmapImage bi = new BitmapImage();
				bi.BeginInit();
				bi.UriSource = new Uri(value.ToString());
				bi.CacheOption = BitmapCacheOption.OnLoad;
				try {
					bi.EndInit();
				} catch(Exception e) {
					throw new ApplicationException("Problem loading bitmap: " + value.ToString(), e);
				}
				return bi;
			}

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException("Two way conversion is not supported.");
		}

		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
	}
}
