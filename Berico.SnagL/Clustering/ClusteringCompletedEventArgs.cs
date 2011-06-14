//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------
using System.Collections.Generic;

namespace Berico.SnagL.Infrastructure.Clustering
{

    /// <summary>
    /// Represents arguments for events related to clustering
    /// </summary>
    public class ClusteringCompletedEventArgs
    {
        private HashSet<double> similarityValues = null;
        private double thresholdUsed = double.NaN;
        private bool clusteringActive = true;

        /// <summary>
        /// Creates a new instance of the Berico.SnagL.Events.
        /// ClusteringCompletedEventArgs class
        /// </summary>
        public ClusteringCompletedEventArgs()
            : this(null, double.NaN, true)
        { }

        /// <summary>
        /// Creates a new instance of the Berico.SnagL.Events.
        /// ClusteringCompletedEventArgs class
        /// </summary>
        public ClusteringCompletedEventArgs(bool _clusteringActive)
            : this(null, double.NaN, _clusteringActive)
        { }

        /// <summary>
        /// Creates a new instance of the Berico.SnagL.Events.
        /// ClusteringCompletedEventArgs class using the provided
        /// similarity values collection
        /// </summary>
        /// <param name="_similarityValues"></param>
        public ClusteringCompletedEventArgs(IEnumerable<double> _similarityValues, double _thresholdUsed, bool _clusteringActive)
        {
            if (_similarityValues != null)
                similarityValues = new HashSet<double>(_similarityValues);

            thresholdUsed = _thresholdUsed;
            clusteringActive = _clusteringActive;
        }

        /// <summary>
        /// Gets a collection of all the similarity values that
        /// were calculated.  If there were no values, this will
        /// be null.
        /// </summary>
        public HashSet<double> SimilarityValues
        {
            get { return similarityValues; }
        }

        /// <summary>
        /// Gets the value of the threshold that was used
        /// </summary>
        public double ThresholdUsed
        {
            get { return thresholdUsed; }
        }

        /// <summary>
        /// Gets whether clustering was activated or not
        /// </summary>
        public bool ClusteringActive
        {
            get { return clusteringActive; }
        }
    }
}