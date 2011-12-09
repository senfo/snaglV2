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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Threading;
using System.Windows.Data;

namespace Berico.SnagL.UI
{
    /// <summary>
    /// Represents the physical edge drawn on the graph.
    /// Typically, this would be handled by a UserControl
    /// and XAML but a bug is forcing us to do it manually
    /// in code behind.
    /// </summary>
    public class EdgeLine : Canvas, Berico.SnagL.UI.IEdgeLine
    {

        private readonly double LABEL_OFFSET = 50;
        private Line line = new Line();
        //private Path line = new Path();
        private LineGeometry lineGeometry = new LineGeometry();
        private TextBlock label = new TextBlock();
        private Border labelContainer = new Border();
        private Polygon arrow = new Polygon();
        private Model.EdgeType edgeType = Model.EdgeType.Undirected;
        private bool isVisible = true;

        /// <summary>
        /// Creates a new instance of the EdgeLine class using
        /// the provided edge type
        /// </summary>
        /// <param name="_edgeType"></param>
        public EdgeLine(Model.EdgeType _edgeType) : this(_edgeType, true) { }

        /// <summary>
        /// Creates a new instance of the EdgeLine class using
        /// the provided edge type and visibility setting
        /// </summary>
        public EdgeLine(Model.EdgeType _edgeType, bool _isVisible)
        {
            edgeType = _edgeType;
            IsVisible = _isVisible;

            // Draw the edge line
            Draw();

            this.line.MouseEnter += new MouseEventHandler(line_MouseEnter);
            this.line.MouseLeave += new MouseEventHandler(line_MouseLeave);
        }

        #region Events and Event Handlers

            #region IEdgeLine Members

                /// <summary>
                /// 
                /// </summary>
                public event EventHandler<MouseEventArgs> LineMouseEnter;

                /// <summary>
                /// 
                /// </summary>
                public event EventHandler<MouseEventArgs> LineMouseLeave;

            #endregion

            /// <summary>
            /// 
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void line_MouseLeave(object sender, MouseEventArgs e)
            {
                EventHandler<MouseEventArgs> handler = this.LineMouseLeave;
                if (handler != null)
                {
                    handler(this, e);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void line_MouseEnter(object sender, MouseEventArgs e)
            {
                EventHandler<MouseEventArgs> handler = this.LineMouseEnter;
                if (handler != null)
                {
                    handler(this, e);
                }
            }

        #endregion

        #region Properties

            /// <summary>
            /// Gets or sets the opacity of the edgeline
            /// </summary>
            public double Opacity
            {
                get { return this.line.Opacity; }
                set { this.line.Opacity = value; }
            }

            /// <summary>
            /// Gets or sets the thickness of the edge
            /// </summary>
            public double Thickness
            {
                get { return this.line.StrokeThickness; }
                set
                {
                    this.line.StrokeThickness = value; 

                    // Update arrow
                    arrow.StrokeThickness = value;
                }
            }

            /// <summary>
            /// Gets or sets the color of the edge
            /// </summary>
            public Brush Color
            {
                get { return this.line.Stroke; }
                set
                {
                    this.line.Stroke = value; 

                    // Update arrow
                    arrow.Stroke = value;
                }
            }

            /// <summary>
            /// Gets or sets the background color of the label
            /// </summary>
            public Brush LabelBackgroundColor
            {
                get { return this.labelContainer.Background; }
                set { this.labelContainer.Background = value; }
            }

            /// <summary>
            /// Gets or sets the foreground color of the label
            /// </summary>
            public Brush LabelForegroundColor
            {
                get { return this.label.Foreground; }
                set { this.label.Foreground = value; }
            }

            /// <summary>
            /// Gets or sets the font style for the label
            /// </summary>
            public FontStyle LabelFontStyle
            {
                get { return this.label.FontStyle; }
                set { this.label.FontStyle = value; }
            }

            /// <summary>
            /// Gets or sets the font weight for the label
            /// </summary>
            public FontWeight LabelFontWeight
            {
                get { return this.label.FontWeight; }
                set { this.label.FontWeight = value; }
            }

            /// <summary>
            /// Gets or sets the font for the label
            /// </summary>
            public FontFamily LabelFont
            {
                get { return this.label.FontFamily; }
                set { this.label.FontFamily = value; }
            }

            /// <summary>
            /// Gets or sets whether the text on the label is underlined
            /// </summary>
            public bool LabelTextUnderline
            {
                get { return this.label.TextDecorations == TextDecorations.Underline ? true : false; }
                set { this.label.TextDecorations = (value == true ? TextDecorations.Underline : null); }
            }
           
            /// <summary>
            /// Gets or sets the dash array used by the edge.  A dash array
            /// is used to change the appearance of the edge.
            /// </summary>
            public DoubleCollection StrokeDashArray
            {
                get { return this.line.StrokeDashArray; }
                set { this.line.StrokeDashArray = value; }
            }

            //private Point startLocation = new Point(0, 0);
            //public Point StartLocation
            //{
            //    get { return this.lineGeometry.StartPoint; }
            //    set { DispatcherHelper.UIDispatcher.BeginInvoke(delegate() { this.lineGeometry.StartPoint = value; RepositionElements(); }); }
            //}

            //private Point startLocation = new Point(0, 0);
            //public Point StartLocation
            //{
            //    get { return this.lineGeometry.StartPoint; }
            //    set { DispatcherHelper.UIDispatcher.BeginInvoke(delegate() { this.lineGeometry.StartPoint = value; RepositionElements(); }); }
            //}

            /// <summary>
            /// Gets or sets the starting X coordinate  of the edge
            /// </summary>
            public double X1
            {
                get { return this.line.X1; }
                set { DispatcherHelper.UIDispatcher.BeginInvoke(delegate() { this.line.X1 = value; RepositionElements(); }); }
            }

            /// <summary>
            /// Gets or sets the starting Y coordinate of the edge
            /// </summary>
            public double Y1
            {
                get { return this.line.Y1; }
                set { DispatcherHelper.UIDispatcher.BeginInvoke(delegate() { this.line.Y1 = value; RepositionElements(); }); }
            }

            /// <summary>
            /// Gets or sets the ending X coordinate of the edge
            /// </summary>
            public double X2
            {
                get { return this.line.X2; }
                set { DispatcherHelper.UIDispatcher.BeginInvoke(delegate() { this.line.X2 = value; RepositionElements(); }); }
            }

            /// <summary>
            /// Gets or sets the ending Y coordinate of the edge
            /// </summary>
            public double Y2
            {
                get { return this.line.Y2; }
                set { DispatcherHelper.UIDispatcher.BeginInvoke(delegate() { this.line.Y2 = value; RepositionElements(); }); }
            }

            /// <summary>
            /// Gets or sets the actual value of the label
            /// </summary>
            public string Text
            {
                set
                { 
                    this.label.Text = value;

                    if (string.IsNullOrEmpty(value))
                        this.labelContainer.Visibility = Visibility.Collapsed;
                    else
                        this.labelContainer.Visibility = Visibility.Visible;
                } 
            }

            /// <summary>
            /// Gets or sets whether the EdgeLine is visible
            /// </summary>
            public bool IsVisible
            {
                get { return this.Visibility == Visibility.Visible ? true : false; }
                set { this.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
            }

        #endregion

        private void Draw()
        {
            // Ensure that we are starting with a clean slate
            this.Children.Clear();

            // Configure the label
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.TextAlignment = TextAlignment.Center;
            label.TextWrapping = TextWrapping.Wrap;
            label.MaxWidth = 150;
            label.Foreground = new SolidColorBrush(Colors.Black);

            // Label Container
            labelContainer.HorizontalAlignment = HorizontalAlignment.Center;
            labelContainer.Background = new SolidColorBrush(Colors.White);
            labelContainer.Child = label;
            labelContainer.Visibility = Visibility.Collapsed;
            labelContainer.Margin = new Thickness(0, 11, 0, 0);

            // Setup the arrow if our edge is directed
            if (edgeType == Model.EdgeType.Directed)
            {
                arrow.Fill = new SolidColorBrush(Colors.Black);
                arrow.Stroke = new SolidColorBrush(Colors.Black);
                arrow.Points = new PointCollection()
                {
                    new Point(-6/2, 2),
                    new Point(6/2, 2),
                    new Point(0, 6*1.4 + 2)
                };
            }

            // Position the elements appropriately
            RepositionElements();

            // Add elements to the root canvas
            this.Children.Add(this.line);
            this.Children.Add(this.labelContainer);

            if (edgeType == Model.EdgeType.Directed)
                this.Children.Add(this.arrow);
        }

        /// <summary>
        /// Repositions the elements (label and arrow) on the edge
        /// </summary>
        private void RepositionElements()
        {
            double angle = ComputeAngle();

            if (angle > 360)
                angle = angle - 360;

            PositionElement(labelContainer, LineCenter(), (angle >= 0 && angle <= 180) ? angle - 90 : angle + 90);

            if (edgeType == Model.EdgeType.Directed)
                PositionElement(arrow, LineCenter(15), angle);
        }

        /// <summary>
        /// Positions the given element on the edge
        /// </summary>
        /// <param name="element">The element to be positioned</param>
        private void PositionElement(FrameworkElement element, Point position, double angle)
        {
            // Position the element on it's canvas
            Canvas.SetLeft(element, position.X);
            Canvas.SetTop(element, position.Y);

            // Rotate the element appropriately
            element.RenderTransform = new RotateTransform() { Angle = angle };
        }

        /// <summary>
        /// Determines the center point of the actual line
        /// </summary>
        /// <returns>a Point instance representing the center of the line</returns>
        private Point LineCenter()
        {
            return LineCenter(0);
        }

        /// <summary>
        /// Determines the center point of the actual line
        /// </summary>
        /// <param name="offset">An offset to use when determing the center of the line</param>
        /// <returns>a Point instance representing the center of the line (with offset)</returns>
        private Point LineCenter(double offset)
        {
            Point centerPoint = new Point(0,0);

            double theta = double.NaN;

            // Calulate the lines center point
            double centerX = (this.line.X1 + this.line.X2) / 2;
            double centerY = (this.line.Y1 + this.line.Y2) / 2;

            // Check if an offset was specified
            if (offset > 0)
            {
                // Calulate the angle for the line
                theta = Math.Atan2(this.line.X2 - this.line.X1, this.line.Y2 - this.line.Y1);
            }

            // If a good angle was calculated, use it and the offset
            if (!double.IsNaN(theta))
                centerPoint = new Point(offset * Math.Sin(theta) + centerX, offset * Math.Cos(theta) + centerY);
            else
                centerPoint = new Point(centerX, centerY);

            return centerPoint;
        }

        /// <summary>
        /// Computes the angle of the line (from source
        /// to target)
        /// </summary>
        /// <returns>The edge lines angle</returns>
        private double ComputeAngle()
        {
            return 360 - (Math.Atan2(this.line.X2 - this.line.X1, this.line.Y2 - this.line.Y1) * 180 / Math.PI);
        }


    }
}