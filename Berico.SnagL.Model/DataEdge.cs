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
using Berico.Common;
using System.ComponentModel;
using Berico.SnagL.Model.Attributes;

namespace Berico.SnagL.Model
{
    /// <summary>
    /// Represents an edge that contains additional data
    /// </summary>
    /// <exception cref="System.ArgumentNullException">Thrown in the event that a provided property value is null</exception> 
    /// <exception cref="System.ArgumentException">Thrown in the event that a provided property value is null</exception> 
    public class DataEdge : Edge, IDataEdge
    {
        private string id = string.Empty;
        private string description = string.Empty;
        private string displayValue = string.Empty;
        private AttributeCollection attributes = null;

        /// <summary>
        /// Initialzies a new instance of Berico.LinkAnalysis.Model.DataEdge with
        /// the provided source and target nodes.  The source and target nodes
        /// can not be null and can not be the same.
        /// </summary>
        /// <param name="_source"></param>
        /// <param name="_target"></param>
        public DataEdge(INode _source, INode _target)
            : this(_source, _target, new AttributeCollection())
        { }

        /// <summary>
        /// Initialzies a new instance of Berico.LinkAnalysis.Model.DataEdge with
        /// the provided source and target nodes.  The source and target nodes
        /// can not be null and can not be the same.
        /// </summary>
        /// <param name="_source">The source Node for this edge</param>
        /// <param name="_target">The target Node for this edge</param>
        /// <param name="_attributes">An existing AttributeCollection instance</param>
        public DataEdge(INode _source, INode _target, AttributeCollection _attributes)
            : base(_source, _target)
        {
            // Initialize the attribute collection
            this.attributes = _attributes;
        }

        /// <summary>
        /// Provides an appropriate string value for this edge
        /// </summary>
        /// <returns>An appropriate string value for this edge</returns>
        public override string ToString()
        {
            return string.Format("[ID: {0}, Description: {1}, Source: {2}, Target: {3}]", !string.IsNullOrEmpty(this.id) ? this.id : "N/A", !string.IsNullOrEmpty(this.description) ? this.description : "N/A", this.Source, this.Target);
        }

        #region IDataEdge Members

            /// <summary>
            /// Gets or sets an identifier for this edge.  This is just
            /// some identifying string.  It isn't used as a key so it
            /// doesn't need to be unique but it should identify the edge.
            /// </summary>
            public string ID
            {
                get { return this.id; }
                set
                {
                    if (value != this.id)
                    {
                        string oldValue = this.id;
                        this.id = value;

                        NotifyPropertyChanged("ID", oldValue, value);
                    }
                }
            }

            /// <summary>
            /// Gets or sets a description for this edge.  Most graphs don't 
            /// use this information.
            /// </summary>
            public string Description
            {
                get { return this.description; }
                set
                {
                    if (value != this.description)
                    {
                        string oldValue = this.description;
                        this.description = value;

                        NotifyPropertyChanged("Description", oldValue, value);
                    }
                }
            }

            /// <summary>
            /// Gets or sets a dsiaplay value for this edge.  This isn't used
            /// by most graphs as edges don't typically display a value.
            /// </summary>
            [ExportableProperty("DisplayValue")]
            public string DisplayValue
            {
                get { return this.displayValue; }
                set
                {
                    if (value != this.displayValue)
                    {
                        string oldValue = this.displayValue;
                        this.displayValue = value;

                        NotifyPropertyChanged("DisplayValue", oldValue, value);
                    }
                }
            }

            /// <summary>
            /// Gets the collection of attributes for this node
            /// </summary>
            public AttributeCollection Attributes
            {
                get
                {
                    // If the collection hasn't yet been initialized, initialize it
                    if (this.attributes == null)
                        this.attributes = new AttributeCollection();

                    return attributes; 
                }
            }

            /// <summary>
            /// Creates a new edge that is of the appropriate type.  The
            /// new edge will be a copy of the current edge but with a
            /// different source and target.
            /// </summary>
            /// <param name="source">The source Node</param>
            /// <param name="target">The target Node</param>
            /// <returns>a new edge</returns>
            public override IEdge Copy(INode source, INode target)
            {
                //TODO:  SHOULD BE UPDATED TO BE A MORE COMPLETE COPY

                DataEdge newEdge = new DataEdge(source, target, this.attributes)
                {
                    ID = this.id,
                    Description = this.description,
                    DisplayValue = this.displayValue
                };

                return newEdge;
            }

        #endregion

    }
}