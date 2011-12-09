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
using Berico.SnagL.Model.Attributes;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Berico.SnagL.Model.Tests
{
    [TestClass]
    public class AttributeValueTests : SilverlightTest
    {
        [TestMethod]
        [Tag("AttributeValue")]
        [Description("Test the creation of an AttributeValue with the Value 'Value'")]
        public void TestCreateAttributeValueWithProvidedValue()
        {
            AttributeValue attributeValue = new AttributeValue("Value");

            Assert.AreEqual<string>("Value", attributeValue.Value);
            Assert.AreEqual<string>("Value", attributeValue.DisplayValue);
        }

        [TestMethod]
        [Tag("AttributeValue")]
        [Description("Test the creation of an AttributeValue with the Value 'Value' and DisplayValue 'Display Value'")]
        public void TestCreateAttributeValueWithProvidedValueDisplayValue()
        {
            AttributeValue attributeValue = new AttributeValue("Value", "Display Value");

            Assert.AreEqual<string>("Value", attributeValue.Value);
            Assert.AreEqual<string>("Display Value", attributeValue.DisplayValue);
        }

        [TestMethod]
        [Tag("AttributeValue")]
        [Description("Test the creation of an AttributeValue with the Value 'Value' and a null DisplayValue")]
        public void TestCreateAttributeValueWithNullDisplayValue()
        {
            AttributeValue attributeValue = new AttributeValue("Value", null);

            Assert.AreEqual<string>("Value", attributeValue.Value);
            Assert.AreEqual<string>("Value", attributeValue.DisplayValue);
        }

        [TestMethod]
        [Tag("AttributeValue")]
        [Description("Test the attempted creation of an AttributeValue with a null value")]
        public void TestCreateAttributeValueWithNullValue()
        {
            bool exceptionThrown;
            AttributeValue attribute;

            try
            {
                attribute = new AttributeValue(null);
                exceptionThrown = false;
            }
            catch (ArgumentNullException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
        }

        [TestMethod]
        [Tag("AttributeValue")]
        [Asynchronous]
        [Description("Test changing an AttributeValue's properties and ensuring the PropertyChanged event is fired correctly")]
        public void TestChangingAttributeValueProperties()
        {
            AttributeValue attribute = new AttributeValue("Value");
            object oldValue = null;
            object newValue = null;
            string propertyChanged = string.Empty;

            attribute.PropertyChanged += (object sender, PropertyChangedEventArgs<object> e) =>
            {
                propertyChanged = e.PropertyName;
                oldValue = e.OldValue;
                newValue = e.NewValue;
            };

            // Change Value
            EnqueueCallback(() => attribute.Value = "New Value");
            EnqueueConditional(() => propertyChanged != string.Empty);
            EnqueueCallback(() => Assert.AreEqual<string>("Value", propertyChanged));
            EnqueueCallback(() => Assert.AreEqual<string>("Value", (string)oldValue));
            EnqueueCallback(() => Assert.AreEqual<string>("New Value", (string)newValue));
            EnqueueCallback(() => propertyChanged = string.Empty);

            // Change DisplayValue
            EnqueueCallback(() => attribute.DisplayValue = "New Display Value");
            EnqueueConditional(() => propertyChanged != string.Empty);
            EnqueueCallback(() => Assert.AreEqual<string>("DisplayValue", propertyChanged));
            EnqueueCallback(() => Assert.AreEqual<string>(null, (string)oldValue));
            EnqueueCallback(() => Assert.AreEqual<string>("New Display Value", (string)newValue));
            EnqueueCallback(() => propertyChanged = string.Empty);

            EnqueueTestComplete();
        }

    }
}