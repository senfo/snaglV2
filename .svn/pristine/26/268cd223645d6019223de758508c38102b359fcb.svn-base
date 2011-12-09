//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Berico.SnagL.Model;

namespace Berico.SnagL.Infrastructure.Data
{
    /// <summary>
    /// This class represents a container for edges.  At this point in
    /// time, we are only dsupporting a directed graph which means we
    /// need directed edges.  This class should not be used directly
    /// as it is used directely by the Graph class.
    /// </summary>
    public class EdgeContainer
    {
        private List<IEdge> inEdges = null;
        private List<IEdge> outEdges = null;
        private bool isDirected = false;

        /// <summary>
        /// Initializes a new instance of the Berico.LinkAnalysis.Model.EdgeContainer
        /// class with an indicataion of whether the container holds directed or
        /// undirected edges.
        /// </summary>
        /// <param name="_isDirected"></param>
        public EdgeContainer(bool _isDirected)
        {
            isDirected = _isDirected;
        }

        /// <summary>
        /// Adds the provided edge to the incomming edges collection
        /// </summary>
        /// <param name="edge">The edge to be added to the incomming edges collection</param>
        public void AddIncomingEdge(IEdge edge)
        {
            // Initialize collection if it isn't already (switch to using LAZY)
            if (inEdges == null)
                inEdges = new List<IEdge>();

            // Add the edge to the collection
            inEdges.Add(edge);
        }

        /// <summary>
        /// Adds the provided edge to the outgoing edges collection
        /// </summary>
        /// <param name="edge">The edge to be added to the outgoing edges collection</param>
        public void AddOutgoingEdge(IEdge edge)
        {
            // Initialize collection if it isn't already (switch to using LAZY)
            if (outEdges == null)
                outEdges = new List<IEdge>();

            // Add the edge to the collection
            outEdges.Add(edge);
        }

        /// <summary>
        /// Remove the provided edge from this EdgeContainer
        /// </summary>
        /// <param name="edge">The edge that should be removed</param>
        /// <returns>true if the edge is removed; otherwise false</returns>
        public bool RemoveEdge(IEdge edge)
        {
            bool edgeRemoved = false;

            // Remove it from the incoming edges, if it exists there
            if (ContainsIncommingEdge(edge))
                edgeRemoved = this.inEdges.Remove(edge);

            // Remove it from the incoming edges, if it exists there
            if (ContainsOutgoingEdge(edge))
                edgeRemoved = this.outEdges.Remove(edge);

            return edgeRemoved;
        }

        /// <summary>
        /// Determines if the provided edge is in the collection of incomming
        /// edges or not
        /// </summary>
        /// <param name="edge">The incomming edge to check for</param>
        /// <returns>True if the edge is found; otherwise false</returns>
        public bool ContainsIncommingEdge(IEdge edge)
        {
            if (inEdges == null)
                return false;

            return inEdges.Contains(edge);
        }

        /// <summary>
        /// Determines if the provided edge is in the collection of outgoing
        /// edges or not
        /// </summary>
        /// <param name="edge">The outgoing edge to check for</param>
        /// <returns>True if the edge is found; otherwise false</returns>
        public bool ContainsOutgoingEdge(IEdge edge)
        {
            if (outEdges == null)
                return false;

            return outEdges.Contains(edge);
        }

        /// <summary>
        /// Gets a list of all the incomming edges.  An incomming edge is an edge
        /// from a target to a source (from the perspective of the source).
        /// </summary>
        public List<IEdge> IncommingEdges
        {
            get
            {
                if (inEdges != null)
                    return inEdges;
                else
                    return new List<IEdge>();
            }
        }

        /// <summary>
        /// Gets a list of outgoing edges.  An outgoing edge is an edge from 
        /// a source to a target (from the perspective of the source).
        /// </summary>
        public List<IEdge> OutgoingEdges
        {
            get
            {
                if (outEdges != null)
                    return outEdges;
                else
                    return new List<IEdge>();
            }
        }

        /// <summary>
        /// Gets a list of all edges (both incomming and outgoing).  Since incoming and
        /// outgoing edges are copies of each other, this list will contain duplicates.
        /// </summary>
        public List<IEdge> Edges
        {
            get
            {
                // Check if one of the edge collections is null
                if (outEdges != null && inEdges != null)
                {
                    // Return all the in and out edges
                    return inEdges.Concat(outEdges).ToList();
                }
                else
                {
                    // Check if we have outedges
                    if (outEdges != null)
                        return outEdges;
                    else if (inEdges != null)
                        return inEdges;
                    else
                        return new List<IEdge>();
                }

            }
        }
    }
}