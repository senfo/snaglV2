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
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Windows;
    using Berico.SnagL.Infrastructure.Data.Mapping;
    using Berico.SnagL.Model;

    /// <summary>
    /// Lays out nodes in an hierarchical fashion
    /// </summary>
    [Export(typeof(LayoutBase))]
    public class TreeLayout : AsynchronousLayoutBase
    {
        // TODO:  MAKE THESE CONFIGURABLE
        private const double NODE_SPACING = 40D;
        private const double LAYER_SPACING = 60D;
        private const double TREE_SPACING = 50D;

        private double maxXPosition;
        private ISet<string> processedNodes = new HashSet<string>();
        private IList<IList<NodeMapData>> layers = new List<IList<NodeMapData>>();
        private IList<IList<Tuple<double, double>>> layerSpans = new List<IList<Tuple<double, double>>>();

        /// <summary>
        /// Gets or sets the custom root ranker
        /// </summary>
        public Func<NodeMapData, double> CustomRootRanker { get; set; }

        /// <summary>
        /// Gets a value that indicates whether or not the layout is enabled
        /// </summary>
        public override bool Enabled
        {
            get
            {
                throw new System.NotImplementedException();
            }
            protected set
            {
                throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// Get the name of the layout
        /// </summary>
        public override string LayoutName
        {
            get
            {
                return InternalLayouts.OriginalTree;
            }
        }

        /// <summary>
        /// Performs the actual layout algorithm.
        /// </summary>
        /// <param name="graph">The object containing the graph data</param>
        /// <param name="rootNode">Root node</param>
        protected override void PerformLayout(GraphMapData graph, INode rootNode)
        {
            NodeMapData nodeToProcess = null;
            if (rootNode != null)
            {
                nodeToProcess = graph.Nodes[rootNode.ID];
            }

            // Loop over the nodes until they have all been positioned
            while (processedNodes.Count != graph.Nodes.Count)
            {
                // Get the highest rank node that has not already been
                // positioned
                if (nodeToProcess == null)
                {
                    double maxRanking = double.MinValue;

                    // Loop over the nodes looking for the highest ranked one
                    foreach (NodeMapData node in graph.GetNodes())
                    {
                        double currentRanking = GetNodeRanking(graph, node);
                        if (!processedNodes.Contains(node.Id) && (nodeToProcess == null || maxRanking < currentRanking))
                        {
                            maxRanking = currentRanking;
                            nodeToProcess = node;
                        }
                    }
                }

                // Position the node
                SaveNodePosition(nodeToProcess, maxXPosition, 0D);

                processedNodes.Add(nodeToProcess.Id);
                AddToLayer(nodeToProcess, 0);

                // Layer the graph
                CalulateSubtreePosition(graph, nodeToProcess, 1);

                nodeToProcess = null;
            }

            // reset for next run
            maxXPosition = 0D;
            processedNodes.Clear();
            layers.Clear();
            layerSpans.Clear();
        }

        private void SaveNodePosition(NodeMapData node, double x, double y)
        {
            node.Position = new Point(x, y);
            maxXPosition = Math.Max(x + node.Dimension.Width, maxXPosition);
        }

        /// <summary>
        /// Determines the ranking of the provided node
        /// </summary>
        /// <param name="graph">The object containing the graph data</param>
        /// <param name="node">The node to get the rank for</param>
        /// <returns>the rank for this node</returns>
        private double GetNodeRanking(GraphMapData graph, NodeMapData node)
        {
            // Check if we have a custom ranker to use.  A custom ranker
            // is a custom delegate that is used to determine the nodes
            // rank.
            if (CustomRootRanker != null)
            {
                return CustomRootRanker(node);
            }
            else
            {
                // As a fallback, determine the nodes rank base on the
                // number of edges that it has
                return GetNumberOfEdges(graph, node);
            }
        }

        private static double GetNumberOfEdges(GraphMapData graph, NodeMapData node)
        {
            double numEdges = 0D;

            foreach (EdgeMapData edge in graph.GetEdges())
            {
                if (edge.Source.Equals(node.Id) || edge.Target.Equals(node.Id))
                {
                    numEdges++;
                }
            }

            return numEdges;
        }

        /// <summary>
        /// Adds the provided node view model to the specified layer
        /// </summary>
        /// <param name="node">The node view model to be added</param>
        /// <param name="currentLayer">The layer to add the view model too</param>
        private void AddToLayer(NodeMapData node, int currentLayer)
        {
            // Loop over all the layers up to the specified layer
            while (layers.Count <= currentLayer)
            {
                layers.Add(new List<NodeMapData>());
            }

            // Add the provided node view model to the layer
            layers[currentLayer].Add(node);
        }

        /// <summary>
        /// Adds the provided layer span (start and end point) to the
        /// specified layer
        /// </summary>
        /// <param name="layer">The layer to add the span too</param>
        /// <param name="startX"></param>
        /// <param name="endX"></param>
        private void AddSpanToLayer(int layer, double startX, double endX)
        {
            // Loop over all the layers up to the specified layer
            while (layerSpans.Count <= layer)
            {
                layerSpans.Add(new List<Tuple<double, double>>());
            }

            // Add the layer span to the specified layer
            layerSpans[layer].Add(Tuple.Create<double, double>(startX, endX));
        }

        private static ICollection<EdgeMapData> GetNodesEdges(GraphMapData graph, NodeMapData node)
        {
            ICollection<EdgeMapData> edges = new List<EdgeMapData>();

            foreach (EdgeMapData edge in graph.GetEdges())
            {
                if (edge.Source.Equals(node.Id) || edge.Target.Equals(node.Id))
                {
                    edges.Add(edge);
                }
            }

            return edges;
        }

        private NodeMapData GetOppositeNode(GraphMapData graph, EdgeMapData edge, NodeMapData node)
        {
            if (edge.Source.Equals(node.Id))
            {
                return graph.Nodes[edge.Target];
            }
            else
            {
                return graph.Nodes[edge.Source];
            }
        }

        /// <summary>
        /// Calulates the subtrees overall position
        /// </summary>
        /// <param name="graph">The object containing the graph data</param>
        /// <param name="node">The root node view model for this tree</param>
        /// <param name="layer">The layer (or row in the tree) being analyzed</param>
        private void CalulateSubtreePosition(GraphMapData graph, NodeMapData node, int layer)
        {
            IList<NodeMapData> children = new List<NodeMapData>();
            double totalWidth = 0D;

            //TODO:  MAYBE REVISE TO USE GET NEIGHBORS

            // Loop over the provided nodes edges in order to get its children
            foreach (EdgeMapData edge in GetNodesEdges(graph, node))
            {
                // TODO:  HANDLE EDGE VISIBILITY

                // Get the node at the opposite end of this edge
                NodeMapData childNode = GetOppositeNode(graph, edge, node);

                // Ensure that the node has not already been positioned
                if (childNode != null && !childNode.IsHidden && !processedNodes.Contains(childNode.Id))
                {
                    // Position the node
                    children.Add(childNode);
                    totalWidth += childNode.Dimension.Width;
                    processedNodes.Add(childNode.Id);
                }
            }

            // If there are no children, then we are done
            if (children.Count == 0)
            {
                return;
            }

            // Now we need to analyze and lay out all the children as a unit
            totalWidth += (children.Count - 1D) * NODE_SPACING;

            double currentX = node.Position.X - ((totalWidth - children[0].Dimension.Width) / 2D);
            double currentY = node.Position.Y + node.Dimension.Height + LAYER_SPACING;

            // Determine if the layers overlap
            double intersectAmount = IntersectsOnLayer(layer, currentX, currentX + totalWidth);

            // Check if we had an intersection
            if (intersectAmount != 0)
            {
                double pushAmount = intersectAmount / 2D;
                currentX -= pushAmount;

                // Give the nodes a little shove so they no longer
                // intersect with another subtree
                PushNodesOver(layer, node, pushAmount);

                // Make sure to push over the nodes in the root layer as well
            }

            AddSpanToLayer(layer, currentX, currentX + totalWidth);

            // Once we get here we have a good position, so we need to set it
            foreach (NodeMapData currentChildNode in children)
            {
                SaveNodePosition(currentChildNode, currentX, currentY);
                AddToLayer(currentChildNode, layer);

                currentX += currentChildNode.Dimension.Width + NODE_SPACING;
            }

            // Now we need to do the same thing for each
            // of the children node, recursively
            foreach (NodeMapData currentChildNode in children)
            {
                CalulateSubtreePosition(graph, currentChildNode, layer + 1);
            }
        }

        /// <summary>
        /// Determines if any subtrees overlap to any degree
        /// </summary>
        /// <param name="layer">The layer being analyzed</param>
        /// <param name="startX">The starting position of the subtree</param>
        /// <param name="endX">The width or ending position of the subtree</param>
        /// <returns>the amount of overlap</returns>
        private double IntersectsOnLayer(int layer, double startX, double endX)
        {
            // Check that we have multiple layers
            if (layerSpans.Count > layer)
            {
                // Iterate over each layer span and see if it inserects with
                // the span that starts and ends at the provided locations
                foreach (Tuple<double, double> currentLayerSpan in layerSpans[layer])
                {
                    if (startX <= currentLayerSpan.Item1 && endX >= currentLayerSpan.Item1)
                    {
                        return endX - currentLayerSpan.Item1;
                    }

                    if (startX <= currentLayerSpan.Item2 && endX >= currentLayerSpan.Item2)
                    {
                        return startX - currentLayerSpan.Item2;
                    }

                    if (startX <= currentLayerSpan.Item2 && endX >= currentLayerSpan.Item1)
                    {
                        return startX - currentLayerSpan.Item2;
                    }
                }
            }

            return 0D;
        }

        /// <summary>
        /// Pushes over all nodes in the specified layer by the specified amount
        /// </summary>
        /// <param name="layer">The current layer being analyzed</param>
        /// <param name="endNode">The last node view model to be processed</param>
        /// <param name="amount">The amount to shove the nodes over</param>
        private void PushNodesOver(int layer, NodeMapData endNode, double amount)
        {
            // Ensure that we have any layers
            if (layers.Count > layer)
            {
                // push the nodes over
                foreach (NodeMapData currentNode in layers[layer])
                {
                    if (currentNode.Id.Equals(endNode.Id))
                    {
                        break;
                    }

                    currentNode.Position = new Point(currentNode.Position.X + amount, currentNode.Position.Y);
                }

                // Continue pushing nodes over for each layer (recursively)
                PushNodesOver(layer + 1, endNode, amount);
            }
        }
    }
}
