//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using Berico.SnagL.Infrastructure.Modularity.Contracts;

namespace Berico.SnagL.Infrastructure.Modularity
{
    /// <summary>
    /// Represents arguments for events related to tool bar items
    /// </summary>
    public class ToolBarItemEventArgs
    {
        /// <summary>
        /// Gets the view model of the tool bar item that fired the event
        /// </summary>
        public IToolbarItemViewModelExtension ToolBarItem { get; private set; }

        /// <summary>
        /// Gets the scope of the data that the toolbar item clicked can
        /// influence
        /// </summary>
        public string Scope { get; private set; }

        /// <summary>
        /// Creates a new instance of the Berico.SnagL.Events.
        /// ToolBarItemEventArgs class using the provided 
        /// IToolbarItemViewModelExtension
        /// </summary>
        /// <param name="_toolbarItem">The toolbar item involved in the event</param>
        public ToolBarItemEventArgs(IToolbarItemViewModelExtension _toolbarItem, string _scope)
        {
            ToolBarItem = _toolbarItem;
            Scope = _scope;
        }

    }
}