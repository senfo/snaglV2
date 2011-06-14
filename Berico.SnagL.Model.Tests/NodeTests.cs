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
using Berico.Common.Events;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Berico.SnagL.Model.Tests
{
    [TestClass]
    public class NodeTests : SilverlightTest
    {

        [TestMethod]
        [Tag("Node")]
        [Description("Test the succesfull creation of a Node")]
        public void TestCreateNode()
        {
            Node node = new Node("Test Node 1");
            Assert.IsInstanceOfType(node, typeof(Node));
        }

        [TestMethod]
        [Tag("Node")]
        [Description("Test the creation of a Node using invalid parameters")]
        public void TestCreateNodeWithNullParameters()
        {
            bool exceptionThrown;
            string errorMessage;

            try
            {
                Node node = new Node(null);

                exceptionThrown = false;
                errorMessage = string.Empty;
            }
            catch (ArgumentNullException ex)
            {
                exceptionThrown = true;
                errorMessage = ex.Message;
            }

            Assert.IsTrue(exceptionThrown);
            Assert.IsTrue(errorMessage.Contains("A display value must be provided for each node"));
        }

        [TestMethod]
        [Tag("Node")]
        [Asynchronous]
        [Description("Test changing a Node's properties and ensuring the PropertyChanged event is fired correctly")]
        public void TestChangingNodeProperties()
        {
            Node node = new Node("Test Node 1");
            string propertyChanged = string.Empty;

            node.PropertyChanged += (object sender, PropertyChangedEventArgs<string> e) =>
            {
                propertyChanged = e.PropertyName;
            };

            EnqueueCallback(() => node.DisplayValue = "NEW DISP VAL");
            EnqueueConditional(() => propertyChanged != string.Empty);
            EnqueueCallback(() => Assert.AreEqual<string>("NEW DISP VAL", node.DisplayValue));
            EnqueueCallback(() => propertyChanged = string.Empty);

            EnqueueCallback(() => node.Description = "NEW DESC");
            EnqueueConditional(() => propertyChanged != string.Empty);
            EnqueueCallback(() => Assert.AreEqual<string>("NEW DESC", node.Description));
            EnqueueCallback(() => propertyChanged = string.Empty);

            EnqueueTestComplete();
        }
    }
}