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
using Berico.SnagL.Infrastructure.Ranking;
using Berico.SnagL.Model;

namespace Berico.SnagL.Infrastructure.Modularity.Contracts
{
    /// <summary>
    /// Represents an algorithm for ranking nodes on a graph
    /// </summary>
    public interface IRanker
    {
        /// <summary>
        /// Indicates that the ranking operation has completed
        /// </summary>
        event EventHandler<RankingEventArgs> RankingCompleted;

        /// <summary>
        /// Gets the name of this ranking algorithm
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a description for this ranking algorithm
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Calculates the rank for the graph
        /// specified by the provided scope
        /// </summary>
        /// <param name="scope">The scope for the graph to be ranked</param>
        /// <returns>the ranking for the nodes on the graph in the form
        /// of a dictionary of nodes and their scores.  The ranking is
        /// normalized between 0 and 1.</returns>
        Dictionary<INode, double> CalculateRank(string scope);

        /// <summary>
        /// Asynchronously calculates the rank for the graph
        /// specified by the provided scope
        /// </summary>
        /// <param name="scope">The scope for the graph to be ranked</param>
        void CalculateRankAsync(string scope);
    }
}
