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
using System.Windows;

namespace Berico.SnagL.Infrastructure.Modularity.Contracts
{
    /// <summary>
    /// Provides the contract for all Tool Panel item extension view models.
    /// A toolbar item extension represents an item (some control) on the
    /// toolbar.  This is primarily used by MEF for importing and exporting.
    /// </summary>
    public interface IToolPanelItemViewModelExtension
    {
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
        /// Gets or sets a name for this tool panel item.
        /// </summary>
        string ToolName { get; set; }

        /// <summary>
        /// Gets or sets whether the tool panel is currently
        /// enabled or not
        /// </summary>
        bool IsEnabled { get; set; }
    }
}