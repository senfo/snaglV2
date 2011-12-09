//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using Berico.Common;
using Berico.SnagL.Model.Attributes;

namespace Berico.SnagL.Model
{
    /// <summary>
    /// Provides a template for an edge that is capabale of storing 
    /// additional data
    /// </summary>
    public interface IDataEdge : IEdge
    {
        /// <summary>
        /// Gets or sets an identifier for this edge.  This value does not 
        /// need to be unique since it isn't used as a key.
        /// </summary>
        [ExportableProperty("ID")]
        string ID { get; set; }

        /// <summary>
        /// Gets or sets a description for this edge.  Most graphs don't 
        /// use this information.
        /// </summary>
        [ExportableProperty("Description")]
        string Description { get; set; }

        /// <summary>
        /// Gets or sets a dsiaplay value for this edge.  This isn't used
        /// by most graphs as edges don't typically display a value.
        /// </summary>
        [ExportableProperty("DisplayValue")]
        string DisplayValue { get; set; }

        /// <summary>
        /// Gets the collection of attributes for this edge
        /// </summary>
        AttributeCollection Attributes { get; }
    }
}