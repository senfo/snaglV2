//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using Berico.SnagL.Model;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Berico.SnagL.Model.Tests
{
    //TODO:  MORE EDGE UNIT TESTS

    [TestClass]
    public class EdgeTests : SilverlightTest
    {
        private Node testNode1 = new Node("Test Node 1");
        private Node testNode2 = new Node("Test Node 2");

        [TestMethod]
        [Tag("Edge")]
        [Description("Test the succesfull creation of an Edge")]
        public void TestCreateEdge()
        {
            Edge edge = new Edge(testNode1, testNode2);
            Assert.IsInstanceOfType(edge, typeof(Edge));
        }

        [TestMethod]
        [Tag("Edge")]
        [Description("Test the creation of an Edge using null parameters")]
        public void TestCreateEdgeWithNullParameters()
        {
            bool exceptionThrown;
            string errorMessage;

            try
            {
                Edge edge = new Edge(null, null);

                exceptionThrown = false;
                errorMessage = String.Empty;
            }
            catch (ArgumentNullException ex)
            {
                exceptionThrown = true;
                errorMessage = ex.Message;
            }

            Assert.IsTrue(exceptionThrown);
            Assert.IsTrue(errorMessage.Contains("The provided target edge was null"));
        }

        [TestMethod]
        [Tag("Edge")]
        [Description("Test the creation of an Edge using invalid parameters")]
        public void TestCreateSelfLoopEdge()
        {
            bool exceptionThrown;
            string errorMessage = string.Empty;

            try
            {
                Edge edge = new Edge(testNode1, testNode1);
                exceptionThrown = false;
            }
            catch (ArgumentException ex)
            {
                exceptionThrown = true;
                errorMessage = ex.Message;
            }

            Assert.IsTrue(exceptionThrown);
            Assert.IsTrue(errorMessage.Contains("Self-Loop edges (Source and Target nodes are the same) are not currently supported"));
        }

        [TestMethod]
        [Tag("Edge")]
        [Description("Test the ToString() method returns a string in the expected format")]
        public void TestToString()
        {
            Edge edge = new Edge(testNode1, testNode2);
            string expected = string.Format("[Source: {0}, Target: {1}]", this.testNode1, this.testNode2);
            string actual = edge.ToString();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [Tag("Edge")]
        [Description("Tests that to nodes that are not equal are actually not equal")]
        public void TestUnequalNodesAreNotEqual()
        {
            Edge edge0 = new Edge(testNode1, testNode2);
            Edge edge1 = new Edge(new Node("Sean Node - 1"), new Node("Sean Node - 2"));

            Assert.AreNotEqual<Edge>(edge0, edge1);
        }

        [TestMethod]
        [Tag("Edge")]
        [Description("Tests that to nodes that are not equal are actually not equal")]
        public void TestEqualNodesAreEqual()
        {
            Edge edge0 = new Edge(testNode1, testNode2);
            Edge edge1 = new Edge(new Node("Test Node 1"), new Node("Test Node 2"));

            Assert.AreEqual<Edge>(edge0, edge1);
        }

        //[TestMethod]
        //[Tag("Edge")]
        //[Asynchronous]
        //[Description("Test changing an Edge's properties and ensuring the PropertyChanged event is fired correctly")]
        //public void TestChangingEdgeProperties()
        //{
        //    Edge edge = new Edge(testNode1, testNode2);
        //    string propertyChanged = string.Empty;

        //    edge.PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) =>
        //    {
        //        propertyChanged = e.PropertyName;
        //    };

        //    EnqueueCallback(() => edge.ID = "NEW ID");
        //    EnqueueConditional(() => propertyChanged != string.Empty);
        //    EnqueueCallback(() => Assert.AreEqual<string>("NEW ID", edge.ID));
        //    EnqueueCallback(() => propertyChanged = string.Empty);

        //    EnqueueCallback(() => edge.DisplayValue = "NEW DISP VAL");
        //    EnqueueConditional(() => propertyChanged != string.Empty);
        //    EnqueueCallback(() => Assert.AreEqual<string>("NEW DISP VAL", edge.DisplayValue));
        //    EnqueueCallback(() => propertyChanged = string.Empty);

        //    EnqueueCallback(() => edge.Description = "NEW DESC");
        //    EnqueueConditional(() => propertyChanged != string.Empty);
        //    EnqueueCallback(() => Assert.AreEqual<string>("NEW DESC", edge.Description));
        //    EnqueueCallback(() => propertyChanged = string.Empty);

        //    EnqueueTestComplete();
        //}
    }
}