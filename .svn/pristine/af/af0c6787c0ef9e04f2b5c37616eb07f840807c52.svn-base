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
    /// This custom converter makes it possible for different data templates
    /// to be used in an ItemsControl based on the type.  This means the source
    /// bound to the ItemsControl can contain different types and they will 
    /// display appropriately.
    /// </summary>
    public class DataTemplateValueConverter : IValueConverter
    {
        // The ending portion of the key for target data templates
        private const string DataTemplateKeyPostfix = ".DataTemplate";

        #region IValueConverter Members

            /// <summary>
            /// Converts the provided value to a DataTemplate (which is specified in
            /// Application.Resources)
            /// </summary>
            /// <param name="value">The source data being passed to the target</param>
            /// <param name="targetType">The System.Type of data expected by the target dependency property</param>
            /// <param name="parameter">An optional parameter to be used in the converter logic</param>
            /// <param name="culture">The culture of the conversion</param>
            /// <returns>The value to be passed to the target dependency property</returns>
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                if (value != null)
                {
                    // Construct our key based on the name of the type and
                    // '.DataTemplate'
                    string key = value.GetType().Name + DataTemplateKeyPostfix;

                    // Check if the application contains a resource with this name
                    if (Application.Current.Resources.Contains(key))
                    {
                        // Return the correct DataTemplate
                        return Application.Current.Resources[key] as DataTemplate;
                    }
                }

                return null;
            }

            /// <summary>
            /// Modifies the target data before passing it to the source object. This method
            /// is called only in System.Windows.Data.BindingMode.TwoWay bindings.  This isn't
            /// used in this conversion and just returns null.
            /// </summary>
            /// <param name="value">The target data being passed to the source</param>
            /// <param name="targetType">The System.Type of data expected by the source object</param>
            /// <param name="parameter">An optional parameter to be used in the converter logic</param>
            /// <param name="culture">The culture of the conversion</param>
            /// <returns>The value to be passed to the source object</returns>
            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                return null;
            }

        #endregion
    }
}