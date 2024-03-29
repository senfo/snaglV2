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
using System.Windows.Browser;
using Berico.SnagL.Infrastructure.Controls;
using Berico.SnagL.Infrastructure.Controls.ViewModels;
using Berico.SnagL.Infrastructure.Controls.Views;
using Berico.SnagL.Infrastructure.Data;
using Berico.SnagL.Infrastructure.Data.Formats;
using Berico.SnagL.Infrastructure.Data.Mapping;
using Berico.SnagL.Infrastructure.Data.Mapping.JS;
using Berico.SnagL.Infrastructure.Events;
using Berico.SnagL.Infrastructure.Graph;
using Berico.SnagL.Infrastructure.Graph.Events;
using Berico.SnagL.Infrastructure.Layouts;
using Berico.SnagL.Infrastructure.Modularity.Contracts;
using Berico.SnagL.Infrastructure.Modularity.Toolbar;
using Berico.SnagL.Model;
using Berico.SnagL.UI;
using Berico.Windows.Controls;

namespace Berico.SnagL.Infrastructure.Interop
{
    /// <summary>
    /// Provides interop with the host through JavaScript
    /// </summary>
    public class ScriptableGraph
    {
        #region Fields

        /// <summary>
        /// Stores a value indicating whether or not external resources have been loaded
        /// </summary>
        private static bool _externalResourcesLoaded;

        /// <summary>
        /// Stores a value indicating whether or not the graph has been loaded
        /// </summary>
        private static bool _graphLoaded;

        /// <summary>
        /// The extensions on the toolbar
        /// </summary>
        private IEnumerable<IToolbarItemViewExtension> _toolbarExtensions;

        /// <summary>
        /// The custom message dialog control
        /// </summary>
        private readonly CustomMessageDialog _customMessageDialog = new CustomMessageDialog();

        /// <summary>
        /// The custom message modal dialog view model
        /// </summary>
        private readonly CustomMessageDialogViewModel _customMessageDialogVm = new CustomMessageDialogViewModel();

        /// <summary>
        /// Stores a reference to the object responsible for loading live data
        /// </summary>
        private readonly ILiveData _liveData = new LiveData();

        #endregion

        #region Events

        /// <summary>
        /// Occurs when SnagL has finished loading
        /// </summary>
        [ScriptableMember]
        public event EventHandler<EventArgs> SnaglLoaded;

        /// <summary>
        /// Occurs when a node is double-clicked
        /// </summary>
        [ScriptableMember]
        public event EventHandler<ScriptableNodeEventArgs> NodeDoubleClicked;

        /// <summary>
        /// Occurs when the mouse cursor enters a node
        /// </summary>
        [ScriptableMember]
        public event EventHandler<ScriptableNodeEventArgs> NodeMouseEnter;

        /// <summary>
        /// Occurs when the mouse cursor leaves a node
        /// </summary>
        [ScriptableMember]
        public event EventHandler<ScriptableNodeEventArgs> NodeMouseLeave;

        /// <summary>
        /// Occurs when the mouse cursor enters an edge
        /// </summary>
        [ScriptableMember]
        public event EventHandler<ScriptableEdgeEventArgs> EdgeMouseEnter;

        /// <summary>
        /// Occurs when the mouse cursor leaves an edge
        /// </summary>
        [ScriptableMember]
        public event EventHandler<ScriptableEdgeEventArgs> EdgeMouseLeave;

        /// <summary>
        /// Occurs when the SnagL graph is resized
        /// </summary>
        [ScriptableMember]
        public event EventHandler<EventArgs> GraphResized;

        /// <summary>
        /// Occurs when SnagL has finished loading data
        /// </summary>
        [ScriptableMember]
        public event EventHandler<DataLoadedEventArgs> DataLoaded;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a reference to all the nodes on the graph
        /// </summary>
        [ScriptableMember(ScriptAlias = "nodes")]
        public IEnumerable<INode> Nodes
        {
            get
            {
                return GraphManager.Instance.DefaultGraphComponentsInstance.Data.Nodes;
            }
        }

        /// <summary>
        /// Gets the total number of nodes that exist on the graph
        /// </summary>
        [ScriptableMember(ScriptAlias = "nodeCount")]
        public int NodeCount
        {
            get
            {
                return GraphManager.Instance.DefaultGraphComponentsInstance.Data.Nodes.Count();
            }
        }

        /// <summary>
        /// Gets the total number of edges that exist on the graph
        /// </summary>
        [ScriptableMember(ScriptAlias = "edgeCount")]
        public int EdgeCount
        {
            get
            {
                return GraphManager.Instance.DefaultGraphComponentsInstance.Data.Size;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not live data is currently enabled
        /// </summary>
        [ScriptableMember(ScriptAlias = "liveEnabled")]
        public bool LiveEnabled
        {
            get
            {
                return GraphManager.Instance.LiveEnabled;
            }
            set
            {
                GraphManager.Instance.LiveEnabled = value;
            }
        }

        /// <summary>
        /// Gets a value indicating the current graph pan
        /// </summary>
        [ScriptableMember(ScriptAlias = "pan")]
        public Point Pan
        {
            get
            {
                return ViewModelLocator.GraphDataStatic.Pan;
            }
        }

        /// <summary>
        /// Gets a value indicating the current scale
        /// </summary>
        [ScriptableMember(ScriptAlias = "scale")]
        public double Scale
        {
            get
            {
                return ViewModelLocator.GraphDataStatic.Scale;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the HostInterop class
        /// </summary>
        public ScriptableGraph()
        {
            _customMessageDialog.Closed += new EventHandler(CustomMessageDialogClosed);

            SnaglEventAggregator.DefaultInstance.GetEvent<SnaglLoadedEvent>().Subscribe(SnaglLoadedEventHandler, false);
            SnaglEventAggregator.DefaultInstance.GetEvent<NodeDoubleClickEvent>().Subscribe(NodeDoubleClickEventHandler, false);
            SnaglEventAggregator.DefaultInstance.GetEvent<NodeMouseEnterEvent>().Subscribe(NodeMouseEnterEventHandler, false);
            SnaglEventAggregator.DefaultInstance.GetEvent<NodeMouseLeaveEvent>().Subscribe(NodeMouseLeaveEventHandler, false);
            SnaglEventAggregator.DefaultInstance.GetEvent<EdgeMouseEnterEvent>().Subscribe(EdgeMouseEnterEventHandler, false);
            SnaglEventAggregator.DefaultInstance.GetEvent<EdgeMouseLeaveEvent>().Subscribe(EdgeMouseLeaveEventHandler, false);
            SnaglEventAggregator.DefaultInstance.GetEvent<GraphResizedEvent>().Subscribe(GraphResizedEventHandler, false);
            SnaglEventAggregator.DefaultInstance.GetEvent<DataLoadedEvent>().Subscribe(DataLoadedEventHandler, false);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Performs a search with the given criteria
        /// </summary>
        /// <param name="searchMode">Filter or Find</param>
        /// <param name="searchText">the text to search for</param>
        [ScriptableMember(ScriptAlias = "performSearch")]
        public void PerformSearch(string searchMode, string searchText)
        {
            switch (searchMode)
            {
                case "Filter":
                    ViewModelLocator.SearchToolVMStatic.SearchMode = SearchToolMode.Filter;
                    break;

                case "Find":
                    ViewModelLocator.SearchToolVMStatic.SearchMode = SearchToolMode.Find;
                    break;

                default:
                    ViewModelLocator.SearchToolVMStatic.SearchMode = SearchToolMode.Unknown;
                    break;
            }

            ViewModelLocator.SearchToolVMStatic.SearchText = searchText;
            ViewModelLocator.SearchToolVMStatic.PerformAction();
        }

        /// <summary>
        /// Returns the exported graph data
        /// </summary>
        [ScriptableMember(ScriptAlias = "performExport")]
        public string PerformExport()
        {
            // the following is not available because it attempts to open a save file dialog
            // which can only be initiated via a user clicking a button; security feature of silverlight
            //GraphManager.Instance.PerformExport(graphComponents.Scope);

            GraphMLGraphDataFormat graphMlFormat = new GraphMLGraphDataFormat();
            GraphComponents graphComponents = GraphManager.Instance.DefaultGraphComponentsInstance;
            string exportedData = graphMlFormat.Export(graphComponents.Scope);

            return exportedData;
        }

        /// <summary>
        /// Determines if the graph will be panned or nodes will be selected/dragged
        /// </summary>
        /// <param name="isPanMode">true or false</param>
        [ScriptableMember(ScriptAlias = "setIsPanMode")]
        public void SetIsPanMode(bool isPanMode)
        {
            ViewModelLocator.GraphDataStatic.IsPanMode = isPanMode;
        }

        /// <summary>
        /// Sets the visibility of the toolbar
        /// </summary>
        /// <param name="isHidden">true or fale</param>
        [ScriptableMember(ScriptAlias = "setIsToolbarHidden")]
        public void SetIsToolbarHidden(bool isHidden)
        {
            ViewModelLocator.MainStatic.IsToolbarHidden = isHidden;
        }

        /// <summary>
        /// Sets the visibility of the tool panel
        /// </summary>
        /// <param name="isHidden">true or fale</param>
        [ScriptableMember(ScriptAlias = "setIsToolPanelHidden")]
        public void SetIsToolPanelHidden(bool isHidden)
        {
            ViewModelLocator.MainStatic.IsToolPanelHidden = isHidden;
        }

        /// <summary>
        /// Adds the specified edge to the graph
        /// </summary>
        /// <param name="scriptableEdge">Edge to add to the graph</param>
        [ScriptableMember(ScriptAlias = "addEdge")]
        public void AddEdge(ScriptableEdgeMapData scriptableEdge)
        {
            EdgeMapData objEdge = ScriptableMapper.GetEdge(scriptableEdge);

            GraphComponents graphComponents = GraphManager.Instance.DefaultGraphComponentsInstance;

            MappingExtensions.AddEdge(graphComponents, CreationType.Live, objEdge);
        }

        /// <summary>
        /// Removes the edge connecting source node to the target node
        /// </summary>
        /// <param name="sourceId">source node</param>
        /// <param name="targetId">target node</param>
        [ScriptableMember(ScriptAlias = "removeEdge")]
        public void RemoveEdge(string sourceId, string targetId)
        {
            GraphComponents graphComponents = GraphManager.Instance.DefaultGraphComponentsInstance;
            GraphData graphData = graphComponents.Data;

            INode sourceNode = graphData.GetNode(sourceId);
            IEnumerable<IEdge> sourceNodeEdges = graphData.Edges(sourceNode);

            IEdge edge = sourceNodeEdges.First<IEdge>(e => e.Source.ID.Equals(sourceId) && e.Target.ID.Equals(targetId));

            graphComponents.RemoveEdgeViewModel(edge);
        }

        /// <summary>
        /// Adds the specified node to the graph
        /// </summary>
        /// <param name="scriptableNode">Node to add to the graph</param>
        [ScriptableMember(ScriptAlias = "addNode")]
        public void AddNode(ScriptableNodeMapData scriptableNode)
        {
            NodeMapData objNode = ScriptableMapper.GetNode(scriptableNode);

            GraphComponents graphComponents = GraphManager.Instance.DefaultGraphComponentsInstance;

            MappingExtensions.AddNode(graphComponents, CreationType.Live, objNode);
        }

        /// <summary>
        /// Removes the specified node
        /// </summary>
        /// <param name="nodeId">the id of the node to remove</param>
        [ScriptableMember(ScriptAlias = "removeNode")]
        public void RemoveNode(string nodeId)
        {
            GraphComponents graphComponents = GraphManager.Instance.DefaultGraphComponentsInstance;
            GraphData graphData = graphComponents.Data;

            INode node = graphData.GetNode(nodeId);
            INodeShape nodeShape = graphComponents.GetNodeViewModel(node);

            graphComponents.RemoveNodeViewModel(nodeShape);
        }

        /// <summary>
        /// opens a message dialog with the specified title and text
        /// </summary>
        /// <param name="title">text to display in the title</param>
        /// <param name="text">content to display</param>
        [ScriptableMember(ScriptAlias = "showCustomMessageDialog")]
        public void ShowCustomMessageDialog(string title, string text)
        {
            _toolbarExtensions = (from e in ToolbarExtensionManager.Instance.Extensions
                                  where e.ViewModel.IsEnabled
                                  select e).ToList();

            _customMessageDialogVm.Title = title;
            _customMessageDialogVm.Text = text;

            _customMessageDialog.DataContext = _customMessageDialogVm;
            _customMessageDialog.Show();
        }

        /// <summary>
        /// Loads the specified XML onto the graph
        /// </summary>
        /// <param name="xmlData">XML data to load onto the graph</param>
        /// <param name="format">Graph data format</param>
        [ScriptableMember(ScriptAlias = "draw")]
        public void Draw(string xmlData, string format)
        {
            string scope = GraphManager.Instance.DefaultGraphComponentsInstance.Scope;
            GraphDataFormatBase graphFormat = InitializeGraphFormat(format);

            // Attempt to import that data
            GraphManager.Instance.ImportData(xmlData, scope, graphFormat);
        }

        /// <summary>
        /// Lays out the graph with the specified layout
        /// </summary>
        /// <param name="layout">Layout format</param>
        /// <param name="isAnimated">Indicates whether or not the layout should be animated</param>
        [ScriptableMember(ScriptAlias = "layoutGraph")]
        public void LayoutGraph(string layout, bool isAnimated)
        {
            LayoutGraph(layout, isAnimated, scope: String.Empty, rootNode: null);
        }

        /// <summary>
        /// Lays out the graph with the specified layout
        /// </summary>
        /// <param name="layoutName">Name of the layout format</param>
        /// <param name="isAnimated">Indicates whether or not the layout should be animated</param>
        /// <param name="scope">Specifies the default graph scope</param>
        /// <param name="rootNode">Specifies the root node for layouts that require one</param>
        [ScriptableMember(ScriptAlias = "layoutGraph")]
        public void LayoutGraph(string layoutName, bool isAnimated, string scope, INode rootNode)
        {
            GraphManager manager = GraphManager.Instance;
            LayoutBase layout = LayoutManager.Instance.GetLayoutByName(layoutName);
            NodeViewModelBase nodeShape; // TODO: Consider support for INodeShape

            if (rootNode != null)
            {
                nodeShape = GraphManager.Instance.DefaultGraphComponentsInstance.GetNodeViewModel(rootNode) as NodeViewModelBase;
            }
            else
            {
                nodeShape = null;
            }

            manager.LayoutGraph(layout, isAnimated, scope, nodeShape);
        }

        /// <summary>
        /// Loads SnagL using the specified GraphML
        /// </summary>
        /// <param name="xmlData">GraphML to load</param>
        [ScriptableMember]
        [Obsolete("This method has been deprecated by the Draw() method.")]
        public void LoadGraphML(string xmlData)
        {
            // Get the main scope
            string scope = GraphManager.Instance.DefaultGraphComponentsInstance.Scope;
            GraphDataFormatBase format = new GraphMLGraphDataFormat();

            // Attempt to import that data
            GraphManager.Instance.ImportData(xmlData, scope, format);
        }

        /// <summary>
        /// Loads live data onto the graph
        /// </summary>
        /// <param name="xmlData">XML data to containing information to be placed on the graph</param>
        [ScriptableMember]
        public void LoadLiveData(string xmlData)
        {
            _liveData.LoadLiveData(xmlData);
        }

        /// <summary>
        /// Gets an INode object by the specified ID
        /// </summary>
        /// <param name="id">Unique identifier of the node to return</param>
        /// <returns>An INode object with the specified ID if it exists, otherwise null</returns>
        [ScriptableMember(ScriptAlias = "node")]
        public INode Node(string id)
        {
            return GraphManager.Instance.DefaultGraphComponentsInstance.Data.Nodes.FirstOrDefault(n => n.ID == id);
        }

        /// <summary>
        /// Pans the graph by the specified x and y coordinates
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        [ScriptableMember(ScriptAlias = "panBy")]
        public void PanBy(int x, int y)
        {
            ViewModelLocator.GraphDataStatic.Pan = new Point(x, y);
        }

        /// <summary>
        /// Zooms the graph by the specified amount
        /// </summary>
        /// <param name="scale">Amount to zoom the map by</param>
        [ScriptableMember(ScriptAlias = "zoom")]
        public void Zoom(double scale)
        {
            ViewModelLocator.GraphDataStatic.Zoom(scale);
        }

        /// <summary>
        /// Resizes the graph to fit all nodes into the viewable area
        /// </summary>
        [ScriptableMember(ScriptAlias = "zoomToFit")]
        public void ZoomToFit()
        {
            ViewModelLocator.GraphDataStatic.ResizeToFit();
        }

        /// <summary>
        /// Adds a new menu item to the graph's context menu
        /// </summary>
        /// <param name="header">The test to be displayed in the menu</param>
        /// <param name="callback">The Javascript callback function</param>
        [ScriptableMember(ScriptAlias = "addItemToGraphContextMenu")]
        public void AddGraphContextMenuItem(string header, string callback)
        {
            MenuItem menuItem = new MenuItem(5) { Header = header };
            menuItem.Clicked += delegate { ExecuteEdgeJSCallback(callback); };
            ContextMenuManager.Instance.AddGraphMenuItem(menuItem);
        }

        /// <summary>
        /// Adds a new menu item to the node's context menu
        /// </summary>
        /// <param name="header">The test to be displayed in the menu</param>
        /// <param name="callback">The Javascript callback function</param>
        [ScriptableMember(ScriptAlias = "addItemToNodeContextMenu")]
        public void AddNodeContextMenuItem(string header, string callback)
        {
            MenuItem menuItem = new MenuItem(5) { Header = header };
            menuItem.Clicked += delegate { ExecuteNodeJSCallback(ContextMenuManager.Instance.TargetNodeVM, callback); };
            ContextMenuManager.Instance.AddNodeMenuItem(menuItem);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Executes the JavaScript function specified by <paramref name="callbackName"/>
        /// </summary>
        /// <param name="callbackName">The name of the JavaScript function to execute</param>
        private static void ExecuteEdgeJSCallback(string callbackName)
        {
            //TODO:  VALIDATE CALLBACK
            try
            {
                HtmlPage.Window.Invoke(callbackName);
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Unable to find the '" + callbackName + "' method in the host application", "Unknown callback", MessageBoxButton.OK);
                // Swallow error for now
                //TODO:  LOG THIS CASE
            }
        }

        /// <summary>
        /// Executes the JavaScript function specified by <paramref name="callbackName"/>
        /// </summary>
        /// <param name="sender">The NodeViewModel that raised the event</param>
        /// <param name="callbackName">The name of the JavaScript function to execute</param>
        private static void ExecuteNodeJSCallback(object sender, string callbackName)
        {
            //TODO:  VALIDATE CALLBACK
            try
            {
                NodeViewModelBase nodeVM = (NodeViewModelBase)sender;
                NodeMapData nodeMapData = MappingExtensions.GetNode(nodeVM);
                ScriptableNodeMapData scriptableNodeMapData = ScriptableMapper.GetNode(nodeMapData);

                HtmlPage.Window.Invoke(callbackName, scriptableNodeMapData);
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Unable to find the '" + callbackName + "' method in the host application", "Unknown callback", MessageBoxButton.OK);
                // Swallow error for now
                //TODO:  LOG THIS CASE
            }
        }

        /// <summary>
        /// Initializes the proper graph format using the specified format
        /// </summary>
        /// <param name="format">Graph format</param>
        /// <returns>An intialized concrete GraphDataFormatBase</returns>
        /// <exception cref="InvalidOperationException">Thrown if the specified format is invalid</exception>
        private static GraphDataFormatBase InitializeGraphFormat(string format)
        {
            GraphDataFormatBase graphFormat;

            switch (format.ToLower())
            {
                case "graphml":
                    graphFormat = new GraphMLGraphDataFormat();
                    break;

                case "trac":
                    graphFormat = new TracGraphDataFormat();
                    break;

                case "anb":
                    graphFormat = new AnbGraphDataFormat();
                    break;
                default:
                    throw new InvalidOperationException("Unknown graph format");

            }

            return graphFormat;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Event handler for handling the custom dialogs DialogClosed event
        /// </summary>
        /// <param name="sender">The custom message dialog</param>
        /// <param name="e">Any event arguments that might be passed</param>
        private void CustomMessageDialogClosed(object sender, EventArgs e)
        {
            foreach (IToolbarItemViewExtension extension in _toolbarExtensions)
            {
                extension.ViewModel.IsEnabled = true;
            }
        }

        /// <summary>
        /// Event handler for handling the SnaglLoaded event
        /// </summary>
        /// <param name="args">Any event arguments that might be passed</param>
        public void SnaglLoadedEventHandler(SnaglLoadedEventArgs args)
        {
            if (args.ExternalResourcesLoaded)
            {
                _externalResourcesLoaded = args.ExternalResourcesLoaded;
            }

            if (args.GraphLoaded)
            {
                _graphLoaded = args.GraphLoaded;
            }

            if (_externalResourcesLoaded && _graphLoaded && SnaglLoaded != null)
            {
                SnaglLoaded(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Event handler for the NodeDoubleClickEvent
        /// </summary>
        /// <param name="args">Arguments for the event</param>
        public void NodeDoubleClickEventHandler(NodeViewModelMouseEventArgs<System.Windows.Input.MouseButtonEventArgs> args)
        {
            if (NodeDoubleClicked != null)
            {
                NodeDoubleClicked(this, ScriptableNodeEventArgs.Create(args));
            }
        }

        /// <summary>
        /// Event handler for the NodeMouseEnterEvent
        /// </summary>
        /// <param name="args">Arguments for the event</param>
        public void NodeMouseEnterEventHandler(NodeViewModelMouseEventArgs<System.Windows.Input.MouseEventArgs> args)
        {
            if (NodeMouseEnter != null)
            {
                NodeMouseEnter(this, ScriptableNodeEventArgs.Create(args));
            }
        }

        /// <summary>
        /// Event handler for the NodeMouseLeaveEvent
        /// </summary>
        /// <param name="args">Arguments for the event</param>
        public void NodeMouseLeaveEventHandler(NodeViewModelMouseEventArgs<System.Windows.Input.MouseEventArgs> args)
        {
            if (NodeMouseLeave != null)
            {
                NodeMouseLeave(this, ScriptableNodeEventArgs.Create(args));
            }
        }

        /// <summary>
        /// Event handler for the EdgeMouseEnterEvent
        /// </summary>
        /// <param name="args">Arguments for the event</param>
        public void EdgeMouseEnterEventHandler(EdgeViewModelMouseEventArgs<System.Windows.Input.MouseEventArgs> args)
        {
            if (EdgeMouseEnter != null)
            {
                EdgeMouseEnter(this, ScriptableEdgeEventArgs.Create(args));
            }
        }

        /// <summary>
        /// Event handler for the EdgeMouseLeaveEvent
        /// </summary>
        /// <param name="args">Arguments for the event</param>
        public void EdgeMouseLeaveEventHandler(EdgeViewModelMouseEventArgs<System.Windows.Input.MouseEventArgs> args)
        {
            if (EdgeMouseLeave != null)
            {
                EdgeMouseLeave(this, ScriptableEdgeEventArgs.Create(args));
            }
        }

        /// <summary>
        /// Event handler for the GraphResizedEvent event
        /// </summary>
        /// <param name="args">Arguments for the event</param>
        public void GraphResizedEventHandler(EventArgs args)
        {
            if (GraphResized != null)
            {
                GraphResized(this, args);
            }
        }

        /// <summary>
        /// Event handler for the DataLoaded event
        /// </summary>
        /// <param name="args"></param>
        public void DataLoadedEventHandler(DataLoadedEventArgs args)
        {
            if (DataLoaded != null)
            {
                DataLoaded(this, args);
            }
        }

        #endregion
    }
}