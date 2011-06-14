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
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using Berico.Common.Diagnostics;
using Berico.SnagL.Infrastructure.Clustering;
using Berico.SnagL.Infrastructure.Data.Formats;
using Berico.SnagL.Infrastructure.Data.Mapping;
using Berico.SnagL.Infrastructure.Events;
using Berico.SnagL.Infrastructure.Graph;
using Berico.SnagL.Infrastructure.Graph.Events;
using Berico.SnagL.Infrastructure.Layouts;
using Berico.SnagL.Infrastructure.Modularity;
using Berico.SnagL.Infrastructure.Modularity.Contracts;
using Berico.SnagL.Model;
using Berico.SnagL.UI;
using GalaSoft.MvvmLight.Threading;
using ImageTools;
using Berico.SnagL.Infrastructure.Data.Attributes;

namespace Berico.SnagL.Infrastructure.Data
{
    /// <summary>
    /// Responsible for managing graph data for SnagL.  This
    /// includes creating, importing and exporting data as well
    /// as maintaining and suporting multiple sets of data.
    /// </summary>
    public class GraphManager
    {
        private static GraphManager instance;
        private static object syncRoot = new object();
        private Dictionary<string, GraphComponents> graphComponentsInstances;
        private string defaultComponentInstanceScope = string.Empty;

        /// <summary>
        /// Indicates whether or not Live mode is enabled
        /// </summary>
        private volatile bool _liveEnabled;

        /// <summary>
        /// Gets or sets a value to indicate whether or not live is enabled
        /// </summary>
        public virtual bool LiveEnabled
        {
            get
            {
                return _liveEnabled;
            }
            set
            {
                _liveEnabled = value;

                if (_liveEnabled)
                {
                    SnaglEventAggregator.DefaultInstance.GetEvent<LiveDataStartedEvent>().Publish(EventArgs.Empty);
                }
                else
                {
                    SnaglEventAggregator.DefaultInstance.GetEvent<LiveDataEndedEvent>().Publish(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Creates a new GraphDataManager.  This method is private
        /// and inaccessible.
        /// </summary>
        private GraphManager() { }

        /// <summary>
        /// Gets or sets a list of data formats that are supported.  This
        /// list is composed and maintained by MEF.
        /// </summary>
        [ImportMany(typeof(IGraphDataFormat), AllowRecomposition = true)]
        public List<IGraphDataFormat> GraphDataFormats { get; set; }

        /// <summary>
        /// Gets the instance of the GraphDataManager class
        /// </summary>
        public static GraphManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            // Create a new instance of the class and initialze it
                            instance = new GraphManager();
                            instance.Initialize();
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Initialize an instance of the GraphManager class
        /// </summary>
        private void Initialize()
        {
            // Initialize the Graph Components collection
            this.graphComponentsInstances = new Dictionary<string, GraphComponents>();

            // Subscribe to toolbar item events
            SnaglEventAggregator.DefaultInstance.GetEvent<ToolBarItemClickedEvent>().Subscribe(ToolbarItemClickedEventHandler, false);
            SnaglEventAggregator.DefaultInstance.GetEvent<DataLoadedEvent>().Subscribe(DataLoadedEventHandler, false);
            //SnaglEventAggregator.DefaultInstance.GetEvent<ClusteringCompletedEvent>().Subscribe(ClusteringCompletedEventHandler, false);
        }

        /// <summary>
        /// Determines whether there is any data for the given scope
        /// </summary>
        /// <param name="scope">The scope of the data being requested</param>
        /// <returns>true if there is any data present; otherwise false</returns>
        public bool HasData(string scope)
        {
            GraphComponents components = GetGraphComponents(scope);

            // Check if there are any components present
            if (components == null)
                return false;

            // Check if we have any nodes in the data
            if (components.Data.Order > 0)
                return true;
            else
                return false;
        }

        #region Generating Graphs

            /// <summary>
            /// Creates a new random graph
            /// </summary>
            private void CreateRandomGraph(string _scope)
            {
                StopWatch stopWatch = StopWatch.StartNew();
                GraphComponents components = null;

                // Check if the provided scope is null or empty
                if (string.IsNullOrEmpty(_scope))
                {
                    // This is a new graph so we will generate new GraphComponents
                    // for it
                    components = new GraphComponents();
                }
                else
                {
                    // Attempt to get the graph components instance for
                    // the given scope
                    components = GetGraphComponents(_scope);

                    // If we were unable to get an instance, create a
                    // new one
                    if (components == null)
                        components = new GraphComponents(_scope);
                }

                // Generate the graph
                components.GenerateGraph(500, 200);

                stopWatch.Stop();
                System.Diagnostics.Debug.WriteLine("Graph Generated:  {0} seconds", stopWatch.Elapsed.Seconds);

                //TODO: THE NEXT PORTION OF CODE (THAT ADDS OR UPDATES THE COMPONENTS) SHOULD BE RELOCATED TO ITS OWN METHOD

                // Check if the default instance (which is the first instance
                // created) has been initialized yet
                if (this.defaultComponentInstanceScope == string.Empty)
                {
                    // TODO:  ENSURE THIS IS VALID IN THE FUTURE AS THE MAIN GRAPH MAY NOT ALWAYS POPULATE FIRST (BUT SHOULD BE)

                    // Save the newly created components as the default
                    this.defaultComponentInstanceScope = _scope;
                }

                // Now we need to update or add the components to the collection of components
                // Check if the collection has never been initialized
                if (this.graphComponentsInstances == null)
                {
                    // Initialize the collection
                    this.graphComponentsInstances = new Dictionary<string, GraphComponents>();
                }

                // Check if we have no items
                if (this.graphComponentsInstances.Count == 0)
                {
                    // Add the components instance to the collection
                    this.graphComponentsInstances.Add(components.Scope, components);
                }
                else
                {
                    // Ensure that the scope doesn't already exist
                    if (this.graphComponentsInstances.ContainsKey(components.Scope))
                    {
                        // Update the components instance for the specified scope
                        this.graphComponentsInstances[components.Scope] = components;
                    }
                    else
                    {
                        // Add the new instance for the specified scope
                        this.graphComponentsInstances.Add(components.Scope, components);
                    }
                }

            }

        #endregion

        #region Retrieving Graph Content

            /// <summary>
            /// Gets the default graph components instance 
            /// </summary>
            /// <returns>the default graph components instance</returns>
            public GraphComponents DefaultGraphComponentsInstance
            {
                get
                {
                    // Check if we have a scope for the default component instance
                    // which tells us if we already have a default component instance
                    if (string.IsNullOrEmpty(this.defaultComponentInstanceScope))
                    {
                        // Create a new instance of the GraphComponents class
                        GraphComponents graphComponents = new GraphComponents();

                        // Save the scope
                        this.defaultComponentInstanceScope = graphComponents.Scope;

                        // Save the newly created graph componets instance
                        this.graphComponentsInstances[this.defaultComponentInstanceScope] = graphComponents;

                        // return the new graph components instance
                        return graphComponents;
                    }

                    // Ensure the default instance exists
                    if (this.graphComponentsInstances.ContainsKey(this.defaultComponentInstanceScope))
                    {
                        // Return the default graph components instance
                        return this.graphComponentsInstances[this.defaultComponentInstanceScope];
                    }
                    else
                    {
                        // If we get here, then something went wrong internally because we
                        // should always have an instance in the collection that matches
                        // the saved default component instance scope.
                        throw new System.InvalidOperationException("The saved default component instance scope does not exist in the instance collection");
                    }
                }
            }

            /// <summary>
            /// Returns the GraphComponents instance for the given scope
            /// </summary>
            /// <param name="_scope">The scope of the desired GraphComponents instance</param>
            /// <returns>the desired GraphComponents instance or null</returns>
            public GraphComponents GetGraphComponents(string _scope)
            {
                // Check if we should be returning the default instance
                if (this.defaultComponentInstanceScope == _scope)
                    return DefaultGraphComponentsInstance;

                // Validate the provided scope
                if (string.IsNullOrEmpty(_scope))
                    throw new System.ArgumentNullException("_scope", "An invalid scope ID was provided");

                // Get the appropriate GraphComponents instance
                if (this.graphComponentsInstances != null)
                {
                    // Check if the scope exists in our collection
                    if (this.graphComponentsInstances.ContainsKey(_scope))
                        return this.graphComponentsInstances[_scope];
                }

                // No GraphComponents intance could be found
                // with the given scope
                return null;
            }

        #endregion

        #region Event Handlers

            /// <summary>
            /// Handles the ToolbarItemClicked event
            /// </summary>
            /// <param name="args">The arguments for the event</param>
            public void ToolbarItemClickedEventHandler(ToolBarItemEventArgs args)
            {
                NodeViewModelBase rootNode = GetGraphComponents(args.Scope).NodeSelector.SelectedNode;

                // TODO: USE CONSTANT FOR ITEM NAME
                // TODO: Controls should control their own behavior
                if (args.ToolBarItem.Name == "GENERATOR")
                {
                    CreateRandomGraph(args.Scope);
                }
                else if (args.ToolBarItem.Name.EndsWith("_LAYOUT"))
                {
                    LayoutBase layout = InitializeLayout(args.ToolBarItem.Name, args.Scope);

                    // Perform the layout
                    LayoutGraph(layout, false, args.Scope, rootNode);
                }
                else if (args.ToolBarItem.Name == "IMPORT")
                {
                    PerformImport(args.Scope, new GraphMLGraphDataFormat()); // TODO: Determine which type at runtime
                }
                else if (args.ToolBarItem.Name == "EXPORT")
                {
                    PerformExport(args.Scope);
                }
                else if (args.ToolBarItem.Name == "GRAPH_TO_IMAGE")
                {
                    SaveGraphAsImage();
                }
                else if (args.ToolBarItem.Name == "RESIZE")
                {
                    UI.GraphViewModel graphVM = UI.ViewModelLocator.GraphDataStatic;
                    graphVM.ResizeToFit();

                    //TODO:  THIS SHOULD BE DECOUPLED
                }
            }

            /// <summary>
            /// Handles the DataLoaded event
            /// </summary>
            /// <param name="args">The arguments for the event</param>
            public void DataLoadedEventHandler(DataLoadedEventArgs args)
            {
                if (args.SourceMechanism == CreationType.Live)
                {
                    LayoutGraph(LayoutManager.Instance.ActiveLayout, false, args.Scope);
                }
                else if (AllNodesHaveSamePosition(args.Scope))
                {
                    // Layout the graph
                    LayoutGraph(LayoutManager.Instance.DefaultLayout, true, args.Scope);
                }
                else
                {
                    // Just resize the graph to fit
                    UI.GraphViewModel graphVM = UI.ViewModelLocator.GraphDataStatic;
                    graphVM.ResizeToFit();
                }
            }

            /// <summary>
            /// Handles the ClusteringCompleted event
            /// </summary>
            /// <param name="args">The arguments for the event</param>
            public void ClusteringCompletedEventHandler(Clustering.ClusteringCompletedEventArgs args)
            {
                // Layout the graph
                LayoutGraph(LayoutManager.Instance.DefaultLayout, false);
            }

        #endregion

        #region General External Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetFilters()
        {
            //TODO:  UPDATE THIS METHOD TO NOT INCLUDE EXTENSIONS IF AN IMPORT OR EXPORT IS NOT PROVIDED

            StringBuilder sb = new StringBuilder();

            //TODO:  UPDATE TO SUPPORT MULTIPLE EXTENSIONS (COMMA SEPARATED)

            // Loop over the existing data formats
            foreach (IGraphDataFormat dataFormat in GraphDataFormats.OrderBy(format => format.Priority))
            {
                string extensionPortion = dataFormat.Extension.StartsWith("*.") ? dataFormat.Extension : "*." + dataFormat.Extension;
                string descriptivePortion = string.Format("{0} ({1})", dataFormat.Description, extensionPortion);

                sb.Append(descriptivePortion);
                sb.Append(" | ");
                sb.Append(extensionPortion);
                sb.Append("|");
            }
            sb.Append("All Files (*.*)|*.*");

            return sb.ToString();

        }

        /// <summary>
        /// Redraws the graph with the specified layout type
        /// </summary>
        /// <param name="layout">Type of graph layout</param>
        public void LayoutGraph(LayoutBase layout, bool isAnimted)
        {
            LayoutGraph(layout, isAnimted, DefaultGraphComponentsInstance.Scope);
        }

        /// <summary>
        /// Redraws the graph with the specified layout type
        /// </summary>
        /// <param name="layout">Type of graph layout</param>
        /// <param name="isAnimated">Specifies whether or not to animate the graph when it's laid out</param>
        public void LayoutGraph(LayoutBase layout, bool isAnimated, string scope)
        {
            LayoutGraph(layout, isAnimated, scope, rootNode: null);
        }

        /// <summary>
        /// Redraws the graph with the specified layout type
        /// </summary>
        /// <param name="layout">Type of graph layout</param>
        /// <param name="isAnimated">Specifies whether or not to animate the graph when it's laid out</param>
        /// <param name="scope">Specifies the default graph scope</param>
        /// <param name="rootNode">Specifies the root node for layouts that require one</param>
        public void LayoutGraph(LayoutBase layout, bool isAnimated, string scope, NodeViewModelBase rootNode)
        {
            LayoutManager flyweight = LayoutManager.Instance;

            // Make sure we have a scope
            if (String.IsNullOrWhiteSpace(scope))
            {
                scope = this.defaultComponentInstanceScope;
            }

            // Get the graph as a GraphMapData object
            GraphMapData graphMapData = GetGraphComponents(scope).ExportGraph();

            // Execute the layout
            DispatcherHelper.UIDispatcher.BeginInvoke(() =>
                {
                    if (rootNode != null)
                    {
                        layout.CalculateLayout(graphMapData, rootNode.ParentNode);
                    }
                    else
                    {
                        layout.CalculateLayout(graphMapData);
                    }

                    
                    layout.PositionNodes(isAnimated, graphMapData);
                });
        }

        /// <summary>
        /// Imports GraphML into SnagL on a new graph
        /// </summary>
        /// <param name="data">The graph data to place on the graph</param>
        /// <param name="scope">Specifies the graphs scope</param>
        /// <param name="format">Specifies the graph data format</param>
        public void ImportData(string data, string scope, GraphDataFormatBase format)
        {
            GraphComponents components = null;

            // Check if the provided scope is null or empty
            if (string.IsNullOrEmpty(scope))
            {
                // This is a new graph so we will generate new GraphComponents
                // for it
                components = new GraphComponents();
            }
            else
            {
                // Attempt to get the graph components instance for
                // the given scope
                components = GetGraphComponents(scope);

                // If we were unable to get an instance, create a
                // new one
                if (components == null)
                    components = new GraphComponents();
            }

            components.Clear();

            GlobalAttributeCollection.GetInstance(scope).Clear();

            // Import the data into the provided components
            format.Import(data, components, CreationType.Imported);

            // Check if the default instance (which is the first instance
            // created) has been initialized yet
            if (this.defaultComponentInstanceScope == string.Empty)
            {
                // TODO:  ENSURE THIS IS VALID IN THE FUTURE AS THE MAIN GRAPH MAY NOT ALWAYS POPULATE FIRST (BUT SHOULD BE)

                // Save the newly created components as the default
                this.defaultComponentInstanceScope = components.Scope;
            }

            // Now we need to update or add the components to the collection of components
            // Check if the collection has never been initialized
            if (this.graphComponentsInstances == null)
            {
                // Initialize the collection
                this.graphComponentsInstances = new Dictionary<string, GraphComponents>();
            }

            // Check if we have no items
            if (this.graphComponentsInstances.Count == 0)
            {
                // Add the components instance to the collection
                this.graphComponentsInstances.Add(components.Scope, components);
            }
            else
            {
                // Ensure that the scope doesn't already exist
                if (this.graphComponentsInstances.ContainsKey(components.Scope))
                {
                    // Update the components instance for the specified scope
                    this.graphComponentsInstances[components.Scope] = components;
                }
                else
                {
                    // Add the new instance for the specified scope
                    this.graphComponentsInstances.Add(components.Scope, components);
                }
            }

            //TODO  MAKE SURE THAT WE HAVE DATA

            // Fire the DataLoaded event
            DispatcherHelper.UIDispatcher.BeginInvoke(() =>
                SnaglEventAggregator.DefaultInstance.GetEvent<DataLoadedEvent>().Publish(new DataLoadedEventArgs(components.Scope, CreationType.Imported))
                );
        }

        /// <summary>
        /// Loads the specified data into an existing graph
        /// </summary>
        /// <param name="data">Graph data to load onto the graph</param>
        /// <param name="graphDataFormat">Specifies the graph format (e.g., GraphML)</param>
        public void ImportLiveData(string data, GraphDataFormatBase graphDataFormat)
        {
            ImportLiveData(data, this.defaultComponentInstanceScope, graphDataFormat);
        }

        /// <summary>
        /// Loads the specified data into an existing graph
        /// </summary>
        /// <param name="data">Graph data to load onto the graph</param>
        /// <param name="scope">Specifies the graph scope</param>
        /// <param name="graphDataFormat">Specifies the graph format (e.g., GraphML)</param>
        public void ImportLiveData(string data, string scope, GraphDataFormatBase graphDataFormat)
        {
            GraphComponents components;

            if (String.IsNullOrWhiteSpace(data))
            {
                throw new ArgumentNullException("data");
            }

            if (String.IsNullOrWhiteSpace(scope))
            {
                throw new ArgumentNullException("scope");
            }

            if (!this.graphComponentsInstances.ContainsKey(scope))
            {
                throw new InvalidOperationException("Unable to locate an existing GraphComponents with the specified scope");
            }

            // Get any existing graph compents with the specified scope
            components = this.graphComponentsInstances[scope];

            // Import the data into the provided components
            graphDataFormat.Import(data, components, CreationType.Live);

            // Fire the LiveDataLoaded event
            DispatcherHelper.UIDispatcher.BeginInvoke(() =>
                SnaglEventAggregator.DefaultInstance.GetEvent<LiveDataLoadedEvent>().Publish(new DataLoadedEventArgs(components.Scope, CreationType.Live))
                );
        }

        /// <summary>
        /// Returns a new GraphComponents instance that represents connected nodes
        /// as partition nodes
        /// </summary>
        /// <param name="Scope">The scope of the graph data being partitioned</param>
        /// <returns>a GraphComponents instance</returns>
        public GraphComponents GetConnectedComponents(string _scope)
        {
            GraphComponents partitionedGraph = new GraphComponents(_scope);
                
            // Get all the node view models contained in the target scope
            List<NodeViewModelBase> nodeVMs = new List<NodeViewModelBase>(GetGraphComponents(_scope).GetNodeViewModels().Cast<NodeViewModelBase>().ToList());

            PartitionNode connectedComponent = GetNextConnectedComponent(nodeVMs, _scope);

            // Continue getting the next connected component
            // as long as the last connected component was
            // not null
            while (connectedComponent != null)
            {
                // Instruct the partition node to calculate its dimensions
                connectedComponent.RecalculateDimensions();

                // Add the partition node to the partion graph
                partitionedGraph.AddNodeViewModel(connectedComponent);
                //partitionedGraph.AddNode(connectedComponent);
                    
                // Get the next connected component
                connectedComponent = GetNextConnectedComponent(nodeVMs, _scope);
            }

            return partitionedGraph;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines whether all the nodes are in the same location or now.  This
        /// is used to decide (when data is loaded) whether the node positions
        /// should be preserved or a layout should be performed.
        /// </summary>
        /// <param name="scope">The scope of the graphManager</param>
        /// <returns>true if all the nodes are in the same location; otherwise false</returns>
        private bool AllNodesHaveSamePosition(string scope)
        {
            IEnumerable<INodeShape> nodeVMs = GetGraphComponents(scope).GetNodeViewModels();

            // Return true if we only have 1 node
            if (nodeVMs.Count() <= 1) return true;

            double x = nodeVMs.First().Position.X;
            double y = nodeVMs.First().Position.Y;

            // Loop over all the nodes in the graph
            foreach (INodeShape nodeVM in nodeVMs)
            {
                // Determine if the positions don't match
                if (nodeVM.Position.X != x || nodeVM.Position.Y != y)
                    return false;
            }

            return true;
        }

        private void SaveGraphAsImage()
        {
            //TODO:  ENSURE THAT DATA IS EVEN PRESENT ON THE GRAPH
            //TODO:  THIS SHOULD BE DECOUPLED

            ImageTools.IO.Encoders.AddEncoder<ImageTools.IO.Jpeg.JpegEncoder>();
            ImageTools.IO.Encoders.AddEncoder<ImageTools.IO.Png.PngEncoder>();

            UI.GraphViewModel graphVM = UI.ViewModelLocator.GraphDataStatic;

            ImageTools.ExtendedImage myImage = graphVM.GraphToImage();

            System.Windows.Controls.SaveFileDialog saveFileDialog = new System.Windows.Controls.SaveFileDialog();

            saveFileDialog.Filter = "JPEG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png";
            saveFileDialog.FilterIndex = 1;

            bool? dialogResult = saveFileDialog.ShowDialog();

            if (dialogResult == true)
            {
                using (Stream fs = (Stream)saveFileDialog.OpenFile())
                {
                    myImage.WriteToStream(fs, saveFileDialog.SafeFileName);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void PerformExport(string scope)
        {
            GraphMLGraphDataFormat graphMLFormat = new GraphMLGraphDataFormat();
            string dataToExport = graphMLFormat.Export(scope);

            if (dataToExport != string.Empty)
            {
                System.Windows.Controls.SaveFileDialog saveFileDialog = new System.Windows.Controls.SaveFileDialog();

                saveFileDialog.Filter = string.Format("{0} (*.{1}) | *.{1}", graphMLFormat.Description, graphMLFormat.Extension);
                saveFileDialog.FilterIndex = 1;

                bool? dialogResult = saveFileDialog.ShowDialog();

                if (dialogResult == true)
                {
                    using (Stream fs = (Stream)saveFileDialog.OpenFile())
                    {
                        using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
                        {
                            sw.Write(dataToExport);
                            sw.Flush();
                            sw.Close();
                            fs.Close();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void PerformImport(string _scope, GraphDataFormatBase format)
        {
            string importData = string.Empty;

            System.Windows.Controls.OpenFileDialog openFileDialog = new System.Windows.Controls.OpenFileDialog();

            openFileDialog.Filter = string.Format("{0} (*.{1}) | *.{1}", format.Description, format.Extension);
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;

            bool? dialogResult = openFileDialog.ShowDialog();

            if (dialogResult == true)
            {
                // Open the file and read it into our variable
                importData = openFileDialog.File.OpenText().ReadToEnd();

                //TODO:  ADD ERROR TRAPPING HERE
                if (!string.IsNullOrEmpty(importData))
                {
                    ImportData(importData, _scope, format);
                }
            }
        }

        /// <summary>
        /// Returns a PartitionNode instance for the next connected
        /// component contained in the provided list.  The collection
        /// provided is altered and should not be changed between calls
        /// to this method.
        /// </summary>
        /// <param name="nodeVMs">The collection of node view models to be processed</param>
        /// <param name="partitionGraphScope">The scope for the paritioned graph</param>
        /// <param name="originalScope">The scope for the original graph</param>
        /// <returns>a PartitionNode containing a set of connected nodes</returns>
        private PartitionNode GetNextConnectedComponent(IList<NodeViewModelBase> nodeVMs, string scope)
        {
            // Make sure node view models were provided
            if (nodeVMs.Count == 0) return null;

            // Create a new partition node to hold these components
            PartitionNode partitionNode = new PartitionNode(scope, "PN-" + nodeVMs.Count);

            // Start at the first nodeVMs
            List<NodeViewModelBase> visitedNodes = new List<NodeViewModelBase>();
            List<NodeViewModelBase> rootNodes = new List<NodeViewModelBase>();

            // Add the first node view model to the root and visited lists
            rootNodes.Add(nodeVMs[0]);
            visitedNodes.Add(nodeVMs[0]);

            // Itterate over the root nodes
            while (rootNodes.Count > 0)
            {
                int currentNodeIndex = 0;

                // Add the current node view model to the
                // current PartitionNode
                partitionNode.AddNode(rootNodes[currentNodeIndex]);

                // Itterate over the node's neighbors
                foreach (Model.INode node in GetGraphComponents(rootNodes[currentNodeIndex].Scope).Data.Neighbors(rootNodes[currentNodeIndex].ParentNode))
                {
                    // Get the nodeVM for this node
                    NodeViewModelBase nodeVM = GetGraphComponents(rootNodes[currentNodeIndex].Scope).GetNodeViewModel(node) as NodeViewModelBase;

                    // Check if this node has already been visisted
                    if (!visitedNodes.Contains(nodeVM))
                    {
                        // Add this node to our two holder collections
                        rootNodes.Add(nodeVM);
                        visitedNodes.Add(nodeVM);
                    }
                }

                // Remove the current node since it has been processed
                nodeVMs.Remove(rootNodes[currentNodeIndex]);
                rootNodes.RemoveAt(currentNodeIndex);
            }

            return partitionNode;
        }

        /// <summary>
        /// Initializes an instance of the LayoutBase class based on supplied input
        /// </summary>
        /// <param name="name">Specifies the name of the toolbar</param>
        /// <returns>The requested instance of the Layout base class</returns>
        private LayoutBase InitializeLayout(string name, string scope)
        {
            LayoutBase layout = null;
            LayoutManager flyweight = LayoutManager.Instance;

            if ("SIMPLE_TREE_LAYOUT".Equals(name))
            {
                layout = flyweight.GetLayoutByName(InternalLayouts.SimpleTree);
            }
            else if (name.StartsWith("TREE_"))
            {
                layout = flyweight.GetLayoutByName(InternalLayouts.OriginalTree);
            }
            else if (name.StartsWith("CIRCLE_") || name.StartsWith("RADIAL_")) 
            {
                layout = flyweight.GetLayoutByName(InternalLayouts.Circle);
            }
            else if (name.StartsWith("NETWORK_"))
            {
                layout = flyweight.GetLayoutByName(InternalLayouts.Network);
            }
            else if (name.StartsWith("GRID_"))
            {
                layout = flyweight.GetLayoutByName(InternalLayouts.Grid);
            }
            else
            {
                throw new InvalidOperationException("Unknown or invalid layout specified");
            }

            return layout;
        }

        #endregion
    }
}