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
    public class SliderTests
    {

        [TestMethod]
        [Description("Create a new Slider instance through code")]
        public void InstantiateSliderCode()
        {
            Berico.Windows.Controls.Slider slider = new Berico.Windows.Controls.Slider();
            Assert.IsNotNull(slider, "Slider construction should succeed.");
        }

        [TestMethod]
        [Description("Ensure the default values are correct.")]
        public void VerifyDefaultValues()
        {
            Berico.Windows.Controls.Slider slider = new Berico.Windows.Controls.Slider();

            Assert.Equals(slider.Minimum, 0.0d);
            Assert.Equals(slider.Maximum, 1.0d);
            Assert.Equals(slider.LowerRangeValue, 0.2d);
            Assert.Equals(slider.UpperRangeValue, 0.8d);
            Assert.Equals(slider.SmallChange, 0.1d);
            Assert.Equals(slider.LargeChange, 1.0d);
            Assert.Equals(slider.Value, 0.0d);
        }

        [TestMethod]
        [Description("Coercion Test:  Maximum with an invalid value")]
        public void InvalidMaximum()
        {
            Berico.Windows.Controls.Slider slider = (Slider)XamlReader.Load(Resources.Slider_DefaultXaml);
            Assert.IsInstanceOfType(slider, typeof(Slider), "Loading the Xaml should have created a functional Slider.");

            // Test with appropriate values
            //slider.UpperRangeValue = 50;
            //slider.LowerRangeValue = 10;
            //Assert.IsTrue(slider.UpperRangeValue > slider.Minimum, "UpperRangeValue (50) must be greater than the Minimum (0)");

            // Test that coercion works even when the Minimum value is changed
            //slider.Minimum = 55;
            //Assert.IsTrue(slider.UpperRangeValue >= slider.Minimum, "UpperRangeValue (50) must be greater than, or equal to, the Minimum (55)");
            //slider.Maximum = 100;

            // Test with invalid values (forcing coercion)
            //slider.UpperRangeValue = -1;
            //Assert.IsTrue(slider.UpperRangeValue >= slider.Minimum, "UpperRangeValue (-1) must be greater than, or equal to, the Minimum (0).");
        }

        [TestMethod]
        [Description("Coercion Test:  UpperRangeValue must be greater than the Minimum value.")]
        public void UpperRangeGreaterThanMinimum()
        {
            Berico.Windows.Controls.Slider slider = new Berico.Windows.Controls.Slider();

            slider.Minimum = 0;
            slider.Maximum = 100;

            // Test with appropriate values
            slider.UpperRangeValue = 50;
            slider.LowerRangeValue = 10;
            Assert.IsTrue(slider.UpperRangeValue > slider.Minimum, "UpperRangeValue (50) must be greater than the Minimum (0)");

            // Test that coercion works even when the Minimum value is changed
            slider.Minimum = 55;
            Assert.IsTrue(slider.UpperRangeValue >= slider.Minimum, "UpperRangeValue (50) must be greater than, or equal to, the Minimum (55)");
            slider.Maximum = 100;

            // Test with invalid values (forcing coercion)
            slider.UpperRangeValue = -1;
            Assert.IsTrue(slider.UpperRangeValue >= slider.Minimum, "UpperRangeValue (-1) must be greater than, or equal to, the Minimum (0).");
        }

        [TestMethod]
        [Description("Coercion Test:  UpperRangeValue must be less than the Maximum value.")]
        public void UpperRangeLessThanMaximum()
        {
            Berico.Windows.Controls.Slider slider = new Berico.Windows.Controls.Slider();

            slider.Minimum = 0;
            slider.Maximum = 100;

            // Test with appropriate values
            slider.UpperRangeValue = 50;
            Assert.IsTrue(slider.UpperRangeValue < slider.Maximum, "UpperRangeValue (50) must be less than the Maximum (100)");

            // Test that coercion works even when the Maximum value is changed
            slider.Maximum = 45;
            Assert.IsTrue(slider.UpperRangeValue <= slider.Maximum, "UpperRangeValue (50) must be less than, or equal to, the Maximum (45)");
            slider.Maximum = 100;

            // Test with invalid values (forcing coercion)
            slider.UpperRangeValue = 110;
            Assert.IsTrue(slider.UpperRangeValue <= slider.Maximum, "UpperRangeValue (110) must be less than, or equal to, the Maximum (100).");
        }

        [TestMethod]
        [Description("Coercion Test:  UpperRangeValue must be greater than LowerRangeValue.")]
        public void UpperRangeGreaterThanLowerRange()
        {
            Berico.Windows.Controls.Slider slider = new Berico.Windows.Controls.Slider();

            slider.Minimum = 0;
            slider.Maximum = 100;
            slider.LowerRangeValue = 20;

            // Test with appropriate values
            slider.UpperRangeValue = 50;
            Assert.IsTrue(slider.UpperRangeValue > slider.LowerRangeValue, "UpperRangeValue (50) must be greater than LowerRangeValue");

            // Test with invalid values (forcing coercion)
            slider.UpperRangeValue = 10;
            Assert.IsTrue(slider.UpperRangeValue >= slider.LowerRangeValue, "UpperRangeValue (10) must be greater than, or equal to, LowerRangeValue.");
        }

        [TestMethod]
        [Description("Coercion Test:  LowerRangeValue must be greater than the Minimum value.")]
        public void LowerRangeGreaterThanMinimum()
        {
            Berico.Windows.Controls.Slider slider = new Berico.Windows.Controls.Slider();

            slider.Minimum = 0;
            slider.Maximum = 100;
            slider.UpperRangeValue = 80;

            // Test with appropriate values
            slider.LowerRangeValue = 50;
            Assert.IsTrue(slider.LowerRangeValue > slider.Minimum, "LowerRangeValue (50) must be greater than the Minimum (0)");

            // Test that coercion works even when the Minimum value is changed
            slider.Minimum = 55;
            Assert.IsTrue(slider.LowerRangeValue >= slider.Minimum, "LowerRangeValue (50) must be greater than, or equal to, the Minimum (55)");
            slider.Maximum = 100;

            // Test with invalid values (forcing coercion)
            slider.LowerRangeValue = -1;
            Assert.IsTrue(slider.LowerRangeValue >= slider.Minimum, "LowerRangeValue (-1) must be greater than, or equal to the Minimum (0).");
        }

        [TestMethod]
        [Description("Coercion Test:  LowerRangeValue must be less than the Maximum value.")]
        public void LowerRangeLessThanMaximum()
        {
            Berico.Windows.Controls.Slider slider = new Berico.Windows.Controls.Slider();

            slider.Minimum = 0;
            slider.Maximum = 100;

            // Test with appropriate values
            slider.LowerRangeValue = 50;
            Assert.IsTrue(slider.LowerRangeValue < slider.Maximum, "LowerRangeValue (50) must be less than the Maximum (100)");

            // Test that coercion works even when the Maximum value is changed
            slider.Maximum = 45;
            Assert.IsTrue(slider.LowerRangeValue <= slider.Maximum, "LowerRangeValue (50) must be less than, or equal to, the Maximum (45)");
            slider.Maximum = 100;

            // Test with invalid values (forcing coercion)
            slider.LowerRangeValue = 110;
            Assert.IsTrue(slider.LowerRangeValue <= slider.Maximum, "LowerRangeValue (110) must be less than, or equal to, the Maximum (100).");
        }

        [TestMethod]
        [Description("Coercion Test:  LowerRangeValue must be less than UpperRangeValue.")]
        public void LowerRangeLessThanUpperRange()
        {
            Berico.Windows.Controls.Slider slider = new Berico.Windows.Controls.Slider();

            slider.Minimum = 0;
            slider.Maximum = 100;
            slider.UpperRangeValue = 80;

            // Test with appropriate values
            slider.LowerRangeValue = 20;
            Assert.IsTrue(slider.LowerRangeValue < slider.UpperRangeValue, "LowerRangeValue (50) must be less than UpperRangeValue");

            // Test with invalid values (forcing coercion)
            slider.LowerRangeValue = 10;
            Assert.IsTrue(slider.LowerRangeValue <= slider.UpperRangeValue, "LowerRangeValue (10) must be less than, or equal to, UpperRangeValue.");
        }

    }
}