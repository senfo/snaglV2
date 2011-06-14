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
using System.Linq;
using Berico.Common;
using Berico.SnagL.Model;
using Berico.SnagL.Model.Attributes;
using System.Linq.Expressions;

namespace Berico.SnagL.Infrastructure.Data.Searching
{
    /// <summary>
    /// Responsible for executing and managing searches
    /// conducted against the graph data.
    /// </summary>
    public class SearchManager
    {
        private static SearchManager instance;
        private static object syncRoot = new object();
        private bool isSearchRunning = false;

        /// <summary>
        /// Hidden constructor.
        /// </summary>
        private SearchManager() { }

        /// <summary>
        /// Gets the instance of the SearchManager class
        /// </summary>
        public static SearchManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            // Create the instance of the SearchManager
                            instance = new SearchManager();
                            instance.Initialize();
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Initializes the SearchManager class
        /// </summary>
        private void Initialize()
        {

        }

        /// <summary>
        /// Gets or sets whether a search is currently executing
        /// </summary>
        public bool IsSearchRunning
        {
            get { return this.isSearchRunning; }
            set { this.isSearchRunning = value; }
        }

        #region Events

            /// <summary>
            /// Indicates that the SearchStarted has completed
            /// </summary>
            public event EventHandler<SearchEventArgs> SearchStarted;

            /// <summary>
            /// Fires the SearchStarted event
            /// </summary>
            /// <param name="args">Argumenst for the event</param>
            public virtual void OnSearchStarted(SearchEventArgs args)
            {
                EventHandler<SearchEventArgs> handler = SearchStarted;
                if (handler != null)
                {
                    handler(this, args);
                }
            }

            /// <summary>
            /// Indicates that the search has completed
            /// </summary>
            public event EventHandler<SearchEventArgs> SearchCompleted;

            /// <summary>
            /// Fires the SearchCompleted event
            /// </summary>
            /// <param name="args">Argumenst for the event</param>
            public virtual void OnSearchCompleted(SearchEventArgs args)
            {
                EventHandler<SearchEventArgs> handler = SearchCompleted;
                if (handler != null)
                {
                    handler(this, args);
                }
            }

        #endregion

        /// <summary>
        /// Starts the search process
        /// </summary>
            /// <param name="searchDescriptors">A collection of SearchDescriptors to be used for the query</param>
        /// <param name="options">Options related to the search</param>
            public void Find(List<SearchDescriptor> searchDescriptors, SearchOptions options)
        {

            // Don't perform find functionality if there are no nodes
            if (GraphManager.Instance.DefaultGraphComponentsInstance.Nodes == null || GraphManager.Instance.DefaultGraphComponentsInstance.Nodes.Count() == 0)
                return;

            // If searchText is empty and a filter or selection are
            // currently active, undo them
            if (searchDescriptors == null || searchDescriptors.Count() == 0)
            {
                if (options.Action == SearchAction.Select)
                    GraphManager.Instance.DefaultGraphComponentsInstance.NodeSelector.TurnOffSelection();
                else if (options.Action == SearchAction.Filter)
                    GraphManager.Instance.DefaultGraphComponentsInstance.NodeFilter.TurnOffFilter();

                return;
            }

            // If a filter is already active and we are in filter mode, remove
            // it before conducting the new filter
            if (GraphManager.Instance.DefaultGraphComponentsInstance.NodeFilter.IsActive && options.Action == SearchAction.Filter)
            {
                GraphManager.Instance.DefaultGraphComponentsInstance.NodeFilter.TurnOffFilter();
            }

            List<Node> foundNodes = new List<Node>();

            // Construct the query to be executed
            Expression<Func<Node, bool>> queryExpression = BuildQueryExpression(searchDescriptors);

            // Fire the SearchStarted event
            OnSearchStarted(new SearchEventArgs(queryExpression.ToString(), options.Action));
            this.isSearchRunning = true;

            // Create the query and execute it (returning a List)
            foundNodes = GraphManager.Instance.DefaultGraphComponentsInstance.Nodes.Cast<Model.Node>().Where(queryExpression.Compile()).ToList<Node>();

            // Check if any matching nodes were found
             if (foundNodes.Count > 0)
            {
                if (options.Action == SearchAction.Select)
                {
                    // Unselect all currently selected nodes
                    GraphManager.Instance.DefaultGraphComponentsInstance.NodeSelector.UnselectAll();

                    // Select the found nodes
                    GraphManager.Instance.DefaultGraphComponentsInstance.NodeSelector.Select(foundNodes);
                }
                else if (options.Action == SearchAction.Filter)
                {
                    // Filter on (show only) the found nodes
                    GraphManager.Instance.DefaultGraphComponentsInstance.NodeFilter.Filter(foundNodes);
                }
            }
            else
            {
                // If nothing was found the current filter or search should be removed
                if (options.Action == SearchAction.Select)
                    GraphManager.Instance.DefaultGraphComponentsInstance.NodeSelector.TurnOffSelection();
                else
                    GraphManager.Instance.DefaultGraphComponentsInstance.NodeFilter.TurnOffFilter();                
            }

            // Fire the SearchCompleted event
            OnSearchCompleted(new SearchEventArgs(queryExpression.ToString(), options.Action, foundNodes.Count));
            this.isSearchRunning = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="descriptors"></param>
        /// <returns></returns>
        private Expression<Func<Node, bool>> BuildQueryExpression(List<SearchDescriptor> descriptors)
        {
            var searchPredicate = PredicateBuilder.False<Node>();
            var attributePredicate = PredicateBuilder.True<Node>();

            // Loop over all the provided descriptors
            foreach (SearchDescriptor descriptor in descriptors)
            {
                attributePredicate = attributePredicate.And(descriptor.CreateExpression());
            }

            searchPredicate = searchPredicate.Or(attributePredicate);

            return searchPredicate;
        }

    }
}