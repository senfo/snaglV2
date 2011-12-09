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

namespace Berico.SnagL.Infrastructure.Data.Searching
{
    /// <summary>
    /// Provides event data for the SearchStarted and SearchCompleted events
    /// </summary>
    public class SearchEventArgs : EventArgs
    {

        private string query = string.Empty;
        private SearchAction currentSearchAction = SearchAction.Unknown;
        private int affectedNodes = 0;

        /// <summary>
        /// Creates a new instance of the Berico.LinkAnalysis.SnagL.Data.SearchEventArgs
        /// class using the provided search text and mode.
        /// </summary>
        /// <param name="_query">The text being search for</param>
        /// <param name="_currentSearchToolMode">The current mode (select or filter)
        /// of the search operation</param>
        public SearchEventArgs(string _query, SearchAction _currentSearchAction)
            : base()
        {
            this.query = _query;
            this.currentSearchAction = _currentSearchAction;
        }

        /// <summary>
        /// Creates a new instance of the Berico.LinkAnalysis.SnagL.Data.SearchEventArgs
        /// class using the provided search text and mode.
        /// </summary>
        /// <param name="_query">The text being search for</param>
        /// <param name="_currentSearchToolMode">The current mode (select or filter)
        /// of the search operation</param>
        /// <param name="_affectedNodes">The number of nodes affected by the search operation</param>
        public SearchEventArgs(string _query, SearchAction _currentSearchAction, int _affectedNodes)
            : base()
        {
            this.query = _query;
            this.currentSearchAction = _currentSearchAction;
            this.affectedNodes = _affectedNodes;
        }

        /// <summary>
        /// Gets the query being used 
        /// </summary>
        public string QueryString
        {
            get { return this.query; }
        }

        /// <summary>
        /// Gets the current SearchToolMode
        /// </summary>
        public SearchAction CurrentSearchToolMode
        {
            get { return this.currentSearchAction; }
        }

        /// <summary>
        /// Gets the number of nodes that were found by the search
        /// </summary>
        public int AffectedNodes
        {
            get { return this.affectedNodes; }
        }

    }
}