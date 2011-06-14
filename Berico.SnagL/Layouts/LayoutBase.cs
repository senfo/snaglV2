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
using System.ComponentModel.Composition;
using Berico.Common.Diagnostics;
using Berico.SnagL.Infrastructure.Data;
using Berico.SnagL.Infrastructure.Data.Mapping;
using Berico.SnagL.Infrastructure.Events;
using Berico.SnagL.Infrastructure.Graph;
using Berico.SnagL.Model;

namespace Berico.SnagL.Infrastructure.Layouts
{
    /// <summary>
    /// The base class for all layouts
    /// </summary>
    [Export(typeof(LayoutBase))]
    public abstract class LayoutBase
    {
        #region Fields

        private StopWatch stopWatch = new StopWatch();
        private bool isInBatch = false;
        
        #endregion

        #region Events

        /// <summary>
        /// Indicates that the layout has finished processing nodes
        /// </summary>
        public event EventHandler LayoutFinished;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value that indicates whether or not the layout is enabled
        /// </summary>
        public abstract bool Enabled
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not layout is in batch
        /// </summary>
        public bool IsInBatch
        {
            get { return isInBatch; }
            set { isInBatch = value; }
        }

        /// <summary>
        /// Get the name of the layout
        /// </summary>
        public abstract string LayoutName
        {
            get;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the LayoutBase class
        /// </summary>
        public LayoutBase()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Performs the actual layout algorithm. This will execute on a background thread.
        /// </summary>
        /// <param name="isAnimated">Indicates whether or not the layout should be animated</param>
        /// <param name="graphData">The object containing the graph data</param>
        public void CalculateLayout(GraphMapData graphData)
        {
            CalculateLayout(graphData, rootNode: null);
        }

        /// <summary>
        /// Performs the actual layout algorithm. This will execute on a background thread.
        /// </summary>
        /// <param name="isAnimated">Indicates whether or not the layout should be animated</param>
        /// <param name="graphData">The object containing the graph data</param>
        /// <param name="rootNode">Root node</param>
        public void CalculateLayout(GraphMapData graphData, INode rootNode)
        {
            // Publish appropriate events
            SnaglEventAggregator.DefaultInstance.GetEvent<LayoutExecutingEvent>().Publish(new LayoutEventArgs(LayoutName));
            SnaglEventAggregator.DefaultInstance.GetEvent<UI.TimeConsumingTaskExecutingEvent>().Publish(new UI.TimeConsumingTaskEventArgs());

            // Perform the layout
            PerformLayout(graphData, rootNode);
        }

        /// <summary>
        /// Resonsible for actually positioning the nodes.  This will
        /// execute on the UI thread, once the background thread
        /// running the algorithm has completed.
        /// </summary>
        /// <param name="isAnimated">Indicates whether or not the layout should be animated</param>
        /// <param name="graphData">The object containing the graph data with the updated layout</param>
        public virtual void PositionNodes(bool isAnimated, GraphMapData graphData)
        {
            var nodes = graphData.Nodes;
            var nodeVMs = GraphManager.Instance.DefaultGraphComponentsInstance.NodeViewModels;

            foreach (NodeViewModelBase node in nodeVMs)
            {
                if (nodes.ContainsKey(node.ParentNode.ID))
                {
                    // Position or animated this nodeVM
                    if (isAnimated)
                    {
                        node.AnimateTo(nodes[node.ParentNode.ID].Position);
                    }
                    else
                    {
                        node.Position = nodes[node.ParentNode.ID].Position;
                    }
                }
            }

            // Publish appropriate events
            SnaglEventAggregator.DefaultInstance.GetEvent<LayoutExecutedEvent>().Publish(new LayoutEventArgs(LayoutName));
            SnaglEventAggregator.DefaultInstance.GetEvent<UI.TimeConsumingTaskCompletedEvent>().Publish(new UI.TimeConsumingTaskEventArgs());

            OnLayoutFinished(EventArgs.Empty);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Performs the actual layout algorithm. This will execute on a background thread.
        /// </summary>
        /// <param name="graphData">The object containing the graph data</param>
        protected virtual void PerformLayout(GraphMapData graphData)
        {
            PerformLayout(graphData, rootNode: null);
        }

        /// <summary>
        /// Performs the actual layout algorithm. This will execute on a background thread.
        /// </summary>
        /// <param name="graphData">The object containing the graph data</param>
        /// <param name="rootNode">Root node</param>
        protected abstract void PerformLayout(GraphMapData graphData, INode rootNode);

        /// <summary>
        /// Handles the LayoutFinished event
        /// </summary>
        /// <param name="args">Any event arguments that might be passed</param>
        protected void OnLayoutFinished(EventArgs args)
        {
            if (LayoutFinished != null)
                LayoutFinished(this, args);
        }

        #endregion
    }
}