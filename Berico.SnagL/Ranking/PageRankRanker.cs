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
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Berico.SnagL.Infrastructure.Data;
using Berico.SnagL.Infrastructure.Graph;
using Berico.SnagL.Infrastructure.Modularity.Contracts;
using Berico.SnagL.Model;

namespace Berico.SnagL.Infrastructure.Ranking
{
    /// <summary>
    /// Represents a Ranking algorithm that ranks nodes on a graph
    /// based on their PageRank.  PageRank is the algorithm used
    /// by the Goolge search engine to rank pages.
    /// 
    /// The PageRank algorithm is:
    ///   PR(A) = (1 - d) + d(PR(T1)/C(T1) + ... + PR(Tn)/C(Tn)
    /// 
    /// where:
    ///   PR(A)  = PageRank of node A
    ///   PR(T1) = The current PageRank of node T1
    ///   C(T1)  = The number of edges from node T1
    ///   d      = Damping factor betwen 0 and 1 (typically 0.85)
    /// 
    /// Additional information on the PageRank algorithm can be
    /// found at http://www.markhorrell.com/seo/pagerank.html.
    /// </summary>
    [Export(typeof(IRanker))]
    public class PageRankRanker : RankerBase
    {
        private const double Damping = 0.85;
        private const int NumIterations = 20;
        private GraphComponents _graph;

        /// <summary>
        /// Creates a new instance of the PageRankRanker class
        /// </summary>
        public PageRankRanker() : base("PageRank", "Ranks nodes based on their relative importance within the graph") { }

        /// <summary>
        /// Calculates the rank for the graph specified by the provided scope.
        /// For ranks normalized between 0 and 1, use the CalculateNormalizedRank
        /// method.
        /// </summary>
        /// <param name="scope">The scope for the graph to be ranked</param>
        /// <returns>the ranking for the nodes on the graph in the form
        /// of a dictionary of nodes and their scores.  The ranking is
        /// normalized between 0 and 1.</returns>
        public override Dictionary<INode, double> CalculateRank(string scope)
        {
            // Valid the scope parameter
            if (string.IsNullOrEmpty(scope))
                throw new ArgumentNullException("Scope", "No scope was provided");

            // Initialize the results dictionary
            Dictionary<INode, double> results = new Dictionary<INode, double>();

            // Get the graph data using the provided scope
            _graph = GraphManager.Instance.GetGraphComponents(scope);

            // Iterate over the algorithm multiple times in order
            // to hone the results to the 'true' value
            for (int i = 0; i < NumIterations; i++)
            {
                // Loop over all the nodes in the graph
                foreach (INode node in _graph.Nodes)
                {
                    // Get the degree for the current node
                    results[node] = DetermineNodeRank(node, results);
                }
            }

            return results;
        }

        /// <summary>
        /// Calculates the PageRank of the specified node
        /// </summary>
        /// <param name="node">The node to calculate the PageRank for</param>
        /// <param name="pageRanks">A collection of the currently calculated page ranks</param>
        /// <returns>the PageRank for the specified node</returns>
        private double DetermineNodeRank(INode node, Dictionary<INode, double> pageRanks)
        {
            // Represents the sum of the PR(T1)/C(T1) portion of the PageRank algorithm
            double sum = (from pageRank in pageRanks
                          let currentNode = pageRank.Key
                          let currentNodeRank = pageRank.Value
                          where AreNodesConnected(node, currentNode)
                          select currentNodeRank/NeighborCount(currentNode)).Sum();

            // Return actual rank value, computed as (1 - d) + d(PR(T1)/C(T1))
            return (1 - Damping) + Damping * sum;
        }

        /// <summary>
        /// Determines if the two provided nodes are connected by an edge
        /// </summary>
        /// <param name="node">The first node to get edges for</param>
        /// <param name="otherNode">The node ot check linkage for</param>
        /// <returns>true if the two nodes are linked together; otherwise false</returns>
        private bool AreNodesConnected(INode node, INode otherNode)
        {
            // Loop over the edges in the graph
            return _graph.GetEdges(node).
                Where(edge => !_graph.GetEdgeViewModel(edge).IsHidden &&
                    !(_graph.GetEdgeViewModel(edge) is SimilarityDataEdge)).
                Any(edge => ((NodeViewModelBase) _graph.GetOppositeNode(edge, node)).ParentNode.Equals(otherNode));
        }

        /// <summary>
        /// Returns the number of neighboring nodes that are not connected
        /// by a similarity edge
        /// </summary>
        /// <param name="currentNode">The node to get the count for</param>
        /// <returns>the count of neighbors for the specified node</returns>
        private double NeighborCount(INode currentNode)
        {
            return _graph.GetEdges(currentNode).
                Count(edge => !_graph.GetEdgeViewModel(edge).IsHidden &&
                    !(_graph.GetEdgeViewModel(edge) is SimilarityDataEdge));
        }

    }
}
