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
    /// Converter to convert between a boolean and enum value
    /// </summary>
    public class EnumBooleanValueConverter : IValueConverter
    {
        #region IValueConverter Members

            /// <summary>
            /// Converts the specified value to the specified type using
            /// the provided parameter
            /// </summary>
            /// <param name="value">The enum value to be converted to a bool</param>
            /// <param name="targetType">The type expected by the dependency property</param>
            /// <param name="parameter">The enum value to check against</param>
            /// <param name="culture">The culture of the conversion</param>
            /// <returns>The boolean value to be passed to the target dependency property</returns>
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                string parameterString = parameter as string;
                
                // Validate that a parameter value was provided
                if (parameterString == null)
                    return DependencyProperty.UnsetValue;

                // Validate that the target enum is valid
                if (Enum.IsDefined(value.GetType(), value) == false)
                    return DependencyProperty.UnsetValue;

                // Gets the value for the enum using the provided parameter which
                // specifes the enum value name
                object parameterValue = Enum.Parse(value.GetType(), parameterString, true);

                // Returns true if the provided value equals the specified
                // parameter value
                return parameterValue.Equals(value);
            }

            /// <summary>
            /// Converts the specified boolean value to the specifed enum
            /// using the provided parameter
            /// </summary>
            /// <param name="value">The bool value to be converted to the enum</param>
            /// <param name="targetType">The type expected by the source object</param>
            /// <param name="parameter">The enum value to be used</param>
            /// <param name="culture">The culture of the conversion</param>
            /// <returns>The enum value to be passed to the source object</returns>
            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                string parameterString = parameter as string;

                // Validate that a parameter value was provided
                if (parameterString == null || value.Equals(false))
                    return DependencyProperty.UnsetValue;

                // Returns the enum value using the provided parameter
                return Enum.Parse(targetType, parameterString, true);
            }

        #endregion
    }
}