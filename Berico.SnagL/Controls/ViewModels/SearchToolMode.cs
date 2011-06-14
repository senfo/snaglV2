//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

namespace Berico.SnagL.Infrastructure.Controls
{
    /// <summary>
    /// Specifies the SearchTool's mode
    /// </summary>
    public enum SearchToolMode
    {
        /// <summary>
        /// Indicates no current mode
        /// </summary>
        Unknown,
        /// <summary>
        /// Indicates that nodes will be selected
        /// </summary>
        Find,
        /// <summary>
        /// Indicates that nodes will be filtered
        /// </summary>
        Filter
    }

}