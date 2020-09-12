using System;
using System.Globalization;
using Xamarin.Forms;

namespace iBeaconProto.Converters
{
    public class MonitoringStatusToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "Stop Monitor" : "Start Monitor";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
