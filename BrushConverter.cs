using JW8307A.ViewModels;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace JW8307A
{
    public class BrushConverterIl : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                int i = System.Convert.ToInt32(parameter);
                var ilLowLimit = Person.IlLowerThre[i];
                var ilUpperLimit = Person.IlUpperThre[i];

                float fValue;
                if (value != null)
                {
                    fValue = System.Convert.ToSingle((string)value);
                }
                else
                {
                    return Brushes.Black;
                }
                if (fValue >= ilLowLimit && fValue <= ilUpperLimit)
                {
                    return Brushes.Black;
                }
                return Brushes.Red;
            }
            catch (Exception ex)
            {
                // ignored
            }

            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class BrushConverterRl : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                int i = System.Convert.ToInt32(parameter);
                var rlLowLimit = Person.RlLowerThre[i];
                var rlUpperLimit = Person.RlUpperThre[i];

                float fValue;
                if (value != null)
                {
                    fValue = System.Convert.ToSingle((string)value);
                }
                else
                {
                    return Brushes.Black;
                }
                if (fValue >= rlLowLimit && fValue <= rlUpperLimit)
                {
                    return Brushes.Black;
                }
                return Brushes.Red;
            }
            catch (Exception ex)
            {
                // ignored
            }

            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}