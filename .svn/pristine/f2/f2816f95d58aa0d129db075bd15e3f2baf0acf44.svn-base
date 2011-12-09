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
using System.Collections.Specialized;
using System.Linq;
using Berico.SnagL.Infrastructure.Data;
using Berico.SnagL.Infrastructure.Events;
using Berico.SnagL.Model;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Berico.SnagL.Infrastructure.Tests
{
    [TestClass]
    public class GraphDataTests : SilverlightTest
    {
        private bool _eventFired = false;
        private IEdge _affectedEdge = null;

        [TestMethod]
        [Tag("GraphData")]
        [Description("Test the creation of a GraphData with with a scope")]
        public void TestCreateGraphData()
        {
            string expected = "scope";
            GraphData g = new GraphData(expected);
            string actual = g.Scope;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [Tag("GraphData")]
        [Description("Verifies the GraphData.Size returns zero when GraphData is first initialized")]
        public void TestGraphDataSizeShouldBeZeroWhenFirstInitialized()
        {
            GraphData g = new GraphData("scope");

            Assert.AreEqual(0, g.Size);
        }

        [TestMethod]
        [Tag("GraphData")]
        [Description("Verifies the default graph type is directed")]
        public void TestDefaultGraphType()
        {
            GraphData g = new GraphData("scope");

            Assert.AreEqual<GraphType>(g.Type, GraphType.Directed);
        }

        [TestMethod]
        [Tag("GraphData")]
        [Description("Verifies the AddNode method works")]
        public void TestAddingNode()
        {
            GraphData g = new GraphData("scope");
            Node node = new Node("Node1");

            g.AddNode(node);

            Assert.IsTrue(g.ContainsNode(node));
        }

        [TestMethod]
        [Asynchronous]
        [Tag("GraphData")]
        [Description("Verifies the CollectionChanged event is fired when a new Node is added")]
        public void TestCollectionChangedEventFiredWhenNodeAdded()
        {
            bool eventFired = false;
            NotifyCollectionChangedAction action = NotifyCollectionChangedAction.Reset;
            GraphData g = new GraphData("scope");
            Node node = new Node("Node1");

            g.CollectionChanged += (sender, e) =>
            {
                action = e.Action;
                eventFired = true;
            };

            EnqueueCallback(() => g.AddNode(node));
            EnqueueConditional(() => eventFired);

            EnqueueCallback(() => Assert.IsTrue(eventFired));
            EnqueueCallback(() => Assert.AreEqual<NotifyCollectionChangedAction>(NotifyCollectionChangedAction.Add, action));
            EnqueueTestComplete();
        }

        [TestMethod]
        [Tag("GraphData")]
        [Description("Verifies the Contains method returns true when a Node exists in the collection")]
        public void TestContainsNode()
        {
            GraphData g = new GraphData("scope");
            Node node = new Node("Node1");

            g.AddNode(node);

            Assert.IsTrue(g.ContainsNode(node));
        }

        [TestMethod]
        [Tag("GraphData")]
        [Description("Verifies the ability to add multiple Node objects to the collection")]
        public void TestAddingMultipleNodes()
        {
            GraphData g = new GraphData("scope");
            Node node1 = new Node("Node1");
            Node node2 = new Node("Node2");
            Node node3 = new Node("Node3");
            List<Node> nodes = new List<Node>
            {
                node1,
                node2,
                node3
            };

            g.AddNodes(nodes);

            Assert.AreEqual(nodes.Count, g.Count);
            Assert.IsTrue(g.ContainsNode(node1));
            Assert.IsTrue(g.ContainsNode(node2));
            Assert.IsTrue(g.ContainsNode(node3));
        }

        [TestMethod]
        [Asynchronous]
        [Tag("GraphData")]
        [Description("Verifies adding multiple Node objects to the collection fires the CollectionChanged event")]
        public void TestAddingMultipleNodesRaisesCollectionChangedEvent()
        {
            bool eventFired = false;
            NotifyCollectionChangedAction action = NotifyCollectionChangedAction.Reset;
            GraphData g = new GraphData("scope");
            Node node1 = new Node("Node1");
            Node node2 = new Node("Node2");
            Node node3 = new Node("Node3");
            List<Node> nodes = new List<Node>
            {
                node1,
                node2,
                node3
            };

            g.CollectionChanged += (sender, e) =>
            {
                action = e.Action;
                eventFired = true;
            };

            EnqueueCallback(() => g.AddNodes(nodes));
            EnqueueConditional(() => eventFired);

            EnqueueCallback(() => Assert.AreEqual(nodes.Count, g.Count));
            EnqueueCallback(() => Assert.IsTrue(g.ContainsNode(node1)));
            EnqueueCallback(() => Assert.IsTrue(g.ContainsNode(node2)));
            EnqueueCallback(() => Assert.IsTrue(g.ContainsNode(node3)));
            EnqueueCallback(() => Assert.AreEqual<NotifyCollectionChangedAction>(NotifyCollectionChangedAction.Add, action));
            EnqueueTestComplete();
        }

        [TestMethod]
        [Tag("GraphData")]
        [Description("Verifies an exception is thrown when a null value is passed to the RemoveNode method")]
        public void TestRemoveNodeWithNullParameters()
        {
            bool exptionThrown = false;
            GraphData g = new GraphData("scope");

            try
            {
                g.RemoveNode(null);
            }
            catch (ArgumentNullException)
            {
                exptionThrown = true;
            }

            Assert.IsTrue(exptionThrown);
        }

        [TestMethod]
        [Tag("GraphData")]
        [Description("Verifies the ability to remove a Node from the collection")]
        public void TestRemoveNode()
        {
            GraphData g = new GraphData("scope");
            Node node = new Node("Node");

            g.AddNode(node);

            if (!g.ContainsNode(node))
            {
                Assert.Inconclusive("Node wasn't found in collection after adding it. Verify Add() functionality.");
            }

            // Remove the Node
            g.RemoveNode(node);

            // Make sure the Node has been removed
            Assert.IsFalse(g.ContainsNode(node));
        }

        [TestMethod]
        [Tag("GraphData")]
        [Description("Verifies the ability to remove a Node from the collection")]
        public void TestRemoveMultipleNodes()
        {
            GraphData g = new GraphData("scope");
            Node node1 = new Node("Node1");
            Node node2 = new Node("Node2");
            Node node3 = new Node("Node3");
            Node node4 = new Node("Node4");
            List<Node> nodes = new List<Node>
            {
                node1,
                node2,
                node3
            };

            g.AddNodes(nodes);
            g.AddNode(node4);
            g.RemoveNodes(nodes);

            Assert.AreEqual(1, g.Count);
            Assert.IsTrue(g.ContainsNode(node4));
        }

        [TestMethod]
        [Tag("GraphData")]
        [Description("Verifies the ability to add an Edge to the collection")]
        public void TestAddEdge()
        {
            GraphData g = new GraphData("scope");
            Node source = new Node("Source");
            Node target = new Node("Target");
            Edge edge = new Edge(source, target);

            g.AddNode(source);
            g.AddNode(target);
            g.AddEdge(edge);

            Assert.IsTrue(g.ContainsEdge(edge));
        }

        //[TestMethod]
        //[Tag("GraphData")]
        //[Description("Verifies the ability to add an Edge with no matching Node objects")]
        //public void TestAddEdgeWithNoNodes()
        //{
        //    GraphData g = new GraphData("scope");
        //    Edge edge = new Edge();

        //    g.AddEdge(edge);

        //    Assert.IsTrue(g.ContainsEdge(edge));
        //}

        [TestMethod]
        [Tag("GraphData")]
        [Description("Verifies the ability to remove an Edge from the collection")]
        public void TestRemoveEdge()
        {
            GraphData g = new GraphData("scope");
            Node source = new Node("Source");
            Node target = new Node("Target");
            Edge edge = new Edge(source, target);

            g.AddNode(source);
            g.AddNode(target);
            g.AddEdge(edge);

            if (!g.ContainsEdge(edge))
            {
                Assert.Inconclusive("Edge not found in the collection after adding it.");
            }

            g.RemoveEdge(edge);

            Assert.IsFalse(g.ContainsEdge(edge));
        }

        [TestMethod]
        [Tag("GraphData")]
        [Description("Verifies the ability to remove an Edge from the collection")]
        public void TestRemoveEdgeDoesntRemoveNodes()
        {
            GraphData g = new GraphData("scope");
            Node source = new Node("Source");
            Node target = new Node("Target");
            Edge edge = new Edge(source, target);

            g.AddNode(source);
            g.AddNode(target);
            g.AddEdge(edge);

            if (!g.ContainsEdge(edge))
            {
                Assert.Inconclusive("Edge not found in the collection after adding it.");
            }

            g.RemoveEdge(edge);

            if (g.ContainsEdge(edge))
            {
                Assert.Inconclusive("Edge still found in the collection after removing it.");
            }

            Assert.IsTrue(g.ContainsNode(source));
            Assert.IsTrue(g.ContainsNode(target));
        }

        [TestMethod]
        [Asynchronous]
        [Tag("GraphData")]
        [Description("Verifies the CollectionChanged event is fired when an Edge is added to the collection")]
        public void TestAddingEdgeFiresCollectionChangedEvent()
        {
            bool eventFired = false;
            NotifyCollectionChangedAction action = NotifyCollectionChangedAction.Reset;
            GraphData g = new GraphData("scope");
            Node source = new Node("Source");
            Node target = new Node("Target");
            Edge edge = new Edge(source, target);

            // TODO: This test is invalid because added Node objects to the collection will fire the same event
            g.CollectionChanged += (sender, e) =>
            {
                action = e.Action;
                eventFired = true;
            };

            EnqueueCallback(() => g.AddNode(source));
            EnqueueCallback(() => g.AddNode(target));
            EnqueueCallback(() => g.AddEdge(edge));
            EnqueueConditional(() => eventFired);

            EnqueueCallback(() => Assert.IsTrue(eventFired));
            EnqueueCallback(() => Assert.AreEqual<NotifyCollectionChangedAction>(NotifyCollectionChangedAction.Add, action));
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Tag("GraphData")]
        [Description("Verifies the PropertyChanged event is fired when an Edge is added to the collection")]
        public void TestRemovingEdgeFiresCollectionChangedEvent()
        {
            SnaglEventAggregator eventAggregator = SnaglEventAggregator.DefaultInstance;
            GraphData g = new GraphData("scope");
            Node source = new Node("Source");
            Node target = new Node("Target");
            Edge edge = new Edge(source, target);

            g.AddNode(source);
            g.AddNode(target);
            g.AddEdge(edge);

            // Don't replace this with a lambda
            eventAggregator.GetEvent<EdgeRemovedEvent>().Subscribe(OnEdgeRemoved);

            EnqueueCallback(() => g.RemoveEdge(edge));
            EnqueueConditional(() => _eventFired);

            EnqueueCallback(() => Assert.IsTrue(_eventFired));
            EnqueueCallback(() => Assert.AreEqual<IEdge>(edge, _affectedEdge));
            EnqueueTestComplete();
        }

        [TestMethod]
        [Tag("GraphData")]
        [Description("Verifies the proper predecessor for a Node is returned")]
        public void TestOnePredecessor()
        {
            GraphData g = new GraphData("scope");
            INode source = new Node("Source");
            INode target = new Node("Target");
            Edge edge = new Edge(source, target);

            g.AddNode(source);
            g.AddNode(target);
            g.AddEdge(edge);

            INode actual = g.Predecessors(target).First();

            Assert.AreEqual<INode>(source, actual);
        }

        [TestMethod]
        [Tag("GraphData")]
        [Description("Verifies the proper predecessors for a Node is returned")]
        public void TestMultiplePredecessors()
        {
            GraphData g = new GraphData("scope");
            INode source = new Node("Source");
            INode target = new Node("Target");
            INode superTarget = new Node("SuperTarget");
            Edge edge1 = new Edge(source, target);
            Edge edge2 = new Edge(target, superTarget);
            List<INode> predecessors;

            g.AddNode(source);
            g.AddNode(target);
            g.AddNode(superTarget);
            g.AddEdge(edge1);
            g.AddEdge(edge2);

            predecessors = new List<INode>(g.Predecessors(superTarget));

            Assert.IsTrue(predecessors.Contains(target));
        }

        [TestMethod]
        [Tag("GraphData")]
        [Description("Verifies the proper successor for a Node is returned")]
        public void TestOneSuccessor()
        {
            GraphData g = new GraphData("scope");
            INode source = new Node("Source");
            INode target = new Node("Target");
            Edge edge = new Edge(source, target);

            g.AddNode(source);
            g.AddNode(target);
            g.AddEdge(edge);

            INode actual = g.Successors(source).First();

            Assert.AreEqual<INode>(target, actual);
        }

        [TestMethod]
        [Tag("GraphData")]
        [Description("Verifies the proper successors for a Node is returned")]
        public void TestMultipleSuccessors()
        {
            GraphData g = new GraphData("scope");
            Node source = new Node("Source");
            Node target = new Node("Target");
            Node superTarget = new Node("SuperTarget");
            Edge edge1 = new Edge(source, target);
            Edge edge2 = new Edge(target, superTarget);
            List<INode> predecessors;

            g.AddNode(source);
            g.AddNode(target);
            g.AddNode(superTarget);
            g.AddEdge(edge1);
            g.AddEdge(edge2);

            predecessors = new List<INode>(g.Successors(source));

            Assert.IsTrue(predecessors.Contains(target));
        }

        /// <summary>
        /// The following method is required to be public by the TestRemovingEdgeFiresCollectionChangedEvent() method.
        /// </summary>
        /// <param name="e">Arguments passed when the edge is removed</param>
        public void OnEdgeRemoved(EdgeEventArgs e)
        {
            _affectedEdge = e.AffectedEdge;
            _eventFired = true;
        }
    }
}