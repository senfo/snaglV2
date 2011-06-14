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
using Berico.Common;

namespace Berico.SnagL.Infrastructure.Clustering
{
    /// <summary>
    /// Represents a point, angle and anchor within a collection 
    /// of points, used to assist in calculating the convex hull
    /// </summary>
    public class ConvexHullPoint : IComparable<ConvexHullPoint>
    {
        private Point point;
        private double angle;
        private ConvexHullPoint anchor;

        /// <summary>
        /// Instantiates a new instance of Berico.Snagl.Infrastructure.Clustering.ConvexHUllPoint
        /// using the provided parameters
        /// </summary>
        /// <param name="_point">The coordinates for this point</param>
        /// <param name="_angle">The angle of this point</param>
        /// <param name="_anchor">The anchor for this point</param>
        public ConvexHullPoint(Point _point, double _angle, ConvexHullPoint _anchor)
        {
            point = _point;
            angle = _angle;
            anchor = _anchor;
        }

        /// <summary>
        /// Gets the coordinates (X and Y) for this point
        /// </summary>
        public Point Point
        { 
            get { return this.point; }
        }

        /// <summary>
        /// Gets the angle for this point
        /// </summary>
        public double Angle
        {
            get { return this.angle; } 
        }

        /// <summary>
        /// Gets the anchor for this point
        /// </summary>
        public ConvexHullPoint Anchor
        {
            get { return this.anchor; } 
        }


        #region IComparable<ConvexHullPoint> Members

            /// <summary>
            /// Compares the provided ConvexHullPoint with the current instance
            /// </summary>
            /// <param name="comparisonPoint">The point to be used for comparison</param>
            /// <returns>a value indicating the relative order of the compared
            /// objects</returns>
            public int CompareTo(ConvexHullPoint comparisonPoint)
            {
                int angleComparison = this.angle.CompareTo(comparisonPoint.Angle);

                if (angleComparison == 0 || this.anchor == null)
                {
                    Point anchorPoint = this.anchor.Point;

                    double sourceDistance = this.Point.Distance(anchorPoint);
                    double targetDistance = comparisonPoint.Point.Distance(anchorPoint);

                    return sourceDistance.CompareTo(targetDistance);
                }
                else
                    return angleComparison;
            }

        #endregion
    }
}
