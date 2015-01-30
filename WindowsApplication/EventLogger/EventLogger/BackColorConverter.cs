using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace EventLogger
{
    [ValueConversion(typeof(object), typeof(object))]
    public class BackColorConverter : BaseConverter, IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (((string)value).Contains("IntegratedBedsideTerminal.exe"))
                return new SolidColorBrush() { Color = Colors.Green };
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
