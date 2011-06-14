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
    /// Represents a relationship between two vertices (nodes)
    /// on in a graph data structure
    /// </summary>
    public class Edge : IEdge, INotifyPropertyChanged<object>
    {
        private INode source = null;
        private INode target = null;
        private EdgeType type = EdgeType.Directed;

        /// <summary>
        /// Gets or sets the mechanism for which the object was loaded onto the graph
        /// </summary>
        public CreationType SourceMechanism
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the Edge class
        /// </summary>
        public Edge()
        {
        }

        /// <summary>
        /// Initializes a new instance of Berico.LinkAnalysis.Model.Edge with
        /// the provided source and target nodes.  The source and target nodes
        /// can not be null and can not be the same.
        /// </summary>
        /// <param name="_source">The source Node for this edge</param>
        /// <param name="_target">The target Node for this edge</param>
        public Edge(INode _source, INode _target)
        {
            // At this time, we are not supporting self-loops
            // so don't allow one to be created
            if (_source != null && _source.Equals(_target))
                throw new ArgumentException("Self-Loop edges (Source and Target nodes are the same) are not currently supported");

            // Ensure that the source node isn't null
            //if (_source == null)
            //    throw new ArgumentNullException("Source", "The provided source edge was null");

            // Ensure that the target node isn't null
            if (_target == null)
                throw new ArgumentNullException("Target", "The provided target edge was null");
            
            this.source = _source;
            this.target = _target;
        }

        #region IEdge Members

            /// <summary>
            /// Gets the type of edge that this is.
            /// </summary>
            public EdgeType Type
            {
                get { return this.type; }
                set { this.type = value; }
            }

            /// <summary>
            /// Gets or sets the source node for this edge 
            /// </summary>
            public INode Source
            {
                get
                {
                    return this.source;
                }
                set
                {
                    this.source = value;
                }
            }

            /// <summary>
            /// Gets the target node for this edge
            /// </summary>
            public INode Target
            {
                get
                {
                    return this.target;
                }
                set
                {
                    this.target = value;
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
            public virtual IEdge Copy(INode source, INode target)
            {
                return new Edge(source, target);
            }

            /// <summary>
            /// Event that indicates some underlying property has changed
            /// </summary>
            public event EventHandler<PropertyChangedEventArgs<object>> PropertyChanged;

        #endregion

        /// <summary>
        /// Fires the PropertyChanged event
        /// </summary>
        /// <param name="info">A string that contains the name of the property that has changed</param>
        /// <param name="oldValue">The old value</param>
        /// <param name="newValue">The new value</param>
        protected void NotifyPropertyChanged(string info, object oldValue, object newValue)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs<object>(info, newValue, oldValue));
        }

        /// <summary>
        /// Provides an appropriate string value for this edge
        /// </summary>
        /// <returns>An appropriate string value for this edge</returns>
        public override string ToString()
        {
            return string.Format("[Source: {0}, Target: {1}]", this.source, this.target);
        }

        /// <summary>
        /// Returns the hash code for this edge.  The hash is based off of the
        /// hash of both the Source and Target properties.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code</returns>
        public override int GetHashCode()
        {
            int hashCode = Source.GetHashCode();
            return hashCode ^= Target.GetHashCode();
        }

        /// <summary>
        /// Determines whether this instance of Berico.LinkAnalysis.Model.IEdge and a
        /// specified object, which must also be a Berico.LinkAnalysis.Model.IEdge
        /// object, have the same value.  The main source for comparison is the Source 
        /// and Node properties.
        /// </summary>
        /// <param name="obj">A System.Object</param>
        /// <returns>true if obj is a Berico.LinkAnalysis.Model.IEdge and its value is the 
        /// same as this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {

            if (obj == null)
                return false;

            if (this.GetType() != obj.GetType())
                return false;

            return Equals(obj as IEdge);

        }

        /// <summary>
        /// Determines whether this instance and another specified Berico.LinkAnalysis.Model.IEdge
        /// object have the same value.  The main source for comparison is the Source and Node
        /// properties.
        /// </summary>
        /// <param name="obj">A Berico.LinkAnalysis.Model.IEdge object to be compared to this
        /// instance for equality</param>
        /// <returns>true if the value of the obj parameter is the same as this instance; otherwise,
        /// false.</returns>
        public bool Equals(IEdge obj)
        {

            if (obj == null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (this.GetHashCode() != obj.GetHashCode())
                return false;

            return ((Source.Equals(obj.Source)) && ((Target.Equals(obj.Target))));
        }

        /// <summary>
        /// Determines whether two specified Berico.LinkAnalysis.Model.IEdge objects have the same value
        /// </summary>
        /// <param name="leftHandSide">A Berico.LinkAnalysis.Model.IEdge</param>
        /// <param name="rightHandSide">A Berico.LinkAnalysis.Model.IEdge</param>
        /// <returns>true if the value of lefthandside is the same as the value of righthandside; otherwise, false</returns>
        public static bool operator ==(Edge leftHandSide, Edge rightHandSide)
        {
            // Check if leftHandSide is null
            if (ReferenceEquals(leftHandSide, null))
                return ReferenceEquals(rightHandSide, null);

            return (leftHandSide.Equals(rightHandSide));
        }

        /// <summary>
        /// Determines whether two specified Berico.LinkAnalysis.Model.IEdge objects have different values
        /// </summary>
        /// <param name="leftHandSide">A Berico.LinkAnalysis.Model.IEdge</param>
        /// <param name="rightHandSide">A Berico.LinkAnalysis.Model.IEdge</param>
        /// <returns>true if the value of lefthandside is different from the value of righthandside;
        /// otherwise, false</returns>
        public static bool operator !=(Edge leftHandSide, Edge rightHandSide)
        {
            return !(leftHandSide == rightHandSide);
        }

    }
}