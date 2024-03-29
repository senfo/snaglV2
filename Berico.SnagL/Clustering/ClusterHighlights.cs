﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Berico.SnagL.Infrastructure.Data;
using Berico.SnagL.Infrastructure.Graph;
using Berico.SnagL.UI;

namespace Berico.SnagL.Infrastructure.Clustering
{
    /// <summary>
    /// Contains the logic necessary for generating cluster highlights
    /// </summary>
    public class ClusterHighlights
    {
        #region Fields

        /// <summary>
        /// Stores a collection of highlights on the graph
        /// </summary>
        private ICollection<Polygon> _highlightPolygons;

        /// <summary>
        /// Stores the polygons with the partition node it represents a highlight for
        /// </summary>
        private Dictionary<Polygon, PartitionNode> _clusterPolygons = new Dictionary<Polygon, PartitionNode>();

        /// <summary>
        /// The cluster performed on the graph
        /// </summary>
        private Cluster _cluster;

        /// <summary>
        /// Indicates whether or not the mouse is captured by a click event handler
        /// </summary>
        private bool _mouseCaptured;

        /// <summary>
        /// Stores the vertical position of the mouse for dragging and dropping
        /// </summary>
        private double _mouseVerticalPosition;

        /// <summary>
        /// Stores the horizontal position of the mouse for dragging and dropping
        /// </summary>
        private double _mouseHorizontalPosition;

        /// <summary>
        /// Stores the vertical position of the mouse when the drag stars
        /// </summary>
        private double _startMouseVerticalPosition;

        /// <summary>
        /// Stores the horizontal position of the mouse when the drag stars
        /// </summary>
        private double _startMouseHorizontalPosition;

        /// <summary>
        /// Unique identifier for the scope we're working with
        /// </summary>
        private readonly string _scope;

        /// <summary>
        /// Used to indicate the last used color
        /// </summary>
        private static int _colorIndex;

        /// <summary>
        /// Provide the colors used to individually destinquish clusters
        /// </summary>
        private static readonly List<SolidColorBrush> ClusterColors = new List<SolidColorBrush>
        {
            new SolidColorBrush(Color.FromArgb(130, Colors.Blue.R, Colors.Blue.G, Colors.Blue.B)),
            new SolidColorBrush(Color.FromArgb(130, Colors.Green.R, Colors.Green.G, Colors.Green.B)),
            new SolidColorBrush(Color.FromArgb(130, Colors.Purple.R, Colors.Purple.G, Colors.Purple.B)),
            new SolidColorBrush(Color.FromArgb(130, Colors.Orange.R, Colors.Orange.G, Colors.Orange.B)),
            new SolidColorBrush(Color.FromArgb(130, Colors.Red.R, Colors.Red.G, Colors.Red.B)),
            new SolidColorBrush(Color.FromArgb(130, Colors.Magenta.R, Colors.Magenta.G, Colors.Magenta.B)),
            new SolidColorBrush(Color.FromArgb(130, Colors.Gray.R, Colors.Gray.G, Colors.Gray.B)),
            new SolidColorBrush(Color.FromArgb(130, Colors.Brown.R, Colors.Brown.G, Colors.Brown.B)),
        };

        #endregion

        #region Constructors

        /// <summary>
        /// Prevents an instance of the ClusterHighlights class from being instantiated
        /// </summary>
        private ClusterHighlights()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the ClusterHighlights class
        /// </summary>
        /// <param name="scope">Scope for which to apply clustering highlights to</param>
        public ClusterHighlights(string scope)
        {
            _scope = scope;

            //TODO: HANDLE EVENTS
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a highlight to the graph to indicate which data is clustered together
        /// </summary>
        /// <returns>A collection of polygron of clustered highlights on the graph</returns>
        public ICollection<Polygon> HighlightClusters()
        {
            RemoveHighlights();

            _cluster = new Cluster(GraphManager.Instance.GetGraphComponents(_scope));
            _cluster.EdgePredicate = delegate(Model.IEdge e) { return e is Model.SimilarityDataEdge; };

            GraphComponents clusteredGraph = _cluster.GetClusteredGraph();
            foreach (PartitionNode pn in clusteredGraph.GetNodeViewModels())
            {
                if (pn.Nodes.Count > 1)
                {
                    Polygon highlightPolygon = CreateHighlightPolygon(pn);
                    _clusterPolygons[highlightPolygon] = pn;
                }
            }
            clusteredGraph.Dispose();

            _highlightPolygons = _clusterPolygons.Keys;

            return _highlightPolygons;
        }

        /// <summary>
        /// Removes the highlights from the graph
        /// </summary>
        public void RemoveHighlights()
        {
            if (_highlightPolygons != null)
            {
                // Loop over each polygon
                foreach (Polygon highlight in _highlightPolygons)
                {
                    // Remove the polygon from the graph surface
                    ViewModelLocator.GraphDataStatic.RemoveUIElementFromGraph(highlight);
                    
                    // Unhook events
                    highlight.MouseEnter -= new MouseEventHandler(OnHighlightPolygonMouseEnter);
                    highlight.MouseLeftButtonDown -= new MouseButtonEventHandler(OnHighlightPolygonMouseLeftButtonDown);
                    highlight.MouseMove -= new MouseEventHandler(OnHighlightPolygonMouseMove);
                    highlight.MouseLeave -= new MouseEventHandler(OnHighlightPolygonMouseLeave);
                    highlight.MouseLeftButtonUp -= new MouseButtonEventHandler(OnHighlightPolygonMouseLeftButtonUp);
                    highlight.MouseRightButtonDown -= new MouseButtonEventHandler(OnPolygonMouseRightButtonDown);
                }

                _highlightPolygons = null;
                _clusterPolygons = new Dictionary<Polygon, PartitionNode>();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates the polygons that represent the highlights for clustered data
        /// </summary>
        /// <param name="pn">The partition node containing nodes in the cluster</param>
        /// <returns>The polygon created for the highlight</returns>
        private Polygon CreateHighlightPolygon(PartitionNode pn)
        {
            PointCollection convexHull = ConvertListOfPointsToPointCollection(_cluster.CalculateConvexHullForCluster(pn));

            return DrawHighlightPolygon(convexHull);
        }

        /// <summary>
        /// Draws and adds the actual highlight polygon to the graph surface
        /// </summary>
        /// <param name="convexHull">A collection of points representing node positions on the graph</param>
        /// <returns>The polygon created for the highlight</returns>
        private Polygon DrawHighlightPolygon(PointCollection convexHull)
        {
            Polygon highlightPolygon = new Polygon
            {
                AllowDrop = true,
                Stroke = ClusterColors[_colorIndex],
                StrokeLineJoin = PenLineJoin.Round,
                StrokeThickness = 3,
                Fill = ClusterColors[_colorIndex],
                Points = convexHull
            };

            _colorIndex = _colorIndex == ClusterColors.Count - 1 ? 0 : _colorIndex + 1;
            highlightPolygon.SetValue(Canvas.ZIndexProperty, 0);

            highlightPolygon.MouseEnter += new MouseEventHandler(OnHighlightPolygonMouseEnter);
            highlightPolygon.MouseLeftButtonDown += new MouseButtonEventHandler(OnHighlightPolygonMouseLeftButtonDown);
            highlightPolygon.MouseMove += new MouseEventHandler(OnHighlightPolygonMouseMove);
            highlightPolygon.MouseLeave += new MouseEventHandler(OnHighlightPolygonMouseLeave);
            highlightPolygon.MouseLeftButtonUp += new MouseButtonEventHandler(OnHighlightPolygonMouseLeftButtonUp);
            highlightPolygon.MouseRightButtonDown += new MouseButtonEventHandler(OnPolygonMouseRightButtonDown);

            // Add the polygon to the graph surface
            ViewModelLocator.GraphDataStatic.AddUIElementToGraph(highlightPolygon);

            return highlightPolygon;
        }

        /// <summary>
        /// Converts the provided list of Point values to a PointCollection
        /// </summary>
        /// <param name="listOfPoints">A collection of points</param>
        /// <returns>a PointCollection containing all the point values
        /// in the provided list</returns>
        private static PointCollection ConvertListOfPointsToPointCollection(IEnumerable<Point> listOfPoints)
        {
            PointCollection pointCollection = new PointCollection();
            
            // Loop over the collection of Points and add each one
            // to the PointCollection
            foreach (Point currentPoint in listOfPoints)
                pointCollection.Add(currentPoint);

            return pointCollection;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the Mouse Enter event for the highlight polygon
        /// </summary>
        /// <param name="sender">The polygon that raised the event</param>
        /// <param name="e">Specifies properties about the mouse click</param>
        private void OnHighlightPolygonMouseEnter(object sender, MouseEventArgs e)
        {
            Polygon polygon = (Polygon)sender;
            PartitionNode partitionNode = _clusterPolygons[polygon];
            List<string> existingValues = new List<string>();
            List<KeyValuePair<string, string>> attributes = new List<KeyValuePair<string, string>>();
            IEnumerable<AttributeSimilarityDescriptor> descriptors = AttributeSimilarityManager.Instance.SimilarityDescriptors;

            if (!_mouseCaptured)
            {
                foreach (var descriptor in descriptors)
                foreach (var node in partitionNode.Nodes)
                {
                    var nodeAttributes = from a in node.ParentNode.Attributes
                                         where a.Key == descriptor.AttributeName
                                         select a;

                    foreach (var attribute in nodeAttributes)
                    {
                        if (!existingValues.Contains(attribute.Value.Value))
                        {
                            attributes.Add(new KeyValuePair<string, string>(attribute.Key, attribute.Value.Value));
                            existingValues.Add(attribute.Value.Value);
                        }
                    }
                }
            }

            // Show the popup
            ViewModelLocator.AttributePopupVMStatic.Show(attributes);
        }

        /// <summary>
        /// Handles the Mouse Enter event for the highlight polygon
        /// </summary>
        /// <param name="sender">The polygon that raised the event</param>
        /// <param name="e">Specifies properties about the mouse click</param>
        private static void OnHighlightPolygonMouseLeave(object sender, MouseEventArgs e)
        {
            ViewModelLocator.AttributePopupVMStatic.Close();
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event for dragging and dropping of the highlight polygon
        /// </summary>
        /// <param name="sender">The polygon that raised the event</param>
        /// <param name="e">Specifies properties about the mouse click</param>
        private void OnHighlightPolygonMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Polygon polygon = (Polygon)sender;

            // Make sure we don't pan the graph
            e.Handled = true;

            _mouseCaptured = true;
            _mouseHorizontalPosition = e.GetPosition(polygon).X;
            _mouseVerticalPosition = e.GetPosition(polygon).Y;
            _startMouseHorizontalPosition = (double)polygon.GetValue(Canvas.LeftProperty);
            _startMouseVerticalPosition = (double)polygon.GetValue(Canvas.TopProperty);

            polygon.CaptureMouse();
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event for dragging and dropping of the highlight polygon
        /// </summary>
        /// <param name="sender">The polygon that raised the event</param>
        /// <param name="e">Specifies properties about the mouse click</param>
        private void OnHighlightPolygonMouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseCaptured)
            {
                Polygon polygon = (Polygon)sender;

                // Determine the current position of the highlight
                double horizontalDelta = (e.GetPosition(polygon).X - _mouseHorizontalPosition);
                double verticleDelta = (e.GetPosition(polygon).Y - _mouseVerticalPosition);
                double newLeft = horizontalDelta + (double)polygon.GetValue(Canvas.LeftProperty);
                double newTop = verticleDelta + (double)polygon.GetValue(Canvas.TopProperty);

                // Update polygons position
                polygon.SetValue(Canvas.TopProperty, newTop);
                polygon.SetValue(Canvas.LeftProperty, newLeft);

                // Update the global position variables
                _mouseHorizontalPosition = e.GetPosition(polygon).X;
                _mouseVerticalPosition = e.GetPosition(polygon).Y;
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event for dragging and dropping of the highlight polygon
        /// </summary>
        /// <param name="sender">The polygon that raised the event</param>
        /// <param name="e">Specifies properties about the mouse click</param>
        private void OnHighlightPolygonMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Polygon polygon = (Polygon)sender;
            PartitionNode partitionNode = _clusterPolygons[polygon];
            double horizontalelta = _startMouseHorizontalPosition - (double)polygon.GetValue(Canvas.LeftProperty);
            double verticleDelta = _startMouseVerticalPosition - (double)polygon.GetValue(Canvas.TopProperty);

            // Update node positions
            foreach (NodeViewModelBase node in partitionNode.Nodes)
            {
                double x = node.Position.X;
                double y = node.Position.Y;

                x -= horizontalelta;
                y -= verticleDelta;

                node.Position = new Point(x, y);
            }

            _mouseCaptured = false;

            polygon.ReleaseMouseCapture();
        }

        /// <summary>
        /// Handles the Right Button Down event for the cluster highlight polygon
        /// </summary>
        /// <param name="sender">The polygon that raised the event</param>
        /// <param name="e">Specifies properties about the mouse click</param>
        private void OnPolygonMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_mouseCaptured)
            {
                e.Handled = true;
            }
        }

        #endregion
    }
}
