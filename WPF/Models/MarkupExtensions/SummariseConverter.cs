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
	public class SummariseConverter : MarkupExtension, IValueConverter {
		public SummariseConverter() {}

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(value == null) return null;

			var length = int.Parse(parameter as string);
			var str = value.ToString();
			return str.Summarise(length);

		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return null;
		}

		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
	}
}
