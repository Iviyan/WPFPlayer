using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Player
{
    class IsEqualConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => Object.Equals(value, parameter);
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
    class IsNotEqualConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => !Object.Equals(value, parameter);
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    class IsNullOrWhiteSpaceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => String.IsNullOrWhiteSpace(value as string);
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    class IsNotNullOrWhiteSpaceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => !String.IsNullOrWhiteSpace(value as string);
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }


    class SecondsToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int s = System.Convert.ToInt32(value);

            int ts = s % 60; s /= 60;
            string r = ts.ToString().PadLeft(2, '0');
            if (s <= 9) return $"{s}:{r}";

            int tm = s % 60; s /= 60;
            r = $"{tm.ToString().PadLeft(2, '0')}:{r}";
            if (s == 0) return r;
            if (s <= 9) return $"{s}:{r}";

            int th = s % 24; s /= 24;
            r = $"{th.ToString().PadLeft(2, '0')}:{r}";
            if (s == 0) return r;
            
            int td = s;
            return $"{td}:{r}";

        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
