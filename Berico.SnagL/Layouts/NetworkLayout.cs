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
    using System.Linq;
    using System.Windows;
    using Berico.SnagL.Infrastructure.Clustering;
    using Berico.SnagL.Infrastructure.Data;
    using Berico.SnagL.Infrastructure.Data.Mapping;
    using Berico.SnagL.Infrastructure.Graph;
    using Berico.SnagL.Model;

    /// <summary>
    /// Represents a more organized ForceDirected layout.  The graph is broken
    /// into partitions (groups of connected nodes).  Each partition is then
    /// broken into clusters (based on similarity clustering).  Each cluster
    /// is laid out using a Grid layout then each partition is laid out using
    /// the regular force directed layout.  Finally, the entire graph is laid
    /// out using a Grid.
    /// </summary>
    [Export(typeof(LayoutBase))]
    public class NetworkLayout : AsynchronousLayoutBase
    {
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
                return InternalLayouts.Network;
            }
        }

        /// <summary>
        /// Performs the actual layout algorithm.
        /// </summary>
        /// <param name="graph">The object containing the graph data</param>
        /// <param name="rootNode">Root node</param>
        protected override void PerformLayout(GraphMapData graph, INode rootNode)
        {
            GraphComponents graphComponents = GraphManager.Instance.DefaultGraphComponentsInstance;
            ICollection<ICollection<GraphMapData>> subGraphs = ComputeLayout(graphComponents);

            foreach (ICollection<GraphMapData> subGraph in subGraphs)
            {
                foreach (GraphMapData sGraph in subGraph)
                {
                    ICollection<NodeMapData> subGraphNodes = sGraph.GetNodes();
                    foreach (NodeMapData node in subGraphNodes)
                    {
                        graph.Nodes[node.Id].Position = node.Position;
                    }
                }
            }

            //GraphSharpUtility.FSAOverlapRemoval(graph);
        }

        /// <summary>
        /// Performs the actual layout algorithm. This will execute on a background thread.
        /// </summary>
        /// <param name="graphComponents">The object containing the graph data</param>
        public ICollection<ICollection<GraphMapData>> ComputeLayout(GraphComponents graphComponents)
        {
            // Create a GraphComponents instance that is a partitioned
            // representation of the original graph.  Each node in this
            // graph is a partition node.
            GraphComponents connectedComponents = GraphManager.Instance.GetConnectedComponents(graphComponents.Scope);
            IEnumerable<INodeShape> partitionedGraphs = connectedComponents.GetNodeViewModels();

            IDictionary<string, ICollection<GraphMapData>> subGraphToNodeMap = new Dictionary<string, ICollection<GraphMapData>>();
            foreach (PartitionNode partitionedGraph in partitionedGraphs)
            {
                GraphComponents partitionedGraphComponents = partitionedGraph.GetGraph();
                ICollection<GraphMapData> partitionedGraphMapData = LayoutByClusters(partitionedGraphComponents);
                RecalculateDimension(partitionedGraph, partitionedGraphMapData);

                subGraphToNodeMap[partitionedGraph.ID] = partitionedGraphMapData;
            }

            // Layout the overall graph
            GraphMapData clusteredGraphMapData = GetGraph(connectedComponents);
            IDictionary<string, Point> originalPositions = GetOriginalPositions(clusteredGraphMapData);
            GridLayout gridLayout = new GridLayout();
            gridLayout.CalculateLayout(clusteredGraphMapData);
            ApplyOffsetToSubGraphs(originalPositions, clusteredGraphMapData, subGraphToNodeMap);

            return subGraphToNodeMap.Values;
        }

        // TODO new method to get Clusters, then the layout would be called

        /// <summary>
        /// Clusters and lays out the provided graph
        /// </summary>
        /// <param name="partitionedGraphComponents">The graph that needs to be clustered and layed out</param>
        private ICollection<GraphMapData> LayoutByClusters(GraphComponents partitionedGraphComponents)
        {
            // Create a Cluster instance for the graph
            Cluster clusterer = new Cluster(partitionedGraphComponents);

            // Specify the predicate to be used by the Cluster class.  In this case
            // we are determining clusters based on edges createdf during similarity
            // clustering.
            clusterer.EdgePredicate = delegate(IEdge edge)
            {
                bool isSimilarityDataEdge = edge is SimilarityDataEdge;
                return isSimilarityDataEdge;
            };

            // Create the clusters and return a new graph.  Each node on the
            // graph will be represented as a PartitionNode
            GraphComponents clusteredGraphComponents = clusterer.GetClusteredGraph();

            bool isAttributeLayout = true;
            // If there is no different between the initial grpah that was provided and
            // out clustered graph, we didn't really find clusters (most likely because
            // similarity clustering was not performed).
            int clusteredNodesCount = clusteredGraphComponents.GetNodeViewModels().Count();
            int partitionedNodesCount = partitionedGraphComponents.GetNodeViewModels().Count();
            if (clusteredNodesCount == partitionedNodesCount)
            {
                // TODO handle this better than just re-running

                // Rerun clustering without a predicate.  This means that clusters will
                // be based on regular edges.
                clusterer = new Cluster(partitionedGraphComponents);
                clusteredGraphComponents = clusterer.GetClusteredGraph();

                isAttributeLayout = false;
            }

            // Get all the nodes that are in the clustered graph.  Remember that these 
            // are partition nodes.
            IEnumerable<INodeShape> clusteredNodes = clusteredGraphComponents.GetNodeViewModels();
            ICollection<GraphMapData> clusteredGraphMapDatas = new List<GraphMapData>(clusteredNodes.Count());
            IDictionary<string, GraphMapData> subGraphToNodeMap = new Dictionary<string, GraphMapData>();
            foreach (PartitionNode clusteredNode in clusteredNodes)
            {
                // Create an appropriate layout to use for this cluster
                AsynchronousLayoutBase clusterLayout = GetClusterLayout(isAttributeLayout);
                GraphComponents clusteredNodeGraphComponents = clusteredNode.GetGraph();
                GraphMapData clusteredNodeGraphMapData = GraphComponentsUtility.GetGraph(clusteredNodeGraphComponents);
                clusteredGraphMapDatas.Add(clusteredNodeGraphMapData);
                subGraphToNodeMap[clusteredNode.ID] = clusteredNodeGraphMapData;

                // Run the layout.  This is laying out the individual cluster itself
                clusterLayout.CalculateLayout(clusteredNodeGraphMapData);

                // Recalculate the partion nodes dimensions
                RecalculateDimension(clusteredNode, new GraphMapData[] { clusteredNodeGraphMapData });
            }

            // Now we need to layout the entired clustered graph so it looks
            // more organized
            GraphMapData clusteredGraphMapData = GetGraph(clusteredGraphComponents);
            IDictionary<string, Point> originalPositions = GetOriginalPositions(clusteredGraphMapData);
            FRLayout frLayout = new FRLayout();
            frLayout.CalculateLayout(clusteredGraphMapData);
            ApplyOffsetToSubGraphs(originalPositions, clusteredGraphMapData, subGraphToNodeMap);

            return clusteredGraphMapDatas;
        }

        /// <summary>
        /// Instantiates an appropriate layout instance
        /// </summary>
        /// <param name="isAttributeCluster">Indicates whether we have attribute similarity clusters or not</param>
        /// <returns>a new, appropriate, LayoutBase instance</returns>
        private static AsynchronousLayoutBase GetClusterLayout(bool isAttributeCluster)
        {
            AsynchronousLayoutBase clusterLayout;

            // Check if our clusters are based on attribute similarity
            if (isAttributeCluster)
            {
                clusterLayout = new GridLayout();
            }
            else
            {
                clusterLayout = new FRLayout(false);
            }

            return clusterLayout;
        }

        private static void RecalculateDimension(PartitionNode subGraph, ICollection<GraphMapData> graphMapDatas)
        {
            Point topLeft = new Point(double.MaxValue, double.MaxValue);
            Point bottomRight = new Point(double.MinValue, double.MinValue);

            // Loop through all node view models to get the bounding area
            foreach (GraphMapData graphMapData in graphMapDatas)
            {
                ICollection<NodeMapData> nodeMapDatas = graphMapData.GetNodes();
                foreach (NodeMapData nodeMapData in nodeMapDatas)
                {
                    topLeft.X = Math.Min(topLeft.X, nodeMapData.Position.X - nodeMapData.Dimension.Width / 2D);
                    topLeft.Y = Math.Min(topLeft.Y, nodeMapData.Position.Y - nodeMapData.Dimension.Height / 2D);

                    bottomRight.X = Math.Max(bottomRight.X, nodeMapData.Position.X + nodeMapData.Dimension.Width / 2D);
                    bottomRight.Y = Math.Max(bottomRight.Y, nodeMapData.Position.Y + nodeMapData.Dimension.Height / 2D);
                }
            }

            Size dimensions = new Size();

            // Set the new dimensions based on the calculation performed
            dimensions.Width = Math.Max(bottomRight.X - topLeft.X, 1D);
            dimensions.Height = Math.Max(bottomRight.Y - topLeft.Y, 1D);

            subGraph.Width = dimensions.Width;
            subGraph.Height = dimensions.Height;
        }

        private static GraphMapData GetGraph(GraphComponents graphComponents)
        {
            GraphMapData graphMapData = new GraphMapData();

            IEnumerable<INodeShape> uiNodeViewModels = graphComponents.GetNodeViewModels();
            double i = 0D;
            foreach (PartitionNode uiNodeVM in uiNodeViewModels)
            {
                NodeMapData objNode = new TextNodeMapData(uiNodeVM.ID);
                graphMapData.Add(objNode);

                // Properties
                Size dimension = new Size(uiNodeVM.Width, uiNodeVM.Height);
                objNode.Dimension = dimension;
                //objNode.Position = uiNodeVM.Position;
                objNode.Position = new Point(i, i);
                objNode.IsHidden = uiNodeVM.IsHidden;

                i++;
            }

            return graphMapData;
        }

        private static IDictionary<string, Point> GetOriginalPositions(GraphMapData clusteredGraphMapData)
        {
            IDictionary<string, Point> originalPositions = new Dictionary<string, Point>();

            ICollection<NodeMapData> nodes = clusteredGraphMapData.GetNodes();
            foreach (NodeMapData node in nodes)
            {
                originalPositions[node.Id] = node.Position;
            }

            return originalPositions;
        }

        private static void ApplyOffsetToSubGraphs(IDictionary<string, Point> originalPositions, GraphMapData parentGraph, IDictionary<string, GraphMapData> subGraphToNodeMap)
        {
            foreach (NodeMapData parentGraphNode in parentGraph.GetNodes())
            {
                Point originalPosition = originalPositions[parentGraphNode.Id];
                double xOffset = parentGraphNode.Position.X - originalPosition.X;
                double yOffset = parentGraphNode.Position.Y - originalPosition.Y;

                foreach (NodeMapData subGraphNode in subGraphToNodeMap[parentGraphNode.Id].GetNodes())
                {
                    Point offsetPosition = new Point(subGraphNode.Position.X + xOffset, subGraphNode.Position.Y + yOffset);
                    subGraphNode.Position = offsetPosition;
                }
            }
        }

        private static void ApplyOffsetToSubGraphs(IDictionary<string, Point> originalPositions, GraphMapData parentGraph, IDictionary<string, ICollection<GraphMapData>> subGraphToNodeMap)
        {
            foreach (NodeMapData parentGraphNode in parentGraph.GetNodes())
            {
                Point originalPosition = originalPositions[parentGraphNode.Id];
                double xOffset = originalPosition.X - parentGraphNode.Position.X;
                double yOffset = originalPosition.Y - parentGraphNode.Position.Y;

                foreach (GraphMapData subGraph in subGraphToNodeMap[parentGraphNode.Id])
                {
                    foreach (NodeMapData subGraphNode in subGraph.GetNodes())
                    {
                        Point offsetPosition = new Point(subGraphNode.Position.X + xOffset, subGraphNode.Position.Y + yOffset);
                        subGraphNode.Position = offsetPosition;
                    }
                }
            }
        }
    }
}
