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
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Markup;
using Berico.Windows.Controls;

namespace Berico.Windows.Controls.Test
{
    [TestClass]
    public class GridSplitterTests
    {
        [TestMethod]
        [Description("Create a new GridSplitter instance through code")]
        public void InstantiateGridSplitterCode()
        {
            Berico.Windows.Controls.GridSplitter splitter = new Berico.Windows.Controls.GridSplitter();
            Assert.IsNotNull(splitter, "GridSplitter construction should succeed.");
        }

        [TestMethod]
        [Description("Create a new GridSplitter instance through xaml")]
        public void InstantiateGridSplitterXaml()
        {
            object splitter = XamlReader.Load(Resources.GridSplitter_DefaultXaml);
            Assert.IsInstanceOfType(splitter, typeof(GridSplitter), "Loading the Xaml should have created a GridSplitter.");
        }

        [TestMethod]
        [Description("Checks the accuracy of the expected default values")]
        public void CheckDefaults()
        {
            Berico.Windows.Controls.GridSplitter splitter = new Berico.Windows.Controls.GridSplitter();

            Assert.AreEqual(splitter.CollapseMode, GridSplitterCollapseMode.None, "CollapseMode should be None, by default.");
            Assert.IsFalse(splitter.IsCollapsed, "IsCollapsed should be false, by default.");
            Assert.IsTrue(splitter.IsAnimated, "IsAnimated should be true, by default.");

        }

        [TestMethod]
        [Description("Set the HorizontalHandleStyle property through code")]
        public void HorizontalHandleStyleSetViaCode()
        {
            GridSplitter splitter = new GridSplitter();
            Style s = new Style(typeof(System.Windows.Controls.Primitives.ToggleButton));
            splitter.HorizontalHandleStyle = s;
            Assert.AreEqual(s, splitter.GetValue(GridSplitter.HorizontalHandleStyleProperty), "Assigning a new style to the HandleStyleProperty should set the style.");
        }
    }
}