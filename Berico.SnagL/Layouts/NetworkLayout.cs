﻿//-------------------------------------------------------------
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
				throw new NotImplementedException();
			}
			protected set
			{
				throw new NotImplementedException();
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
			// this ensures that the end result will remain the same across multiple runs
			// because each node will have the same starting position
			new GridLayout().CalculateLayout(graph);

            System.Diagnostics.Debug.WriteLine("");
            foreach (Delegate d in ContextMenuManager.Instance.GetContextMenuOpeningInvocationList())
            {
                System.Diagnostics.Debug.WriteLine((d.Target as GraphComponents).Scope);
            }

			// Create a GraphComponents instance that is a partitioned
			// representation of the original graph. Each node in this
			// graph is a partition node.
			GraphComponents connectedGraphComponents = GraphManager.Instance.GetConnectedComponents(GraphManager.Instance.DefaultGraphComponentsInstance.Scope);
			IEnumerable<INodeShape> connectedComponents = connectedGraphComponents.GetNodeViewModels();

            System.Diagnostics.Debug.WriteLine("");
            foreach (Delegate d in ContextMenuManager.Instance.GetContextMenuOpeningInvocationList())
            {
                System.Diagnostics.Debug.WriteLine((d.Target as GraphComponents).Scope);
            }

			foreach (PartitionNode connectedComponent in connectedComponents)
			{
				using (GraphComponents connectedGraph = connectedComponent.GetGraph())
                {
				    LayoutByClusters(graph, connectedGraph);
                }
			}

			// Layout the overall graph
			GraphMapData connectedGraphMapData = GetClusteredGraph(graph, connectedGraphComponents);
			IDictionary<string, Point> originalPositions = GetOriginalPositions(connectedGraphMapData);
			GridLayout gridLayout = new GridLayout();
			gridLayout.CalculateLayout(connectedGraphMapData);
			ApplyOffsetToSubGraphs(graph, connectedGraphComponents, originalPositions, connectedGraphMapData);

		    connectedGraphComponents.Dispose();

            System.Diagnostics.Debug.WriteLine("");
            foreach (Delegate d in ContextMenuManager.Instance.GetContextMenuOpeningInvocationList())
            {
                System.Diagnostics.Debug.WriteLine((d.Target as GraphComponents).Scope);
            }
		}

		/// <summary>
		/// Clusters and lays out the provided graph
		/// </summary>
		/// <param name="graphMapData">The graph to update</param>
		/// <param name="connectedGraph">The graph that needs to be clustered and layed out</param>
		private static void LayoutByClusters(GraphMapData graphMapData, GraphComponents connectedGraph)
		{
            System.Diagnostics.Debug.WriteLine("");
            foreach (Delegate d in ContextMenuManager.Instance.GetContextMenuOpeningInvocationList())
            {
                System.Diagnostics.Debug.WriteLine((d.Target as GraphComponents).Scope);
            }

			// Create a Cluster instance for the graph
			//Cluster clusterer = new Cluster(connectedGraph);


			// Specify the predicate to be used by the Cluster class.  In this case
			// we are determining clusters based on edges createdf during similarity
			// clustering.
			//clusterer.EdgePredicate = delegate(IEdge edge)
			//{
			//	bool isSimilarityDataEdge = edge is SimilarityDataEdge;
			//	return isSimilarityDataEdge;
			//};

			// Create the clusters and return a new graph.  Each node on the
			// graph will be represented as a PartitionNode
			//GraphComponents clusteredGraphComponents = clusterer.GetClusteredGraph();

			//bool isAttributeLayout = true;
			// If there is no different between the initial graph that was provided and
			// out clustered graph, we didn't really find clusters (most likely because
			// similarity clustering was not performed).
			//int clusteredNodesCount = clusteredGraphComponents.GetNodeViewModels().Count();
			//int connectedNodesCount = connectedGraph.GetNodeViewModels().Count();
			//if (clusteredNodesCount == connectedNodesCount)
			//{
				// TODO handle this better than just re-running

				// Rerun clustering without a predicate.  This means that clusters will
				// be based on regular edges.
            Cluster clusterer = new Cluster(connectedGraph);
            GraphComponents clusteredGraphComponents = clusterer.GetClusteredGraph();

			bool isAttributeLayout = false;
			//}

            System.Diagnostics.Debug.WriteLine("");
            foreach (Delegate d in ContextMenuManager.Instance.GetContextMenuOpeningInvocationList())
            {
                System.Diagnostics.Debug.WriteLine((d.Target as GraphComponents).Scope);
            }

			// Get all the nodes that are in the clustered graph. Remember that these are partition nodes.
			IEnumerable<INodeShape> clusteredComponents = clusteredGraphComponents.GetNodeViewModels();
			foreach (PartitionNode clusteredComponent in clusteredComponents)
			{
				// Create an appropriate layout to use for this cluster
				AsynchronousLayoutBase clusterLayout = GetClusterLayout(isAttributeLayout);
                using (GraphComponents clusteredGraph = clusteredComponent.GetGraph())
                {
                    GraphMapData clusteredGraphMapData = GetGraph(graphMapData, clusteredGraph);

                    // Run the layout.  This is laying out the individual cluster itself
                    clusterLayout.CalculateLayout(clusteredGraphMapData);
                }

                System.Diagnostics.Debug.WriteLine("");
                foreach (Delegate d in ContextMenuManager.Instance.GetContextMenuOpeningInvocationList())
                {
                    System.Diagnostics.Debug.WriteLine((d.Target as GraphComponents).Scope);
                }
			}

            System.Diagnostics.Debug.WriteLine("");
            foreach (Delegate d in ContextMenuManager.Instance.GetContextMenuOpeningInvocationList())
            {
                System.Diagnostics.Debug.WriteLine((d.Target as GraphComponents).Scope);
            }

			// Now we need to layout the entired clustered graph so it looks more organized
			GraphMapData clusteredGraphComponentsGraphMapData = GetClusteredGraph(graphMapData, clusteredGraphComponents);
			IDictionary<string, Point> originalPositions = GetOriginalPositions(clusteredGraphComponentsGraphMapData);
			FRLayout frLayout = new FRLayout();
			frLayout.CalculateLayout(clusteredGraphComponentsGraphMapData);
			ApplyOffsetToSubGraphs(graphMapData, clusteredGraphComponents, originalPositions, clusteredGraphComponentsGraphMapData);
            
            clusteredGraphComponents.Dispose();

            System.Diagnostics.Debug.WriteLine("");
            foreach (Delegate d in ContextMenuManager.Instance.GetContextMenuOpeningInvocationList())
            {
                System.Diagnostics.Debug.WriteLine((d.Target as GraphComponents).Scope);
            }
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
				clusterLayout = new FRLayout();
			}

			return clusterLayout;
		}

		private static GraphMapData GetGraph(GraphMapData graphMapData, GraphComponents clusteredGraph)
		{
			GraphMapData clusteredGraphMapData = new GraphMapData();

			// Nodes
			IEnumerable<INodeShape> uiNodeViewModels = clusteredGraph.GetNodeViewModels();
			foreach (NodeViewModelBase uiNodeVM in uiNodeViewModels)
			{
				NodeMapData nodeMapData = graphMapData.Nodes[uiNodeVM.ParentNode.ID];
				clusteredGraphMapData.Add(nodeMapData);

				// Edges
				IEnumerable<IEdge> uiEdgeViewModels = clusteredGraph.GetEdges(uiNodeVM.ParentNode);
				foreach (IEdge uiEdgeVM in uiEdgeViewModels)
				{
					string edgeKey = uiEdgeVM.Source.ID + uiEdgeVM.Target.ID;
					EdgeMapData edgeMapData = graphMapData.Edges[edgeKey];
					clusteredGraphMapData.Add(edgeMapData);
				}
			}

			return clusteredGraphMapData;
		}

		private static GraphMapData GetClusteredGraph(GraphMapData graphMapData, GraphComponents graphComponents)
		{
			GraphMapData graphComponentsMapData = new GraphMapData();

			IEnumerable<INodeShape> clusteredComponents = graphComponents.GetNodeViewModels();
			foreach (PartitionNode clusteredComponent in clusteredComponents)
			{
				NodeMapData nodeMapData = new TextNodeMapData(clusteredComponent.ID);
				graphComponentsMapData.Add(nodeMapData);

				// Properties
				object[] dimensionAndPosition = GetPartitionNodeDimensionAndPosition(graphMapData, clusteredComponent);
				nodeMapData.Dimension = (Size)dimensionAndPosition[0];
				nodeMapData.Position = (Point)dimensionAndPosition[1];

				nodeMapData.IsHidden = clusteredComponent.IsHidden;

				IEnumerable<IEdge> clusteredComponentEdges = graphComponents.GetEdges(clusteredComponent);
				foreach (IEdge clusteredComponentEdge in clusteredComponentEdges)
				{
					EdgeMapData edgeMapData = new EdgeMapData(clusteredComponentEdge.Source.ID, clusteredComponentEdge.Target.ID);
					graphComponentsMapData.Add(edgeMapData);
				}
			}

			return graphComponentsMapData;
		}

		private static object[] GetPartitionNodeDimensionAndPosition(GraphMapData graphMapData, PartitionNode partitionNode)
		{
			Point topLeft = new Point(double.MaxValue, double.MaxValue);
			Point bottomRight = new Point(double.MinValue, double.MinValue);

			// Loop through all node view models to get the bounding area
			foreach (NodeViewModelBase nodeViewModelBase in partitionNode.Nodes)
			{
				NodeMapData nodeMapData = graphMapData.Nodes[nodeViewModelBase.ParentNode.ID];

				topLeft.X = Math.Min(topLeft.X, nodeMapData.Position.X - nodeMapData.Dimension.Width / 2D);
				topLeft.Y = Math.Min(topLeft.Y, nodeMapData.Position.Y - nodeMapData.Dimension.Height / 2D);

				bottomRight.X = Math.Max(bottomRight.X, nodeMapData.Position.X + nodeMapData.Dimension.Width / 2D);
				bottomRight.Y = Math.Max(bottomRight.Y, nodeMapData.Position.Y + nodeMapData.Dimension.Height / 2D);
			}

			// Set the new dimensions based on the calculation performed
			double width = Math.Max(bottomRight.X - topLeft.X, 1D);
			double height = Math.Max(bottomRight.Y - topLeft.Y, 1D);

			Size dimension = new Size(width, height);

			// get the center of the partitionNode
			Point position = new Point(topLeft.X + width / 2D, topLeft.Y + height / 2D);

			object[] result = new object[] { dimension, position };
			return result;
		}

		private static IDictionary<string, Point> GetOriginalPositions(GraphMapData clusteredGraphMapData)
		{
			ICollection<NodeMapData> nodes = clusteredGraphMapData.GetNodes();
			IDictionary<string, Point> originalPositions = new Dictionary<string, Point>(nodes.Count);

			foreach (NodeMapData node in nodes)
			{
				originalPositions[node.Id] = node.Position;
			}

			return originalPositions;
		}

		private static void ApplyOffsetToSubGraphs(GraphMapData graph, GraphComponents clusteredGraphComponents, IDictionary<string, Point> originalPositions, GraphMapData partitionGraph)
		{
			IEnumerable<INodeShape> clusteredComponents = clusteredGraphComponents.GetNodeViewModels();
			foreach (PartitionNode clusteredComponent in clusteredComponents)
			{
				Point originalPosition = originalPositions[clusteredComponent.ID];
				double xOffset = partitionGraph.Nodes[clusteredComponent.ID].Position.X - originalPosition.X;
				double yOffset = partitionGraph.Nodes[clusteredComponent.ID].Position.Y - originalPosition.Y;

				IList<NodeViewModelBase> viewModelNodes = clusteredComponent.Nodes;
				foreach (NodeViewModelBase viewModelNode in viewModelNodes)
				{
					NodeMapData nodeMapData = graph.Nodes[viewModelNode.ParentNode.ID];

					Point offsetPosition = new Point(nodeMapData.Position.X + xOffset, nodeMapData.Position.Y + yOffset);
					nodeMapData.Position = offsetPosition;
				}
			}
		}
	}
}
