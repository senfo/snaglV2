﻿//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

namespace Berico.SnagL.Model
{
    /// <summary>
    /// Represents a node that doesn't actually exist. Used for handling edges that were
    /// added during live import before their corresponding nodes existed.
    /// </summary>
    public class GhostNode : INode
    {
        #region Properties

        /// <summary>
        /// Gets the unique identifier for the node
        /// </summary>
        public string ID
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the mechanism for which the object was added to the graph
        /// </summary>
        public CreationType SourceMechanism
        {
            get;
            set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Prevents an instance of the <see cref="GhostNode"/> class from being instantiated
        /// </summary>
        private GhostNode()
        {
        }

        /// <summary>
        /// Initializes a new instance of the GhostNode class
        /// </summary>
        /// <param name="nodeId">Unique identifier for the node</param>
        public GhostNode(string nodeId)
        {
            ID = nodeId;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Compares this instance with a specified object or <see cref="GhostNode"/> and returns an integer that indicates whether this instance precedes, follows, or appears in the same position in the sort order as the specified object or <see cref="GhostNode"/>.
        /// </summary>
        /// <param name="obj">The <see cref="INode"/> to compare with this instance</param>
        /// <returns>A 32-bit signed integer that indicates whether this instance precedes, follows, or appears in the same position in the sort order as the <paramref name="obj"/> parameter</returns>
        public int CompareTo(object obj)
        {
            return ID.CompareTo(((INode)obj).ID);
        }

        /// <summary>
        /// Returns a hash code for this <see cref="GhostNode"/>
        /// </summary>
        /// <returns>A 32-bit signed integer hash code</returns>
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        #endregion
    }
}
