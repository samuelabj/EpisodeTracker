using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using MediaReign.Core;

namespace EpisodeTracker.WPF.Models.MarkupExtensions {

	[ValueConversion(typeof(object), typeof(string))]
	public class IsNullConverter : MarkupExtension, IValueConverter {
		public IsNullConverter() { }

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value == null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return null;
		}

		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
	}
}
