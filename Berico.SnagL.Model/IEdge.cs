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
using Berico.Common.Events;

namespace Berico.SnagL.Model
{
    /// <summary>
    /// Provides a template for an edge.  An edge represents
    /// some relationship between to vertices (nodes) in a
    /// graph.
    /// </summary>
    public interface IEdge : IGraphObject
    {
        /// <summary>
        /// Gets the type of edge that this is.  Currently, only
        /// 'Directed' edges are supported.
        /// </summary>
        EdgeType Type { get; }

        /// <summary>
        /// Gets or sets the source Node for this edge
        /// </summary>
        INode Source { get; set; }

        /// <summary>
        /// Gets or sets the target Node for this edge
        /// </summary>
        INode Target { get; set; }

        /// <summary>
        /// Creates a new edge that is of the appropriate type.  The
        /// new edge will be a copy of the current edge but with a
        /// different source and target.
        /// </summary>
        /// <param name="source">The source Node</param>
        /// <param name="target">The target Node</param>
        /// <returns>a new edge</returns>
        IEdge Copy(INode source, INode target);

        /// <summary>
        /// Event that indicates some underlying property has chaned
        /// </summary>
        event EventHandler<PropertyChangedEventArgs<object>> PropertyChanged;
    }
}