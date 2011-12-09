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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Berico.Windows.Controls.Test
{

    [TestClass]
    public class ItemsControlTest   
    {

        [TestMethod]
        [Description("This test checks the classes default values.")]
        public void CheckDefaultsTest()
        {
            ItemsControl itemsControl = new ItemsControl();

            Assert.IsFalse(itemsControl.HasItems, "By default, HasItems should return False.");

        }

        [TestMethod]
        [Description("This test ensures that the HasItems property returns the appropriate value.")]
        public void HasItemsTests()
        {

            ItemsControl itemsControl = new ItemsControl();

            // Test HasItems before adding items to the control.
            Assert.IsFalse(itemsControl.HasItems, "The HasItems property should be False.");

            // Add a few items to the Items collection.
            itemsControl.Items.Add("Test 1");
            itemsControl.Items.Add("Test 2");
            itemsControl.Items.Add("Test 3");

            // Test HasItems after adding items to the control.
            Assert.IsTrue(itemsControl.HasItems, "The HasItems property should be True.");

        }

    }
}