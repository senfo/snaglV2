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
using Berico.SnagL.Infrastructure.Data;
using Berico.SnagL.Infrastructure.Data.Attributes;
using Berico.SnagL.Infrastructure.Graph;
using Berico.SnagL.Infrastructure.Layouts;
using Berico.SnagL.Model;
using Berico.SnagL.UI;
using Berico.SnagL.Infrastructure.Events;
using GalaSoft.MvvmLight.Threading;
using System.ComponentModel;
using System.Linq;
using Berico.SnagL.Infrastructure.Data.Mapping;

namespace Berico.SnagL.Infrastructure.Clustering
{
    /// <summary>
    /// Manages the clustering of similar nodes
    /// </summary>
    public class SimilarityClustering
    {
        private const double MINIMUM_SIMILARITY_THRESHOLD = 0.5;

        private string scope = string.Empty;
        private double similarityThreshold = double.NaN;

        private Dictionary<IEdge, double> edgeSimilarityValues;
        private List<SimilarityDataEdge> edgesToAdd;
        private GlobalAttributeCollection globalAttributeCollection;
        private Queue<INode> nodesToBeAdded = new Queue<INode>();
        private readonly AttributeSimilarityManager attributeSimilarityManager;
        private readonly BackgroundWorker worker;
        private readonly ClusterHighlights clusterHighlights;
        private readonly Dictionary<INode, List<SimilarityEdgeViewModel>> attributeEdgeCache = new Dictionary<INode, List<SimilarityEdgeViewModel>>();

        /// <summary>
        /// Create a new instance of the SimilarityClustering class
        /// </summary>
        public SimilarityClustering()
        {
            this.scope = GraphManager.Instance.DefaultGraphComponentsInstance.Scope;
            clusterHighlights = new ClusterHighlights(scope);

            // Get a reference to the GlobaleAttributeCollection
            globalAttributeCollection = GlobalAttributeCollection.GetInstance(scope);
            
            // Initialize the AttributeSimilarityManager and obtain a reference to it
            AttributeSimilarityManager.InitialSetup(scope);
            attributeSimilarityManager = AttributeSimilarityManager.Instance;

            // Initialize the background worker
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += new DoWorkEventHandler(DoWorkHandler);
            worker.ProgressChanged += new ProgressChangedEventHandler(ProgressChangedHandler);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompletedHandler);
        }

        /// <summary>
        /// Performs the clustering operation
        /// </summary>
        /// <param name="_threshold">The threshold to use when determining overall similarity</param>
        /// <param name="_similarityDescriptors">A list of attribute similarity descriptors</param>
        public void PerformClustering(double _threshold, List<AttributeSimilarityDescriptor> _similarityDescriptors)
        {
            this.similarityThreshold = _threshold;
            this.attributeSimilarityManager.SetDescriptors(_similarityDescriptors);

            DispatcherHelper.UIDispatcher.BeginInvoke(() =>
                SnaglEventAggregator.DefaultInstance.GetEvent<UI.TimeConsumingTaskExecutingEvent>().Publish(new UI.TimeConsumingTaskEventArgs())
            );

            // Start the worker (which fires the DoWork event)
            worker.RunWorkerAsync();
        }

        /// <summary>
        /// Performs the clustering operation
        /// </summary>
        private void ClusterNodes()
        {
            List<Node> nodes = new List<Node>(GraphManager.Instance.DefaultGraphComponentsInstance.Nodes.Cast<Node>());

            // Make sure that we actually have nodes to work with
            if (nodes.Count == 0)
                return;

            // Reset similarity value colelction
            edgeSimilarityValues = new Dictionary<IEdge, double>();

            // Loop over all the nodes
            for (int i = 0; i < nodes.Count; i++)
            {
                worker.ReportProgress((i / nodes.Count) * 100, nodes[i]);

                // Compare the current node to all other nodes
                for (int j = i + 1; j < nodes.Count; j++)
                {
                    // Calculate how similar these two nodes are
                    double similarity = AttributeSimilarityManager.Instance.ComputeNodeSimilarity(nodes[i], nodes[j]);

                    // Store the similarity value
                    edgeSimilarityValues.Add(GenerateSimilarityDataEdge(nodes[i], nodes[j], similarity), similarity);
                }
            }

            // Clear the descriptors
            //this.attributeSimilarityManager.ClearDescriptors();

            // Set threshold to the maximum
            if (double.IsNaN(similarityThreshold))
                similarityThreshold = edgeSimilarityValues.Values.Max();

            // Loop over the similarity edges that were created
            foreach (SimilarityDataEdge newEdge in edgeSimilarityValues.Keys)
            {
                // Check if the similarity value is within our calculated threshold
                if (edgeSimilarityValues[newEdge] >= MINIMUM_SIMILARITY_THRESHOLD && edgeSimilarityValues[newEdge] >= similarityThreshold)
                {
                    // Add this similarity edge into our master list of
                    // edges that should be added
                    edgesToAdd.Add(newEdge);
                }
            }
        }

        /// <summary>
        /// Removes any Similarity edges that belong to
        /// the provided node
        /// </summary>
        /// <param name="node">The node to remove edges from</param>
        private void RemoveSimilarityEdges(INode node)
        {
            GraphManager.Instance.DefaultGraphComponentsInstance.RemoveEdgeViewModels<SimilarityDataEdge>(node);
        }

        /// <summary>
        /// Removes the current clustering
        /// </summary>
        public void RemoveClustering()
        {
            // Loop over all the nodes and remove the default stiffness
            foreach (INode node in GraphManager.Instance.DefaultGraphComponentsInstance.Nodes)
            {
                RemoveSimilarityEdges(node);
            }

            // Remove clustering highlights
            clusterHighlights.RemoveHighlights();

            // Fire the ClusteringCompleted event
            SnaglEventAggregator.DefaultInstance.GetEvent<ClusteringCompletedEvent>().Publish(new ClusteringCompletedEventArgs(false));

            // Layout the graph
            LayoutGraph(InternalLayouts.Fr);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="similarityValue"></param>
        /// <returns></returns>
        private SimilarityDataEdge GenerateSimilarityDataEdge(INode source, INode target, double similarityValue)
        {
            SimilarityDataEdge newSimDataEdge = new SimilarityDataEdge(similarityValue, source, target);

            // Add an attribute to the edge, containing the similarity value
            newSimDataEdge.Attributes.Add("Similarity Value", new Model.Attributes.AttributeValue(similarityValue.ToString()));

            //TODO:  THIS SHOULD BE REMOVED IF EDGES ARE NOT VISIBLE

            return newSimDataEdge;
        }

        /// <summary>
        /// Creates a new SimilarityEdgeViewModel or retrieves
        /// an existing one from the cache
        /// </summary>
        /// <param name="sourceNode">The source node</param>
        /// <param name="targetNode">The target node</param>
        /// <param name="similarityValue">The similarity value for the provided source and target node</param>
        /// <returns>a new (or cached) SimilarityEdgeViewModel</returns>
        private void GenerateSimilarityEdgeVM(SimilarityDataEdge similarityEdge)
        {
            List<SimilarityEdgeViewModel> nodeSimilarityEdges;
            SimilarityEdgeViewModel newEdgeVM = null;

            //TODO:  WE NEED TO CLEAR THE CACHE AT SOME POINT IN TIME

            // Get the cache list of edges for the source node
            if (attributeEdgeCache.TryGetValue(similarityEdge.Source, out nodeSimilarityEdges))
            {
                // Loop over all the edges to try and find if we already have one
                // for the given source and target nodes
                foreach (SimilarityEdgeViewModel currentEdgeVM in nodeSimilarityEdges)
                {
                    if (currentEdgeVM.ParentEdge.Target == similarityEdge.Target)
                    {
                        // The edge was previously cached
                        newEdgeVM = currentEdgeVM;
                        break;
                    }
                }
            }
            else
            {
                // Initialize the edge cache list
                attributeEdgeCache[similarityEdge.Target] = new List<SimilarityEdgeViewModel>();
            }

            // If we found an edge, we should return it
            if (newEdgeVM != null)
                GraphManager.Instance.DefaultGraphComponentsInstance.AddEdgeViewModel(newEdgeVM);

            // If we are here, we need to create a brand new edge
            newEdgeVM = new SimilarityEdgeViewModel(similarityEdge, this.scope);

            GraphManager.Instance.DefaultGraphComponentsInstance.AddEdgeViewModel(newEdgeVM);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layoutName"></param>
        private void LayoutGraph(string layoutName)
        {
            // Get a light weight version of the graph data to be
            // used with the layout
            GraphMapData graphMapData = GraphComponentsUtility.GetGraph(GraphManager.Instance.GetGraphComponents(scope));

            // Get the instance of the LinLog layout
            LayoutBase graphLayout = LayoutManager.Instance.GetLayoutByName(layoutName);

            // Wire up the LayoutFinished event
            graphLayout.LayoutFinished += new EventHandler(LayoutFinishedHandler);

            // Execute the layout on the UI thread
            DispatcherHelper.UIDispatcher.BeginInvoke(() =>
            {
                graphLayout.CalculateLayout(graphMapData);
                graphLayout.PositionNodes(false, graphMapData);
            });
        }

        #region Events and Event Handlers

            /// <summary>
            /// Handles the DoWork event
            /// </summary>
            /// <param name="sender">The object that fired the event</param>
            /// <param name="e">The arguments for the event</param>
            private void DoWorkHandler(object sender, DoWorkEventArgs e)
            {
                // Actualy do the clustering
                edgesToAdd = new List<SimilarityDataEdge>();

                // Actually cluster the nodes
                ClusterNodes();
            }

            /// <summary>
            /// Handles the RunWorkerCompleted event
            /// </summary>
            /// <param name="sender">The object that fired the event</param>
            /// <param name="e">The arguments for the event</param>
            private void RunWorkerCompletedHandler(object sender, RunWorkerCompletedEventArgs e)
            {
                // Check to see if we have any edges to be added
                if (edgesToAdd.Count > 0)
                {
                    // Loop over the edges to be added
                    foreach (SimilarityDataEdge edge in edgesToAdd)
                    {
                        // Create the edge view model and add it to the graph
                        GenerateSimilarityEdgeVM(edge);
                    }
                }

                // Fire the ClusteringCompleted event
                DispatcherHelper.UIDispatcher.BeginInvoke(() =>
                    SnaglEventAggregator.DefaultInstance.GetEvent<ClusteringCompletedEvent>().Publish(new ClusteringCompletedEventArgs(edgeSimilarityValues.Values.Where(value => value >= MINIMUM_SIMILARITY_THRESHOLD), this.similarityThreshold, true))
                    );

                LayoutGraph(InternalLayouts.LinLog);

            }

            /// <summary>
            /// Handles the ProgressChanged event.  We are using this
            /// to remove existing similarity edges as each node is
            /// processed.
            /// </summary>
            /// <param name="sender">The object that fired the event</param>
            /// <param name="e">Arguments for the event</param>
            void ProgressChangedHandler(object sender, ProgressChangedEventArgs e)
            {
                // Determine if the user state object provided is a Node or not
                if (e.UserState is Node)
                {
                    // Remove any existing similarity edges 
                    // for the current node
                    RemoveSimilarityEdges(e.UserState as Node);
                }
            }

            /// <summary>
            /// Handles the LayoutFinished event
            /// </summary>
            /// <param name="sender">The object that fired the event</param>
            /// <param name="e">Arguments for the event</param>
            private void LayoutFinishedHandler(object sender, EventArgs e)
            {
                // Draw the highlights
                clusterHighlights.HighlightClusters();
            }

        #endregion
    }
}