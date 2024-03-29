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
using System.Threading;
using Berico.Common.Diagnostics;
using Berico.SnagL.Infrastructure.Data.Mapping;
using Berico.SnagL.Infrastructure.Events;
using Berico.SnagL.Model;

namespace Berico.SnagL.Infrastructure.Layouts
{
    /// <summary>
    /// Extends <see cref="LayoutBase"/> to add asynchronous functionality for layouts
    /// </summary>
    public abstract class AsynchronousLayoutBase : LayoutBase
    {
        #region Fields

        /// <summary>
        /// Stores a reference to the graph data for the graphs scope
        /// </summary>
        private GraphMapData _graphData;

        // For reference purposes, let me note that I initially
        // attempted to use the BackgroundWorker to drive
        // the asynchronous operations for this class.  I came
        // across an issue where the RunWorkerCompleted event
        // was never fired.  There appeared to be no reason for 
        // this.  Switching to handling threading myself resolved
        // the issue.

        private StopWatch stopWatch = new StopWatch();
        private ManualResetEvent resetEvent;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a reference to the node data
        /// </summary>
        public NodeMapData NodeData
        {
            get;
            private set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the AsynchronousLayoutBase class
        /// </summary>
        public AsynchronousLayoutBase()
        {
        }

        #endregion

        /// <summary>
        /// Performs the actual layout algorithm. This will execute on a background thread.
        /// </summary>
        /// <param name="isAnimated">Indicates whether or not the layout should be animated</param>
        /// <param name="graphData">The object containing the graph data</param>
        public void BeginCalculateLayout(GraphMapData graphData)
        {
            BeginCalculateLayout(graphData, null);
        }

        /// <summary>
        /// Performs the actual layout algorithm. This will execute on a background thread.
        /// </summary>
        /// <param name="isAnimated">Indicates whether or not the layout should be animated</param>
        /// <param name="graphData">The object containing the graph data</param>
        /// <param name="rootNode">Root node</param>
        public void BeginCalculateLayout(GraphMapData graphData, INode rootNode)
        {
            stopWatch.Start();

            // Update the global variables for threading
            _graphData = graphData;

            // Publish appropriate events
            SnaglEventAggregator.DefaultInstance.GetEvent<LayoutExecutingEvent>().Publish(new LayoutEventArgs(LayoutName));
            SnaglEventAggregator.DefaultInstance.GetEvent<UI.TimeConsumingTaskExecutingEvent>().Publish(new UI.TimeConsumingTaskEventArgs());

            using (resetEvent = new ManualResetEvent(false))
            {
                // Run the LayoutGraph method on a new thread
                ThreadPool.QueueUserWorkItem(new WaitCallback(LayoutGraph));

                // Force the main thread to wait here
                resetEvent.WaitOne();

                ExecutionComplete();
            }
        }

        /// <summary>
        /// Calls the PerformLayout method, which is implemented by
        /// any class that inherits AsynchronousLayoutBase.
        /// </summary>
        /// <param name="state">Represents some object being passed to the
        /// method.  This is not used at this time but required for any
        /// WaitCallback delegate.</param>
        private void LayoutGraph(object state)
        {
            PerformLayout(_graphData);

            // Instruct the main thread to wake back up
            resetEvent.Set();
        }

        /// <summary>
        /// This method is called when the BackgroundWorker.RunWorkerCompleted is fired
        /// and handles finalizing the layout.
        /// </summary>
        protected virtual void ExecutionComplete()
        {
            // Debugging information
            stopWatch.Stop();

            // Publish appropriate events
            SnaglEventAggregator.DefaultInstance.GetEvent<LayoutExecutedEvent>().Publish(new LayoutEventArgs(LayoutName));

            // Check if this class is running in a batch
            if (!IsInBatch)
                SnaglEventAggregator.DefaultInstance.GetEvent<UI.TimeConsumingTaskCompletedEvent>().Publish(new UI.TimeConsumingTaskEventArgs());

            // Fire the LayoutFinished event
            OnLayoutFinished(EventArgs.Empty);
        }
    }
}