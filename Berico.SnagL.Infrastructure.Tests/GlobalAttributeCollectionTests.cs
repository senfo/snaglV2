//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using Berico.SnagL.Infrastructure.Data.Attributes;
using Berico.SnagL.Model.Attributes;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;

namespace Berico.LinkAnalysis.Model.Test
{
    [TestClass]
    public class GlobalAttributeCollectionTest : SilverlightTest
    {
        [TestMethod]
        [Tag("GlobalAttributeCollection")]
        [Description("Test to ensure the requested instance of the GlobalAttributeCollection is returned")]
        public void TestGettingSingletonInstanceWithSameNameAreSameInstance()
        {
            GlobalAttributeCollection globalAttributeCollection1 = GlobalAttributeCollection.GetInstance("SnagL");
            GlobalAttributeCollection globalAttributeCollection2 = GlobalAttributeCollection.GetInstance("SnagL");

            Assert.AreEqual<GlobalAttributeCollection>(globalAttributeCollection1, globalAttributeCollection2);
        }

        [TestMethod]
        [Tag("GlobalAttributeCollection")]
        [Description("Test to ensure two different requested instances of the GlobalAttributeCollection are different")]
        public void TestGettingSingletonInstancesWithDifferentNamesAreDifferentInstances()
        {
            GlobalAttributeCollection globalAttributeCollection1 = GlobalAttributeCollection.GetInstance("SnagL1");
            GlobalAttributeCollection globalAttributeCollection2 = GlobalAttributeCollection.GetInstance("SnagL2");

            Assert.AreNotEqual<GlobalAttributeCollection>(globalAttributeCollection1, globalAttributeCollection2);
        }

        [TestMethod]
        [Tag("GlobalAttributeCollection")]
        [Description("Verifies an ArgumentNullException is thrown if user attempts to add a null Attribute")]
        public void TestAddMethodThrowsExceptionIfNullAttribute()
        {
            bool exceptionCaught = false;
            GlobalAttributeCollection globalAttributeCollection = GlobalAttributeCollection.GetInstance("SnagL");
            AttributeValue value = new AttributeValue("SnagL");

            try
            {
                globalAttributeCollection.Add(null, value);
            }
            catch (System.ArgumentNullException)
            {
                exceptionCaught = true;
            }

            Assert.IsTrue(exceptionCaught);
        }

        [TestMethod]
        [Tag("GlobalAttributeCollection")]
        [Description("Verifies an ArgumentNullException is thrown if user attempts to add a null AttributeValue")]
        public void TestAddMethodThrowsExceptionIfNullAttributeValue()
        {
            bool exceptionCaught = false;
            GlobalAttributeCollection globalAttributeCollection = GlobalAttributeCollection.GetInstance("SnagL");
            Attribute attribute = new Attribute("SnagL");

            try
            {
                globalAttributeCollection.Add(attribute, null);
            }
            catch (System.ArgumentNullException)
            {
                exceptionCaught = true;
            }

            Assert.IsTrue(exceptionCaught);
        }

        [TestMethod]
        [Tag("GlobalAttributeCollection")]
        [Description("Verifies the collection contains the attribute after adding it")]
        public void VerifyCollectionContainsAttributeAfterAddingIt()
        {
            GlobalAttributeCollection globalAttributeCollection = GlobalAttributeCollection.GetInstance("SnagL");
            Attribute attribute = new Attribute("SnagL");
            AttributeValue value = new AttributeValue("SnagL");
            bool actual;

            globalAttributeCollection.Add(attribute, value);
            actual = globalAttributeCollection.ContainsAttribute(attribute);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        [Tag("GlobalAttributeCollection")]
        [Description("Verifies the collection contains the attribute after adding it and getting it by name")]
        public void VerifyCollectionContainsAttributeAfterAddingItRetrieveByName()
        {
            GlobalAttributeCollection globalAttributeCollection = GlobalAttributeCollection.GetInstance("SnagL");
            Attribute attribute = new Attribute("SnagL");
            AttributeValue value = new AttributeValue("SnagL");
            bool actual;

            globalAttributeCollection.Add(attribute, value);
            actual = globalAttributeCollection.ContainsAttribute("SnagL");

            Assert.IsTrue(actual);
        }

        [TestMethod]
        [Tag("GlobalAttributeCollection")]
        [Description("Verifies the GetAttribute method returns the requested attribute by name")]
        public void TestGetAttributeByName()
        {
            Attribute actual;
            Attribute expected = new Attribute("SnagL");
            AttributeValue value = new AttributeValue("SnagL");
            GlobalAttributeCollection globalAttributeCollection = GlobalAttributeCollection.GetInstance("SnagL");

            globalAttributeCollection.Add(expected, value);
            actual = globalAttributeCollection.GetAttribute(expected.Name);

            Assert.AreEqual<Attribute>(expected, actual);
        }

        [TestMethod]
        [Tag("GlobalAttributeCollection")]
        [Description("Ensures the AttributeValue exists after it has been added to global collection")]
        public void TestAttributeValueExistsAfterAdd()
        {
            GlobalAttributeCollection globalAttributeCollection = GlobalAttributeCollection.GetInstance("SnagL");
            Attribute attribute = new Attribute("SnagL");
            AttributeValue original = new AttributeValue("SnagL");

            globalAttributeCollection.Add(attribute, original);

            Assert.IsTrue(globalAttributeCollection[attribute].Contains("SnagL"));
        }

        [TestMethod]
        [Tag("GlobalAttributeCollection")]
        [Asynchronous]
        [Description("Ensures the AttributeListUpdated is raised when an item is added")]
        public void TestCollectionChangedWithAdd()
        {
            bool eventRaised = false;
            Attribute attribute = new SnagL.Infrastructure.Data.Attributes.Attribute("SnagL");
            AttributeValue newValue = new AttributeValue("SnagL");
            GlobalAttributeCollection globalAttributeCollection = GlobalAttributeCollection.GetInstance("SnagL");

            globalAttributeCollection.Clear();

            globalAttributeCollection.AttributeListUpdated += (sender, e) => {
                eventRaised = true;
            };

            EnqueueCallback(() => globalAttributeCollection.Add(attribute, newValue));
            EnqueueConditional(() => eventRaised);

            EnqueueCallback(() => Assert.IsTrue(eventRaised));
            EnqueueTestComplete();
        }

        [TestMethod]
        [Tag("GlobalAttributeCollection")]
        [Asynchronous]
        [Description("Ensures the AttributeListUpdated event is raised when an item is updated")]
        public void TestCollectionChangedWithUpdate()
        {
            bool eventRaised = false;
            NotifyCollectionChangedAction action = NotifyCollectionChangedAction.Add;
            Attribute attribute = new SnagL.Infrastructure.Data.Attributes.Attribute("SnagL");
            AttributeValue original = new AttributeValue("SnagL");
            AttributeValue newValue = new AttributeValue("SnagLNew");
            GlobalAttributeCollection globalAttributeCollection = GlobalAttributeCollection.GetInstance("SnagL");

            globalAttributeCollection.Clear();

            globalAttributeCollection.AttributeListUpdated += (sender, e) =>
            {
                action = e.Action;
                eventRaised = true;
            };

            EnqueueCallback(() => globalAttributeCollection.Add(attribute, original));
            EnqueueCallback(() => globalAttributeCollection.Update(attribute, newValue, original));
            EnqueueConditional(() => eventRaised);

            EnqueueCallback(() => Assert.IsTrue(action == NotifyCollectionChangedAction.Replace));
            EnqueueCallback(() => Assert.IsTrue(eventRaised));
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Tag("GlobalAttributeCollection")]
        [Description("Ensures the value is updated")]
        public void TestAttributeValueUpdated()
        {
            bool eventRaised = false;
            GlobalAttributeCollection globalAttributeCollection = GlobalAttributeCollection.GetInstance("SnagL");
            Attribute attribute = new Attribute("SnagL");
            AttributeValue original = new AttributeValue("SnagL");
            AttributeValue expected = new AttributeValue("SnagLNew");
            AttributeValue actual = null;

            globalAttributeCollection.AttributeListUpdated += (sender, e) =>
            {
                actual = e.NewValue;
                eventRaised = true;
            };

            EnqueueCallback(() => globalAttributeCollection.Clear());
            EnqueueCallback(() => globalAttributeCollection.Add(attribute, original));
            EnqueueCallback(() => globalAttributeCollection.Update(attribute, expected, original));
            EnqueueConditional(() => eventRaised);

            EnqueueCallback(() => Assert.IsTrue(globalAttributeCollection[attribute].Contains(expected.Value)));
            EnqueueCallback(() => Assert.AreEqual(expected.Value, actual.Value));
            EnqueueTestComplete();
        }

        [TestMethod]
        [Tag("GlobalAttributeCollection")]
        [Description("Ensures the value is updated")]
        public void TestClearMethodClearsGlobalAttributeCollection()
        {
            GlobalAttributeCollection globalAttributeCollection = GlobalAttributeCollection.GetInstance("SnagL");
            Attribute attribute = new Attribute("SnagL");
            AttributeValue original = new AttributeValue("SnagL");

            globalAttributeCollection.Add(attribute, original);

            // Make sure everything is working. Otherwise, we might have an invalid test.
            if (!(globalAttributeCollection.GetAttributes().Count > 0))
            {
                Assert.Inconclusive("Attribute collection count wasn't > 0 after attribute added.");
            }

            globalAttributeCollection.Clear();
            Assert.IsTrue(globalAttributeCollection.GetAttributes().Count == 0);
        }

        [TestMethod]
        [Tag("GlobalAttributeCollection")]
        [Description("Ensures the attribute is removed from the GlobalAttributeCollection when referenced by name")]
        public void TestRemoveByStringRemovesAttribute()
        {
            GlobalAttributeCollection globalAttributeCollection = GlobalAttributeCollection.GetInstance("SnagL");
            Attribute attribute = new Attribute("SnagL");
            AttributeValue original = new AttributeValue("SnagL");

            globalAttributeCollection.Add(attribute, original);

            if (!globalAttributeCollection.ContainsAttribute(attribute))
            {
                Assert.Inconclusive("The Attribute that was just added to the GlobalAttributeCollection was not found");
            }

            globalAttributeCollection.Remove(attribute.Name);
            Assert.IsFalse(globalAttributeCollection.ContainsAttribute(attribute));
        }

        [TestMethod]
        [Tag("GlobalAttributeCollection")]
        [Description("Ensures the attribute is removed from the GlobalAttributeCollection")]
        public void TestRemoveByAttributeRemovesAttribute()
        {
            GlobalAttributeCollection globalAttributeCollection = GlobalAttributeCollection.GetInstance("SnagL");
            Attribute attribute = new Attribute("SnagL");
            AttributeValue original = new AttributeValue("SnagL");

            globalAttributeCollection.Add(attribute, original);

            if (!globalAttributeCollection.ContainsAttribute(attribute))
            {
                Assert.Inconclusive("The Attribute that was just added to the GlobalAttributeCollection was not found");
            }

            globalAttributeCollection.Remove(attribute);
            Assert.IsFalse(globalAttributeCollection.ContainsAttribute(attribute));
        }

        [TestMethod]
        [Tag("GlobalAttributeCollection")]
        [Description("Ensures the attribute is located within the GlobalAttributeCollection after being added and refereded by name")]
        public void TestContainsAttributeByString()
        {
            GlobalAttributeCollection globalAttributeCollection = GlobalAttributeCollection.GetInstance("SnagL");
            Attribute attribute = new Attribute("SnagL");
            AttributeValue original = new AttributeValue("SnagL");

            globalAttributeCollection.Add(attribute, original);
            Assert.IsTrue(globalAttributeCollection.ContainsAttribute(attribute.Name));
        }

        [TestMethod]
        [Tag("GlobalAttributeCollection")]
        [Description("Ensures the attribute is located within the GlobalAttributeCollection after being added")]
        public void TestContainsAttribute()
        {
            GlobalAttributeCollection globalAttributeCollection = GlobalAttributeCollection.GetInstance("SnagL");
            Attribute attribute = new Attribute("SnagL");
            AttributeValue original = new AttributeValue("SnagL");

            globalAttributeCollection.Add(attribute, original);
            Assert.IsTrue(globalAttributeCollection.ContainsAttribute(attribute));
        }

        [TestMethod]
        [Tag("GlobalAttributeCollection")]
        [Description("Ensures the attribute value is accessible the string accessor after being added")]
        public void TestAttributeAccessor()
        {
            GlobalAttributeCollection globalAttributeCollection = GlobalAttributeCollection.GetInstance("SnagL");
            Attribute attribute = new Attribute("SnagL");
            AttributeValue original = new AttributeValue("SnagL");

            globalAttributeCollection.Clear();
            globalAttributeCollection.Add(attribute, original);
            Assert.IsTrue(globalAttributeCollection[attribute].Contains("SnagL"));
        }

        [TestMethod]
        [Tag("GlobalAttributeCollection")]
        [Description("Ensures the attribute value is accessible the string accessor after being added")]
        public void TestStringAccessor()
        {
            GlobalAttributeCollection globalAttributeCollection = GlobalAttributeCollection.GetInstance("SnagL");
            Attribute attribute = new Attribute("SnagL");
            AttributeValue original = new AttributeValue("SnagL");

            globalAttributeCollection.Clear();
            globalAttributeCollection.Add(attribute, original);
            Assert.IsTrue(globalAttributeCollection["SnagL"].Count > 0);
        }
    }
}