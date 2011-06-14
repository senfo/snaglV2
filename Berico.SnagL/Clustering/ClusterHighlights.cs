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
using Berico.SnagL.Infrastructure.Data;
using Berico.SnagL.Infrastructure.Graph;
using Berico.SnagL.UI;

namespace Berico.SnagL.Infrastructure.Clustering
{
    /// <summary>
    /// 
    /// </summary>
    public class ClusterHighlights
    {
        private ICollection<Polygon> highlightPolygons;
        private Dictionary<PartitionNode, Polygon> clusterPolygons = new Dictionary<PartitionNode, Polygon>();
        private Cluster cluster;
        private string scope = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_scope"></param>
        public ClusterHighlights(string _scope)
        {
            scope = _scope;

            //TODO: HANDLE EVENTS
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ICollection<Polygon> HighlightClusters()
        {
            RemoveHighlights();

            cluster = new Cluster(GraphManager.Instance.GetGraphComponents(scope));
            cluster.EdgePredicate = delegate(Model.IEdge e) { return e is Model.SimilarityDataEdge; };

            GraphComponents clusteredGraph = cluster.GetClusteredGraph();
            foreach (PartitionNode pn in clusteredGraph.GetNodeViewModels())
            {
                if (pn.Nodes.Count > 1)
                {
                    Polygon highlightPolygon = CreateHighlightPolygon(pn);
                    clusterPolygons[pn] = highlightPolygon;
                }
            }

            highlightPolygons = clusterPolygons.Values;

            return highlightPolygons;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pn"></param>
        /// <returns></returns>
        private Polygon CreateHighlightPolygon(PartitionNode pn)
        {
            PointCollection convexHull = ConvertListOfPointsToPointCollection(cluster.CalculateConvexHullForCluster(pn));

            return DrawHighlightPolygon(convexHull);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RemoveHighlights()
        {
            if (highlightPolygons != null)
            {
                // Loop over each polygon
                foreach (Polygon highlight in highlightPolygons)
                {
                    // Remove the polygon from the graph surface
                    ViewModelLocator.GraphDataStatic.RemoveUIElementFromGraph(highlight);
                }

                highlightPolygons = null;
                clusterPolygons = new Dictionary<PartitionNode, Polygon>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="convexHull"></param>
        /// <returns></returns>
        private Polygon DrawHighlightPolygon(PointCollection convexHull)
        {
            Polygon highlightPolygon = new Polygon()
            {
                Stroke = new SolidColorBrush(Colors.Blue),
                StrokeLineJoin = PenLineJoin.Round,
                StrokeThickness = 3,
                Fill = new SolidColorBrush(Color.FromArgb(130, Colors.Blue.R, Colors.Blue.G, Colors.Blue.B)),
                Points = convexHull,
                IsHitTestVisible = false
            };

            highlightPolygon.SetValue(Canvas.ZIndexProperty, 99);

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
        private PointCollection ConvertListOfPointsToPointCollection(IEnumerable<Point> listOfPoints)
        {
            PointCollection pointCollection = new PointCollection();
            
            // Loop over the collection of Points and add each one
            // to the PointCollection
            foreach (Point currentPoint in listOfPoints)
                pointCollection.Add(currentPoint);

            return pointCollection;
        }
    }
}
