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
using System.Windows.Media;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Collections;

namespace Berico.Common
{
    /// <summary>
    /// This class contains custom extension methods
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Returns a GridLength object based on the provided string value
        /// </summary>
        /// <param name="valueToParse">A string value representation of a grid length</param>
        /// <returns>a GridLength set to the value provided by the valueToParse parameter</returns>
        public static GridLength ToGridLength(this string valueToParse)
        {
            valueToParse = valueToParse.Trim();

            // Check if we are dealing with a star value
            if (valueToParse.EndsWith("*"))
            {
                // Get the actualy numeric value
                string widthValue = valueToParse.Substring(0, valueToParse.Length - 1);
                double starWidth = 1;

                if (string.IsNullOrEmpty(widthValue))
                    starWidth = 1;
                else
                    starWidth = double.Parse(widthValue);

                // I tried returning a GridUnitType.Start GridLength
                // but it didn't work so only the 'Pixel' value is
                // being returned
                return new GridLength(starWidth, GridUnitType.Pixel);
            }

            // Check if it is set to auto
            if (valueToParse.ToLower().Equals("auto"))
                return GridLength.Auto;

            // Return a new GridLength with the appropriate value
            double doubleWidth = double.NaN;

            if (double.TryParse(valueToParse, out doubleWidth))
                return new GridLength(doubleWidth);
            else
                return GridLength.Auto;
        }

        /// <summary>
        /// Converts a double to a radians
        /// </summary>
        /// <param name="input">The double to be converted</param>
        /// <returns>the calculated radian</returns>
        public static double ToRad(this double input)
        {
            return input * Math.PI / 180;
        }

        public static List<T> GetChildObjects<T>(this DependencyObject obj, string name)
            where T : class
        {
            var retVal = new List<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                object child = VisualTreeHelper.GetChild(obj, i);
                if (child.GetType().FullName == typeof(T).FullName && (string.IsNullOrEmpty(name) || (child as FrameworkElement).Name == name))
                    retVal.Add(child as T);

                var children = (child as DependencyObject).GetChildObjects<T>(name);

                if (children != null)
                    retVal.AddRange(children);
            }

            return retVal;
        }

        public static T GetChildObject<T>(this DependencyObject obj, string name)
            where T : class
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                object child = VisualTreeHelper.GetChild(obj, i);
                if (child.GetType().FullName == typeof(T).FullName && (string.IsNullOrEmpty(name) || (child as FrameworkElement).Name == name))
                    return child as T;

                object gc = (child as DependencyObject).GetChildObject<T>(name);

                if (gc != null)
                    return gc as T;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string[] SplitWords(this string target)
        {
            return Regex.Split(target, @"\W+");
        }

        /// <summary>
        /// Determines if a string exists within another string using the
        /// provided comparison type.  The .Net framework version of 
        /// Contains does not let you specify a StringComparison value.
        /// </summary>
        /// <param name="source">The source string</param>
        /// <param name="target">The target string to look for</param>
        /// <param name="comparisonType">The StringComparison value to use when comparing strings </param>
        /// <returns>true if the target string is in the source string; otherwise false</returns>
        public static bool Contains(this string source, string target, StringComparison comparisonType)
        {
            return source.IndexOf(target, comparisonType) >= 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceEnum"></param>
        /// <returns></returns>
        public static List<string> GetNames(this Type enumType)
        {
            if (!enumType.IsEnum)
                throw new InvalidOperationException("Target parameter must be an enumeration.");


            List<string> enumNames = new List<string>();

            foreach (FieldInfo fieldInfo in enumType.GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                enumNames.Add(fieldInfo.Name); 
            }

            return enumNames;
        }

        /// <summary>
        /// Determines if the given point is inside the bounding box
        /// formed by the other two poitns (top left and bottom right)
        /// that were provided.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="topLeft"></param>
        /// <param name="bottomRight"></param>
        /// <returns></returns>
        public static bool IsInBoundingBox(this Point targetPoint, Point topLeft, Point bottomRight)
        {
            if (topLeft.X <= targetPoint.X && targetPoint.X <= bottomRight.X
                && topLeft.Y <= targetPoint.Y && targetPoint.Y <= bottomRight.Y)
                return true;
            else
                return false;
        }
    }
}