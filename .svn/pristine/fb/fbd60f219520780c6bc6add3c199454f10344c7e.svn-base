using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Linq;

namespace Berico.SnagL.Infrastructure.Clustering
{

    /// <summary>
    /// 
    /// </summary>
    public class ConvexHull
    {
        private const double _180DivPI = 57.295779513082320876798154814105000;
        private const int COUNTER_CLOCKWISE = 1;
        private const int CLOCKWISE = -1;
        private static ConvexHullPoint anchor = null;

        /// <summary>
        /// Finds the ConvexHull of a series of points.  See
        /// http://www.cs.princeton.edu/courses/archive/spr10/cos226/demo/ah/GrahamScan.html
        /// for further information.
        /// </summary>
        /// <param name="points">The points to compute the ConvexHull for</param>
        /// <returns>a collection of points that represents the polygon
        /// of the ConvexHull</returns>
        public static ICollection<Point> CalculateConvexHull(ICollection<Point> points)
        {
            if (points.Count <= 3)
                return points;

            List<ConvexHullPoint> chPoints = new List<ConvexHullPoint>();

            // Get the current anchor point
            anchor = new ConvexHullPoint(DetermineAnchorPoint(points), 0, null);

            // Loop over the points
            foreach (Point currentPoint in points)
            {
                // Ensure that this point is not the anchor
                if (currentPoint != anchor.Point)
                {
                    // Create a new ConvexHUllPoint and save it for later use
                    ConvexHullPoint newConvexHullPoint = new ConvexHullPoint(currentPoint, CalculateCartesianAngle(currentPoint.X - anchor.Point.X, currentPoint.Y - anchor.Point.Y), anchor);
                    chPoints.Add(newConvexHullPoint);
                }
            }
            // Sort the points
            chPoints.Sort();

            // Actually calculate and return the ConvexHull
            return GrahamScan(chPoints);
        }

        /// <summary>
        /// Finds the ConvexHull of a series of points.  See
        /// http://www.cs.princeton.edu/courses/archive/spr10/cos226/demo/ah/GrahamScan.html
        /// for further information.
        /// </summary>
        /// <param name="points">The points to compute the ConvexHull for</param>
        /// <returns>a collection of points that represents the polygon
        /// of the ConvexHull</returns>
        private static ICollection<Point> GrahamScan(List<ConvexHullPoint> chPoints)
        {
            Stack<ConvexHullPoint> chPointStack = new Stack<ConvexHullPoint>();
            IEnumerator<ConvexHullPoint> enumerator = chPoints.GetEnumerator();

            chPointStack.Push(anchor);
            chPointStack.Push(chPoints[0]);

            // Advance the enumerator to account for the two points
            // that we already got
            enumerator.MoveNext();
            enumerator.MoveNext();
            int i = 1;

            // Loop over all the points that were provided
            while (i < chPoints.Count)
            {
                // Ensure that stack contains points
                if (chPointStack.Count > 1)
                {
                    ConvexHullPoint firstCHPoint = chPointStack.Pop();
                    ConvexHullPoint secondCHPoint = chPointStack.Pop();

                    chPointStack.Push(secondCHPoint);
                    chPointStack.Push(firstCHPoint);

                    if (Orientation(secondCHPoint, firstCHPoint, enumerator.Current) == COUNTER_CLOCKWISE)
                    {
                        chPointStack.Push(enumerator.Current);
                        enumerator.MoveNext();
                        i++;
                    }
                    else
                        chPointStack.Pop();
                }
                else // No points in stack
                {
                    chPointStack.Push(enumerator.Current);
                    enumerator.MoveNext();
                    i++;
                }
            }

            return GetSortedPoints(chPointStack);
        }

        /// <summary>
        /// Returns a sorted collection of the points contained
        /// in the ConvexHullPoint stack
        /// </summary>
        /// <param name="chPointStack">A collection of ConvexHullPoint instances</param>
        /// <returns>a sorted collection of points</returns>
        private static ICollection<Point> GetSortedPoints(Stack<ConvexHullPoint> chPointStack)
        {
            List<Point> points = new List<Point>(chPointStack.Count);

            double totalX = 0;
            double totalY = 0;

            // Loop over the convex hull points
            foreach (ConvexHullPoint chPoint in chPointStack)
            {
                points.Add(chPoint.Point);
                totalX += chPoint.Point.X;
                totalY += chPoint.Point.Y;
            }

            double averageX = totalX / chPointStack.Count;
            double averageY = totalY / chPointStack.Count;

            // Sort the collection
            points.Sort(delegate(Point p1, Point p2)
            {
                double angle1 = CalculateCartesianAngle(p1.X - averageX, p1.Y - averageY);
                double angle2 = CalculateCartesianAngle(p2.X - averageX, p2.Y - averageY);

                return angle1.CompareTo(angle2);
            });

            return points;
        }

        /// <summary>
        /// Find the bottom-left point and use that as the anchor
        /// </summary>
        /// <param name="points">The points to use</param>
        /// <returns>the point to be used as the anchor</returns>
        private static Point DetermineAnchorPoint(ICollection<Point> points)
        {
            Point currentAnchorPoint = points.First();

            // Loop over the points
            foreach (Point currentPoint in points)
            {
                // Determine the bottom-left point
                if (currentPoint.Y < currentAnchorPoint.Y)
                    currentAnchorPoint = currentPoint;
                else if (currentPoint.Y == currentAnchorPoint.Y)
                    if (currentPoint.X < currentAnchorPoint.X)
                        currentAnchorPoint = currentPoint;
            }

            return currentAnchorPoint;
        }

        private static double CalculateCartesianAngle(double x, double y)
        {
            if ((x > 0.0) && (y > 0.0)) return (Math.Atan(y / x) * _180DivPI);
            else if ((x < 0.0) && (y > 0.0)) return (Math.Atan(-x / y) * _180DivPI) + 90.0;
            else if ((x < 0.0) && (y < 0.0)) return (Math.Atan(y / x) * _180DivPI) + 180.0;
            else if ((x > 0.0) && (y < 0.0)) return (Math.Atan(-x / y) * _180DivPI) + 270.0;
            else if ((x == 0.0) && (y > 0.0)) return 90.0;
            else if ((x < 0.0) && (y == 0.0)) return 180.0;
            else if ((x == 0.0) && (y < 0.0)) return 270.0;
            else
                return 0.0;
        }

        /// <summary>
        /// Determines the orientation of the provided points
        /// </summary>
        /// <param name="chPoint1">The first ConvexHullPoint</param>
        /// <param name="chPoint2">The second ConvexHullPoint</param>
        /// <param name="chPoint3">The third ConvexHullPoint</param>
        /// <returns>a value indicating how the provided points are oriented</returns>
        private static int Orientation(ConvexHullPoint chPoint1, ConvexHullPoint chPoint2, ConvexHullPoint chPoint3)
        {
            return Orientation(chPoint1.Point.X, chPoint1.Point.Y, chPoint2.Point.X, chPoint2.Point.Y, chPoint3.Point.X, chPoint3.Point.Y);
        }

        /// <summary>
        /// Determines the orientation of the provided points
        /// </summary>
        /// <param name="x1">The X value for the first point</param>
        /// <param name="y1">The Y value for the first point</param>
        /// <param name="x2">The X value for the second point</param>
        /// <param name="y2">The Y value for the second point</param>
        /// <param name="px">The X value for the third point</param>
        /// <param name="py">The Y value for the third point</param>
        /// <returns>a value indicating how the provided points are oriented</returns>
        private static int Orientation(double x1, double y1, double x2, double y2, double px, double py)
        {
            double orin = (x2 - x1) * (py - y1) - (px - x1) * (y2 - y1);

            if (orin < 0.0)
                return CLOCKWISE;  // Right-hand orientation
            else
                return COUNTER_CLOCKWISE;  // Left-hand orientation
        }

    }
}
