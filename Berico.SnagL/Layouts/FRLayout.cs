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
    using GraphSharp.Algorithms.Layout.Simple.FDP;
    using QuickGraph;

    /// <summary>
    /// Fruchterman-Reingold Layout Algorithm
    /// </summary>
    [Export(typeof(LayoutBase))]
    public class FRLayout : AsynchronousLayoutBase
    {
        private bool useNodePositions;

        public FRLayout()
        {
            useNodePositions = true;
        }

        public FRLayout(bool useNodePositions)
        {
            this.useNodePositions = useNodePositions;
        }

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
            get
            {
                return InternalLayouts.Fr;
            }
        }

        /// <summary>
        /// Performs the actual layout algorithm.
        /// </summary>
        /// <param name="graph">The object containing the graph data</param>
        /// <param name="rootNode">Root node</param>
        protected override void PerformLayout(GraphMapData graph, INode rootNode)
        {
            AdjacencyGraph<string, Edge<string>> adjacencyGraph = GraphSharpUtility.GetAdjacencyGraph(graph);
            FreeFRLayoutParameters freeFRLayoutParameters;
            if (useNodePositions)
            {
                freeFRLayoutParameters = new FreeFRLayoutParameters()
                {
                    IdealEdgeLength = 125D
                };
            }
            else
            {
                freeFRLayoutParameters = new FreeFRLayoutParameters();

                GridLayout gridLayout = new GridLayout();
                gridLayout.CalculateLayout(graph);
            }
            IDictionary<string, Vector> nodePositions = GraphSharpUtility.GetNodePositions(graph);

            FRLayoutAlgorithm<string, Edge<string>, AdjacencyGraph<string, Edge<string>>> frLayoutAlgorithm = new FRLayoutAlgorithm<string, Edge<string>, AdjacencyGraph<string, Edge<string>>>(adjacencyGraph, nodePositions, freeFRLayoutParameters);
            frLayoutAlgorithm.Compute();

            GraphSharpUtility.SetNodePositions(graph, frLayoutAlgorithm.VertexPositions);
            GraphSharpUtility.FSAOverlapRemoval(graph);
        }
    }
}
