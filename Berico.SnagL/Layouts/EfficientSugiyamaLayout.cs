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
    using GraphSharp.Algorithms.Layout.Simple.Hierarchical;
    using QuickGraph;

    /// <summary>
    /// Efficient Sugiyama Layout Algorithm
    /// </summary>
    [Export(typeof(LayoutBase))]
    public class EfficientSugiyamaLayout : AsynchronousLayoutBase
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
            get { return typeof(EfficientSugiyamaLayout).Name; }
        }

        /// <summary>
        /// Performs the actual layout algorithm.
        /// </summary>
        /// <param name="graph">The object containing the graph data</param>
        /// <param name="rootNode">Root node</param>
        protected override void PerformLayout(GraphMapData graph, INode rootNode)
        {
            AdjacencyGraph<string, Edge<string>> adjacencyGraph = GraphSharpUtility.GetAdjacencyGraph(graph);
            EfficientSugiyamaLayoutParameters efficientSugiyamaLayoutParameters = new EfficientSugiyamaLayoutParameters();
            IDictionary<string, Vector> nodePositions = GraphSharpUtility.GetNodePositions(graph);
            IDictionary<string, Size> nodeSizes = GraphSharpUtility.GetNodeSizes(graph);

            EfficientSugiyamaLayoutAlgorithm<string, Edge<string>, AdjacencyGraph<string, Edge<string>>> efficientSugiyamaLayoutAlgorithm = new EfficientSugiyamaLayoutAlgorithm<string, Edge<string>, AdjacencyGraph<string, Edge<string>>>(adjacencyGraph, efficientSugiyamaLayoutParameters, nodePositions, nodeSizes);
            efficientSugiyamaLayoutAlgorithm.Compute();

            GraphSharpUtility.SetNodePositions(graph, efficientSugiyamaLayoutAlgorithm.VertexPositions);
        }
    }
}
