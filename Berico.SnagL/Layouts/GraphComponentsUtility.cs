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
    using System.Windows;
    using Berico.SnagL.Infrastructure.Data.Mapping;
    using Berico.SnagL.Infrastructure.Graph;
    using Berico.SnagL.Model;

    /// <summary>
    /// Utility methods for interacting with graph components
    /// </summary>
    public static class GraphComponentsUtility
    {
        /// <summary>
        /// Returns bare-bone graph needed for layouts
        /// </summary>
        /// <param name="graphComponents">GraphComponents</param>
        /// <returns>GraphMapData</returns>
        public static GraphMapData GetGraph(GraphComponents graphComponents)
        {
            GraphMapData graph = new GraphMapData();

            // Nodes
            IEnumerable<INodeShape> uiNodeViewModels = graphComponents.GetNodeViewModels();
            foreach (NodeViewModelBase uiNodeVM in uiNodeViewModels)
            {
                NodeMapData objNode = new TextNodeMapData(uiNodeVM.ParentNode.ID);
                graph.Add(objNode);

                // Properties
                Size dimension = new Size(uiNodeVM.Width, uiNodeVM.Height);
                objNode.Dimension = dimension;
                objNode.Position = uiNodeVM.Position;
                objNode.IsHidden = uiNodeVM.IsHidden;
            }

            // Edges
            IEnumerable<IEdgeViewModel> uiEdgeViewModels = graphComponents.GetEdgeViewModels();
            foreach (EdgeViewModelBase uiEdgeVM in uiEdgeViewModels)
            {
                EdgeMapData objEdge = new EdgeMapData(uiEdgeVM.ParentEdge.Source.ID, uiEdgeVM.ParentEdge.Target.ID);
                graph.Add(objEdge);

                // Properties
                objEdge.Type = uiEdgeVM.ParentEdge.Type;
                SimilarityDataEdge uiSDE = uiEdgeVM.ParentEdge as SimilarityDataEdge;
                if (uiSDE != null)
                {
                    objEdge.Weight = uiSDE.Weight;
                }
            }

            return graph;
        }
    }
}
