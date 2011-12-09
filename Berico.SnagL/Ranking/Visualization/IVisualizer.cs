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
    /// Represents some methodology for visualizing the
    /// importance of a node
    /// </summary>
    public interface IVisualizer
    {
        /// <summary>
        /// Performs the visualization routine
        /// </summary>
        /// <param name="target">The node that is the target for this visualization</param>
        /// <param name="importance">A value that indicates how important the target
        ///   is.  This value affects the target visualization.</param>
        void Visualize(NodeViewModelBase target, double importance);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        void ClearVisualization(NodeViewModelBase target);
    }
}
