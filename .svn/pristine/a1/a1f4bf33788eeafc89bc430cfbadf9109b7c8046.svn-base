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
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Berico.SnagL.Infrastructure.Graph;
using Berico.SnagL.Model;

namespace Berico.SnagL.Infrastructure.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class PartitionNode : INode, INodeShape
    {

        private List<NodeViewModelBase> nodes = null;
        private List<IEdge> externalConnections = new List<IEdge>();
        private List<IEdge> externalIncommingConnections = new List<IEdge>();

        /// <summary>
        /// Gets or sets the mechanism for which the object was loaded onto the graph
        /// </summary>
        public CreationType SourceMechanism
        {
            get;
            set;
        }

        public PartitionNode(string _scope)
            : this(_scope, string.Empty)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_scope"></param>
        public PartitionNode(string _scope, string _id)
        {
            ID = _id;
            nodes = new List<NodeViewModelBase>();
        }

        /// <summary>
        /// 
        /// </summary>
        public IList<NodeViewModelBase> Nodes
        {
            get { return nodes.AsReadOnly(); }
        }

        /// <summary>
        /// Gets a list of edges that are completely external
        /// to this PartitionNode
        /// </summary>
        public IEnumerable<IEdge> ExternalConnections
        {
            get { return externalConnections.AsReadOnly(); }
        }

        /// <summary>
        /// Gets a list of edges whose target node is part of this
        /// PartitionNode and whose source is not
        /// </summary>
        public IEnumerable<IEdge> ExternalIncommingConnections
        {
            get { return externalIncommingConnections.AsReadOnly(); }
        }

        /// <summary>
        /// Gets a list of all edges (both incomming and outgoing).  Since incoming and
        /// outgoing edges are copies of each other, this list will contain duplicates.
        /// </summary>
        public List<IEdge> ExternalEdges
        {
            get
            {
                // Check if one of the edge collections is null
                if (externalConnections != null && externalIncommingConnections != null)
                {
                    // Return all the in and out edges
                    return externalConnections.Concat(externalIncommingConnections).ToList();
                }
                else
                {
                    // Check if we have outedges
                    if (externalIncommingConnections != null)
                        return externalIncommingConnections;
                    else if (externalConnections != null)
                        return externalConnections;
                    else
                        return new List<IEdge>();
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Point CalculateAveragePosition()
        {
            double totalX = 0;
            double totalY = 0;

            // Loop over all the node view models
            foreach (NodeViewModelBase nodeVM in nodes)
            {
                totalX += nodeVM.Position.X;
                totalY += nodeVM.Position.Y;
            }

            return new Point((totalX / nodes.Count), (totalY / nodes.Count));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deltaX"></param>
        /// <param name="deltaY"></param>
        private void RepositionNodes(double deltaX, double deltaY)
        {
            // Loop over all the node view models in the internal collection
            // and move them by the provided amounts
            foreach (NodeViewModelBase nodeVM in nodes)
            {
                Point currentNodePosition = nodeVM.Position;
                Point newPosition = new Point(currentNodePosition.X += deltaX, currentNodePosition.Y += deltaY);

                // Set the new position of the node
                nodeVM.Position = newPosition;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Size CalculateDimensions()
        {
            Point topLeft = new Point(long.MaxValue, long.MaxValue);
            Point bottomRight = new Point(long.MinValue, long.MinValue);

            // Loop through all node view models to get the bounding area
            foreach (NodeViewModelBase nodeVM in nodes)
            {
                if (double.IsNaN(nodeVM.Width) || double.IsNaN(nodeVM.Height))
                    continue;

                topLeft.X = System.Math.Min(topLeft.X, nodeVM.Position.X - nodeVM.Width / 2);
                topLeft.Y = System.Math.Min(topLeft.Y, nodeVM.Position.Y - nodeVM.Height / 2);
                bottomRight.X = System.Math.Max(bottomRight.X, nodeVM.Position.X + nodeVM.Width / 2);
                bottomRight.Y = System.Math.Max(bottomRight.Y, nodeVM.Position.Y + nodeVM.Height / 2);
            }

            // Reset the position for this partition node
            //Position = new Point((bottomRight.X + topLeft.X) / 2, (bottomRight.Y + topLeft.Y) / 2);

            Size dimensions = new Size();

            // Set the new dimensions based on the calculation performed
            dimensions.Height = Math.Max(bottomRight.Y - topLeft.Y, 1);
            dimensions.Width = Math.Max(bottomRight.X - topLeft.X, 1);

            return dimensions;
        }

        /// <summary>
        /// 
        /// </summary>
        public void RecalculateDimensions()
        {
            Size calculatedDimension = CalculateDimensions();

            Height = calculatedDimension.Height;
            Width = calculatedDimension.Width;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeVM"></param>
        public void AddNode(NodeViewModelBase nodeVM)
        {
            // Check and make sure that the provided node view
            // model is not already part of this partition node
            if (nodes.Contains(nodeVM))
                return;

            // Add the node to the partition
            nodes.Add(nodeVM);

            // Create a temporary edge collection
            List<Edge> tempEdgeList = new List<Edge>();

            // Analyze the collection of existing external connections
            foreach (Edge edge in externalConnections)
            {
                // Check if the current node is this edge's
                // Target node
                if (edge.Target.Equals(nodeVM.ParentNode))
                {
                    // Add this edge to our temporary edge
                    // collection
                    tempEdgeList.Add(edge);
                }  // Check if the node is this edge's Source node
                else if (edge.Source.Equals(nodeVM.ParentNode))
                {
                    // Since this node is the source node for this
                    // edge, and the node is part of the Partition, 
                    // it needs to be removed from the external
                    // incomming connections collection.
                    externalIncommingConnections.Remove(edge);
                }
            }

            // Remove all the edges in the temporary collection
            // from the external edges collection.  
            foreach (Edge edge in tempEdgeList)
                externalConnections.Remove(edge);

            tempEdgeList.Clear();

            // Analyze the collection of existing external incomming
            // connections
            foreach (Edge edge in externalIncommingConnections)
            {
                // Check if the current node is this edge's
                // Source node
                if (edge.Source.Equals(nodeVM.ParentNode))
                {
                    // Add this edge to our temporary edge
                    // collection
                    tempEdgeList.Add(edge);
                }
                else if (edge.Target.Equals(nodeVM.ParentNode))
                {
                    externalConnections.Remove(edge);
                }
            }

            // Remove all the edges in the temporary collection
            // from the external incomming edges collection.  
            foreach (Edge edge in tempEdgeList)
                externalIncommingConnections.Remove(edge);

            //TODO: VALIDATE GRAPHMANAGER CALL RESULTS BEFORE USING VALUES

            // Loop over all the edges for this nodes
            foreach (Edge edge in GraphManager.Instance.GetGraphComponents(nodeVM.Scope).Data.Edges(nodeVM.ParentNode))
            {
                NodeViewModelBase targetNodeVM = (GraphManager.Instance.GetGraphComponents(nodeVM.Scope).GetNodeViewModel(edge.Target)) as NodeViewModelBase;
                NodeViewModelBase sourceNodeVM = (GraphManager.Instance.GetGraphComponents(nodeVM.Scope).GetNodeViewModel(edge.Source)) as NodeViewModelBase;

                // Check if the current node is this edge's 
                // Target node
                if (edge.Target != nodeVM.ParentNode && !nodes.Contains(targetNodeVM))
                {
                    // If we are here, this edge is an outgoing edge
                    // whose target is not part of this PartionNode
                    externalConnections.Add(edge);
                }
                else if (edge.Source != nodeVM.ParentNode && !nodes.Contains(sourceNodeVM))
                {
                    // If we are here, this edge is an incomming edge
                    // whose source is not part of this PartitionNode
                    externalIncommingConnections.Add(edge);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public GraphComponents GetGraph()
        {
            // Validate the internal nodes collection
            if (nodes.Count == 0)
                return null;

            // Get the scope from the first node in the
            // nodes collection
            string scope = nodes[0].Scope;

            // Create a new instance of GraphComponents using the scope
            // previously obtained from one of the node view models that
            // is part of this partition node
            GraphComponents graphComponents = new GraphComponents(scope);
            List<IEdge> edges = new List<IEdge>();

            // Loop over all the node view models, contained in 
            // this partition node, and add them to the new graph
            foreach (NodeViewModelBase nodeVM in nodes)
            {
                // Add this node view model to the graph
                graphComponents.AddNodeViewModel(nodeVM);

                // Get all the edges for this node
                edges.AddRange(GraphManager.Instance.GetGraphComponents(nodeVM.Scope).Data.Edges(nodeVM.ParentNode).ToList());
            }

            // Loop over the edges that were found for all the
            // nodes in the graph
            foreach (IEdge edge in edges)
            {
                // Ensure that both ends of the edge are in this graph
                if (graphComponents.Data.ContainsNode(edge.Source) && graphComponents.Data.ContainsNode(edge.Target))
                    graphComponents.Data.AddEdge(edge);
            }

            return graphComponents;
        }

        /// <summary>
        /// Compares this instance with a specified object or <see cref="Node"/> and returns an integer that indicates whether this instance precedes, follows, or appears in the same position in the sort order as the specified object or <see cref="Node"/>.
        /// </summary>
        /// <param name="obj">The <see cref="INode"/> to compare with this instance</param>
        /// <returns>A 32-bit signed integer that indicates whether this instance precedes, follows, or appears in the same position in the sort order as the <paramref name="obj"/> parameter</returns>
        public int CompareTo(object obj)
        {
            return ID.CompareTo(((INode)obj).ID);
        }

        /// <summary>
        /// Returns a hash code for this <see cref="Node"/>
        /// </summary>
        /// <returns>A 32-bit signed integer hash code</returns>
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        #region INodeShape Members

            /// <summary>
            /// 
            /// </summary>
            public double Height { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public double Width { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public Point Position
            {
                get { return CalculateAveragePosition(); }
                set
                {
                    Point currentPosition = Position;
                    RepositionNodes(value.X - currentPosition.X, value.Y - currentPosition.Y);
                }
            }

            /// <summary>
            /// Gets the center point of this node.  This value is derrived from the 
            /// current Height and Width of the node and represents where an edge
            /// is attached.
            /// </summary>
            public Point CenterPoint
            {
                get
                {
                    // double.NaN represents 'Auto'
                    if (double.IsNaN(Height) || double.IsNaN(Width))
                        return Position;
                    else
                        return new Point(Position.X + (Width / 2), Position.Y + (Height / 2));
                }
            }

        #endregion

        #region IScopingContainer<string> Members


            private string scope = string.Empty;

            /// <summary>
            ///
            /// </summary>
            public string Scope
            {
                get { return this.scope; }
                private set { this.scope = value; }
            }

        #endregion

        #region INode Members

        /// <summary>
        /// 
        /// </summary>
        public string ID { get; set; }

        #endregion


         #region INodeShape Members

            private bool isHidden = false;
            public bool IsHidden
            {
                get
                {
                    return isHidden;
                }
                set
                {
                    // We don't want to do this for a partition node
                }
            }

            public Point AttachmentPoint
            {
                get { return CenterPoint; }
            }

        #endregion

    }       
}