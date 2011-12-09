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
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Windows;
    using Berico.SnagL.Infrastructure.Data.Mapping;
    using Berico.SnagL.Model;
    using Berico.SnagL.UI;

    /// <summary>
    /// Lays out nodes into rows and columns
    /// </summary>
    [Export(typeof(LayoutBase))]
    public class GridLayout : AsynchronousLayoutBase
    {
        private static readonly Thickness MARGIN = new Thickness(5D, 5D, 5D, 0D);

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
                return InternalLayouts.Grid;
            }
        }

        /// <summary>
        /// Performs the actual layout algorithm.
        /// </summary>
        /// <param name="graph">The object containing the graph data</param>
        /// <param name="rootNode">Root node</param>
        protected override void PerformLayout(GraphMapData graph, INode rootNode)
        {
            ICollection<NodeMapData> nodes = graph.GetNodes();

            // First we determine the amount of space that we are dealing with
            double totalArea = 0D;
            foreach (NodeMapData node in nodes)
            {
                double nodeWidth = node.Dimension.Width + MARGIN.Left + MARGIN.Right;
                double nodeHeight = node.Dimension.Height + MARGIN.Top + MARGIN.Bottom;

                totalArea += nodeWidth * nodeHeight;
            }

            // TODO NEED TO DECOUPLE THIS
            GraphViewModel graphVM = ViewModelLocator.GraphDataStatic;

            // Calculate the bounding height and width of our square
            double boundingHeight = Math.Sqrt(totalArea);
            double boundingWidth = boundingHeight * (graphVM.Width / graphVM.Height);
            boundingHeight = boundingHeight / (graphVM.Width / graphVM.Height);

            //TODO: THE LINES ABOVE ARE THE ONLY REASON WE ARE COUPLED TO THE GRAPH VIEW MODEL

            double currentX = 0D;
            double currentY = 0D;
            double maxColumnWidth = 0D;

            double maxHeight = nodes.Max(node => node.Dimension.Height) + MARGIN.Top + MARGIN.Bottom;
            int columnSize = (int)Math.Round(boundingHeight / maxHeight);

            List<NodeMapData> nodesList = nodes.ToList<NodeMapData>();
            nodesList.Sort(NodeSizeComparer);
            foreach (NodeMapData node in nodesList)
            {
                if (currentY > boundingHeight)
                {
                    currentX += maxColumnWidth + MARGIN.Left + MARGIN.Right;
                    maxColumnWidth = 0D;
                    currentY = 0D;
                }

                maxColumnWidth = Math.Max(node.Dimension.Width, maxColumnWidth);

                Point position = new Point(currentX, currentY);
                node.Position = position;

                currentY += maxHeight;
            }
        }

        private static int NodeSizeComparer(NodeMapData first, NodeMapData second)
        {
            double areaFirst = first.Dimension.Width * first.Dimension.Height;
            double areaSecond = second.Dimension.Width * second.Dimension.Height;

            return -areaFirst.CompareTo(areaSecond);
        }
    }
}