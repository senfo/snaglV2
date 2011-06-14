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
    using System;
    using System.ComponentModel.Composition;
    using System.Windows;
    using Berico.SnagL.Infrastructure.Data.Mapping;
    using Berico.SnagL.Model;

    /// <summary>
    /// Lays out the nodes in a circular fashion with a central node
    /// </summary>
    [Export(typeof(LayoutBase))]
    public class RadialLayout : AsynchronousLayoutBase
    {
        private const double MIN_NODE_ARC_SPACING = 150D;

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
                return InternalLayouts.Radial;
            }
        }

        /// <summary>
        /// Performs the actual layout algorithm.
        /// </summary>
        /// <param name="graph">The object containing the graph data</param>
        /// <param name="rootNode">Root node</param>
        protected override void PerformLayout(GraphMapData graph, INode rootNode)
        {
            double currentAngle = 0D; // Represents the current angle

            int numNodes = graph.Nodes.Count;
            double angle = GetAngle(numNodes);
            double radius = GetRadius(numNodes); // The computed radius of the circle

            // Loop through each node, perform the appropriate calculations,
            // then move it to the correct position on the graph.
            foreach (NodeMapData node in graph.GetNodes())
            {
                if (rootNode != null && node.Id.Equals(rootNode.ID))
                {
                    Point position = new Point(0D, 0D);
                    node.Position = position;
                }
                else
                {
                    //Calculate radians
                    double radians = Math.PI * currentAngle / 180D;
                    double x = Math.Cos(radians) * radius;
                    double y = Math.Sin(radians) * radius;

                    Point position = new Point(x, y);
                    node.Position = position;

                    currentAngle += angle;
                }
            }
        }

        /// <summary>
        /// Determines the appropriate angle
        /// </summary>
        /// <param name="nodeVMs"></param>
        /// <returns></returns>
        private static double GetAngle(int numNodes)
        {
            return 360D / (numNodes - 1D);
        }

        /// <summary>
        /// This method returns the radius of the circle that is needed
        /// to encompass all the nodes that are curently in the graph.
        /// </summary>
        /// <param name="nodeVMs">A collection of the node view models that make up the current graph</param>
        /// <returns>the radius of the circle</returns>
        private static double GetRadius(int numNodes)
        {
            // Calculate the radius and return it
            return (MIN_NODE_ARC_SPACING * (numNodes - 1D)) / (2D * Math.PI);
        }
    }
}
