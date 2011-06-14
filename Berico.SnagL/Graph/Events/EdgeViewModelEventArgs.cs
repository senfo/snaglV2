//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using System.Windows.Input;
using GalaSoft.MvvmLight;
using System;
using Berico.SnagL.Infrastructure.Events;
using Berico.SnagL.Infrastructure.Graph;

namespace Berico.SnagL.Infrastructure.Graph.Events
{
    /// <summary>
    /// Represents arguments for general events related to edge view models
    /// </summary>
    public class EdgeViewModelEventArgs : IScopingContainer<string>
    {
        /// <summary>
        /// Gets the view model of the edge that fired the event
        /// </summary>
        public IEdgeViewModel EdgeViewModel { get; private set; }


        /// <summary>
        /// Creates a new instance of the Berico.SnagL.Events.
        /// EdgeViewModelEventArgs class using the provided 
        /// EdgeViewModelBase
        /// </summary>
        /// <param name="_edgeVM">The view model class for the edge involved
        /// in the event</param>
        /// <param name="_sourceID">The ID for graph that this object belongs to</param>
        public EdgeViewModelEventArgs(IEdgeViewModel _edgeVM, string _scope)
        {
            EdgeViewModel = _edgeVM;
            Scope = _scope;
        }

        #region IScopingContainer<string> Members

            private string scope = string.Empty;
            public string Scope
            {
                get { return this.scope; }
                private set { this.scope = value; }
            }

        #endregion
    }

}