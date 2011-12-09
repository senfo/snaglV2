//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Data;

namespace Berico.Common.UI.Converters
{
    /// <summary>
    /// Covnerter to convert a boolean value to Visibility
    /// (and vice versa).  A boolean (true or false) can be
    /// passed as a parameter which is used to indicate if
    /// the value being converted should be inverted first
    /// (meaning true = false and false = true).
    /// </summary>
    public class VisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {

                bool visibility = (bool)value;

                // If a parameter is passed, it will be 'true' (which inidicates
                // the resulting value should be inverted); otherwise it will be
                // 'false' (which indicates the resulting value should not be
                // inverted.
                if (parameter != null)
                {
                    if (bool.Parse(parameter.ToString()))
                        visibility = !visibility;
                }

                return visibility ? Visibility.Visible : Visibility.Collapsed;
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {

                bool visibility = ((value is Visibility) && ((Visibility)value) == Visibility.Visible);

                // If a parameter is passed, it will be 'true' (which inidicates
                // the resulting value should be inverted); otherwise it will be
                // 'false' (which indicates the resulting value should not be
                // inverted.
                if (parameter != null)
                {
                    if (bool.Parse(parameter.ToString()))
                        visibility = !visibility;
                }

                return visibility;
            }

        #endregion
    }
}