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
using System;

namespace Berico.SnagL.Infrastructure.Modularity.Contracts
{
    /// <summary>
    /// Provides the contract for all toolbar item extension view models.
    /// A toolbar item extension represents an item (some control) on the
    /// toolbar.  This is primarily used by MEF for importing and exporting.
    /// </summary>
    public interface IToolbarItemViewModelExtension
    {
        event EventHandler<EventArgs> ToolbarItemSelected;

        /// <summary>
        /// Gets or sets the index for this item on the toolbar.  The indexes do not
        /// have to be in order and range from left (lower values) to right
        /// (upper values).  Any indexes with the same value will be grouped
        /// together while other items are seperated by a seperator.
        /// </summary>
        int Index { get; set; }

        /// <summary>
        /// Gets or sets the description for this item.  This value is used
        /// as the tooltip for this item.
        /// </summary>
        string Description { get; set;  }

        /// <summary>
        /// Gets or sets a name for this toolbar item.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets a command object that handles the the event that
        /// a toolbar item was selected
        /// </summary>
        ICommand ItemSelected { get; }

        /// <summary>
        /// Gets or sets whether this item is enabled or not
        /// </summary>
        bool IsEnabled { get; set; }

    }
}