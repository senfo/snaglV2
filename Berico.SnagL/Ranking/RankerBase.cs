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
using System.ComponentModel;
using Berico.SnagL.Infrastructure.Events;
using Berico.SnagL.Infrastructure.Modularity.Contracts;
using Berico.SnagL.Model;

namespace Berico.SnagL.Infrastructure.Ranking
{
    /// <summary>
    /// Base class for all ranking algorithms
    /// </summary>
    public abstract class RankerBase : IRanker
    {
        #region Fields
            
            private readonly string _name = string.Empty;
            private readonly string _description = "No description provided";
            private readonly BackgroundWorker _worker;
            private bool _normalizeResults;

        #endregion

        /// <summary>
        /// Creates a new instance of the RankingBase class
        /// </summary>
        /// <param name="name">The name to assign to this ranking algorithm</param>
        /// <param name="description">The description for this ranking algorithm</param>
        protected RankerBase(string name, string description)
        {
            // Validate the name
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("Name", "No valid name was provided for this ranker");

            _name = name;
            _description = description;

            // Initialize the background worker
            _worker = new BackgroundWorker {WorkerReportsProgress = false, WorkerSupportsCancellation = false};

            _worker.DoWork += DoWorkHandler;
            _worker.RunWorkerCompleted += RunWorkerCompletedHandler;
        }

        /// <summary>
        /// Calculate the normalized rank for the graph specified by the provided scope
        /// </summary>
        /// <param name="scope">The scope for the graph to be ranked</param>
        /// <returns>the ranking for the nodes on the graph in the form
        /// of a dictionary of nodes and their scores</returns>
        public Dictionary<INode, double> CalculateNormalizedRank(string scope)
        {
            // Calculate the rank for the specified graph
            Dictionary<INode, double> ranks = CalculateRank(scope);

            return NormalizeResults(ranks);
        }

        /// <summary>
        /// Asynchronously calculate the normalized rank for the graph specified 
        /// by the provided scope
        /// </summary>
        /// <param name="scope">The scope for the graph to be ranked</param>
        public void CalculateNormalizedRankAsync(string scope)
        {
            _normalizeResults = true;
            CalculateRankAsync(scope);
        }

        #region IRanker Members

            /// <summary>
            /// Gets the name of this ranker
            /// </summary>
            public string Name
            {
                get { return _name; }
            }

            /// <summary>
            /// Gets a description for this ranker
            /// </summary>
            public string Description
            {
                get { return _description; }
            }

            /// <summary>
            /// Calculates the rank (normalized between 0 and 1) for the graph
            /// specified by the provided scope.  This method is implemented
            /// by implementing classes.
            /// </summary>
            /// <param name="scope">The scope for the graph to be ranked</param>
            /// <returns>the ranking for the nodes on the graph in the form
            /// of a dictionary of nodes and their scores.</returns>
            public abstract Dictionary<INode, double> CalculateRank(string scope);

            /// <summary>
            /// Asynchronously calculates the rank for the graph specified by
            /// the provided scope
            /// </summary>
            /// <param name="scope">The scope for the graph to be ranked</param>
            public void CalculateRankAsync(string scope)
            {
                // Validate the provided scope
                if (string.IsNullOrEmpty(scope))
                    throw new ArgumentException("No valid scope was provided", "scope");

                SnaglEventAggregator.DefaultInstance.GetEvent<UI.TimeConsumingTaskExecutingEvent>().Publish(new UI.TimeConsumingTaskEventArgs());

                // Start the background worker
                _worker.RunWorkerAsync(scope);
            }

            /// <summary>
            /// Indicates that the ranking operation has completed
            /// </summary>
            public event EventHandler<RankingEventArgs> RankingCompleted;

        #endregion

        #region Events and Event Handlers

            /// <summary>
            /// Handles the DoWork event
            /// </summary>
            /// <param name="sender">The object that fired the event</param>
            /// <param name="e">The arguments for the event</param>
            private void DoWorkHandler(object sender, DoWorkEventArgs e)
            {
                // Call the underlining CalculateRank method
               e.Result = CalculateRank(e.Argument.ToString());
            }

            /// <summary>
            /// Handles the RunWorkerCompleted event
            /// </summary>
            /// <param name="sender">The object that fired the event</param>
            /// <param name="e">The arguments for the event</param>
            private void RunWorkerCompletedHandler(object sender, RunWorkerCompletedEventArgs e)
            {
                SnaglEventAggregator.DefaultInstance.GetEvent<UI.TimeConsumingTaskCompletedEvent>().Publish(new UI.TimeConsumingTaskEventArgs());

                Dictionary<INode, double> results;

                if (_normalizeResults)
                    results = NormalizeResults(e.Result as Dictionary<INode, double>);
                else
                    results = e.Result as Dictionary<INode, double>;

                _normalizeResults = false;

                // Fire the RankingCompleted event
                OnRankingCompleted(new RankingEventArgs(_name, results));
            }

            /// <summary>
            /// Fires the RankingCompleted event
            /// </summary>
            /// <param name="args">Argumenst for the event</param>
            public virtual void OnRankingCompleted(RankingEventArgs args)
            {
                EventHandler<RankingEventArgs> handler = RankingCompleted;
                if (handler != null)
                {
                    handler(this, args);
                }
            }

        #endregion

        /// <summary>
        /// Normalizes (from 0 to 1) the provided ranking results
        /// </summary>
        /// <returns>a dictionary containing the normalized ranking results</returns>
        private Dictionary<INode, double> NormalizeResults(Dictionary<INode, double> rankingResults)
        {
            Dictionary<INode, double> normalizedRanks = new Dictionary<INode, double>();

            double min = double.MaxValue;
            double max = double.MinValue;

            // Loop over all the rank entries
            foreach (KeyValuePair<INode, double> kv in rankingResults)
            {
                min = Math.Min(kv.Value, min);
                max = Math.Max(kv.Value, max);
            }

            // Calculate the range of the rank values
            double range = Math.Max(max - min, 0.001);

            // Loop over the ranks
            foreach (KeyValuePair<INode, double> kv in rankingResults)
            {
                // Stores the rank value normalized between 0 and 1
                normalizedRanks[kv.Key] = (kv.Value - min) / range;
            }

            return normalizedRanks;
        }

    }
}
