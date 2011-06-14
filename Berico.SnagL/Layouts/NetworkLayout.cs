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
            ComputeLayout(graphComponents);

            computedGraph = GraphComponentsUtility.GetGraph(graphComponents);
            ICollection<NodeMapData> computedGraphNodes = computedGraph.GetNodes();
            foreach (NodeMapData node in computedGraphNodes)
            {
                graph.Nodes[node.Id].Position = node.Position;
            }

            GraphSharpUtility.FSAOverlapRemoval(graph);
        }

        private GraphMapData computedGraph;

        private static GraphMapData GetGraph(GraphComponents graphComponents)
        {
            GraphMapData graph = new GraphMapData();

            IEnumerable<INodeShape> uiNodeViewModels = graphComponents.GetNodeViewModels();
            foreach (PartitionNode uiNodeVM in uiNodeViewModels)
            {
                NodeMapData objNode = new TextNodeMapData(uiNodeVM.ID);
                graph.Add(objNode);

                // Properties
                Size dimension = new Size(uiNodeVM.Width, uiNodeVM.Height);
                objNode.Dimension = dimension;
                objNode.Position = uiNodeVM.Position;
                objNode.IsHidden = uiNodeVM.IsHidden;
            }

            return graph;
        }

        /// <summary>
        /// Performs the actual layout algorithm. This will execute on a background thread.
        /// </summary>
        /// <param name="graphComponents">The object containing the graph data</param>
        public void ComputeLayout(GraphComponents graphComponents)
        {
            // Create a GraphComponents instance that is a partitioned
            // representation of the original graph.  Each node in this
            // graph is a partition node.
            GraphComponents connectedComponents = GraphManager.Instance.GetConnectedComponents(graphComponents.Scope);

            foreach (PartitionNode partitionNode in connectedComponents.GetNodeViewModels())
            {
                LayoutByClusters(partitionNode.GetGraph());
                partitionNode.RecalculateDimensions();// TODO this will be easy to change to use _mapdata
            }

            // Layout the overall graph
            GridLayoutOrig layout = new GridLayoutOrig();
            layout.ComputeLayout(false, connectedComponents, null);
            layout.PositionNodes(false);
            /* TODO the node ids in connectedComponents differ from the original node ids in graphComponents
            computedGraph = GetGraph(connectedComponents);
            GridLayout gridLayout = new GridLayout();
            gridLayout.CalculateLayout(computedGraph);
            */
        }

        /// <summary>
        /// Clusters and lays out the provided graph
        /// </summary>
        /// <param name="partitionedGraphComponents">The graph that needs to be clustered and layed out</param>
        private void LayoutByClusters(GraphComponents partitionedGraphComponents)
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
            GraphComponents clusteredGraph = clusterer.GetClusteredGraph();

            bool isAttributeLayout = true;
            // If there is no different between the initial grpah that was provided and
            // out clustered graph, we didn't really find clusters (most likely because
            // similarity clustering was not performed).
            int clusteredGraphVMCount = clusteredGraph.GetNodeViewModels().Count();
            int graphComponentsVMCount = partitionedGraphComponents.GetNodeViewModels().Count();
            if (clusteredGraphVMCount == graphComponentsVMCount)
            {
                // TODO handle this better than just re-running

                // Rerun clustering without a predicate.  This means that clusters will
                // be based on regular edges.
                clusterer = new Cluster(partitionedGraphComponents);
                clusteredGraph = clusterer.GetClusteredGraph();

                isAttributeLayout = false;
            }

            // Get all the nodes that are in the clustered graph.  Remember that these 
            // are partition nodes.
            IEnumerable<INodeShape> nodes = clusteredGraph.GetNodeViewModels();
            foreach (INodeShape nodeVM in nodes)
            {
                PartitionNode pn = nodeVM as PartitionNode;

                // Create an appropriate layout to use for this cluster
                object individualLayout = GetClusterLayout(isAttributeLayout);

                if (individualLayout is GridLayoutOrig)
                {
                    GridLayoutOrig gridLayout = individualLayout as GridLayoutOrig;

                    // Run the layout.  This is laying out the individual cluster itself
                    gridLayout.ComputeLayout(false, pn.GetGraph(), null);
                    gridLayout.PositionNodes(false);

                    // Recalculate the partiion nodes dimensions
                    pn.RecalculateDimensions();
                }
                else
                {
                    ForceDirected fdLayout = individualLayout as ForceDirected;

                    // Run the layout.  This is laying out the individual cluster itself
                    fdLayout.ComputeLayout(false, pn.GetGraph(), null);
                    fdLayout.PositionNodes(false);

                    // Recalculate the partiion nodes dimensions
                    pn.RecalculateDimensions();
                }
            }

            // Now we need to layout the entired clustered graph so it looks
            // more organized
            ForceDirected clusteredGraphLayout = new ForceDirected();
            clusteredGraphLayout.ComputeLayout(false, clusteredGraph, null);
            clusteredGraphLayout.PositionNodes(false);
        }

        /// <summary>
        /// Instantiates an appropriate layout instance
        /// </summary>
        /// <param name="isAttributeCluster">Indicates whether we have attribute similarity clusters or not</param>
        /// <returns>a new, appropriate, LayoutBase instance</returns>
        private object GetClusterLayout(bool isAttributeCluster)
        {
            object clusterLayout;

            // Check if our clusters are based on attribute similarity
            if (isAttributeCluster)
            {
                clusterLayout = new GridLayoutOrig();
            }
            else
            {
                ForceDirected forceDirectedLayout = new ForceDirected();
                forceDirectedLayout.MaxIterations = 200;

                clusterLayout = forceDirectedLayout;
            }

            return clusterLayout;
        }
    }
}
