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

namespace Berico.Common
{
    /// <summary>
    /// Provides utility functions for the Point class
    /// </summary>
    public static class PointUtils
    {

        /// <summary>
        /// Compute the distance between the two point values
        /// </summary>
        /// <param name="source">The source point</param>
        /// <param name="target">The target point</param>
        /// <returns>the distance between the two provided points</returns>
        public static double Distance(this Point source, Point target)
        {
            return Math.Sqrt(
                (source.X - target.X) * (source.X - target.X) +
                (source.Y - target.Y) * (source.Y - target.Y)
                );
        }

        /// <summary>
        /// Determines the upper left point using the two provided points 
        /// </summary>
        /// <param name="first">The first point</param>
        /// <param name="second">The second poin</param>
        /// <returns>the upper left point</returns>
        public static Point GetTopLeft(this Point first, Point second)
        {
            Point topLeft = new Point(double.MaxValue, double.MaxValue);

            topLeft.X = Math.Min(first.X, second.X);
            topLeft.Y = Math.Min(first.Y, second.Y);

            return topLeft;
        }

        /// <summary>
        /// Determines the lower right point using the two provided points
        /// </summary>
        /// <param name="first">The first point</param>
        /// <param name="second">The second poin</param>
        /// <returns>the lower right point</returns>
        public static Point GetBottomRight(this Point first, Point second)
        {
            Point bottomRight = new Point(double.MinValue, double.MinValue);

            bottomRight.X = Math.Max(first.X, second.X);
            bottomRight.Y = Math.Max(first.Y, second.Y);

            return bottomRight;
        }
    }
}
