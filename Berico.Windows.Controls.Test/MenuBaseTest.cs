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
    public class MenuBaseTest
    {

        [TestMethod]
        [Description("This test checks the classes default values.")]
        public void CheckDefaultsTest()
        {
            MenuBase menuBase = new MenuBase();

            Assert.IsFalse(menuBase.OpenOnClick, "By default, OpenOnClick should return false.");
            Assert.AreEqual<double>(menuBase.HideDelay.TimeSpan.TotalSeconds, 1, "By default, HideDelay should be 1 second.");
            Assert.AreEqual<double>(menuBase.ShowDelay.TimeSpan.TotalSeconds, 1, "By default, ShowDelay should be 1 second.");

        }

    }

}