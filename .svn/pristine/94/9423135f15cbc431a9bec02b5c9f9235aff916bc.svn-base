//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

namespace Berico.SnagL.Infrastructure.Data.Searching
{
    /// <summary>
    /// Represents a set of options for a search
    /// </summary>
    public class SearchOptions
    {
        private const SearchAction DEFAULT_ACTION = SearchAction.Select;

        /// <summary>
        /// Creates a default instance of the SearchOptions class
        /// </summary>
        public SearchOptions() : this(DEFAULT_ACTION) { }

        /// <summary>
        /// Creates a new instance of the SearchOptions class using the provided
        /// action value
        /// </summary>
        /// <param name="action">The SearchAction to be performed after once a search
        /// has been completed</param>
        public SearchOptions(SearchAction action)
        {
            Action = action;
       }

        /// <summary>
        /// Gets or sets the SearchAction to perform once the search is complete.
        /// The default action is to select the nodes
        /// </summary>
        public SearchAction Action { get; set; }

    }
}