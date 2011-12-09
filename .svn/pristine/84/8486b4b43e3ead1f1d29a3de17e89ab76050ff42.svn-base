//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Berico.SnagL.Infrastructure.Data;
using Berico.SnagL.Model;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Berico.SnagL.Infrastructure.Tests
{
    /// <summary>
    /// Contains test methods that verify Live functionality
    /// </summary>
    [TestClass]
    public class LiveTests : SilverlightTest
    {
        #region GraphData Tests

        [TestMethod]
        [Tag("Live")]
        [Description("Test that an edge with source GhostNode isn't added to physical graph")]
        public void Verify_Edge_With_Source_Ghost_Node_Not_Added_To_Physical_Graph()
        {
            IEnumerable<IEdge> actualSourceEdges;
            IEnumerable<IEdge> actualTargetEdges;
            IEnumerable<IEdge> expectedSourceEdges = null;
            IEnumerable<IEdge> expectedTargetEdges = null;
            INode temp1 = new Node("temp1"); // Hack because an exception is thrown if you attempt to add an edge to a graph if the graph doesn't have at least one node
            INode source = PrepareNode("Node1", ghostNode: true);
            INode target = PrepareNode("Node2", ghostNode: false);
            IEdge orphan = new Edge(source, target);
            GraphData graphData = new GraphData("scope");

            graphData.AddNode(temp1); // Hack (see temp1 comment above)

            // Add an edge with ghost nodes to the graph
            graphData.AddEdge(orphan);

            // Get the actual count for nodes
            actualSourceEdges = graphData.Edges(source);
            actualTargetEdges = graphData.Edges(target);

            Assert.AreEqual(expectedSourceEdges, actualSourceEdges);
            Assert.AreEqual(expectedTargetEdges, actualTargetEdges);
        }

        [TestMethod]
        [Tag("Live")]
        [Description("Test that an edge with with source GhostNode isn't added to physical graph")]
        public void Verify_Edge_With_Target_Ghost_Node_Not_Added_To_Physical_Graph()
        {
            IEnumerable<IEdge> actualSourceEdges;
            IEnumerable<IEdge> actualTargetEdges;
            IEnumerable<IEdge> expectedSourceEdges = null;
            IEnumerable<IEdge> expectedTargetEdges = null;
            INode temp1 = new Node("temp1"); // Hack because an exception is thrown if you attempt to add an edge to a graph if the graph doesn't have at least one node
            INode source = PrepareNode("Node1", ghostNode: false);
            INode target = PrepareNode("Node2", ghostNode: true);
            IEdge orphan = new Edge(source, target);
            GraphData graphData = new GraphData("scope");

            graphData.AddNode(temp1); // Hack (see temp1 comment above)

            // Add an edge with ghost nodes to the graph
            graphData.AddEdge(orphan);

            // Get the actual count for nodes
            actualSourceEdges = graphData.Edges(source);
            actualTargetEdges = graphData.Edges(target);

            Assert.AreEqual(expectedSourceEdges, actualSourceEdges);
            Assert.AreEqual(expectedTargetEdges, actualTargetEdges);
        }

        [TestMethod]
        [Tag("Live")]
        [Description("Test that an edge with with GhostNodes isn't added to physical graph")]
        public void Verify_Edge_With_Two_Ghost_Nodes_Not_Added_To_Physical_Graph()
        {
            IEnumerable<IEdge> actualSourceEdges;
            IEnumerable<IEdge> actualTargetEdges;
            IEnumerable<IEdge> expectedSourceEdges = null;
            IEnumerable<IEdge> expectedTargetEdges = null;
            INode temp1 = new Node("temp1"); // Hack because an exception is thrown if you attempt to add an edge to a graph if the graph doesn't have at least one node
            INode source = PrepareNode("Node1", ghostNode: true);
            INode target = PrepareNode("Node2", ghostNode: true);
            IEdge orphan = new Edge(source, target);
            GraphData graphData = new GraphData("scope");

            graphData.AddNode(temp1); // Hack (see temp1 comment above)

            // Add an edge with ghost nodes to the graph
            graphData.AddEdge(orphan);

            // Get the actual count for nodes
            actualSourceEdges = graphData.Edges(source);
            actualTargetEdges = graphData.Edges(target);

            Assert.AreEqual(expectedSourceEdges, actualSourceEdges);
            Assert.AreEqual(expectedTargetEdges, actualTargetEdges);
        }

        [TestMethod]
        [Tag("Live")]
        [Description("Test that an edge with without GhostNodes is added to physical graph")]
        public void Verify_Edge_With_No_Ghost_Nodes_Is_Added_To_Physical_Graph()
        {
            int actualSourceEdges;
            int actualTargetEdges;
            int expectedSourceEdges = 1;
            int expectedTargetEdges = 1;
            INode source = PrepareNode("Node1", ghostNode: false);
            INode target = PrepareNode("Node2", ghostNode: false);
            IEdge edge = new Edge(source, target);
            GraphData graphData = new GraphData("scope");

            // Add an edge without ghost nodes to the graph
            graphData.AddNode(source);
            graphData.AddNode(target);
            graphData.AddEdge(edge);

            // Get the actual count for nodes
            actualSourceEdges = graphData.Edges(source).Count();
            actualTargetEdges = graphData.Edges(target).Count();

            Assert.AreEqual(expectedSourceEdges, actualSourceEdges);
            Assert.AreEqual(expectedTargetEdges, actualTargetEdges);
        }

        [TestMethod]
        [Tag("Live")]
        [Description("Test that an edge is added to the graph once its corresponding, real source node is added")]
        public void Verify_Edge_Added_To_Physical_Graph_Once_Corresponding_Source_Node_Added()
        {
            int actualSourceEdges;
            int actualTargetEdges;
            int expectedSourceEdges = 1;
            int expectedTargetEdges = 1;
            INode source = PrepareNode("Node1", ghostNode: true);
            INode target = PrepareNode("Node2", ghostNode: false);
            IEdge orphan = new Edge(source, target);
            GraphData graphData = new GraphData("scope");

            // Add an edge with ghost nodes to the graph
            graphData.AddNode(target);
            graphData.AddEdge(orphan);

            // Add the missing node
            source = PrepareNode("Node1", ghostNode: false);
            graphData.AddNode(source);

            // Get the actual count for nodes
            actualSourceEdges = graphData.Edges(source).Count();
            actualTargetEdges = graphData.Edges(target).Count();

            Assert.AreEqual(expectedSourceEdges, actualSourceEdges);
            Assert.AreEqual(expectedTargetEdges, actualTargetEdges);
        }

        [TestMethod]
        [Tag("Live")]
        [Description("Test that an edge is added to the graph once its corresponding, real target node is added")]
        public void Verify_Edge_Added_To_Physical_Graph_Once_Corresponding_Target_Node_Added()
        {
            int actualSourceEdges;
            int actualTargetEdges;
            int expectedSourceEdges = 1;
            int expectedTargetEdges = 1;
            INode source = PrepareNode("Node1", ghostNode: false);
            INode target = PrepareNode("Node2", ghostNode: true);
            IEdge orphan = new Edge(source, target);
            GraphData graphData = new GraphData("scope");

            // Add an edge with ghost nodes to the graph
            graphData.AddNode(source);
            graphData.AddEdge(orphan);

            // Add the missing node
            target = PrepareNode("Node2", ghostNode: false);
            graphData.AddNode(target);

            // Get the actual count for nodes
            actualSourceEdges = graphData.Edges(source).Count();
            actualTargetEdges = graphData.Edges(target).Count();

            Assert.AreEqual(expectedSourceEdges, actualSourceEdges);
            Assert.AreEqual(expectedTargetEdges, actualTargetEdges);
        }

        [TestMethod]
        [Tag("Live")]
        [Description("Test that an edge is added to the graph once all of its corresponding, real nodes are added")]
        public void Verify_Edge_Added_To_Physical_Graph_Once_All_Corresponding_Target_Nodes_Added()
        {
            int actualSourceEdges;
            int actualTargetEdges;
            int expectedSourceEdges = 1;
            int expectedTargetEdges = 1;
            INode temp1 = new Node("temp1"); // Hack because an exception is thrown if you attempt to add an edge to a graph if the graph doesn't have at least one node
            INode source = PrepareNode("Node1", ghostNode: true);
            INode target = PrepareNode("Node2", ghostNode: true);
            IEdge orphan = new Edge(source, target);
            IEdge actualSource;
            IEdge actualTarget;
            GraphData graphData = new GraphData("scope");

            // Hack (see temp1 comment above)
            graphData.AddNode(temp1);

            // Add an edge with ghost nodes to the graph
            graphData.AddEdge(orphan);

            // Add the missing node
            source = PrepareNode("Node1", ghostNode: false);
            target = PrepareNode("Node2", ghostNode: false);

            graphData.AddNode(source);
            graphData.AddNode(target);

            // Get the actual count for nodes
            actualSourceEdges = graphData.Edges(source).Count();
            actualTargetEdges = graphData.Edges(target).Count();

            actualSource = graphData.Edges(source).FirstOrDefault();
            actualTarget = graphData.Edges(target).FirstOrDefault();

            Assert.AreEqual(expectedSourceEdges, actualSourceEdges);
            Assert.AreEqual(expectedTargetEdges, actualTargetEdges);
            Assert.AreEqual(orphan, actualSource);
            Assert.AreEqual(orphan, actualTarget);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Prepares a node
        /// </summary>
        /// <param name="nodeId">Unique ID of the node for which to create</param>
        /// <param name="ghostNode">Indicates whether or not a GhostNode should be created</param>
        /// <returns>An INode object with the given specifications</returns>
        private static INode PrepareNode(string nodeId, bool ghostNode)
        {
            INode node;

            if (ghostNode)
            {
                node = new GhostNode(nodeId);
            }
            else
            {
                node = new Node(nodeId);
            }

            node.SourceMechanism = CreationType.Live;

            return node;
        }

        #endregion
    }
}