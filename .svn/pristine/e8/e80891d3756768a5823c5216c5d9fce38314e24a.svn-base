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
using System.Windows;
using Berico.SnagL.Infrastructure.Data.Mapping;
using Berico.SnagL.Infrastructure.Graph;
using Berico.SnagL.Model;

namespace Berico.SnagL.Infrastructure.Layouts
{
    /// <summary>
    /// This layout class is responsible for laying
    /// the nodes out into a circular pattern.
    /// </summary>
    [Export(typeof(LayoutBase))]
    public class CircleLayout : AsynchronousLayoutBase
    {
        private const double MIN_NODE_ARC_SPACING = 0;

        /// <summary>
        /// Gets the name of the layout
        /// </summary>
        public override string LayoutName
        {
            get
            {
                return InternalLayouts.Circle;
            }
        }

        /// <summary>
        /// Initializes a new instance of the CircleLayout class
        /// </summary>
        public CircleLayout()
        {
            LayoutType = LayoutTypes.Circle;
        }

        /// <summary>
        /// Performs the actual layout algorithm. This will execute on a background thread.
        /// </summary>
        /// <param name="graphData">The object containing the graph data</param>
        /// <param name="rootNode">Root node</param>
        protected override Dictionary<INodeShape, NodePosition> PerformLayout(GraphMapData graphData, INode rootNode)
        {
            IList<NodeMapData> nodeVMs = new List<NodeMapData>(graphData.GetNodes());
            IDictionary<INodeShape, NodePosition> nodes = new Dictionary<INodeShape, NodePosition>();

            double currentAngle = 0; // Represents the current angle
            double radius = GetRadius(nodeVMs); // The computed radius of the circle

            // Loop through each visible node, perform the appropriate calculations,
            // then move it to the correct position on the graph.
            bool firstPass = true;
            foreach (NodeMapData nodeVM in nodeVMs)
            {
                // Determine the angle for the current node
                if (!firstPass)
                    currentAngle += GetAngle(nodeVM, radius);

                //Calculate radians (radians = degrees * PI/180)
                double radians = currentAngle * Math.PI / 180;
                double x = Math.Cos(radians) * radius;
                double y = Math.Sin(radians) * radius;

                // We're no longer on the first pass
                firstPass = false;

                // Return the node position
                nodes.Add(nodeVM, new NodePosition(new Point(x, y), currentAngle));
            }

            return nodes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lastNode"></param>
        /// <param name="thisNode"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        private static double GetAngle(NodeMapData thisNode, double radius)
        {
            // Is this the first node?
            //if (lastNode == null) return 0;

            // Lay out node after the 
            double circumference = 2 * Math.PI * radius;

            double chordLength = CalculateChordLength(thisNode);

            // Percent of the total circumference we need to move
            double percent = (MIN_NODE_ARC_SPACING + chordLength) / circumference;
            return 360.0 * percent;
        }

        /// <summary>
        /// This method returns the radius of the circle that is needed
        /// to encompass all the nodes that are curently in the graph.
        /// </summary>
        /// <param name="nodeVMs">A collection of the node view models that make up the current graph</param>
        /// <returns>the radius of the circle</returns>
        private static double GetRadius(IList<NodeMapData> nodeVMs)
        {

            // Calculate the circumference of the circle we need
            // based on the size of all the nodes in the graph
            double circumference = nodeVMs.Sum(nodeVM => MIN_NODE_ARC_SPACING + CalculateChordLength(nodeVM));

            // Calculate the radius and return it
            return circumference / (2 * Math.PI);

        }

        /// <summary>
        /// Calulcates the length of the chord for the provided NodeViewModelBase
        /// object.  A 'chord' is a line segment that connects two points on a 
        /// curve.
        /// </summary>
        /// <param name="node">Node for which to calculate the chord length for</param>
        /// <returns>The chord length for the givenn node</returns>
        private static double CalculateChordLength(NodeMapData node)
        {
            // Use the Pythagorean theorem to calculate the length of the chord
            // segment between each node based on the height and width of the
            // provided node.
            return Math.Sqrt(node.Width * node.Width + node.Height * node.Height + 1);
        }
    }
}