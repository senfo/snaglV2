//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using Berico.SnagL.Infrastructure.Graph;

namespace Berico.SnagL.Infrastructure.Ranking.Visualization
{
    /// <summary>
    /// Visualizes the importance of a node by altering
    /// it's size
    /// </summary>
    public class ScaleVisualizer : IVisualizer
    {
        /// <summary>
        /// Visualizes the target node's importance by altering it's size
        /// </summary>
        /// <param name="target">The node that is the target for this visualization</param>
        /// <param name="importance">A value that indicates how important the target
        ///   is.  This value affects the visualization functionality.</param>
        public void Visualize(NodeViewModelBase target, double importance)
        {
            if (importance == 0)
                target.Scale = 0.75;
            else if (importance == 1)
                target.Scale = 3;
            else
                target.Scale = 1.0 + importance;
        }

        /// <summary>
        /// Removes the scale visualization from the target node
        /// </summary>
        /// <param name="target">The node whose visualization is to be removed</param>
        public void ClearVisualization(NodeViewModelBase target)
        {
            target.Scale = 1.0;
        }
    }
}
