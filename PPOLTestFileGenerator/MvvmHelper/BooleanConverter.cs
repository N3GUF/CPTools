using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Comdata.AppSupport.PPOLTestFileGenerator.MvvmHelper
{
    public class BoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return false;
            return (bool)value;
        }
        public object ConvertBack(object value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return false;
            return (bool)value;
        }
    }
}
