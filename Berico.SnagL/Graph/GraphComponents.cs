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
using System.Collections.ObjectModel;
using System.Linq;
using Berico.Common.Events;
using Berico.SnagL.Infrastructure.Data;
using Berico.SnagL.Infrastructure.Events;
using Berico.SnagL.Infrastructure.Graph.Events;
using Berico.SnagL.Model;
using Berico.SnagL.UI;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Command;

namespace Berico.SnagL.Infrastructure.Graph
{
    /// <summary>
    /// Responsible for creating and destroying components related
    /// to the Graph.  'Components' refers to the raw graph data
    /// (nodes and edges) and the classes that represent their
    /// physical counterparts.
    /// 
    /// This class should be used whenever nodes need to be added
    /// or removed from the graph.
    /// </summary>
    public class GraphComponents : IScopingContainer<string>, INotifyPropertyChanged<object>
    {
        #region Fields

        private readonly SnaglEventAggregator eventAggregator = SnaglEventAggregator.DefaultInstance;
        private ObservableCollection<INodeShape> nodeViewModels = new ObservableCollection<INodeShape>();
        private NodeSelector nodeSelector = new NodeSelector();
        private NodeFilter nodeFilter = new NodeFilter();
        private GraphData graphData = null;
        private NodeTypes nodeType = NodeTypes.Text;

        
        private Berico.Windows.Controls.MenuItem hideSelectedMenuItem = null;
        private Berico.Windows.Controls.MenuItem hideUnselectedMenuItem = null;
        private Berico.Windows.Controls.MenuItem hideCurrentNodeMenuItem = null;
        private Berico.Windows.Controls.MenuItem showAllMenuItem = null;

        /// <summary>
        /// Contains a collection of edge view models that are missing corresponding nodes 
        /// </summary>
        private List<IEdgeViewModel> _orphanEdgeViewModels = new List<IEdgeViewModel>();

        /// <summary>
        /// A collection of Berico.SnagL.Infrastructure.Graph.IEdgeViewModel objects and 
        /// the Berico.SnagL.Model.IEdge that contains the data for it
        /// </summary>
        private Dictionary<IEdge, IEdgeViewModel> edgeToEdgeViewModel = new Dictionary<IEdge, IEdgeViewModel>();

        /// <summary>
        /// A collection of Berico.SnagL.Infrastructure.Graph.NodeViewModelBase objects
        /// and the Berico.SnagL.Model.Node that contains the data for it
        /// </summary>
        private Dictionary<INode, INodeShape> nodeToNodeViewModel = new Dictionary<INode, INodeShape>();

        #endregion

        #region Properties

            public IEnumerable<INode> Nodes
            {
                get
                {
                    if (this.graphData != null)
                    {
                        return this.graphData.Nodes;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            /// <summary>
            /// Gets a collection of all the current node and edge view models
            /// </summary>
            public ObservableCollection<INodeShape> NodeViewModels
            {
                get
                {
                    return nodeViewModels;
                }
            }

            /// <summary>
            /// Gets or sets the type of node to show on the graph
            /// </summary>
            public NodeTypes NodeType
            {
                get { return this.nodeType; }
                set { this.nodeType = value; }
            }

            /// <summary>
            /// Gets the NodeSelector instance which is responsible
            /// for handling node selections for the graph
            /// </summary>
            public NodeSelector NodeSelector
            {
                get { return this.nodeSelector; }
            }

            /// <summary>
            /// Gets the NodeFilter instance, which is responsible
            /// for handling node filtering for the graph
            /// </summary>
            public NodeFilter NodeFilter
            {
                get { return this.nodeFilter; }
            }

            /// <summary>
            /// Gets or sets the data portion of the graph components
            /// </summary>
            public GraphData Data
            {
                get
                {
                    return this.graphData;
                }
                set
                {
                    GraphData oldData = this.graphData;
                    this.graphData = value;

                    NotifyPropertyChanged("Data", oldData, value);
                }
            }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the GraphComponents class
        /// </summary>
        public GraphComponents()
            : this(Guid.NewGuid().ToString())
        {
        }

        /// <summary>
        /// Initializes a new instance of the GraphComponents class using
        /// the specified scope ID.
        /// </summary>
        /// <param name="_scope"An ID that can be used to identify the data</param>
        public GraphComponents(string _scope)
        {
            this.scope = _scope;
            Data = new GraphData(this.scope);

            // Graph context menu items
            hideSelectedMenuItem = new Berico.Windows.Controls.MenuItem(1) { Header = "Hide Selected Nodes" };
            hideSelectedMenuItem.Clicked += new System.Windows.RoutedEventHandler(HideSelectedMenuitemClicked);
            hideUnselectedMenuItem = new Berico.Windows.Controls.MenuItem(1) { Header = "Hide Unselected Nodes" };
            hideUnselectedMenuItem.Clicked += new System.Windows.RoutedEventHandler(HideUnselectedMenuItemClicked);
            showAllMenuItem = new Berico.Windows.Controls.MenuItem(2) { Header = "Show All" };
            showAllMenuItem.Clicked += new System.Windows.RoutedEventHandler(ShowAllMenuItemClicked);

            // Node context menu items
            hideCurrentNodeMenuItem = new Berico.Windows.Controls.MenuItem(1) { Header = "Hide This Node" };
            hideCurrentNodeMenuItem.Clicked += new System.Windows.RoutedEventHandler(HideCurrentNodeMenuItemClicked);

            ContextMenuManager.Instance.GraphContextMenuOpening += new EventHandler<EventArgs>(GraphContextMenuOpeningEventHandler);
            ContextMenuManager.Instance.NodeContextMenuOpening += new EventHandler<ContextMenuEventArgs>(NodeContextMenuOpeningEventHandler);
        }

        #endregion

        /// <summary>
        /// Generates a random graph.  This is for testing and 
        /// development purposes only.
        /// </summary>
        public void GenerateGraph(int nodeCount, int edgeCount)
        {
            // Ensure that the graph is clear
            Clear();

            // Create a reference to the graph generator
            GraphGenerator generator = new GraphGenerator();

            Data = generator.GenerateGraph(this.scope, nodeCount, edgeCount);
          
            CreateVMs();
            
            // Fire the DataLoaded event
            DispatcherHelper.UIDispatcher.BeginInvoke(() =>
                this.eventAggregator.GetEvent<DataLoadedEvent>().Publish(new DataLoadedEventArgs(this.scope, CreationType.Imported))
                );
        }

        /// <summary>
        /// Clear all data in this class
        /// </summary>
        public void Clear()
        {
            if (this.nodeViewModels.Count > 0)
            {
                this.graphData.Clear();
                this.nodeViewModels.Clear();
                this.nodeToNodeViewModel.Clear();
                this.edgeToEdgeViewModel.Clear();
            }
        }

        /// <summary>
        /// Creates the view models (node and edge) for the generated
        /// graph.
        /// </summary>
        public void CreateVMs()
        {
            // Loop through all nodes in the model
            foreach (Node node in graphData.Nodes)
            {
                CreateNodeViewModel(node, "/Berico.SnagL;component/Resources/Icons/person.png");

                // Loop through all the edges that this node has
                foreach (IEdge edge in graphData.Edges(node))
                {
                    CreateEdgeViewModel(edge);
                }
            }
        }

        /// <summary>
        /// Gets a reference to a collection of nodes on the graph
        /// </summary>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HideSelectedMenuitemClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            // Filter out all nodes that ARE selected
            NodeSelector.InvertSelection();
            NodeFilter.Filter(new List<NodeViewModelBase>(NodeSelector.SelectedNodes));
            NodeSelector.TurnOffSelection();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HideCurrentNodeMenuItemClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            // Filter out all nodes that are NOT selected
            NodeFilter.Hide(targetNodeVM);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowAllMenuItemClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            NodeFilter.TurnOffFilter();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HideUnselectedMenuItemClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            NodeFilter.Filter(new List<NodeViewModelBase>(NodeSelector.SelectedNodes));
        }

        #region Accessing Graph Data

            /// <summary>
            /// Returns a collection of edges for the provided Node
            /// </summary>
            /// <param name="node">The Node to return the edges for</param>
            /// <returns>the collection of edges for the given Node</returns>
            public IEnumerable<IEdge> GetEdges(INode node)
            {
                if (this.graphData != null)
                    return this.graphData.Edges(node);
                else
                    return null;
            }

            /// <summary>
            /// Returns the number of edges for the provided Node
            /// </summary>
            /// <param name="node">The Node to count the edges for</param>
            /// <returns>the number of edges for the Node</returns>
            public int GetNumberOfEdges(INode node)
            {
                return graphData.Degree(node);
            }

        #endregion

        #region Node View Models

            /// <summary>
            /// Creates a NodeViewModel, for the provided Node, and adds it to the nodes
            /// view model collection.  The NodesViewModel collection is bound to the Graph
            /// control and displays and handles creating appropriate node views for each
            /// view model.
            /// </summary>
            /// <param name="node">The Node instance to create the NodeVIewModel for</param>
            private void CreateNodeViewModel(Node node, string imagePath)
            {
                // Ensure that the node view model doesn't already exist
                if (nodeToNodeViewModel.ContainsKey(node))
                    return;

                // Create new NodeViewModel for this node
                NodeViewModelBase nodeViewModel = NodeViewModelBase.GetNodeViewModel(this.nodeType, node, this.scope);

                // Setup the event handler to catch when a node's position has changed
                nodeViewModel.NodeMoved += new System.EventHandler<System.EventArgs>(nodeViewModel_NodeMoved);
                nodeViewModel.NodeLoaded += new System.EventHandler<System.EventArgs>(nodeViewModel_NodeLoaded);

                if (!string.IsNullOrEmpty(imagePath) && this.nodeType == NodeTypes.Icon)
                    (nodeViewModel as IconNodeViewModel).ImageSource = imagePath;

                nodeToNodeViewModel.Add(node, nodeViewModel);
                nodeViewModels.Add(nodeViewModel);
            }

            #region Adding

                /// <summary>
                /// Adds the provided node view model (and its ParentNode) to the graph
                /// </summary>
                /// <param name="nodeVM">The node view model that should be added to the graph</param>
                public void AddNodeViewModel(INodeShape nodeVM)
                {
                    // Validate the provided nodeVM
                    if (nodeVM == null)
                        throw new ArgumentNullException("NodeVM", "Invalid node view model provided");

                    // Skip the current view model if it already exists
                    if (this.nodeViewModels.Contains(nodeVM))
                        return;

                    // Determine if this is a PartitionNode
                    if (!(nodeVM is PartitionNode))
                    {
                        NodeViewModelBase targetNode = nodeVM as NodeViewModelBase;
                        INode parentNode = targetNode.ParentNode;

                        if (nodeToNodeViewModel.ContainsKey(targetNode.ParentNode))
                        {
                            return;
                        }
                        else
                        {
                            this.graphData.AddNode(parentNode);

                            // Process the orphan edges
                            ProcessOrphanEdges(parentNode);

                            // Setup event handlers for this node VM
                            targetNode.NodeMoved += new System.EventHandler<System.EventArgs>(nodeViewModel_NodeMoved);
                            targetNode.NodeLoaded += new System.EventHandler<System.EventArgs>(nodeViewModel_NodeLoaded);

                            nodeToNodeViewModel.Add(targetNode.ParentNode, nodeVM);
                        }
                    }
                    else
                    {
                        this.graphData.AddNode(nodeVM as PartitionNode);
                    }

                    nodeViewModels.Add(nodeVM);
                }

                /// <summary>
                /// Adds all of the provided node view models (and their ParentNodes) to the graph
                /// </summary>
                /// <param name="nodeVMs">The list of node view models to be added</param>
                public void AddNodeViewModels(List<INodeShape> nodeVMs)
                {
                    // Loop over the provided node view models
                    foreach (INodeShape currentNodeVM in nodeVMs)
                    {
                        AddNodeViewModel(currentNodeVM);
                    }
                }

            #endregion

            #region Removing

                /// <summary>
                /// Removes the provided node view model (and its parent node) from the graph
                /// </summary>
                /// <param name="nodeVM">The node view model to be removed from the graph</param>
                public void RemoveNodeViewModel(INodeShape nodeVM)
                {

                    // Validate the provided nodeVM
                    if (nodeVM == null)
                        throw new System.ArgumentNullException("NodeVM", "Invalid node view model provided");

                    // Make sure the node view model even exists before going
                    // through the motions to remove it
                    if (this.nodeViewModels.Contains(nodeVM))
                    {
                        // Ensure we are not dealing with a partition nodeVM
                        if (!(nodeVM is PartitionNode))
                        {
                            NodeViewModelBase targetNodeVM = nodeVM as NodeViewModelBase;

                            // Remove the parent node from the graph
                            this.graphData.RemoveNode(targetNodeVM.ParentNode);

                            // Remove event handlers for this node VM
                            targetNodeVM.NodeMoved -= new System.EventHandler<System.EventArgs>(nodeViewModel_NodeMoved);
                            targetNodeVM.NodeLoaded -= new System.EventHandler<System.EventArgs>(nodeViewModel_NodeLoaded);

                            // Remove the view model from the internal collections
                            nodeToNodeViewModel.Remove(targetNodeVM.ParentNode);
                        } else
                            this.graphData.RemoveNode(nodeVM as PartitionNode);

                        nodeViewModels.Remove(nodeVM);
                    }

                }

                /// <summary>
                /// Removes all of the provided node view models (and their ParentNodes) from the graph
                /// </summary>
                /// <param name="nodeVMs">The list of node view models to be removed</param>
                public void RemoveNodeViewModels(List<NodeViewModelBase> nodeVMs)
                {
                    // Loop over the provided node view models
                    foreach (NodeViewModelBase currentNodeVM in nodeVMs)
                    {
                        RemoveNodeViewModel(currentNodeVM);
                    }
                }

            #endregion

            #region Events

                /// <summary>
                /// Handles the NodeLoaded event which indicates that a node
                /// has been loaded onto the graph
                /// </summary>
                /// <param name="sender">The object that fired the event</param>
                /// <param name="e">The arguments for the event</param>
                private void nodeViewModel_NodeLoaded(object sender, System.EventArgs e)
                {
                    // Obtain an instance to the node's view model
                    NodeViewModelBase nodeViewModel = sender as NodeViewModelBase;

                    // Get a reference to all edges associated with this node
                    foreach (IEdge edge in graphData.Edges(nodeViewModel.ParentNode))
                    {
                        // Get the EdgeViewModel for this edge
                        if (!edgeToEdgeViewModel.ContainsKey(edge))
                            continue;

                        // Obtain an instance to this edge's view model
                        IEdgeViewModel edgeViewModel = edgeToEdgeViewModel[edge];

                        // Check if the current node is the source or target
                        // so that we know which part of the edge line to
                        // update.  Our rule of thumb is that we draw the line
                        // from source to target.
                        if (edge.Source.Equals(nodeViewModel.ParentNode))
                        {
                            // Update the edge line properties
                            edgeViewModel.X1 = nodeViewModel.CenterPoint.X;
                            edgeViewModel.Y1 = nodeViewModel.CenterPoint.Y;
                        }
                        else
                        {
                            // Update the edge line properties
                            edgeViewModel.X2 = nodeViewModel.CenterPoint.X;
                            edgeViewModel.Y2 = nodeViewModel.CenterPoint.Y;
                        }
                    }
                }

                /// <summary>
                /// Handles the NodeMoved event which indicates that a node's
                /// position has changed
                /// </summary>
                /// <param name="sender">The object that fired the event</param>
                /// <param name="e">The arguments for the event</param>
                private void nodeViewModel_NodeMoved(object sender, System.EventArgs e)
                {
                    //TODO:  THIS CODE EXACTLY DUPLICATES THE LAYOUTUPDATED CODE!!

                    // Obtain an instance to the node's view model
                    NodeViewModelBase nodeViewModel = sender as NodeViewModelBase;

                    // Get a reference to all edges associated with this node
                    foreach (IEdge edge in graphData.Edges(nodeViewModel.ParentNode))
                    {
                        // Get the EdgeViewModel for this edge
                        if (!edgeToEdgeViewModel.ContainsKey(edge))
                            continue;

                        // Obtain an instance to this edge's view model
                        IEdgeViewModel edgeViewModel = edgeToEdgeViewModel[edge];

                        // Check if the current node is the source or target
                        // so that we know which part of the edge line to
                        // update.  Our rule of thumb is that we draw the line
                        // from source to target.
                        if (edge.Source.Equals(nodeViewModel.ParentNode))
                        {
                            // Update the edge line properties
                            edgeViewModel.X1 = nodeViewModel.CenterPoint.X;
                            edgeViewModel.Y1 = nodeViewModel.CenterPoint.Y;
                        }
                        else
                        {
                            // Update the edge line properties
                            edgeViewModel.X2 = nodeViewModel.CenterPoint.X;
                            edgeViewModel.Y2 = nodeViewModel.CenterPoint.Y;
                        }
                    }
                }

            #endregion

            #region Accessing

                /// <summary>
                /// Returns a collection of all the node view models
                /// on the graph
                /// </summary>
                /// <returns>a collection of node view models</returns>
                public IEnumerable<INodeShape> GetNodeViewModels()
                {
                    //return this.nodeToNodeViewModel.Values;
                    return this.nodeViewModels;
                }

                /// <summary>
                /// Returns the view model for the provided Node
                /// </summary>
                /// <param name="node">The Node whose view model is being
                /// requested</param>
                /// <returns>the view model for the provided node</returns>
                public INodeShape GetNodeViewModel(INode node)
                {
                    INodeShape nodeVM = null;

                    if (this.nodeToNodeViewModel.TryGetValue(node, out nodeVM))
                        return nodeVM;

                    return null;
                }

                /// <summary>
                /// Returns the node view model for the node on the opposite
                /// end of edge from the provided node
                /// </summary>
                /// <param name="edge">The edge containg the provided node</param>
                /// <param name="node">The Node opposite of the node the
                /// we are looking for</param>
                /// <returns>the view model for the node at the opposite
                /// end of the provided edge</returns>
                public INodeShape GetOppositeNode(IEdge edge, INode node)
                {
                    //TODO:  CLEAN THIS UP A LITTLE

                    // Check if the Node provided is the edges source node
                    if (edge.Source == node)
                    {
                        // Try and get the view model for the node
                        if (this.nodeToNodeViewModel.ContainsKey(edge.Target))
                            return this.nodeToNodeViewModel[edge.Target];
                        else
                            if (this.Data.ContainsNode(edge.Target) && edge.Target is PartitionNode)
                                return this.Data.GetNode(edge.Target.ID) as PartitionNode;
                            else
                                throw new System.ArgumentException("Provided node is not in the graph");
                    } // Check if the Node provided is the edges target node
                    else if (edge.Target == node)
                    {
                        // Try and get the view model for the node
                        if (this.nodeToNodeViewModel.ContainsKey(edge.Source))
                            return this.nodeToNodeViewModel[edge.Source];
                        else
                            if (this.Data.ContainsNode(edge.Source) && edge.Source is PartitionNode)
                                return this.Data.GetNode(edge.Source.ID) as PartitionNode;
                            else
                                throw new System.ArgumentException("Provided node is not in the graph");
                    }
                    else
                    {
                        // This indicates that the provided node is not part
                        // of the provided edge
                        throw new System.ArgumentException("Attempted to get the opposite end of the provided node that isn't part of the provided edge");
                    }
                }

            #endregion

        #endregion

        //TODO:  WE SHOULD HAVE EVENTS FOR WHEN NODE VIEWMODELS ARE REMOVED OR ADDED

        #region Edge View Models

            /// <summary>
            /// Creates a new EdgeViewModel
            /// </summary>
            /// <param name="edge">The IEdge instance that is responsible
            /// for the edge view model being created</param>
            private void CreateEdgeViewModel(IEdge edge)
            {
                IEdgeViewModel edgeViewModel;
                bool isNew = true;

                // Check if the edge view model already exists
                if (edgeToEdgeViewModel.ContainsKey(edge))
                    isNew = false;

                // Make sure that the edge exists in the graph data structure
                if (!graphData.ContainsEdge(edge))
                {
                    // Since the edge doesn't exist in the graph
                    // data structure, we need to add it
                    graphData.AddEdge(edge);
                }

                // Create a new edge viewmodel
                if (isNew)
                    edgeViewModel = EdgeViewModelBase.GetEdgeViewModel(edge, graphData.Scope);
                else
                    edgeViewModel = edgeToEdgeViewModel[edge];

                // Get the source node view model
                INodeShape sourceNodeViewModel = null;
                INodeShape targetNodeViewModel = null;

                nodeToNodeViewModel.TryGetValue(edgeViewModel.ParentEdge.Source, out sourceNodeViewModel);
                nodeToNodeViewModel.TryGetValue(edgeViewModel.ParentEdge.Target, out targetNodeViewModel);

                if (sourceNodeViewModel != null)
                {
                    edgeViewModel.X1 = sourceNodeViewModel.CenterPoint.X;
                    edgeViewModel.Y2 = sourceNodeViewModel.CenterPoint.Y;
                }

                if (targetNodeViewModel != null)
                {
                    edgeViewModel.X1 = targetNodeViewModel.CenterPoint.X;
                    edgeViewModel.Y2 = targetNodeViewModel.CenterPoint.Y;
                }

                // If this is a new node, add it
                if (isNew)
                {
                    // Add the edge to the edge/edgeviewmodel collection
                    edgeToEdgeViewModel.Add(edge, edgeViewModel);

                    // Fire the EdgeViewModelRemoved event
                    OnEdgeViewModelAdded(new EdgeViewModelEventArgs(edgeViewModel, this.scope));
                }
            }

            #region Adding

                /// <summary>
                /// Adds the provided edgeviewmodel (and it's parent edge)
                /// to the graph
                /// </summary>
                /// <param name="edgeVMs">The edge view model to be added to the graph</param>
                public void AddEdgeViewModel(IEdgeViewModel edgeVM)
                {
                    // Add the ParentEdge for this view model.  The AddEdge method
                    // will check if the edge already exists.
                    this.graphData.AddEdge(edgeVM.ParentEdge);

                    // Check if the edge view model already exists
                    if (!this.edgeToEdgeViewModel.ContainsKey(edgeVM.ParentEdge))
                    {
                        if (graphData.OrphanEdges.Contains(edgeVM.ParentEdge))
                        {
                            _orphanEdgeViewModels.Add(edgeVM);
                        }
                        else
                        {
                            // Now we need to update the edge to edgeviewmodels collection
                            edgeToEdgeViewModel.Add(edgeVM.ParentEdge, edgeVM);

                            // Fire the EdgeViewModelAdded event
                            OnEdgeViewModelAdded(new EdgeViewModelEventArgs(edgeVM, this.scope));
                        }
                    }
                }

                /// <summary>
                /// Adds all the provided edgeviewmodels (and there parent edges)
                /// to the graph
                /// </summary>
                /// <param name="edgeVMs">A list of edge viewmodels that need to be added 
                /// to the graph</param>
                public void AddEdgeViewModels(List<IEdgeViewModel> edgeVMs)
                {
                    // Loop over the edge view models to be added
                    foreach (IEdgeViewModel edgeVM in edgeVMs)
                    {
                        AddEdgeViewModel(edgeVM);
                    }
                }

            #endregion

            #region Removing

                /// <summary>
                /// Removes the edge view model for the given edge (and the edge
                /// itself)
                /// </summary>
                /// <param name="edge">The edge whose view model should be removed</param>
                public void RemoveEdgeViewModel(IEdge edge)
                {
                    IEdgeViewModel edgeVM = null;

                    // Try and get the edge view model for the provided edge
                    if (this.edgeToEdgeViewModel.TryGetValue(edge, out edgeVM))
                    {
                        // Remove the edge view model for this edge
                        RemoveEdgeViewModel(edgeVM);
                    }
                }

                /// <summary>
                /// Removes the edge view model from the graph
                /// </summary>
                /// <param name="edgeVM">The edge view model to be removed</param>
                public void RemoveEdgeViewModel(IEdgeViewModel edgeVM)
                {
                    // Remove the parent edge
                    this.graphData.RemoveEdge(edgeVM.ParentEdge);

                    // Ensure that the provided edge view model exists
                    if (this.edgeToEdgeViewModel.ContainsKey(edgeVM.ParentEdge))
                    {
                        // Remove the edge view model from the collection
                        this.edgeToEdgeViewModel.Remove(edgeVM.ParentEdge);

                        // Fire the EdgeViewModelRemoved event
                        OnEdgeViewModelRemoved(new EdgeViewModelEventArgs(edgeVM, this.scope));
                    }
                }

                /// <summary>
                /// Removes all of the provided edge view models from the graph
                /// </summary>
                /// <param name="edgeVMs">The list of edge view models to be removed</param>
                public void RemoveEdgeViewModels(List<IEdgeViewModel> edgeVMs)
                {
                    // Loop over the edge view models
                    foreach (IEdgeViewModel edgeVM in edgeVMs)
                    {
                        RemoveEdgeViewModel(edgeVM);
                    }
                }

                /// <summary>
                /// Removes all the provided edges from the graph
                /// </summary>
                /// <param name="edges">The list of edges that should be removed</param>
                public void RemoveEdgeViewModels(List<IEdge> edges)
                {
                    // Loop over the edges
                    foreach (IEdge edge in edges)
                    {
                        RemoveEdgeViewModel(edge);
                    }
                }

                /// <summary>
                /// Removes all edge view models (and edges), of the specified type, from the graph
                /// </summary>
                /// <typeparam name="T"></typeparam>
                /// <param name="node"></param>
                public void RemoveEdgeViewModels<T>(INode node) where T : class, IEdge
                {
                    List<IEdge> edgesToBeRemoved = new List<IEdge>();

                    // Loop over all the edges that this node has
                    foreach (IEdge edge in this.graphData.Edges(node))
                    {
                        // Determine if the current edge matches the 
                        // type of edge that we are looking for
                        if (edge is T)
                        {
                            // Add the edge to the list of edges that should be removed
                            edgesToBeRemoved.Add(edge as T);
                        }
                    }

                    // If We have any edges to remove, remove them
                    if (edgesToBeRemoved.Count > 0)
                        RemoveEdgeViewModels(edgesToBeRemoved);
                }

            #endregion

            #region Accessing

                /// <summary>
                /// Returns the view model for the provided Edge
                /// </summary>
                /// <param name="node">The Edge whose view model is being
                /// requested</param>
                /// <returns>the view model for the provided edge</returns>
                public IEdgeViewModel GetEdgeViewModel(IEdge edge)
                {
                    IEdgeViewModel edgeVM = null;

                    if (this.edgeToEdgeViewModel.TryGetValue(edge, out edgeVM))
                        return edgeVM;

                    return null;
                }

                /// <summary>
                /// Returns a collection of all edge view models
                /// </summary>
                /// <returns></returns>
                public IEnumerable<IEdgeViewModel> GetEdgeViewModels()
                {
                    return this.edgeToEdgeViewModel.Values;
                }

                /// <summary>
                /// Returns a collection of edge view models for the provided
                /// Node
                /// </summary>
                /// <param name="node">The node to return edges for</param>
                /// <returns>A collection of edge view models</returns>
                public IEnumerable<IEdgeViewModel> GetEdgeViewModels(Node node)
                {
                    foreach (IEdge edge in graphData.Edges(node))
                    {
                        yield return edgeToEdgeViewModel[edge];
                    }
                }

            #endregion

            #region Events

                /// <summary>
                /// Raises the EdgeViewModelAdded event
                /// </summary>
                /// <param name="e">The event arguments</param>
                protected virtual void OnEdgeViewModelAdded(EdgeViewModelEventArgs args)
                {
                    this.eventAggregator.GetEvent<EdgeViewModelAddedEvent>().Publish(args);
                }

                /// <summary>
                /// Raises the EdgeViewModelRemoved event
                /// </summary>
                /// <param name="e">The event arguments</param>
                protected virtual void OnEdgeViewModelRemoved(EdgeViewModelEventArgs args)
                {
                    this.eventAggregator.GetEvent<EdgeViewModelRemovedEvent>().Publish(args);
                }

                /// <summary>
                /// Fires the PropertyChanged event
                /// </summary>
                /// <param name="info">A string that contains the name of the property that has changed</param>
                /// <param name="oldValue">The old value</param>
                /// <param name="newValue">The new value</param>
                protected void NotifyPropertyChanged(string info, object oldValue, object newValue)
                {
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs<object>(info, newValue, oldValue));
                }
            #endregion

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public void GraphContextMenuOpeningEventHandler(object sender, EventArgs args)
        {
            if (NodeSelector.SelectedNodes.Count > 0)
            {
                hideSelectedMenuItem.IsEnabled = true;
                hideUnselectedMenuItem.IsEnabled = true;

                ContextMenuManager.Instance.AddGraphMenuItem(hideSelectedMenuItem);
                ContextMenuManager.Instance.AddGraphMenuItem(hideUnselectedMenuItem);
            }
            else
            {
                hideSelectedMenuItem.IsEnabled = false;
                hideUnselectedMenuItem.IsEnabled = false;

                ContextMenuManager.Instance.AddGraphMenuItem(hideSelectedMenuItem);
                ContextMenuManager.Instance.AddGraphMenuItem(hideUnselectedMenuItem);
            }
                
            if (NodeFilter.IsActive)
            {
                showAllMenuItem.IsEnabled = true;
                ContextMenuManager.Instance.AddGraphMenuItem(showAllMenuItem);
            }
            else
            {
                showAllMenuItem.IsEnabled = false;
                ContextMenuManager.Instance.AddGraphMenuItem(showAllMenuItem);
            }
        }

        public void NodeContextMenuOpeningEventHandler(object sender, ContextMenuEventArgs args)
        {
            hideCurrentNodeMenuItem.IsEnabled = true;
            ContextMenuManager.Instance.AddNodeMenuItem(hideCurrentNodeMenuItem);

            targetNodeVM = args.Source as NodeViewModelBase;
        }
        private NodeViewModelBase targetNodeVM = null;

        #region IScopingContainer<string> Members

            private string scope = string.Empty;
            public string Scope
            {
                get { return this.scope; }
                private set { this.scope = value; }
            }

        #endregion

        #region INotifyPropertyChanged<object> Members

            public event System.EventHandler<PropertyChangedEventArgs<object>> PropertyChanged;

        #endregion

        #region Private Methods

        /// <summary>
        /// Processes the orphan nodes
        /// </summary>
        /// <param name="node">The node for which to update the orphan edges for</param>
        private void ProcessOrphanEdges(INode node)
        {
            List<IEdgeViewModel> processedEdges = new List<IEdgeViewModel>();

            if (_orphanEdgeViewModels.Count < 1)
            {
                return;
            }

            var orphanVMs = from o in _orphanEdgeViewModels
                            where o.ParentEdge.Source.ID == node.ID || o.ParentEdge.Target.ID == node.ID
                            select o;

            foreach (IEdgeViewModel edge in orphanVMs)
            {
                if (edge.ParentEdge.Source.ID == node.ID)
                {
                    edge.ParentEdge.Source = node;
                }
                else if (edge.ParentEdge.Target.ID == node.ID)
                {
                    edge.ParentEdge.Target = node;
                }

                if (!(edge.ParentEdge.Source is GhostNode) && !(edge.ParentEdge.Target is GhostNode))
                {
                    processedEdges.Add(edge);
                }
            }

            AddEdgeViewModels(processedEdges);

            foreach (IEdgeViewModel edge in processedEdges)
            {
                _orphanEdgeViewModels.Remove(edge);
            }
        }

        #endregion
    }
}