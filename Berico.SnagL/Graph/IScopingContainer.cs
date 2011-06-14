//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

namespace Berico.SnagL.Infrastructure.Graph
{
    /// <summary>
    /// Defines a template for an item that has an assigned scope.
    /// An objects scope should not change during the lifetime of
    /// that object.
    /// </summary>
    /// <typeparam name="TScope">The type of the scope</typeparam>
    public interface IScopingContainer<TScope>
    {
        /// <summary>
        /// Gets the scope (or parent) of this object
        /// </summary>
        TScope Scope { get; }
    }
}