//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

namespace Berico.SnagL.Infrastructure.Layouts
{
    using System.Collections.Generic;
    using System.Windows;
    using Berico.SnagL.Infrastructure.Data.Mapping;
    using GraphSharp;
    using GraphSharp.Algorithms.OverlapRemoval;
    using QuickGraph;

    /// <summary>
    /// Utility methods for interacting with Graph# objects
    /// </summary>
    public static class GraphSharpUtility
    {
        /// <summary>
        /// Get adjacency graph from input graph
        /// </summary>
        /// <param name="graph">GraphMapData</param>
        /// <returns>AdjacencyGraph</returns>
        public static AdjacencyGraph<string, Edge<string>> GetAdjacencyGraph(GraphMapData graph)
        {
            ICollection<NodeMapData> nodes = graph.GetNodes();
            AdjacencyGraph<string, Edge<string>> adjacencyGraph = new AdjacencyGraph<string, Edge<string>>(true, nodes.Count);

            foreach (NodeMapData node in nodes)
            {
                adjacencyGraph.AddVertex(node.Id);
            }

            foreach (EdgeMapData edge in graph.GetEdges())
            {
                Edge<string> quickGraphEdge = new Edge<string>(edge.Source, edge.Target);
                adjacencyGraph.AddEdge(quickGraphEdge);
            }

            return adjacencyGraph;
        }

        /// <summary>
        /// Get bidirectional graph from input graph
        /// </summary>
        /// <param name="graph">GraphMapData</param>
        /// <returns>BidirectionalGraph</returns>
        public static BidirectionalGraph<string, WeightedEdge<string>> GetBidirectionalGraph(GraphMapData graph)
        {
            ICollection<NodeMapData> nodes = graph.GetNodes();
            BidirectionalGraph<string, WeightedEdge<string>> bidirectionalGraph = new BidirectionalGraph<string, WeightedEdge<string>>(true, nodes.Count);

            foreach (NodeMapData node in nodes)
            {
                bidirectionalGraph.AddVertex(node.Id);
            }

            foreach (EdgeMapData edge in graph.GetEdges())
            {
                WeightedEdge<string> weightedEdge = new WeightedEdge<string>(edge.Source, edge.Target, edge.Weight);
                bidirectionalGraph.AddEdge(weightedEdge);
            }

            return bidirectionalGraph;
        }

        /// <summary>
        /// Removes node overlap occurring in the input graph
        /// </summary>
        /// <param name="graph">GraphMapData</param>
        public static void FSAOverlapRemoval(GraphMapData graph)
        {
            ICollection<NodeMapData> nodes = graph.GetNodes();
            IDictionary<string, Rect> rectangles = new Dictionary<string, Rect>(nodes.Count);

            foreach (NodeMapData node in nodes)
            {
                Point location = new Point(node.Position.X, node.Position.Y);
                Rect rect = new Rect(location, node.Dimension);
                rectangles[node.Id] = rect;
            }
            OverlapRemovalParameters overlapRemovalParameters = new OverlapRemovalParameters()
            {
                HorizontalGap = 0F,
                VerticalGap = 0F
            };

            FSAAlgorithm<string> overlapRemoval = new FSAAlgorithm<string>(rectangles, overlapRemovalParameters);
            overlapRemoval.Compute();

            foreach (NodeMapData node in nodes)
            {
                Rect rect = overlapRemoval.Rectangles[node.Id];
                Point pos = new Point(rect.X, rect.Y);
                node.Position = pos;
            }
        }

        /// <summary>
        /// Gets a mapping of node id to vector
        /// </summary>
        /// <param name="graph">GraphMapData</param>
        /// <returns>map of node id to vector</returns>
        public static IDictionary<string, Vector> GetNodePositions(GraphMapData graph)
        {
            ICollection<NodeMapData> nodes = graph.GetNodes();
            IDictionary<string, Vector> nodePositions = new Dictionary<string, Vector>(nodes.Count);

            foreach (NodeMapData node in nodes)
            {
                Vector vPos = new Vector(node.Position.X, node.Position.Y);
                nodePositions[node.Id] = vPos;
            }

            return nodePositions;
        }

        /// <summary>
        /// Updates the input graph node positions with those from the input mapping
        /// </summary>
        /// <param name="graph">GraphMapData</param>
        /// <param name="vertexPositions">map of node id to vector</param>
        public static void SetNodePositions(GraphMapData graph, IDictionary<string, Vector> vertexPositions)
        {
            ICollection<NodeMapData> nodes = graph.GetNodes();
            foreach (NodeMapData node in nodes)
            {
                Vector vPos = vertexPositions[node.Id];
                Point pPos = new Point(vPos.X, vPos.Y);
                node.Position = pPos;
            }
        }

        /// <summary>
        /// Gets a mapping of node id to size
        /// </summary>
        /// <param name="graph">GraphMapData</param>
        /// <returns>map of node id to size</returns>
        public static IDictionary<string, Size> GetNodeSizes(GraphMapData graph)
        {
            ICollection<NodeMapData> nodes = graph.GetNodes();
            IDictionary<string, Size> nodeSizes = new Dictionary<string, Size>(nodes.Count);

            foreach (NodeMapData node in nodes)
            {
                nodeSizes[node.Id] = node.Dimension;
            }

            return nodeSizes;
        }
    }
}
