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
using Berico.SnagL.Infrastructure.Data;
using Berico.SnagL.Infrastructure.Graph;
using GalaSoft.MvvmLight.Threading;
using Berico.SnagL.Model;

namespace Berico.SnagL.Infrastructure.Layouts
{
    /// <summary>
    /// Implements a force-directed layout
    /// </summary>
    public class ForceDirected
    {
        private int maxIterations = 200;
        private static double defaultStiffness = 1;
        private bool useGraphPartitioning = true;
        private double K = 150;
        private Random random = new Random();

        private INodeShape[] nodeVMs;
        private Point[] positions;
        private int[] edgeCounts;
        List<int>[,] cellArray;
        Dictionary<int, int[]> nodeToCell = new Dictionary<int, int[]>();
        private Dictionary<string, int> nodeIDToIndex;
        private static object syncRoot = new object();

        /// <summary>
        /// Gets or sets the stiffness of edges.  A lower value makes the
        /// edges more relaxed.
        /// </summary>
        public static double DefaultStiffness
        {
            get { return defaultStiffness; }
            set
            {
                lock (syncRoot)
                {
                    defaultStiffness = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of times to run the algorithm
        /// </summary>
        public int MaxIterations
        {
            get { return maxIterations; }
            set { maxIterations = value; }
        }

        /// <summary>
        /// Positions the nodes
        /// </summary>
        /// <param name="isAnimated">Indicates whether or not the layout should be animated</param>
        /// <param name="graphComponents">The object containing the graph data</param>
        /// <param name="rootNode">Root node</param>
        public void ComputeLayout(bool isAnimated, GraphComponents graphComponents, INode rootNode)
        {
            nodeVMs = graphComponents.GetNodeViewModels().Where(node => !node.IsHidden).ToArray();
            positions = new Point[nodeVMs.Length];
            edgeCounts = new int[nodeVMs.Length];
            nodeIDToIndex = new Dictionary<string, int>();

            for (int i = 0; i < nodeVMs.Length; i++)
            {
                // Save this nodes position
                positions[i] = nodeVMs[i].Position;

                if (nodeVMs[i] is PartitionNode)
                    nodeIDToIndex[(nodeVMs[i] as PartitionNode).ID] = i;
                else
                    nodeIDToIndex[(nodeVMs[i] as NodeViewModelBase).ParentNode.ID] = i;

                // Get a count of all the edges for this node

                //testcount += GraphComponents.GetNumberOfEdges(nodeVMs[i].ParentNode);

                List<INodeShape> visitedNeighbors = new List<INodeShape>();
                Model.INode currentNode = null;

                if (nodeVMs[i] is PartitionNode)
                    currentNode = nodeVMs[i] as Model.INode;
                else
                    currentNode = (nodeVMs[i] as NodeViewModelBase).ParentNode;

                // Loop over all the edges for this node
                foreach (Model.IEdge edge in graphComponents.GetEdges(currentNode))
                {
                    // Get the node at the opposite end of this edge
                    INodeShape oppositeNode = graphComponents.GetOppositeNode(edge, currentNode);

                    // Ensure that this edge is not a similarity edge and that
                    // the opposite node has not been visited already
                    if (!(edge is Model.SimilarityDataEdge) && !visitedNeighbors.Contains(oppositeNode))
                    {
                        edgeCounts[i]++;
                        visitedNeighbors.Add(oppositeNode);
                    }
                }
            }

            // Old version is doing this next call asynchronously
            if (nodeVMs.Length > 1)
                CalculatePositions(graphComponents);

        }

        protected void CalculatePositions(GraphComponents graphComponents)
        {
            double deltaNorm;
            double otherNorm;
            bool converged = false;
            double T = K;
            double tolerance = 0.02;
            double coolingConstant = 0.99;
            Point tempDelta = new Point();
            Point displacement = new Point();
            TimeSpan globalForceTimespan = new TimeSpan();
            TimeSpan localForceTimespan = new TimeSpan();
            TimeSpan indexOfTime = new TimeSpan();
            DateTime start;
            Model.INode currentNode = null;

            List<int> listOfAllNodeIndices = new List<int>();

            // If we aren't partitioning into grids then use all the nodes every time
            if (!useGraphPartitioning)
            {
                for (int i = 0; i < nodeVMs.Length; i++)
                {
                    listOfAllNodeIndices.Add(i);
                }
            }

            for (int loop = 0; loop < maxIterations && !converged; loop++)
            {
                double max = 200;
                double min = 65;
                double R = max - ((loop / maxIterations) * (max - min) + min);

                if (useGraphPartitioning)
                {
                    // Put all vertices into appropraite cells
                    CalculateNodeCells(R);
                }

                converged = true;

                // Loop over nodes
                for (int currentNodeIndex = 0; currentNodeIndex < nodeVMs.Length; currentNodeIndex++)
                {
                    if (nodeVMs[currentNodeIndex] is PartitionNode)
                        currentNode = nodeVMs[currentNodeIndex] as Model.INode;
                    else
                        currentNode = (nodeVMs[currentNodeIndex] as NodeViewModelBase).ParentNode;

                    displacement = new Point(0, 0);

                    // global repulsive force (huh??)
                    start = DateTime.Now;

                    IList<int> repulsionNodes = null;
                    if (useGraphPartitioning)
                    {
                        // Find all nodes, within a certain distance, to perform repulstion on.
                        // Get nodes within maxDistance from this node.
                        double maxDistance = 50; //TODO:  MAKE THIS CONFIGURABLE
                        repulsionNodes = FindNodesForRepulsion(currentNodeIndex, R, maxDistance);
                    }
                    else
                    {
                        // Just repulse all nodes
                        repulsionNodes = listOfAllNodeIndices;
                    }

                    // Loop over all nodes in repulsion list
                    foreach (int i in repulsionNodes)
                    {
                        // We skip this calculation for the current node
                        if (i != currentNodeIndex)
                        {
                            tempDelta.X = positions[i].X - positions[currentNodeIndex].X;
                            tempDelta.Y = positions[i].Y - positions[currentNodeIndex].Y;

                            if (tempDelta.X == 0)
                                tempDelta.X = random.NextDouble() * .001 * K;
                            if (tempDelta.Y == 0)
                                tempDelta.Y = random.NextDouble() * .001 * K;

                            deltaNorm = Math.Max(1, Math.Abs(tempDelta.X) + Math.Abs(tempDelta.Y));
                            otherNorm = Math.Abs(positions[i].X + Math.Abs(positions[i].Y));

                            double globalForce = GlobalForce(deltaNorm, otherNorm);

                            displacement.X += (tempDelta.X / deltaNorm) * globalForce;
                            displacement.Y += (tempDelta.Y / deltaNorm) * globalForce;
                        }
                    }

                    globalForceTimespan += (DateTime.Now - start);

                    // Local forces
                    start = DateTime.Now;

                    // Loop over all the edges for this node
                    foreach (Model.IEdge edge in graphComponents.GetEdges(currentNode))
                    {
                        DateTime startIndex = DateTime.Now;
                        
                        int index = -1;
                        string nodeID = string.Empty;

                        INodeShape oppositeNode = graphComponents.GetOppositeNode(edge, currentNode);
                        //NodeViewModelBase oppositeNodeVM = GraphComponents.GetOppositeNode(edgeVM.ParentEdge, nodeVMs[currentNode].ParentNode);

                        // Get the ID for the opposite node.  How we do this depends
                        // on the type of node that we are dealing with.
                        if (oppositeNode is PartitionNode)
                            nodeID = (oppositeNode as PartitionNode).ID;
                        else
                            nodeID = (oppositeNode as NodeViewModelBase).ParentNode.ID;

                        if (!nodeIDToIndex.TryGetValue(nodeID, out index))
                        {
                            continue;
                        }
                        indexOfTime += DateTime.Now - startIndex;

                        if (index != -1)
                        {
                            tempDelta = new Point(positions[index].X - positions[currentNodeIndex].X, positions[index].Y - positions[currentNodeIndex].Y);

                            if (tempDelta.X == 0)
                                tempDelta.X = random.NextDouble() * .001 * K;
                            if (tempDelta.Y == 0)
                                tempDelta.Y = random.NextDouble() * .001 * K;

                            deltaNorm = Math.Max(Math.Sqrt((tempDelta.X * tempDelta.X) + (tempDelta.Y * tempDelta.Y)), 1);
                            otherNorm = Math.Max(Math.Sqrt((oppositeNode.Position.X * oppositeNode.Position.X) + (oppositeNode.Position.Y * oppositeNode.Position.Y)), 1);

                            double localForce = LocalForce(deltaNorm, edgeCounts[currentNodeIndex], edge, graphComponents);

                            displacement.X += (tempDelta.X / deltaNorm) * localForce;
                            displacement.Y += (tempDelta.Y / deltaNorm) * localForce;
                        }
                    }

                    localForceTimespan+=(DateTime.Now - start);

                    // Reposition node (huh??)
                    if (displacement.X == 0) displacement.X = 1;

                    double displacementNorm = Math.Sqrt((displacement.X * displacement.X) + (displacement.Y * displacement.Y));
                    double newX = positions[currentNodeIndex].X + (displacement.X / displacementNorm) * Math.Min(T, displacementNorm);
                    double newY = positions[currentNodeIndex].Y + (displacement.Y / displacementNorm) * Math.Min(T, displacementNorm);

                    tempDelta = new Point(newX - positions[currentNodeIndex].X, newY - positions[currentNodeIndex].Y);

                    positions[currentNodeIndex].X = newX;
                    positions[currentNodeIndex].Y = newY;

                    // no clue what this is doing
                    if (Math.Sqrt((tempDelta.X * tempDelta.X) + (tempDelta.Y * tempDelta.Y)) > K * tolerance)
                        converged=false;

                }

                // cool (huh??)
                T *= coolingConstant;
            }

            //System.Diagnostics.Debug.WriteLine(globalForceTimespan);
            //System.Diagnostics.Debug.WriteLine(localForceTimespan);
            //System.Diagnostics.Debug.WriteLine(indexOfTime);
        }

        /// <summary>
        /// Global repulsive force between all nodes
        /// </summary>
        /// <param name="deltanorm"></param>
        /// <param name="norm"></param>
        /// <returns></returns>
        protected double GlobalForce(double deltaNorm, double norm)
        {
            //double C = 0.02;
            double C = 0.5;

            if (deltaNorm < 1) deltaNorm = 1;

            return (-C * norm * K) / deltaNorm;
        }

        /// <summary>
        /// Local attractive force between 2 connected nodesnode
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="neighborCount"></param>
        /// <param name="edge"></param>
        /// <param name="graphComponents"></param>
        /// <returns></returns>
        protected double LocalForce(double distance, double neighborCount, Model.IEdge edge, GraphComponents graphComponents)
        {
            double bigDistanceApart = 60000;

            double length = K;
            double stiffness = defaultStiffness;

            if (neighborCount == 0) neighborCount = 1;

            // Check if the edge we are dealing with is a similarity edge
            if (edge is Model.SimilarityDataEdge)
            {
                Model.SimilarityDataEdge forceDirectedEdge = edge as Model.SimilarityDataEdge;
    
                length = forceDirectedEdge.SpringLength;
                stiffness = forceDirectedEdge.SpringStiffness;
            }

            double sourceNodeWidth = 0;
            double targetNodeWidth = 0;

            // Get the width for the source node.  How we get this depends on 
            // the type of node that we are dealing with.
            if (edge.Source is PartitionNode)
                sourceNodeWidth = (edge.Source as PartitionNode).Width;
            else
                DispatcherHelper.UIDispatcher.BeginInvoke(() => sourceNodeWidth = graphComponents.GetNodeViewModel(edge.Source).Width);

            // Get the width for the target node.  How we get this depends on 
            // the type of node that we are dealing with.
            if (edge.Target is PartitionNode)
                targetNodeWidth = (edge.Target as PartitionNode).Width;
            else
            {
                //targetNodeWidth = GraphComponents.GetNodeViewModel(edge.Target).Width;
                DispatcherHelper.UIDispatcher.BeginInvoke(() => targetNodeWidth = graphComponents.GetNodeViewModel(edge.Target).Width);
            }

            double edgeLength = length + sourceNodeWidth + targetNodeWidth;
            double distanceMultiplier = Math.Max(Math.Abs(distance - edgeLength) / bigDistanceApart, 1);

            return (stiffness * distanceMultiplier) * ((distance - edgeLength) / neighborCount);
        }

        protected void CalculateNodeCells(double cellRadius)
        {
            // find top left and bottom right points of the graph
            Point topLeft = new Point() { X = Double.MaxValue, Y = Double.MaxValue };
            Point bottomRight = new Point() { X = Double.MinValue, Y = Double.MinValue };
            foreach (Point p in positions)
            {
                if (p.X < topLeft.X)
                    topLeft.X = p.X;
                if (p.X > bottomRight.X)
                    bottomRight.X = p.X;
                if (p.Y < topLeft.Y)
                    topLeft.Y = p.Y;
                if (p.Y > bottomRight.Y)
                    bottomRight.Y = p.Y;
            }

            int numAcross = (int)Math.Floor((bottomRight.X - topLeft.X) / cellRadius) + 1;
            int numDown = (int)Math.Floor((bottomRight.Y - topLeft.Y) / cellRadius) + 1;

            cellArray = new List<int>[numAcross, numDown];

            for (int i = 0; i < positions.Length; i++)
            {
                int x = (int)Math.Floor((positions[i].X - topLeft.X) / cellRadius);
                int y = (int)Math.Floor((positions[i].Y - topLeft.Y) / cellRadius);
                nodeToCell[i] = new int[] { x, y };

                if (cellArray[x, y] == null)
                {
                    cellArray[x, y] = new List<int>();
                }

                cellArray[x, y].Add(i);
            }
        }

        protected IList<int> FindNodesForRepulsion(int currentNode, double radiusOfCells, double searchRadius)
        {
            int currXIndex = nodeToCell[currentNode][0];
            int currYIndex = nodeToCell[currentNode][1];
            int maxXIndex = cellArray.GetUpperBound(0);
            int maxYIndex = cellArray.GetUpperBound(1);

            int blockRadius = (int)Math.Ceiling(searchRadius / radiusOfCells);

            int minX = Math.Max(0, currXIndex - blockRadius);
            int minY = Math.Max(0, currYIndex - blockRadius);
            int maxX = Math.Min(currXIndex + blockRadius, maxXIndex);
            int maxY = Math.Min(currYIndex + blockRadius, maxYIndex);

            List<int> currList = new List<int>();
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {

                    if (cellArray[x, y] != null)
                    {
                        currList.AddRange(cellArray[x, y]);
                    }

                }
            }
            return currList;
        }
    

        /// <summary>
        /// Resonsible for actually positioning the nodes.  This will
        /// execute on the UI thread, once the background thread
        /// running the algorithm has completed.
        /// </summary>
        /// <param name="isAnimated">Indicates whether or not the layout should be animated</param>
        public void PositionNodes(bool isAnimated)
        {
            // Loop over all the collection of nodes.  We position
            // nodes here because it must occur on the UI thread.
            for (int i = 0; i < nodeVMs.Length; i++)
            {
                // Position or animated this node
                if (nodeVMs[i] is NodeViewModelBase && isAnimated)
                    (nodeVMs[i] as NodeViewModelBase).AnimateTo(positions[i]);
                else
                    nodeVMs[i].Position = positions[i];
            }
        }

    }
}