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
    /// Represents arguments for general events related to node view models
    /// </summary>
    public class NodeViewModelEventArgs : IScopingContainer<string>
    {
        /// <summary>
        /// Gets the view model of the node that fired the event
        /// </summary>
        public NodeViewModelBase NodeViewModel { get; private set; }


        /// <summary>
        /// Creates a new instance of the Berico.SnagL.Events.
        /// NodeEventArgs class using the provided NodeViewModelBase
        /// and MouseEventArgs.
        /// </summary>
        /// <param name="_nodeVM">The view model class for the node involved
        /// in the event</param>
        /// <param name="_sourceID">The ID for graph that this object belongs to</param>
        public NodeViewModelEventArgs(NodeViewModelBase _nodeVM, string _scope)
        {
            NodeViewModel = _nodeVM;
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