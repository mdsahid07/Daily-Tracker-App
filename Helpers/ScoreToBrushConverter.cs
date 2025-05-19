using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DailyTrackerApp.Helpers
{
    public class ScoreToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double score)
            {
                if (score <= 4)
                    return Brushes.Red;
                if (score <= 7)
                    return Brushes.Yellow;
                return Brushes.Green;
            }

            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
