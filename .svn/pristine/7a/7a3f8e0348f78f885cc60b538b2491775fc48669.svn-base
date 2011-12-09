//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

namespace Berico.SnagL.Infrastructure.Layouts
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Windows;
    using Berico.SnagL.Infrastructure.Data.Mapping;
    using Berico.SnagL.Model;
    using GraphSharp;
    using GraphSharp.Algorithms.Layout.Contextual;
    using QuickGraph;

    /// <summary>
    /// Double Tree Layout Algorithm
    /// </summary>
    [Export(typeof(LayoutBase))]
    public class DoubleTreeLayout : AsynchronousLayoutBase
    {
        /// <summary>
        /// Gets a value that indicates whether or not the layout is enabled
        /// </summary>
        public override bool Enabled
        {
            get
            {
                throw new System.NotImplementedException();
            }
            protected set
            {
                throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// Get the name of the layout
        /// </summary>
        public override string LayoutName
        {
            get { return typeof(DoubleTreeLayout).Name; }
        }

        /// <summary>
        /// Performs the actual layout algorithm.
        /// </summary>
        /// <param name="graph">The object containing the graph data</param>
        /// <param name="selectedNode">selected node</param>
        protected override void PerformLayout(GraphMapData graph, INode selectedNode)
        {
            BidirectionalGraph<string, WeightedEdge<string>> bGraph = GraphSharpUtility.GetBidirectionalGraph(graph);
            IDictionary<string, Vector> nodePositions = GraphSharpUtility.GetNodePositions(graph);
            IDictionary<string, Size> nodeSizes = GraphSharpUtility.GetNodeSizes(graph);
            DoubleTreeLayoutParameters doubleTreeLayoutParameters = new DoubleTreeLayoutParameters();

            DoubleTreeLayoutAlgorithm<string, WeightedEdge<string>, BidirectionalGraph<string, WeightedEdge<string>>> doubleTreeLayoutAlgorithm = new DoubleTreeLayoutAlgorithm<string, WeightedEdge<string>, BidirectionalGraph<string, WeightedEdge<string>>>(bGraph, nodePositions, nodeSizes, doubleTreeLayoutParameters, selectedNode.ID);
            doubleTreeLayoutAlgorithm.Compute();

            GraphSharpUtility.SetNodePositions(graph, doubleTreeLayoutAlgorithm.VertexPositions);
        }
    }
}
