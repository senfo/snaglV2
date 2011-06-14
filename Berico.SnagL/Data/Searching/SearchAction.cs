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
    /// Specifies the action to be performed on the nodes
    /// that are a result of a search
    /// </summary>
    public enum SearchAction
    {
        /// <summary>
        /// Indicates no current action
        /// </summary>
        Unknown,
        /// <summary>
        /// Indicates that nodes will be selected
        /// </summary>
        Select,
        /// <summary>
        /// Indicates that nodes will be filtered
        /// </summary>
        Filter
    }
}