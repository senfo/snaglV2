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
using Berico.Common.Events;
using Berico.SnagL.Model.Attributes;

namespace Berico.SnagL.Model
{
    /// <summary>
    /// Represents a vertex (some arbitray object) in a graph
    /// </summary>
    /// <exception cref="System.ArgumentNullException">Thrown in the event that a provided property value is null</exception>
    public class Node : INode, INotifyPropertyChanged<string>
    {
        private string id = string.Empty;
        private string description = string.Empty;
        private string displayValue = string.Empty;
        private AttributeCollection attributes = null;

        /// <summary>
        /// Gets or sets the mechanism for which the object was added to the graph
        /// </summary>
        public CreationType SourceMechanism
        {
            get;
            set;
        }

        /// <summary>
        /// Initialzies a new instance of Berico.LinkAnalysis.Model.Node with
        /// the provided id name.  ID must be unique for this node
        /// </summary>
        /// <param name="label"></param>
        public Node(string _id) 
        {
            if (string.IsNullOrEmpty(_id))
                throw new ArgumentNullException("Node", "A display value must be provided for each node");

            this.id = _id;
        }

        /// <summary>
        /// Gets an identifier for this node.  This identifier must be
        /// unique among the nodes as it is used to perform equality
        /// comaprisons between nodes.
        /// </summary>
        //[ExportableProperty("ID")]
        public string ID
        {
            get { return this.id; }
        }

        /// <summary>
        /// Gets or sets a description for this node.
        /// </summary>
        [ExportableProperty("Description")]
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
        /// Gets or sets a display value for this node.  This value is
        /// used as the nodes label on the graph.
        /// </summary>
        [ExportableProperty("DisplayValue")]
        public string DisplayValue
        {
            get
            {
                if (String.IsNullOrEmpty(this.displayValue))
                    return this.id;
                else
                    return this.displayValue;
            }
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
        /// Fires the PropertyChanged event
        /// </summary>
        /// <param name="info">A string that contains the name of the property that has changed</param>
        /// <param name="oldValue">The old value</param>
        /// <param name="newValue">The new value</param>
        protected void NotifyPropertyChanged(string info, string oldValue, string newValue)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs<string>(info, newValue, oldValue));
        }

        #region INotifyPropertyChanged<object> Members

            public event EventHandler<PropertyChangedEventArgs<string>> PropertyChanged;

        #endregion

        /// <summary>
        /// Provides an appropriate string value for this node
        /// </summary>
        /// <returns>An appropriate string value for this node</returns>
        public override string ToString()
        {
            return "[Index: " + this.id + ", Label: " + this.displayValue + "]";
        }

        /// <summary>
        /// Compares this instance with a specified object or <see cref="Node"/> and returns an integer that indicates whether this instance precedes, follows, or appears in the same position in the sort order as the specified object or <see cref="Node"/>.
        /// </summary>
        /// <param name="obj">The <see cref="INode"/> to compare with this instance</param>
        /// <returns>A 32-bit signed integer that indicates whether this instance precedes, follows, or appears in the same position in the sort order as the <paramref name="obj"/> parameter</returns>
        public int CompareTo(object obj)
        {
            return ID.CompareTo(((INode)obj).ID);
        }

        /// <summary>
        /// Returns a hash code for this <see cref="Node"/>
        /// </summary>
        /// <returns>A 32-bit signed integer hash code</returns>
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        /// <summary>
        /// Determines whether this instance of Berico.LinkAnalysis.Model.Node and a
        /// specified object, which must also be a Berico.LinkAnalysis.Model.Node
        /// object, have the same value.  The main source for comparison is the nodes
        /// ID property.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (this.GetType() != obj.GetType())
                return false;

            return Equals(obj as Node);
        }

        /// <summary>
        /// Determines whether this instance and another specified Berico.LinkAnalysis.Model.Node
        /// object have the same value.  The main source for comparison is the nodes
        /// ID property.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Equals(Node obj)
        {
            if (obj == null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (this.GetHashCode() != obj.GetHashCode())
                return false;

            return (ID.Equals(obj.ID));
        }

        /// <summary>
        /// Determines whether two specified Berico.LinkAnalysis.Model.Node objects have the same value
        /// </summary>
        /// <param name="leftHandSide">A Berico.LinkAnalysis.Model.Node</param>
        /// <param name="rightHandSide">A Berico.LinkAnalysis.Model.Node</param>
        /// <returns>true if the value of leftHandSide is the same as the value of righthandside;
        /// otherwise, false</returns>
        public static bool operator ==(Node leftHandSide, Node rightHandSide)
        {
            // Check if leftHandSide is null
            if (ReferenceEquals(leftHandSide, null))
                return ReferenceEquals(rightHandSide, null);

            return (leftHandSide.Equals(rightHandSide));
        }

        /// <summary>
        /// Determines whether two specified Berico.LinkAnalysis.Model.Node objects have different values
        /// </summary>
        /// </summary>
        /// <param name="leftHandSide">A Berico.LinkAnalysis.Model.Node</param>
        /// <param name="rightHandSide">A Berico.LinkAnalysis.Model.Node</param>
        /// <returns>true if the value of lefthandside is different from the value of righthandside;
        /// otherwise, false</returns>
        public static bool operator !=(Node leftHandSide, Node rightHandSide)
        {
            return !(leftHandSide == rightHandSide);
        }

    }
}