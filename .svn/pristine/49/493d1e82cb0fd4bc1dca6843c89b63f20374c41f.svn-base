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
using Berico.SnagL.Model;

namespace Berico.SnagL.Infrastructure.Ranking
{
    /// <summary>
    /// Provides event data for Ranking related events
    /// </summary>
    public class RankingEventArgs : EventArgs
    {
        private readonly string _rankerName = string.Empty;
        private readonly Dictionary<INode, double> _rankingResults;

        /// <summary>
        /// Creates a new instance of the Berico.LinkAnalysis.SnagL.Data.SearchEventArgs
        /// class using the provided search text and mode.
        /// </summary>
        /// <param name="rankerName"></param>
        /// <param name="rankingResults"></param>
        public RankingEventArgs(string rankerName, Dictionary<INode, double> rankingResults)
        {
            _rankerName = rankerName;
            _rankingResults = rankingResults;
        }

        /// <summary>
        /// Gets the name of the ranker that was executed 
        /// </summary>
        public string RankerName
        {
            get { return _rankerName; }
        }

        /// <summary>
        /// Gets the results from the ranking operation
        /// </summary>
        public Dictionary<INode, double> Results
        {
            get { return _rankingResults; }
        }

    }
}