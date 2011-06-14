//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

namespace Berico.SnagL.Infrastructure.Layouts
{
    /// <summary>
    /// Provides a list of layout types
    /// </summary>
    public enum LayoutTypes
    {
        /// <summary>
        /// Lays out nodes in a circular fashion
        /// </summary>
        Circle,
        /// <summary>
        /// lays out nodes in a circule around a
        /// central node
        /// </summary>
        Radial,
        /// <summary>
        /// Lays out nodes in a hierarchical fashion
        /// </summary>
        Tree,
        /// <summary>
        /// Lays out nodes into a grid (columns and rows)
        /// </summary>
        Grid,
        /// <summary>
        /// Lays out nodes in an organized manner with respect
        /// to clustering.  This is a customized form of
        /// a ForceDirected layout.
        /// </summary>
        Network,
        /// <summary>
        /// Lays out nodes in an organized manner
        /// </summary>
        ForceDirected,
        LinLog
    }
}