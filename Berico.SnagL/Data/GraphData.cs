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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Berico.Common.Collections;
using Berico.SnagL.Infrastructure.Events;
using Berico.SnagL.Infrastructure.Graph;
using Berico.SnagL.Model;

namespace Berico.SnagL.Infrastructure.Data
{
    /// <summary>
    /// This class represents a mathmetical graph data structue
    /// that consists of a series of arbitrary objects (known as
    /// nodes) that are connected to each other.  The connection
    /// between nodes is known as an edge.
    /// </summary>
    public class GraphData : IScopingContainer<string>, INotifyCollectionChanged
    {

        private readonly SnaglEventAggregator eventAggregator = SnaglEventAggregator.DefaultInstance;
        // TODO:  SHOULD REPLACE WITH OBSERVABLE DICTIONARY COLLECTION
        private KeyedDictionaryEntryCollection<INode> nodeEdges;
        private GraphType type = GraphType.Directed;
        private int edgeCount = 0;

        /// <summary>
        /// Initializes a new, default, instance of Berico.LinkAnalysis.Model.Graph
        /// </summary>
        public GraphData(string _scope)
        {
            this.scope = _scope;

            // Initalize the primary storage dictionary for the graph
            nodeEdges = new KeyedDictionaryEntryCollection<INode>();

            // Initialize the orphan edge collection
            OrphanEdges = new Collection<IEdge>();
        }

        /// <summary>
        /// Gets the EdgeContainer for the specified Node in the current
        /// Berico.LinkANalysis.Model.Graph object.
        /// </summary>
        /// <param name="node">A node in the current Berico.LinkAnalysis.Model.Graph object</param>
        /// <returns>A Berico.LinkAnalysis.Model.EdgeContainer</returns>
        public EdgeContainer this[INode node]
        {
            get
            {
                EdgeContainer edgeContainer = null;

                // Check if the attribute exists
                bool result = this.nodeEdges.Contains(node);

                if (result)
                {
                    // Get the EdgeContainer
                    edgeContainer = (EdgeContainer)this.nodeEdges[node].Value;
                }

                return edgeContainer;
            }
        }

        /// <summary>
        /// Gets the type of graph that this is.  Currently, only
        /// 'Directed' graphs are supported.
        /// </summary>
        public GraphType Type
        {
            get { return this.type; }
        }

        /// <summary>
        /// Gets the order (number of nodes) of the current graph
        /// </summary>
        public int Order
        {
            get { return nodeEdges.Keys.Count; }
        }

        /// <summary>
        /// Gets the size (total number of edges) of the current graph
        /// </summary>
        public int Size
        {
            get { return this.edgeCount; }
        }

        /// <summary>
        /// Gets the number of items in the collection
        /// </summary>
        public int Count
        {
            get { return this.nodeEdges.Count; }
        }

        /// <summary>
        /// Gets a references to a collection of edges without corresponding nodes
        /// </summary>
        public Collection<IEdge> OrphanEdges
        {
            get;
            private set;
        }

        /// <summary>
        /// Clears the internal collection of nodes and edges
        /// </summary>
        public void Clear()
        {
            // Check if there is anything to be cleared
            if (Count > 0)
            {
                // Clear the collection
                nodeEdges.Clear();

                // Throw the CollectionChanged event
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        #region Working with nodes

            /// <summary>
            /// Determines if the provided node is in the collection of nodes
            /// and edges
            /// </summary>
            /// <param name="node">The Node to look for</param>
            /// <returns>true if the Node was found; otherwise false</returns>
            public bool ContainsNode(INode node)
            {
                return this.nodeEdges.Contains(node);
            }

            /// <summary>
            /// Adds a new Node to the collection of nodes and edges
            /// </summary>
            /// <param name="node">The Node to be added</param>
            /// <returns>true if the Node was successfully added; otherwise false</returns>
            public bool AddNode(INode node)
            {
                List<IEdge> processedOrphans = new List<IEdge>(OrphanEdges.Count);

                // Valid the node
                if (node == null)
                    throw new ArgumentNullException("node", "The provided node was null");

                if (node is GhostNode)
                {
                    throw new ArgumentException("GhostNode objects cannot be added directly to the graph at this time");
                }

                // Check if node already exists
                if (this.ContainsNode(node))
                    return false;

                // Create an EdgeContainer for this node and add the data to the collection
                this.nodeEdges.Add(new DictionaryEntry(node, new EdgeContainer(this.type == GraphType.Directed ? true : false)));

                // Determine whether we have any orphan edges waiting to be added
                if (node.SourceMechanism == CreationType.Live && OrphanEdges.Count > 0)
                {
                    var edges = from e in OrphanEdges
                                where e.Source.ID == node.ID || e.Target.ID == node.ID
                                select e;

                    foreach (IEdge edge in edges)
                    {
                        if (edge.Source is GhostNode && edge.Source.ID == node.ID)
                        {
                            edge.Source = node;
                        }

                        if (edge.Target is GhostNode && edge.Target.ID == node.ID)
                        {
                            edge.Target = node;
                        }

                        if (!(edge.Source is GhostNode) && !(edge.Target is GhostNode))
                        {
                            AddEdge(edge);
                            processedOrphans.Add(edge);
                        }
                    }

                    // If node was added to the graph, remove it from ophan collection
                    foreach (IEdge edge in processedOrphans)
                    {
                        OrphanEdges.Remove(edge);
                    }
                }

                DictionaryEntry entry;
                int index = GetIndexAndEntry(node, out entry);

                if (index > -1)
                {
                    // Check if this is a regular node and not a partition node
                    if (node is Node)
                    {
                        // Fire the NodeAdded event
                        this.OnNodeAdded(new NodeEventArgs(node as Node, null));

                        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new KeyValuePair<Node, EdgeContainer>(entry.Key as Node, entry.Value as EdgeContainer), index));

                        // Handle the nodes PropertyChanged event
                        (node as Node).PropertyChanged += new EventHandler<Common.Events.PropertyChangedEventArgs<string>>(Node_PropertyChanged);
                    }

                    return true;
                }
                else
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                    return false;
                }
            }

            /// <summary>
            /// Adds a range of Nodes to the collection of nodes and edges
            /// </summary>
            /// <param name="nodes">The collection of nodes to be added</param>
            /// <returns>the number of nodes that were successfully added</returns>
            public int AddNodes(IEnumerable<Node> nodes)
            {
                int nodesAdded = 0;

                // Loop through each node in the provided
                // nodes collection
                foreach (Node node in nodes)
                {
                    // Add the node by calling the standard
                    // AddNode method
                    if (AddNode(node))
                        nodesAdded++;
                }

                return nodesAdded;
            }

            /// <summary>
            /// Removes a node from the graph
            /// </summary>
            /// <param name="node">The node to be removed</param>
            /// <returns>true if the node was removed; otherwise false</returns>
            public bool RemoveNode(INode node)
            {
                // Valid the node
                if (node == null)
                    throw new ArgumentNullException("node", "The provided node was null");

                // If the node doesn't exist, return true
                if (!this.ContainsNode(node))
                    return false;

                DictionaryEntry entry;
                int index = GetIndexAndEntry(node, out entry);

                // Fire the CollectionChanged event
                if (index > -1)
                {
                    if (!(node is PartitionNode))
                    {
                        // Remove the edges for this node
                        List<IEdge> edgesForRemoval = new List<IEdge>(this.Edges(node));
                        this.RemoveEdges(edgesForRemoval);

                        // Remove the node from the node list
                        this.nodeEdges.Remove(node);

                        // Fire the NodeRemoved event
                        OnNodeRemoved(new NodeEventArgs(node as Model.Node, null));
                    }

                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new KeyValuePair<Node, EdgeContainer>(entry.Key as Node, entry.Value as EdgeContainer), index));

                    if (entry.Value is Node)
                    {
                        (entry.Value as Node).PropertyChanged -= new EventHandler<Common.Events.PropertyChangedEventArgs<string>>(Node_PropertyChanged);
                    }
                }

                return true;
            }

            /// <summary>
            /// Removes a range of nodes from the graph
            /// </summary>
            /// <param name="edges">The collection of nodes to be removed</param>
            /// <returns>the number of nodes that were successfully removed</returns>
            public int RemoveNodes(IEnumerable<Node> nodes)
            {
                int nodesRemoved = 0;

                // Loop through each node in the provided
                // nodes collection
                foreach (Node node in nodes)
                {
                    // Remove the node by calling the regular
                    // RemoveNode method
                    if (RemoveNode(node))
                        nodesRemoved++;
                }

                return nodesRemoved;
            }

            /// <summary>
            /// Gets a node by its specified ID
            /// </summary>
            /// <param name="id">Unique ID of the node for which to return</param>
            /// <returns>The node matching the specified node ID or null if it doesn't exist</returns>
            public INode GetNode(string id)
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    throw new ArgumentNullException("id");
                }

                return Nodes.FirstOrDefault(node => node.ID.ToLower().Equals(id.ToLower()));
            }

            /// <summary>
            /// Gets an enumerable collection of nodes that are in the
            /// master nodes and edges collection.
            /// </summary>
            public IEnumerable<INode> Nodes
            {
                get { return this.nodeEdges.Keys; }
            }

            public IEnumerable<INodeShape> GetPartitionNodes()
            {
                foreach (INode node in Nodes)
                {
                    if (node is PartitionNode)
                        yield return node as INodeShape;
                }
            }

            /// <summary>
            /// Returns an enumerable collection of nodes that are the children 
            /// of the provided node.  A Node is indicated as a 'child' (or
            /// successor) if it is the 'Target' in an edge between it and the
            /// provided node.
            /// </summary>
            /// <param name="node">A Berico.LinkAnslysis.Model.Node object</param>
            /// <returns>a collection of Node objects</returns>
            public IEnumerable<INode> Successors(INode node)
            {
                List<INode> nodes = new List<INode>();

                // Loop through all the outgoing edges and return all
                // 'Target' nodes
                foreach (Edge edge in (this.nodeEdges[node].Value as EdgeContainer).OutgoingEdges)
                {
                    nodes.Add(edge.Target);
                }

                return nodes;
            }

            /// <summary>
            /// Returns an enumerable collection of nodes that are the parents 
            /// of the provided node.  A Node is indicated as a 'parent' (or
            /// predecessor) if it is the 'Source' in an edge between it and the
            /// provided node.
            /// </summary>
            /// <param name="node">A Berico.LinkAnslysis.Model.Node object</param>
            /// <returns>a collection of Node objects</returns>
            public IEnumerable<INode> Predecessors(INode node)
            {
                // Loop through all the incomming edges and return all
                // 'Source' nodes
                foreach (Edge edge in (this.nodeEdges[node].Value as EdgeContainer).IncommingEdges)
                {
                    yield return edge.Source;
                }
            }

            /// <summary>
            /// Returns an enumerable collection of all nodes that are connected
            /// to the specified node.
            /// </summary>
            /// <param name="node">Node for which to return the neighbors for</param>
            /// <returns>A collection of <see cref="INode"/> objects that neighbor the specified <paramref name="node"/></returns>
            public IEnumerable<INode> Neighbors(INode node)
            {
                List<INode> nodes = new List<INode>();

                // Return a combination of the provided nodes successors and
                // predecessors
                return Successors(node).Concat<INode>(Predecessors(node));
            }

            #region Events

                /// <summary>
                /// Handles the PropertyChanged event for a Node
                /// </summary>
                /// <param name="sender">The Node that fired the event</param>
                /// <param name="e">The arguments for the event</param>
                private void Node_PropertyChanged(object sender, Common.Events.PropertyChangedEventArgs<string> e)
                {
                    // We need to pass this event on so we are going to fire
                    // a new, more specific, event and provide the Node
                    // whose property has changed.
                    OnNodePropertyChanged(new NodeEventArgs(sender as Node, e));
                }

                /// <summary>
                /// Fires the NodeAdded event
                /// </summary>
                /// <param name="args">The arguments for the event</param>
                public virtual void OnNodeAdded(NodeEventArgs args)
                {
                    this.eventAggregator.GetEvent<NodeAddedEvent>().Publish(args);
                }

                /// <summary>
                /// Fires the NodeRemoved event
                /// </summary>
                /// <param name="args">The arguments for the event</param>
                public virtual void OnNodeRemoved(NodeEventArgs args)
                {
                    this.eventAggregator.GetEvent<NodeRemovedEvent>().Publish(args);
                }

                /// <summary>
                /// Fires the NodePropertyChanged event
                /// </summary>
                /// <param name="args">The arguments for the event</param>
                public virtual void OnNodePropertyChanged(NodeEventArgs args)
                {
                    this.eventAggregator.GetEvent<NodePropertyChangedEvent>().Publish(args);
                }

            #endregion

        #endregion

        #region Working with edges

            //TODO: THROW COLLECTION CHANGED WHEN EDGE CONTAINER IS UPDATED

            /// <summary>
            /// Adds the provided Edge to the collection of nodes and edges
            /// </summary>
            /// <param name="edge">The edge to be added</param>
            /// <returns>true if the Edge was successfully added; otherwise false</returns>
            public bool AddEdge(IEdge edge)
            {
                // Check if edge already exists
                if (ContainsEdge(edge))
                    return false;

                // Ensure that we don't have a self loop (which shouldn't happen)
                if (edge.Target == edge.Source)
                    return false;

                // Edge is missing a corresponding node; add it to the orphan edge collection
                if (edge.Source is GhostNode || edge.Target is GhostNode)
                {
                    if (!OrphanEdges.Contains(edge))
                    {
                        this.OrphanEdges.Add(edge);
                    }
                }

                // We need to handle directed and undirected graphs differently
                if (this.type == GraphType.Directed)
                {
                    // Confirm that the Source node exists
                    if (this.ContainsNode(edge.Source) && !(edge.Target is GhostNode))
                    {
                        // Add the edge to the outgoing edges collection
                        // for the source node
                        (this.nodeEdges[edge.Source].Value as EdgeContainer).AddOutgoingEdge(edge);
                    }

                    // Confirm that the Target node exists
                    if (this.ContainsNode(edge.Target) && !(edge.Source is GhostNode))
                    {
                        // Add the edge to the incoming edge collection for
                        // the target node
                        (this.nodeEdges[edge.Target].Value as EdgeContainer).AddIncomingEdge(edge);
                    }

                    // Fire the EdgeAdded event and increment edge count
                    OnEdgeAdded(new EdgeEventArgs(edge, null));
                    this.edgeCount++;

                    // Handle the nodes PropertyChanged event
                    (edge as Edge).PropertyChanged += new EventHandler<Common.Events.PropertyChangedEventArgs<object>>(Edge_PropertyChanged);

                    return true;
                }

                return false;
            }

            /// <summary>
            /// Adds a range of Edges to the collection of nodes and edges
            /// </summary>
            /// <param name="edges">The collection of edges to be added</param>
            /// <returns>the number of edges that were successfully added</returns>
            public int AddEdges(IEnumerable<IEdge> edges)
            {
                int edgesAdded = 0;

                // Loop through each edge in the provided
                // edges collection
                foreach (IEdge edge in edges)
                {
                    // Add the node by calling the standard
                    // AddNode method
                    if (AddEdge(edge))
                        edgesAdded++;
                }

                return edgesAdded;
            }

            /// <summary>
            /// Removes the provided edge form the graph
            /// </summary>
            /// <param name="edge">The edge to be removed</param>
            /// <returns>true if the edge was removed; otherwise false</returns>
            public bool RemoveEdge(IEdge edge)
            {
                bool sourceEdgeRemoved = false;
                bool targetEdgeRemoved = false;

                if (this.type == GraphType.Directed)
                {
                    // Ensure the source node exists
                    if (this.nodeEdges.Contains(edge.Source))
                    {
                        // Get the edge container for the source node
                        EdgeContainer sourceEdges = this[edge.Source];

                        if (sourceEdges != null)
                            sourceEdgeRemoved = sourceEdges.RemoveEdge(edge);
                    }

                    // Ensure that the target node exists
                    if (this.nodeEdges.Contains(edge.Target))
                    {
                        // Get the edge container for the target node
                        EdgeContainer targetEdges = this[edge.Target];

                        if (targetEdges != null)
                            targetEdgeRemoved = targetEdges.RemoveEdge(edge);
                    }

                    // Verify that the edge was successfully removed
                    if (sourceEdgeRemoved || targetEdgeRemoved)
                    {
                        // Fire the EdgeRemoved event and decrement edge count
                        OnEdgeRemoved(new EdgeEventArgs(edge, null));
                        this.edgeCount--;

                        // Stop handling the property changed event
                        (edge as Edge).PropertyChanged -= new EventHandler<Common.Events.PropertyChangedEventArgs<object>>(Edge_PropertyChanged);

                        return true;
                    }
                    else
                        return false;

                }
                else
                    return false;

            }

            /// <summary>
            /// Removes a range of Edges from the graph
            /// </summary>
            /// <param name="edges">The collection of edges to be removed</param>
            /// <returns>the number of edges that were successfully removed</returns>
            public int RemoveEdges(IEnumerable<IEdge> edges)
            {
                int edgesRemoved = 0;

                // Loop through each edge in the provided
                // edges collection
                foreach (IEdge edge in edges)
                {
                    // Remove the edge by calling the regular
                    // RemoveEdge method
                    if (RemoveEdge(edge))
                        edgesRemoved++;
                }

                return edgesRemoved;
            }

            /// <summary>
            /// Determines if the provided edge is in the collection of nodes
            /// and edges
            /// </summary>
            /// <param name="edge">>The Node to look for</param>
            /// <returns>true if the Edge was found; otherwise false</returns>
            public bool ContainsEdge(IEdge edge)
            {
                if (edge.Source != null && this.nodeEdges.Keys.Contains(edge.Source))
                    return (this.nodeEdges[edge.Source].Value as EdgeContainer).ContainsOutgoingEdge(edge);
                else
                    return false;
            }

            /// <summary>
            /// Determines if an edge exists for the provided source and target
            /// nodes
            /// </summary>
            /// <param name="source">The source Node</param>
            /// <param name="target">The target Node</param>
            /// <returns>true if the Edge was found; otherwise false</returns>
            public bool ContainsEdge(INode source, INode target)
            {

                // This would be handled differently if the graph was
                // not directed
                if (this.type == GraphType.Directed)
                {
                    // Find any outgoing edges with the provided source node
                    foreach (Edge outgoingEdge in (this.nodeEdges[source].Value as EdgeContainer).OutgoingEdges)
                    {
                        // Check if this edge has the provided target node
                        if (outgoingEdge.Target.Equals(target))
                            return true;
                    }
                }

                return false;
            }

            /// <summary>
            /// Determines if the provided edge is a duplex edge.  A
            /// duplex edge is one where two edges exist where their
            /// source and target nodes are inverted.
            /// </summary>
            /// <param name="edge"></param>
            /// <returns>true if the edge is a duplex edge; otherwise false</returns>
            public bool IsDuplexEdge(IEdge edge)
            {
                // Check if the edge is null
                if (edge == null)
                    return false;

                // Check if the edge exists on the GraphData(
                if (ContainsEdge(edge))
                {
                    // Check if an edge exists that has the inverse
                    // of the source edge's target and source
                    if (ContainsEdge(edge.Target, edge.Source))
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }

            /// <summary>
            /// Returns the specified nodes Degree related to incomming edges.  A nodes
            /// degree referes to the number of edges that it has.
            /// </summary>
            /// <param name="node">A Berico.LinkAnalysis.Model.Node</param>
            /// <returns>the number of incomming edges for the provided node</returns>
            public int InDegree(INode node)
            {
                return (this.nodeEdges[node].Value as EdgeContainer).IncommingEdges.Count;
            }

            /// <summary>
            /// Returns the specified nodes Degree related to outgoing edges.  A nodes
            /// degree referes to the number of edges that it has.
            /// </summary>
            /// <param name="node">A Berico.LinkAnalysis.Model.Node</param>
            /// <returns>the number of outgoing edges for the provided node</returns>
            public int OutDegree(INode node)
            {
                return (this.nodeEdges[node].Value as EdgeContainer).OutgoingEdges.Count;
            }

            /// <summary>
            /// Returns the specified nodes Degree.  A nodes
            /// degree referes to the number of edges that it has.
            /// </summary>
            /// <param name="node">A Berico.LinkAnalysis.Model.Node</param>
            /// <returns>the number of edges for the provided node</returns>
            public int Degree(INode node)
            {
                return InDegree(node) + OutDegree(node);
            }

            /// <summary>
            /// Returns a collection of all the incomming edges.  An incomming edge is 
            /// an edge from a target to a source (from the perspective of the source).
            /// </summary>
            /// <param name="node">A Berico.LinkAnalysis.Model.Node object</param>
            /// <returns>A list of Berico.LinkAnalysis.Model.Edge objects</returns>
            public IEnumerable<IEdge> InEdges(INode node)
            {
                return (this.nodeEdges[node].Value as EdgeContainer).IncommingEdges;
            }

            /// <summary>
            /// Returns a collection of all the outgoing edges.  An outgoing edge is an  
            /// edge from a source to a target (from the perspective of the source).
            /// </summary>
            /// <param name="node">A Berico.LinkAnalysis.Model.Node object</param>
            /// <returns>A list of Berico.LinkAnalysis.Model.Edge objects</returns>
            public IEnumerable<IEdge> OutEdges(INode node)
            {
                return (this.nodeEdges[node].Value as EdgeContainer).OutgoingEdges;
            }

            /// <summary>
            /// Returns a collection of all edges (both incomming and outgoing).  Since incoming
            ///  and outgoing edges are copies of each other, this list will contain duplicates.
            /// </summary>
            /// <param name="node">A Berico.LinkAnalysis.Model.Node object</param>
            /// <returns>A list of Berico.LinkAnalysis.Model.Edge objects if the specified node is on
            /// the graph, otherwise null</returns>
            public IEnumerable<IEdge> Edges(INode node)
            {
                if (node == null)
                {
                    throw new ArgumentNullException("node");
                }

                if (nodeEdges.Contains(node))
                {
                    return (this.nodeEdges[node].Value as EdgeContainer).Edges;
                }

                return null;
            }

            #region Events

                /// <summary>
                /// Handles the PropertyChanged event for an Edge
                /// </summary>
                /// <param name="sender">The Edge that fired the event</param>
                /// <param name="e">The arguments for the event</param>
                void Edge_PropertyChanged(object sender, Common.Events.PropertyChangedEventArgs<object> e)
                {
                    // We need to pass this event on so we are going to fire
                    // a new, more specific, event and provide the Edge
                    // whose property has changed.
                    OnEdgePropertyChanged(new EdgeEventArgs(sender as Edge, e));
                }

                /// <summary>
                /// Fires the EdgePropertyChanged event
                /// </summary>
                /// <param name="edge">The edge whose property was changed</param>
                /// <param name="propertyName">The name of the property that changed</param>
                public virtual void OnEdgePropertyChanged(EdgeEventArgs args)
                {
                    this.eventAggregator.GetEvent<EdgePropertyChangedEvent>().Publish(args);
                }

                /// <summary>
                /// Fires the EdgeAdded event
                /// </summary>
                /// <param name="edge">The edge that was added</param>
                public virtual void OnEdgeAdded(EdgeEventArgs args)
                {
                    this.eventAggregator.GetEvent<EdgeAddedEvent>().Publish(args);
                }

                /// <summary>
                /// Fires the EdgeAdded event
                /// </summary>
                /// <param name="edge">The edge that was removed</param>
                public virtual void OnEdgeRemoved(EdgeEventArgs args)
                {
                    this.eventAggregator.GetEvent<EdgeRemovedEvent>().Publish(args);
                }

            #endregion

        #endregion

        /// <summary>
        /// Returns the index and entry in the collection for the item
        /// with the provided node
        /// </summary>
        /// <param name="node">The node to return the information for</param>
        /// <param name="entry"></param>
        /// <returns>the index of the entry with the given key; -1 if no entry is found</returns>
        private int GetIndexAndEntry(INode node, out DictionaryEntry entry)
        {
            entry = new DictionaryEntry();
            int index = -1;

            // Check if the node exists in the internal collection
            if (this.nodeEdges.Contains(node))
            {
                entry = this.nodeEdges[node];
                index = this.nodeEdges.IndexOf(entry);
            }

            return index;
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (CollectionChanged != null)
                CollectionChanged(this, args);
        }

        #region INotifyCollectionChanged Members

            public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region IScopingContainer<string> Members

            private string scope = string.Empty;
            public string Scope
            {
                get { return this.scope; }
                private set { this.scope = value; }
            }

        #endregion
    }
}