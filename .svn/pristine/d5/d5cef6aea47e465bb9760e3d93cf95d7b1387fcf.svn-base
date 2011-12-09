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

namespace Berico.SnagL.Infrastructure.Graph.Events
{
    /// <summary>
    /// Represents arguments for mouse events related to edge view models
    /// </summary>
    public class EdgeViewModelMouseEventArgs<T> : EdgeViewModelEventArgs, IScopingContainer<string> where T : MouseEventArgs
    {

        /// <summary>
        /// Gets the mouse event arguments from the event
        /// </summary>
        public T MouseArgs { get; private set; }

        /// <summary>
        /// Creates a new instance of the Berico.SnagL.Events.
        /// EdgeViewModelMouseEventArgs class using the provided
        /// IEdgeViewModel and MouseEventArgs.
        /// </summary>
        /// <param name="_nodeVM">The view model class for the node involved
        /// in the event</param>
        /// <param name="_args">The mouse arguments related to the event</param>
        /// <param name="_sourceID">The ID for graph that this object belongs to</param>
        public EdgeViewModelMouseEventArgs(IEdgeViewModel _edgeVM, T _args, string _scope)
            : base(_edgeVM, _scope)
        {
            MouseArgs = _args;
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