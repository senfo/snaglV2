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
using Berico.SnagL.Infrastructure.Data;
using Berico.SnagL.Infrastructure.Modularity.Contracts;
using Berico.SnagL.Model;

namespace Berico.SnagL.Infrastructure.Ranking
{
    /// <summary>
    /// Represents a Ranking algorithm that ranks nodes on a
    /// graph based on their centrality (the number of edges 
    /// that they have)
    /// </summary>
    [Export(typeof(IRanker))]
    public class DegreeCentralityRanker : RankerBase
    {
        /// <summary>
        /// Creates a new instance of the DegreeCentralityRanker class
        /// </summary>
        public DegreeCentralityRanker() : base("Degree Centrality", "Ranks nodes based on their degree (number of edges)") {}

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
            GraphData graph = GraphManager.Instance.GetGraphComponents(scope).Data;

            // Loop over all the nodes in the graph
            foreach (INode node in graph.Nodes)
            {
                // Get the degree for the current node
                results[node] = graph.Degree(node);
            }

            return results;
        }
    }
}
