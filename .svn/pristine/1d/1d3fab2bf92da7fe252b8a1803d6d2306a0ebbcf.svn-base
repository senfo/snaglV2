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
using Berico.SnagL.Model.Attributes;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Berico.SnagL.Model.Test
{
    [TestClass]
    public class AttributeCollectionTests : SilverlightTest
    {

        [TestMethod]
        [Tag("AttributeCollection")]
        [Description("Test adding an attribute to the collection")]
        public void TestAddingAttributeWithValue()
        {
            AttributeCollection attributes = new AttributeCollection();
            AttributeValue comparisonAttributeValue = new AttributeValue("Value");

            attributes.Add("Name", comparisonAttributeValue);

            Assert.AreEqual<string>(comparisonAttributeValue.Value, attributes["Name"].Value);
        }

        [TestMethod]
        [Tag("AttributeCollection")]
        [Description("Test adding an invalid Attribute instance")]
        public void TestAddingWithNullAttribute()
        {
            bool exceptionThrown;
            AttributeCollection attributes = new AttributeCollection();
            AttributeValue comparisonAttributeValue = new AttributeValue("Value");

            try
            {
                attributes.Add(null, comparisonAttributeValue);
                exceptionThrown = false;
            }
            catch (ArgumentNullException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
        }

        [TestMethod]
        [Tag("AttributeCollection")]
        [Description("Test adding an existing attribute")]
        public void TestAddingExistingAttribute()
        {
            bool exceptionThrown;
            AttributeCollection attributes = new AttributeCollection();
            AttributeValue newAttributeValue = new AttributeValue("Test Value");

            // Add attribute and attribute value
            attributes.Add("Test", newAttributeValue);

            Assert.AreEqual<string>(newAttributeValue.Value, attributes["Test"].Value);

            AttributeValue existingAttributeValue = new AttributeValue("Test Value");

            try
            {
                attributes.Add("Test", existingAttributeValue);
                exceptionThrown = false;
            }
            catch (ArgumentException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
        }

        [TestMethod]
        [Tag("AttributeCollection")]
        [Description("Test updating an attribute")]
        public void TestUpdatingAttribute()
        {
            AttributeCollection attributes = new AttributeCollection();
            AttributeValue newAttributeValue = new AttributeValue("Test Value");

            // Add attribute and attribute value
            attributes.Add("Test", newAttributeValue);

            Assert.AreEqual<string>(newAttributeValue.Value, attributes["Test"].Value);

            AttributeValue updatedAttributeValue = new AttributeValue("New Test Value");

            // Add attribute and attribute value
            attributes.Update("Test", updatedAttributeValue);

            // Ensure that the attribute has an updated value
            Assert.AreEqual<string>(updatedAttributeValue.Value, attributes["Test"].Value);
        }

        [TestMethod]
        [Tag("AttributeCollection")]
        [Description("Test updating an attribute with identical value")]
        public void TestUpdatingIdenticalAttribute()
        {
            AttributeCollection attributes = new AttributeCollection();
            AttributeValue newAttributeValue = new AttributeValue("Test Value");

            // Add attribute and attribute value
            attributes.Add("Test", newAttributeValue);

            Assert.AreEqual<string>(newAttributeValue.Value, attributes["Test"].Value);

            // Try updating the attribute with the same value
            attributes.Update("Test", newAttributeValue);

            // Ensure that the attribute has an updated value
            Assert.AreEqual<string>(newAttributeValue.Value, attributes["Test"].Value);
        }

        [TestMethod]
        [Tag("AttributeCollection")]
        [Description("Test reterieving an Attribute from the collection")]
        public void TestGettingAnAttributeThatExists()
        {
            AttributeCollection attributes = new AttributeCollection();
            AttributeValue newAttributeValue = new AttributeValue("Test Value");

            // Add the attribute
            attributes.Add("Test", newAttributeValue);

            // Check if the attribute exists (by name)
            Assert.AreEqual<AttributeValue>(attributes.GetAttributeValue("Test"), newAttributeValue);
            Assert.AreEqual<AttributeValue>(attributes["Test"], newAttributeValue);
            
            // Check if the attribute exists (by attribute)
            Assert.AreEqual<AttributeValue>(attributes.GetAttributeValue("Test"), newAttributeValue);
            Assert.AreEqual<AttributeValue>(attributes["Test"], newAttributeValue);
        }

        [TestMethod]
        [Tag("AttributeCollection")]
        [Description("Test trying to retrieve an Attribute that doesn't exist")]
        public void TestGettingAnAttributeThatDoesNotExist()
        {
            AttributeCollection attributes = new AttributeCollection();
            AttributeValue foundAttributeValue = attributes.GetAttributeValue("Test");

            Assert.IsNull(foundAttributeValue);
        }

        [TestMethod]
        [Tag("AttributeCollection")]
        [Description("Test to determine if attribute exists in collection using and attribute that does exist")]
        public void TestSuccessfullCheckIfAttributeExists()
        {
            AttributeCollection attributes = new AttributeCollection();
            AttributeValue newAttributeValue = new AttributeValue("Test Value");

            // Add the attribute
            attributes.Add("Test", newAttributeValue);

            Assert.IsTrue(attributes.ContainsAttribute("Test"));
        }

        [TestMethod]
        [Tag("AttributeCollection")]
        [Description("Test to determine if attribute exists in collection using and attribute that does not exist")]
        public void TestFailedCheckIfAttributeExists()
        {
            AttributeCollection attributes = new AttributeCollection();
            Assert.IsFalse(attributes.ContainsAttribute("Not Found"));
        }

        [TestMethod]
        [Tag("AttributeCollection")]
        [Description("Test of counting the Attributes in the collection")]
        public void TestCountingCollectionItems()
        {
            AttributeCollection attributes = new AttributeCollection();

            Assert.AreEqual<int>(0, attributes.Count);

            attributes.Add("Test1", new AttributeValue("Test Value1"));
            attributes.Add("Test2", new AttributeValue("Test Value2"));
            attributes.Add("Test3", new AttributeValue("Test Value3"));
            attributes.Add("Test4", new AttributeValue("Test Value4"));

            Assert.AreEqual<int>(4, attributes.Count);
        }

        [TestMethod]
        [Tag("AttributeCollection")]
        [Description("Test clearing all Attributes in the collection")]
        public void TestClearingCollection()
        {
            AttributeCollection attributes = new AttributeCollection();

            attributes.Add("Test1", new AttributeValue("Test Value1"));
            attributes.Add("Test2", new AttributeValue("Test Value2"));
            attributes.Add("Test3", new AttributeValue("Test Value3"));
            attributes.Add("Test4", new AttributeValue("Test Value4"));

            Assert.AreEqual<int>(4, attributes.Count);

            attributes.Clear();

            Assert.AreEqual<int>(0, attributes.Count);
        }

        [TestMethod]
        [Tag("AttributeCollection")]
        [Description("Test removing an Attribute from the collection")]
        public void TestRemovingAttribute()
        {
            AttributeCollection attributes = new AttributeCollection();

            attributes.Add("Test1", new AttributeValue("Test Value1"));
            attributes.Add("Test2", new AttributeValue("Test Value2"));
            attributes.Add("Test3", new AttributeValue("Test Value3"));
            attributes.Add("Test4", new AttributeValue("Test Value4"));

            Assert.IsTrue(attributes.ContainsAttribute("Test3"));
            Assert.IsTrue(attributes.Remove("Test3"));
            Assert.IsFalse(attributes.ContainsAttribute("Test3"));
        }

        [TestMethod]
        [Tag("AttributeCollection")]
        [Description("Test removing an Attribute, that doesn't exist")]
        public void TestRemovingInvalidAttribute()
        {
            AttributeCollection attributes = new AttributeCollection();

            attributes.Add("Test1", new AttributeValue("Test Value1"));
            attributes.Add("Test2", new AttributeValue("Test Value2"));
            attributes.Add("Test3", new AttributeValue("Test Value3"));
            attributes.Add("Test4", new AttributeValue("Test Value4"));

            Assert.IsFalse(attributes.Remove("Test5"));
        }

        [TestMethod]
        [Tag("AttributeCollection")]
        [Description("Test enumerating all attributes")]
        public void TestEnumeratingAttributes()
        {
            AttributeCollection attributes = new AttributeCollection();

            Assert.AreEqual<int>(0, attributes.Count);

            attributes.Add("Test1", new AttributeValue("Test Value1"));
            attributes.Add("Test2", new AttributeValue("Test Value2"));
            attributes.Add("Test3", new AttributeValue("Test Value3"));
            attributes.Add("Test4", new AttributeValue("Test Value4"));

            List<string> attributeNames = new List<string>(attributes.Attributes);
            for (int i = 1; i <= attributeNames.Count; i++)
            {
                Assert.AreEqual<string>(attributeNames[i - 1], "Test" + i);
            }
        }

        [TestMethod]
        [Tag("AttributeCollection")]
        [Description("Test enumerating all attribute values")]
        public void TestEnumeratingAttributeValues()
        {
            AttributeCollection attributes = new AttributeCollection();

            Assert.AreEqual<int>(0, attributes.Count);

            attributes.Add("Test1", new AttributeValue("Test Value1"));
            attributes.Add("Test2", new AttributeValue("Test Value2"));
            attributes.Add("Test3", new AttributeValue("Test Value3"));
            attributes.Add("Test4", new AttributeValue("Test Value4"));

            List<AttributeValue> attributeValues = new List<AttributeValue>(attributes.AttributeValues);
            for (int i = 1; i <= attributeValues.Count; i++)
            {
                Assert.AreEqual<string>(attributeValues[i - 1].Value, "Test Value" + i);
            }
        }

        [TestMethod]
        [Tag("AttributeCollection")]
        [Description("Test enumerating the collection itself")]
        public void TestEnumeratingCollections()
        {
            AttributeCollection attributes = new AttributeCollection();

            Assert.AreEqual<int>(0, attributes.Count);

            attributes.Add("Test1", new AttributeValue("Test Value1"));
            attributes.Add("Test2", new AttributeValue("Test Value2"));
            attributes.Add("Test3", new AttributeValue("Test Value3"));
            attributes.Add("Test4", new AttributeValue("Test Value4"));

            int counter = 0;
            foreach (KeyValuePair<string, AttributeValue> kv in attributes)
            {
                System.Diagnostics.Debug.WriteLine(kv.ToString());
                counter++;
            }

            Assert.IsTrue(counter == 4);
        }

        [TestMethod]
        [Tag("AttributeCollection")]
        [Asynchronous]
        [Description("Test that ensures CollectionChanged event fires when an item is added to the collection")]
        public void TestCollectionChangedWithAdd()
        {
            AttributeCollection attributes = new AttributeCollection();

            bool eventRaised = false;
            NotifyCollectionChangedAction action = NotifyCollectionChangedAction.Reset;
            int newIndex = -1;
            string newAttributeName = string.Empty;
            AttributeValue newAttributeValue = null;

            // Setup the event handler
            attributes.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) =>
            {
                action = e.Action;
                newAttributeName = ((KeyValuePair<string, AttributeValue>)e.NewItems[0]).Key;
                newAttributeValue = ((KeyValuePair<string, AttributeValue>)e.NewItems[0]).Value;
                newIndex = e.NewStartingIndex;
                eventRaised = true;
            };

            // Make change to property
            EnqueueCallback(() => attributes.Add("Test1", new AttributeValue("Test Value1")));

            // Wait for event to complete
            EnqueueConditional(() => eventRaised != false);

            // Test that event was appropriately fires and handled
            EnqueueCallback(() => Assert.AreEqual<string>("Test1", newAttributeName));
            EnqueueCallback(() => Assert.AreEqual<string>("Test Value1", newAttributeValue.Value));
            EnqueueCallback(() => Assert.AreEqual<NotifyCollectionChangedAction>(action, NotifyCollectionChangedAction.Add));

            // Reset testing fields
            EnqueueCallback(() => action = NotifyCollectionChangedAction.Reset);
            EnqueueCallback(() => newAttributeName = string.Empty);
            EnqueueCallback(() => newAttributeValue = null);
            EnqueueCallback(() => newIndex = -1);
            EnqueueCallback(() => eventRaised = false);

            EnqueueTestComplete();
        }

        [TestMethod]
        [Tag("AttributeCollection")]
        [Asynchronous]
        [Description("Test that ensures CollectionChanged event fires when an item is removed from the collection")]
        public void TestCollectionChangedWithRemove()
        {
            AttributeCollection attributes = new AttributeCollection();

            bool eventRaised = false;
            NotifyCollectionChangedAction action = NotifyCollectionChangedAction.Reset;
            int newIndex = -1;
            string newAttributeName = string.Empty;
            AttributeValue newAttributeValue = null;
            int oldIndex = -1;
            string oldAttributeName = string.Empty;
            AttributeValue oldAttributeValue = null;

            attributes.Add("Test1", new AttributeValue("Test Value1"));

            // Setup the event handler
            attributes.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) =>
            {
                action = e.Action;
                newAttributeName = e.NewItems != null ? ((KeyValuePair<string, AttributeValue>)e.NewItems[0]).Key : string.Empty;
                newAttributeValue = e.NewItems != null ? ((KeyValuePair<string, AttributeValue>)e.NewItems[0]).Value : null;
                newIndex = e.NewStartingIndex;
                oldAttributeName = e.OldItems != null ? ((KeyValuePair<string, AttributeValue>)e.OldItems[0]).Key : string.Empty;
                oldAttributeValue = e.OldItems != null ? ((KeyValuePair<string, AttributeValue>)e.OldItems[0]).Value : null;
                oldIndex = e.OldStartingIndex;

                eventRaised = true;
            };

            // Make change to property
            EnqueueCallback(() => attributes.Remove("Test1"));

            // Wait for event to complete
            EnqueueConditional(() => eventRaised != false);

            // Test that event was appropriately fires and handled
            EnqueueCallback(() => Assert.AreEqual<string>(string.Empty, newAttributeName));
            EnqueueCallback(() => Assert.IsNull(newAttributeValue));
            EnqueueCallback(() => Assert.AreEqual<string>("Test1", oldAttributeName));
            EnqueueCallback(() => Assert.AreEqual<string>("Test Value1", oldAttributeValue.Value));
            EnqueueCallback(() => Assert.AreEqual<NotifyCollectionChangedAction>(action, NotifyCollectionChangedAction.Remove));
            EnqueueCallback(() => Assert.IsTrue(attributes.Count == 0));

            // Reset testing fields
            EnqueueCallback(() => action = NotifyCollectionChangedAction.Reset);
            EnqueueCallback(() => newAttributeName = string.Empty);
            EnqueueCallback(() => newAttributeValue = null);
            EnqueueCallback(() => newIndex = -1);
            EnqueueCallback(() => oldAttributeName = string.Empty);
            EnqueueCallback(() => oldAttributeValue = null);
            EnqueueCallback(() => oldIndex = -1);
            EnqueueCallback(() => eventRaised = false);

            EnqueueTestComplete();
        }

        [TestMethod]
        [Tag("AttributeCollection")]
        [Asynchronous]
        [Description("Test that ensures CollectionChanged event fires when an item is uodated")]
        public void TestCollectionChangedWithUpdatee()
        {
            AttributeCollection attributes = new AttributeCollection();

            bool eventRaised = false;
            NotifyCollectionChangedAction action = NotifyCollectionChangedAction.Reset;
            int newIndex = -1;
            string newAttributeName = string.Empty;
            AttributeValue newAttributeValue = null;
            int oldIndex = -1;
            string oldAttributeName = string.Empty;
            AttributeValue oldAttributeValue = null;

            attributes.Add("Test1", new AttributeValue("Test Value1"));

            // Setup the event handler
            attributes.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) =>
            {
                action = e.Action;
                newAttributeName = e.NewItems != null ? ((KeyValuePair<string, AttributeValue>)e.NewItems[0]).Key : string.Empty;
                newAttributeValue = e.NewItems != null ? ((KeyValuePair<string, AttributeValue>)e.NewItems[0]).Value : null;
                newIndex = e.NewStartingIndex;
                oldAttributeName = e.OldItems != null ? ((KeyValuePair<string, AttributeValue>)e.OldItems[0]).Key : string.Empty;
                oldAttributeValue = e.OldItems != null ? ((KeyValuePair<string, AttributeValue>)e.OldItems[0]).Value : null;
                oldIndex = e.OldStartingIndex;

                eventRaised = true;
            };

            // Make change to property
            EnqueueCallback(() => attributes.Update("Test1", new AttributeValue("Updated Value1")));

            // Wait for event to complete
            EnqueueConditional(() => eventRaised != false);

            // Test that event was appropriately fires and handled
            EnqueueCallback(() => Assert.AreEqual<string>("Test1", oldAttributeName));
            EnqueueCallback(() => Assert.AreEqual<string>("Test1", newAttributeName));
            EnqueueCallback(() => Assert.AreEqual<string>("Test Value1", oldAttributeValue.Value));
            EnqueueCallback(() => Assert.AreEqual<string>("Updated Value1", newAttributeValue.Value));
            EnqueueCallback(() => Assert.AreEqual<NotifyCollectionChangedAction>(action, NotifyCollectionChangedAction.Replace));

            // Reset testing fields
            EnqueueCallback(() => action = NotifyCollectionChangedAction.Reset);
            EnqueueCallback(() => newAttributeName = string.Empty);
            EnqueueCallback(() => newAttributeValue = null);
            EnqueueCallback(() => newIndex = -1);
            EnqueueCallback(() => oldAttributeName = string.Empty);
            EnqueueCallback(() => oldAttributeValue = null);
            EnqueueCallback(() => oldIndex = -1);
            EnqueueCallback(() => eventRaised = false);

            EnqueueTestComplete();
        }

        [TestMethod]
        [Tag("AttributeCollection")]
        [Asynchronous]
        [Description("Test changing the property of an Attribute in the collection and ensuring the AttributePropertyChanged event is fired correctly")]
        public void TestAttributePropertyChangedEvent()
        {
            AttributeCollection attributes = new AttributeCollection();

            object oldValue = null;
            object newValue = null;
            string propertyChanged = string.Empty;

            // Setup the event handler
            attributes.AttributeValuePropertyChanged += (object sender, Berico.Common.Events.PropertyChangedEventArgs<object> e) =>
                {
                    oldValue = e.OldValue;
                    newValue = e.NewValue;
                    propertyChanged = e.PropertyName;
                };

            // Add some attributes to the collection
            attributes.Add("Test1", new AttributeValue("Test Value1"));
            attributes.Add("Test2", new AttributeValue("Test Value2"));
            attributes.Add("Test3", new AttributeValue("Test Value3"));
            attributes.Add("Test4", new AttributeValue("Test Value4"));

            // Retrieve one of the attributes to test
            AttributeValue testAttributeValue = attributes.GetAttributeValue("Test3");

            // Make change to property
            EnqueueCallback(() => testAttributeValue.Value = "New Value");

            // Wait for event to complete
            EnqueueConditional(() => propertyChanged != string.Empty);

            // Test that event was appropriately fires and handled
            EnqueueCallback(() => Assert.AreEqual<string>("Value", propertyChanged)); 
            EnqueueCallback(() => Assert.AreEqual<string>("New Value", newValue as string));
            EnqueueCallback(() => Assert.AreEqual<string>("Test Value3", oldValue as string));

            // Reset testing fields
            EnqueueCallback(() => propertyChanged = string.Empty);
            EnqueueCallback(() => oldValue = null);
            EnqueueCallback(() => newValue = null);

            EnqueueTestComplete();
        }
    }
}