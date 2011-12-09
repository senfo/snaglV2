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
using System.Windows.Media;

namespace Berico.Common
{
    /// <summary>
    /// This utility class provides conversion related functions
    /// </summary>
    public static class Conversion
    {
        //TODO: UPDATE TO TYPE CONVERTER
        //TODO: CHECK ON PERFORMANCE

        /// <summary>
        /// Converts the specified color name into a Color object and returns
        /// an appropriate SolidColorBrush
        /// </summary>
        /// <param name="colorName"></param>
        /// <returns>a SolidColorBrush set to the specified color; otherwise
        /// a SolidColorBrush set to Colors.Black</returns>
        public static SolidColorBrush ColorNameToBrush(string colorName)
        {
            Type colorType = typeof(Colors);
            object objColor = null;

            // Use reflection to return a Color object for the provided
            // string value
            if (colorType.GetProperty(colorName) != null)
                objColor = colorType.InvokeMember(colorName, System.Reflection.BindingFlags.GetProperty, null, null, null);
            else
                return new SolidColorBrush(Colors.Black);

            // Create a SolidColorBrush using the new color object
            if (objColor == null)
                return new SolidColorBrush(Colors.Black);
            else
                return new SolidColorBrush((Color)objColor);
               
        }

        //TODO: UPDATE TO TYPE CONVERTER

        /// <summary>
        /// Converts a hexidecimal color value into an ARGB value and returns
        /// an appropriate SolidColorBrush
        /// </summary>
        /// <param name="hexValue">a color specified in hexidecimal value</param>
        /// <returns>a SolidColorBrush set to the specified hex color; Otherwise
        /// a SolidColorBrush set to Colors.Black</returns>
        public static SolidColorBrush HexColorToBrush(string hexValue)
        {
            hexValue = hexValue.Replace("#", "");

            // Validate the hex value provided
            if (string.IsNullOrEmpty(hexValue))
                return new SolidColorBrush(Colors.Black);

            if (hexValue.Length < 6 || hexValue.Length > 8)
                return new SolidColorBrush(Colors.Black);

            byte a = 255;
            byte r = 255;
            byte g = 255;
            byte b = 255;

            int startPosition = 0;

            // If the hex string is 8 characters long then it is an ARGB
            // value that includes an Alpha channel which we must deal with.
            if (hexValue.Length == 8)
            {
                a = byte.Parse(hexValue.Substring(startPosition, 2), System.Globalization.NumberStyles.HexNumber);
                startPosition = 2;
            }

            // Handle the RGB values now
            r = byte.Parse(hexValue.Substring(startPosition, 2), System.Globalization.NumberStyles.HexNumber);
            g = byte.Parse(hexValue.Substring(startPosition + 2, 2), System.Globalization.NumberStyles.HexNumber);
            b = byte.Parse(hexValue.Substring(startPosition + 4, 2), System.Globalization.NumberStyles.HexNumber);

            // Return a newly created brush
            return new SolidColorBrush(Color.FromArgb(a, r, g, b));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool TryParse(this System.Windows.Point source, string value, out System.Windows.Point point)
        {
            // Set the default value for the output parameter
            point = default(System.Windows.Point);

            // Attempt to get the two coordinate values from the string
            string[] coordinates = value.Split(',');

            double x = default(double);
            double y = default(double);

            // Ensure that we have two items in the collection
            if (coordinates.Length == 2)
            {
                // Attempt to get the X value
                if (!double.TryParse(coordinates[0], out x))
                    return false;

                // Attempt to get the Y value
                if (!double.TryParse(coordinates[1], out y))
                    return false;

                // Create the new point and assign it to the
                // output parameter
                point = new System.Windows.Point(x, y);

                return true;
            }
                return false;

        }

    }
}