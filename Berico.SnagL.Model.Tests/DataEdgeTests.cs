//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Berico.SnagL.Model.Tests
{
    /// <summary>
    /// Contains unit tests for the DataEdge class
    /// </summary>
    [TestClass]
    public class DataEdgeTests
    {
        private Node testNode1 = new Node("Test Node 1");
        private Node testNode2 = new Node("Test Node 2");

        [TestMethod]
        [Tag("DataEdge")]
        [Description("Test the succesfull creation of a DataEdge")]
        public void TestCreateDataEdge()
        {
            DataEdge edge = new DataEdge(testNode1, testNode2);
            Assert.IsInstanceOfType(edge, typeof(DataEdge));
        }

        [TestMethod]
        [Tag("DataEdge")]
        [Description("Test the ToString() method returns an ID with the value NA if ID is null or empty")]
        public void ToStringShouldReturnIdNAIfIdNullOrEmpty()
        {
            string expected = "ID: N/A";
            DataEdge edge = new DataEdge(testNode1, testNode2)
            {
                ID = null
            };
            string actual = edge.ToString().Split(',')[0].Replace("[", string.Empty);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [Tag("DataEdge")]
        [Description("Test the ToString() method returns an ID with the value expected value if ID is not null or empty")]
        public void ToStringShouldReturnIdIfIdNotNullOrEmpty()
        {
            string expected = "ID: SnagL Test ID";
            DataEdge edge = new DataEdge(testNode1, testNode2)
            {
                ID = "SnagL Test ID"
            };
            string actual = edge.ToString().Split(',')[0].Replace("[", string.Empty);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [Tag("DataEdge")]
        [Description("Test the ToString() method returns N/A for the description if it's not null or empty")]
        public void ToStringShouldReturnNAIfDescriptionNotNullOrEmpty()
        {
            string expected = "Description: N/A";
            DataEdge edge = new DataEdge(testNode1, testNode2);
            string actual = edge.ToString().Split(',')[1].TrimStart(' ');

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [Tag("DataEdge")]
        [Description("Test the ToString() method returns the expected description if it's not null or empty")]
        public void ToStringShouldReturnDescriptionIfDescriptionNotNullOrEmpty()
        {
            string expected = "Description: SnagL Test Description";
            DataEdge edge = new DataEdge(testNode1, testNode2)
            {
                Description = "SnagL Test Description"
            };
            string actual = edge.ToString().Split(',')[1].TrimStart(' ');

            Assert.AreEqual(expected, actual);
        }
    }
}