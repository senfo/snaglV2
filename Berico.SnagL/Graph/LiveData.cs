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
using System.Threading;
using Berico.SnagL.Infrastructure.Data;
using Berico.SnagL.Infrastructure.Data.Formats;
using Berico.SnagL.Infrastructure.Events;
using Berico.SnagL.Infrastructure.Graph.Events;
using Berico.SnagL.Infrastructure.Layouts;
using Berico.SnagL.UI;
using GalaSoft.MvvmLight.Threading;

namespace Berico.SnagL.Infrastructure.Graph
{
    /// <summary>
    /// Provides the ability to work with LiveData from the host
    /// </summary>
    public class LiveData : ILiveData
    {
        #region Fields

        /// <summary>
        /// Used for syncing threads
        /// </summary>
        private static object syncObj = new object();

        /// <summary>
        /// Stores graph data from the host
        /// </summary>
        private static Queue<string> graphData = new Queue<string>();

        /// <summary>
        /// Updates the SnagL graph with the supplied data.
        /// This is a temporary delegate to deal with deferred excecution problem with a lambda expression
        /// </summary>
        /// <param name="data">Graph data to push onto the graph</param>
        /// <param name="scope">Unqiue graph scope</param>
        /// <param name="graphDataFormat">Specifies the graph data format</param>
        /// <param name="queueCount">Number of items in the queue</param>
        private delegate void UpdateSnagl(string data, string scope, GraphDataFormatBase graphDataFormat, int queueCount);

        #endregion

        #region Events

        /// <summary>
        /// Occurs when an import of live data is started
        /// </summary>
        public event EventHandler<EventArgs> LiveDataStarted;

        /// <summary>
        /// Occurs when an import of live data has completed
        /// </summary>
        public event EventHandler<EventArgs> LiveDataEnded;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the LiveData class
        /// </summary>
        public LiveData()
        {
            // Initialize event handlers
            SnaglEventAggregator.DefaultInstance.GetEvent<LiveDataStartedEvent>().Subscribe(LiveDataStartedEventHandler, false);
            SnaglEventAggregator.DefaultInstance.GetEvent<LiveDataEndedEvent>().Subscribe(LiveDataEndedEventHandler, false);
            SnaglEventAggregator.DefaultInstance.GetEvent<LiveDataLoadedEvent>().Subscribe(LiveDataLoadedEventHandler, false);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads live data onto the graph
        /// </summary>
        /// <param name="xmlData">XML data containing information to be placed on the graph</param>
        public void LoadLiveData(string xmlData)
        {
            int count;

            // Add the graph data to the queue
            lock (syncObj)
            {
                graphData.Enqueue(xmlData);
                count = graphData.Count;
            }

            // Raise the LiveDataEnqueuedEvent event
            SnaglEventAggregator.DefaultInstance.GetEvent<LiveDataEnqueuedEvent>().Publish(new LiveDataEventArgs(count));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Processes live data
        /// </summary>
        /// <param name="graphDataFormat">Used to specify the format of the graph</param>
        private static void ProcessLiveData(object graphDataFormat)
        {
            int count;
            string data;
            string scope = GraphManager.Instance.DefaultGraphComponentsInstance.Scope;
            UpdateSnagl import = new UpdateSnagl(UpdateSnaglWithLiveData);

            while (GraphManager.Instance.LiveEnabled)
            {
                lock (syncObj)
                {
                    if (graphData.Count > 0)
                    {
                        // Read the data from the queue
                        data = graphData.Dequeue();
                        count = graphData.Count;

                        // Don't update unless you MAKE SURE you know what you're doing and understand deferred execution
                        DispatcherHelper.UIDispatcher.BeginInvoke(import, data, scope, graphDataFormat, count);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the SnagL graph with the supplied data.
        /// This is a temporary delegate to deal with deferred excecution problem with a lambda expression
        /// </summary>
        /// <param name="data">Graph data to push onto the graph</param>
        /// <param name="scope">Unqiue graph scope</param>
        /// <param name="graphDataFormat">Specifies the graph data format</param>
        /// <param name="queueCount">Number of items in the queue</param>
        private static void UpdateSnaglWithLiveData(string data, string scope, GraphDataFormatBase graphDataFormat, int queueCount)
        {
            // Import the data
            GraphManager.Instance.ImportLiveData(data, scope, graphDataFormat);

            // Raise the LiveDataDequeued event
            SnaglEventAggregator.DefaultInstance.GetEvent<LiveDataDequeuedEvent>().Publish(new LiveDataEventArgs(queueCount));
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Event handler for handling the LiveDataStarted event
        /// </summary>
        /// <param name="args">Any event arguments that might be passed</param>
        public void LiveDataStartedEventHandler(EventArgs args)
        {
            Thread thread = new Thread(new ParameterizedThreadStart(ProcessLiveData));

            // Start live processing
            thread.Start(new GraphMLGraphDataFormat());

            // Raise the LiveDataStartedEvent
            if (LiveDataStarted != null)
            {
                LiveDataStarted(this, args);
            }
        }

        /// <summary>
        /// Event handler for handling the LiveDataEnded event
        /// </summary>
        /// <param name="args">Any event arguments that might be passed</param>
        public void LiveDataEndedEventHandler(EventArgs args)
        {
            if (LiveDataEnded != null)
            {
                LiveDataEnded(this, args);
            }
        }

        /// <summary>
        /// Event handler for the LiveDataLoaded event
        /// </summary>
        /// <param name="args">Any event arguments that might be passed</param>
        public void LiveDataLoadedEventHandler(DataLoadedEventArgs args)
        {
            lock (syncObj)
            {
                LayoutBase activeLayout = LayoutManager.Instance.ActiveLayout;

                // Refresh the graph layout
                DispatcherHelper.UIDispatcher.BeginInvoke(() => GraphManager.Instance.LayoutGraph(activeLayout, false));
            }
        }

        #endregion
    }
}