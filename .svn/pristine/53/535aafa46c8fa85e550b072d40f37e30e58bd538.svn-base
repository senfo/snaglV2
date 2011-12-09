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
using Berico.SnagL.Infrastructure.Graph;
using Berico.SnagL.Model;
using Berico.SnagL.UI;

namespace Berico.SnagL.Infrastructure.Layouts
{
    /// <summary>
    /// Lays out nodes into rows and columns
    /// </summary>
    public class GridLayoutOrig
    {
        private Dictionary<INodeShape, Point> nodeToNewPositions = new Dictionary<INodeShape, Point>();
        private Thickness margin = new Thickness(5, 5, 5, 0);

        /// <summary>
        /// Performs the actual layout algorithm. This will execute on a background thread.
        /// </summary>
        /// <param name="isAnimated">Indicates whether or not the layout should be animated</param>
        /// <param name="graphComponents">The object containing the graph data</param>
        /// <param name="rootNode">Root node</param>
        public void ComputeLayout(bool isAnimated, GraphComponents graphComponents, INode rootNode)
        {
            //TODO:  NEED TO DECOUPLE THIS
            GraphViewModel graphVM = ViewModelLocator.GraphDataStatic;
            List<INodeShape> nodeVMs = graphComponents.GetNodeViewModels().ToList(); // Get all the node view models

            // Reset the new node position collection
            nodeToNewPositions.Clear();

            // First we determine the amount of space that we are dealing with
            double totalArea = 0;
            foreach (INodeShape nodeVMBase in nodeVMs)
            {
                double nodeWidth = nodeVMBase.Width + this.margin.Left + this.margin.Right;
                double nodeHeight = nodeVMBase.Height + this.margin.Top + this.margin.Bottom;

                totalArea += nodeWidth * nodeHeight;
            }

            nodeVMs.Sort(NodeSizeComparer);

            // Calculate the bounding height and width of our square
            double boundingHeight  =  CalculateWidthAndHeight(totalArea);
            double boundingWidth = boundingHeight * (graphVM.Width / graphVM.Height);
            boundingHeight = boundingHeight / (graphVM.Width / graphVM.Height);

            //TODO: THE LINES ABOVE ARE THE ONLY REASON WE ARE COUPLED TO THE GRAPH VIEW MODEL

            double currentX = 0;
            double currentY = 0;
            double maxColumnWidth = 0;

            double maxHeight = nodeVMs.Max(nodeVM => nodeVM.Height) + margin.Top + margin.Bottom;
            int columnSize = (int)Math.Round(boundingHeight / maxHeight);

            //var column = nodeVMs.Take(columnSize);
            foreach (INodeShape nodeVM in nodeVMs)
            {
                if (currentY > boundingHeight)
                {
                    currentX += maxColumnWidth + margin.Left + margin.Right;
                    maxColumnWidth = 0;
                    currentY = 0;
                }

                maxColumnWidth = Math.Max(nodeVM.Width, maxColumnWidth);

                nodeToNewPositions.Add(nodeVM, new Point(currentX, currentY));

                currentY += maxHeight;
                //currentX += nodeVM.Width + margin.Left + margin.Right;
            }
        }

        private int NodeSizeComparer(INodeShape first, INodeShape second)
        {
            double areaFirst = first.Width * first.Height;
            double areaSecond = second.Width * second.Height;

            return -areaFirst.CompareTo(areaSecond);
        }

        private double CalculateWidthAndHeight(double area)
        {
            return Math.Sqrt(area);
        }

        /// <summary>
        /// Resonsible for actually positioning the nodes.  This will
        /// execute on the UI thread, once the background thread
        /// running the algorithm has completed.
        /// </summary>
        /// <param name="isAnimated">Indicates whether or not the layout should be animated</param>
        public void PositionNodes(bool isAnimated)
        {
            // Loop over all the collection of nodes and their target positions.
            // We position nodes here because it must occur on the UI thread.
            foreach (INodeShape nodeVM in nodeToNewPositions.Keys)
            {
                // Position or animated this nodeVM
                if (nodeVM is NodeViewModelBase && isAnimated)
                    (nodeVM as NodeViewModelBase).AnimateTo(nodeToNewPositions[nodeVM]);
                else
                    nodeVM.Position = nodeToNewPositions[nodeVM];
            }
        }
    }
}