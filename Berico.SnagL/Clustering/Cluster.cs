﻿//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Berico.SnagL.Infrastructure.Data;
using Berico.SnagL.Infrastructure.Graph;
using Berico.SnagL.Model;

namespace Berico.SnagL.Infrastructure.Clustering
{
    /// <summary>
    /// 
    /// </summary>
    public class Cluster
    {
        private GraphComponents _partitionedGraph;
        private int _currentId = 0;
        private readonly GraphComponents _sourceGraph;
        private readonly Dictionary<INodeShape, PartitionNode> _nodeToPartition = new Dictionary<INodeShape, PartitionNode>();
        private readonly List<PartitionNode> _partitionNodes = new List<PartitionNode>();

        /// <summary>
        /// 
        /// </summary>
        public Cluster(GraphComponents graphComponents)
        {
            _sourceGraph = graphComponents;
        }

        public GraphComponents GetClusteredGraph()
        {
            IEnumerable<INodeShape> nodes = _sourceGraph.GetNodeViewModels();
            _partitionedGraph = new GraphComponents(_sourceGraph.Scope);

            // Loop over all the nodes in the list
            foreach (NodeViewModelBase nodeVM in nodes)
            {
                // Create a partition node for clusters
                PartitionNode pn = GetClusterAsPartitionNode(nodeVM);

                // Add the partition node to the graph
                _partitionedGraph.AddNodeViewModel(pn);
            }

            // Loop over all of the partition nodes in our partition graph
            foreach (PartitionNode pn in _partitionNodes)
            {
                // Loop over all the external connections
                foreach (Edge edge in pn.ExternalConnections)
                {
                    INodeShape targetNodeVM = GraphManager.Instance.GetGraphComponents(_sourceGraph.Scope).GetNodeViewModel(edge.Target);

                    // Check if the edge's target node is in our partition collection
                    if (_nodeToPartition.ContainsKey(targetNodeVM))
                    {
                        IEdge newEdge = edge.Copy(pn, _nodeToPartition[targetNodeVM]);
                        _partitionedGraph.Data.AddEdge(newEdge);
                    }
                }
            }

            return _partitionedGraph;
        }

        /// <summary>
        /// 
        /// </summary>
        public Predicate<IEdge> EdgePredicate { get; set; }

        public ICollection<Point> CalculateConvexHullForCluster(PartitionNode pn)
        {
            double padding = 0;

            List<Point> points = new List<Point>();

            foreach (NodeViewModelBase nodeVM in pn.Nodes)
            {
                points.Add(new Point(nodeVM.Position.X - nodeVM.Width / 2 - padding, nodeVM.Position.Y - nodeVM.Height / 2 - padding));
                points.Add(new Point(nodeVM.Position.X - nodeVM.Width / 2 - padding, nodeVM.Position.Y + nodeVM.Height / 2 + padding));
                points.Add(new Point(nodeVM.Position.X + nodeVM.Width / 2 + padding, nodeVM.Position.Y + nodeVM.Height / 2 + padding));
                points.Add(new Point(nodeVM.Position.X + nodeVM.Width / 2 + padding, nodeVM.Position.Y - nodeVM.Height / 2 - padding));
            }

            return ConvexHull.CalculateConvexHull(points);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialNode"></param>
        /// <returns></returns>
        private PartitionNode GetClusterAsPartitionNode(INodeShape initialNode)
        {
            // Determine if the provided node is already
            // stored in our internal collection.  If it 
            // is, return it.
            if (_nodeToPartition.ContainsKey(initialNode))
                return _nodeToPartition[initialNode];

            PartitionNode pn = new PartitionNode(_sourceGraph.Scope, "Partition" + (_currentId++).ToString());
            _partitionNodes.Add(pn);

            Dictionary<INodeShape, bool> nodesInCluster = new Dictionary<INodeShape, bool>();
            Queue<INodeShape> nodesToTriangulate = new Queue<INodeShape>();

            nodesInCluster[initialNode] = true;
            nodesToTriangulate.Enqueue(initialNode);

            while (nodesToTriangulate.Count > 0)
            {
                List<INodeShape> tempList = FindTrianglesToAdd(nodesToTriangulate.Dequeue(), nodesInCluster);

                foreach (INodeShape nodeVM in tempList)
                {
                    // Add the node view model to our queue
                    nodesToTriangulate.Enqueue(nodeVM);
                }
            }

            // Loop over all the node view models in the dictionary
            foreach (NodeViewModelBase nodeVM in nodesInCluster.Keys)
            {
                // Add the node to the partition
                pn.AddNode(nodeVM);

                // Save the updated partition node
                _nodeToPartition[nodeVM] = pn;
            }

            return pn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentNode"></param>
        /// <param name="nodesInCluster"></param>
        /// <returns></returns>
        private List<INodeShape> FindTrianglesToAdd(INodeShape currentNode, Dictionary<INodeShape, bool> nodesInCluster)
        {

            //this dictionary is actually a meaningless matching - the only purpose
            //is to ensure constant time for the contains() method
            Dictionary<INodeShape, bool> nodeToInTriangle = new Dictionary<INodeShape, bool>();
            List<INodeShape> nodesFoundForCluster = new List<INodeShape>();
            bool neighborInGraph = false;

            // Loop over all the neighbors for the current node
            foreach (INodeShape neighbor in GetNeighbors(currentNode))
            {
                // Determing if the neighbor we are dealing with is a partition node
                if (neighbor is PartitionNode)
                    neighborInGraph = GraphManager.Instance.GetGraphComponents(_sourceGraph.Scope).Data.ContainsNode(neighbor as PartitionNode);
                else
                    neighborInGraph = GraphManager.Instance.GetGraphComponents(_sourceGraph.Scope).Data.ContainsNode((neighbor as NodeViewModelBase).ParentNode);

                if (!nodeToInTriangle.ContainsKey(neighbor) && neighborInGraph)
                {
                    int neighborCount = 0;

                    // Get the count of neighbors for this node.  We determine
                    // this differently depending on the type of node.
                    if (neighbor is PartitionNode)
                        neighborCount = GraphManager.Instance.GetGraphComponents(_sourceGraph.Scope).Data.Neighbors(neighbor as PartitionNode).Count();
                    else
                        neighborCount = GraphManager.Instance.GetGraphComponents(_sourceGraph.Scope).Data.Neighbors((neighbor as NodeViewModelBase).ParentNode).Count();

                    if (neighborCount == 1)
                    {
                        nodeToInTriangle[neighbor] = true;
                        if (!nodesInCluster.ContainsKey(neighbor))
                        {
                            nodesInCluster[neighbor] = true;
                            nodesFoundForCluster.Add(neighbor);

                            //if neighbor was already put in its own cluster, remove that cluster from the partitioning
                            if (_nodeToPartition.ContainsKey(neighbor))
                            {
                                _partitionNodes.Remove(_nodeToPartition[neighbor]);
                                _partitionedGraph.RemoveNodeViewModel(_nodeToPartition[neighbor]);
                                _nodeToPartition.Remove(neighbor);
                            }
                        }
                    }

                    foreach (NodeViewModelBase neighbor2 in GetNeighbors(neighbor))
                    {
                        if (GetNeighbors(neighbor2).Contains(currentNode))
                        {
                            if (!nodeToInTriangle.ContainsKey(neighbor))
                            {
                                nodeToInTriangle[neighbor] = true;
                                if (!nodesInCluster.ContainsKey(neighbor))
                                {
                                    nodesInCluster[neighbor] = true;
                                    nodesFoundForCluster.Add(neighbor);
                                }
                            }

                            if (!nodeToInTriangle.ContainsKey(neighbor2))
                            {
                                nodeToInTriangle[neighbor2] = true;
                                if (!nodesInCluster.ContainsKey(neighbor2))
                                {
                                    nodesInCluster[neighbor2] = true;
                                    nodesFoundForCluster.Add(neighbor2);
                                }
                            }
                        }
                    }
                }
            }

            return nodesFoundForCluster;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeVM"></param>
        /// <returns></returns>
        private IList<INodeShape> GetNeighbors(INodeShape nodeVM)
        {
            List<INodeShape> goodNeighbors = new List<INodeShape>();

            // The edges are stored in the primary graph scope, they were not copied
            // to the partition graph

            IEnumerable<IEdge> edges = null;

            // Get the collection of edges for this node.  How we get the edges
            // depends on what type of node we are dealing with.
            if (nodeVM is PartitionNode)
                edges = GraphManager.Instance.GetGraphComponents(nodeVM.Scope).GetEdges(nodeVM as PartitionNode);
            else
                edges = GraphManager.Instance.GetGraphComponents(nodeVM.Scope).GetEdges((nodeVM as NodeViewModelBase).ParentNode);

            // Loop over the edges
            foreach (IEdge edge in edges)
            {
                // Determine if an edge predicate was provided and if it is true or not
                if (EdgePredicate == null || EdgePredicate(edge))
                {
                    INodeShape oppositeNode = null;

                    // Get the node at the opposite end of this edge.  How we get
                    // the node depends on what type of node we are dealing with.
                    if (nodeVM is PartitionNode)
                        oppositeNode = GraphManager.Instance.GetGraphComponents(nodeVM.Scope).GetOppositeNode(edge, nodeVM as PartitionNode);
                    else
                        oppositeNode = GraphManager.Instance.GetGraphComponents(nodeVM.Scope).GetOppositeNode(edge, (nodeVM as NodeViewModelBase).ParentNode);

                    goodNeighbors.Add(oppositeNode);
                }
            }

            return goodNeighbors;
        }
    }
}