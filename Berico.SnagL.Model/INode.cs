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

namespace Berico.SnagL.Model
{
    /// <summary>
    /// Provides the base template for a node
    /// </summary>
    public interface INode : IGraphObject, IComparable
    {
        /// <summary>
        /// Gets an identifier for this node.  This identifier must be
        /// unique among the nodes as it is used to perform equality
        /// comparisons between nodes.
        /// </summary>
        string ID { get; }
    }
}