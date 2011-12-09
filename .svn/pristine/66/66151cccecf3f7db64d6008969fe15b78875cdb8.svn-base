using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace Berico.SnagL.Infrastructure.Clustering
{
    /// <summary>
    /// 
    /// </summary>
    public class ClusteringEventArgs : EventArgs
    {
        private HashSet<double> similarityValues = null;
        private List<string> targetAttributes = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        public ClusteringEventArgs(List<string> _targetAttributes) : this(_targetAttributes, null) { }

        /// <summary>
        /// 
        /// </summary>
        public ClusteringEventArgs(List<string> _targetAttributes, HashSet<double> _similarityValues)
            : base() 
        {
            targetAttributes = _targetAttributes;
            similarityValues = _similarityValues;
        }

        /// <summary>
        /// 
        /// </summary>
        public HashSet<double> SimilarityValues
        {
            get { return similarityValues; }
        }

        /// <summary>
        /// Gets a collection of the attributes that were part
        /// of the clustering operation
        /// </summary>
        public List<string> TargetAttributes
        {
            get { return targetAttributes; }
        }
    }
}
