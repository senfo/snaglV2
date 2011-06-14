//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using Berico.Common.Events;
using Berico.SnagL.Infrastructure.Data.Attributes;
using Berico.SnagL.Infrastructure.Modularity.Contracts;
using Berico.SnagL.Infrastructure.Similarity;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Berico.SnagL.Infrastructure.Tests
{
    [TestClass]
    public class AttributeTests : SilverlightTest
    {
        [TestMethod]
        [Tag("Attribute")]
        [Description("Test that the attribute is visible, by default")]
        public void TestAttributesDefaultToVisible()
        {
            Attribute attribute = new Attribute("Name");

            Assert.IsTrue(attribute.Visible);
        }

        [TestMethod]
        [Tag("Attribute")]
        [Description("Test that the attribute defaults to SemanticType.Unknown")]
        public void TestAttributesDefaultToUnknownSemanticType()
        {
            Attribute attribute = new Attribute("Name");

            Assert.IsTrue(attribute.SemanticType == SemanticType.Unknown);
        }

        [TestMethod]
        [Tag("Attribute")]
        [Description("Test the creation of an Attribute with the Name 'Name' and default visibility setting")]
        public void TestCreateAttributeWithProvidedName()
        {
            Attribute attribute = new Attribute("Name");

            Assert.AreEqual<string>("Name", attribute.Name);
            Assert.IsTrue(attribute.Visible);
            Assert.AreEqual<SemanticType>(attribute.SemanticType, SemanticType.Unknown);
        }

        [TestMethod]
        [Tag("Attribute")]
        [Description("Test the creation of a hidden Attribute with the Name 'Name'")]
        public void TestCreateHiddenAttributeWithProvidedName()
        {
            Attribute attribute = new Attribute("Name", false);

            Assert.AreEqual<string>("Name", attribute.Name);
            Assert.IsFalse(attribute.Visible);
            Assert.AreEqual<SemanticType>(attribute.SemanticType, SemanticType.Unknown);
        }

        [TestMethod]
        [Tag("Attribute")]
        [Description("Test the creation of a visible Attribute with the Name 'Name'")]
        public void TestCreateVisibleAttributeWithName()
        {
            Attribute attribute = new Attribute("Name", true);

            Assert.AreEqual<string>("Name", attribute.Name);
            Assert.IsTrue(attribute.Visible);
            Assert.AreEqual<SemanticType>(attribute.SemanticType, SemanticType.Unknown);
        }

        [TestMethod]
        [Tag("Attribute")]
        [Description("Test the creation of a visible Attribute with the Name 'Name' and SemanticType of Name")]
        public void TestCreateAttributeWithNameSemanticType()
        {
            Attribute attribute = new Attribute("Name", SemanticType.Name);

            Assert.AreEqual<string>("Name", attribute.Name);
            Assert.IsTrue(attribute.Visible);
            Assert.AreEqual<SemanticType>(attribute.SemanticType, SemanticType.Name);
        }

        [TestMethod]
        [Tag("Attribute")]
        [Description("Test the creation of a visible Attribute with the Name 'Name', SemanticType of Name and preferred Similarity")]
        public void TestCreateAttributeWithNameSemanticTypePrefferedSim()
        {
            Attribute attribute = new Attribute("Name", typeof(LevenshteinDistanceStringSimilarityMeasure) , SemanticType.Name);

            Assert.AreEqual<string>("Name", attribute.Name);
            Assert.IsTrue(attribute.Visible);
            Assert.AreEqual<string>(attribute.PreferredSimilarityMeasure, typeof(LevenshteinDistanceStringSimilarityMeasure).FullName);
            Assert.AreEqual<SemanticType>(attribute.SemanticType, SemanticType.Name);
        }

        [TestMethod]
        [Tag("Attribute")]
        [Description("Test the attempted creation of an Attribute with a null Name")]
        public void TestCreateAttributeWithNullName()
        {
            bool exceptionThrown;
            string errorMessage;

            try
            {
                Attribute attribute = new Attribute(null);

                exceptionThrown = false;
                errorMessage = string.Empty;
            }
            catch (System.ArgumentNullException ex)
            {
                exceptionThrown = true;
                errorMessage = ex.Message;
            }

            Assert.IsTrue(exceptionThrown);
            Assert.IsTrue(errorMessage.Contains("A name must be provided for this attribute"));
        }

        [TestMethod]
        [Tag("Attribute")]
        [Description("Test the attempted creation of an Attribute with a preffered similarity measure that is the wrong type")]
        public void TestCreateAttributeWithInvalidPreferredSimilarity()
        {
            bool exceptionThrown;
            string errorMessage;

            try
            {
                Attribute attribute = new Attribute("Name", typeof(Attribute));

                exceptionThrown = false;
                errorMessage = string.Empty;
            }
            catch (System.ArgumentException ex)
            {
                exceptionThrown = true;
                errorMessage = ex.Message;
            }

            Assert.IsTrue(exceptionThrown);
            Assert.IsTrue(errorMessage.Contains("The provided type was not a valid Similarity Measure"));
        }

        [TestMethod]
        [Tag("Attribute")]
        [Asynchronous]
        [Description("Test changing an Attribute's properties and ensuring the PropertyChanged event is fired correctly")]
        public void TestChangingAttributeProperties()
        {
            Attribute attribute = new Attribute("Name");
            object oldValue = null;
            object newValue = null;
            string propertyChanged = string.Empty;

            attribute.PropertyChanged += (object sender, PropertyChangedEventArgs<object> e) =>
            {
                propertyChanged = e.PropertyName;
                oldValue = e.OldValue;
                newValue = e.NewValue;
            };

            // Change semantic type
            EnqueueCallback(() => attribute.SemanticType = SemanticType.Name);
            EnqueueConditional(() => propertyChanged != string.Empty);
            EnqueueCallback(() => Assert.AreEqual<string>("SemanticType", propertyChanged));
            EnqueueCallback(() => Assert.AreEqual<SemanticType>(SemanticType.Unknown, (SemanticType)oldValue));
            EnqueueCallback(() => Assert.AreEqual<SemanticType>(SemanticType.Name, (SemanticType)newValue));
            EnqueueCallback(() => oldValue = null);
            EnqueueCallback(() => newValue = null);
            EnqueueCallback(() => propertyChanged = string.Empty);

            // Change visibility
            EnqueueCallback(() => attribute.Visible = false);
            EnqueueConditional(() => propertyChanged != string.Empty);
            EnqueueCallback(() => Assert.AreEqual<string>("Visible", propertyChanged));
            EnqueueCallback(() => Assert.IsTrue((bool)oldValue));
            EnqueueCallback(() => Assert.IsFalse((bool)newValue));
            EnqueueCallback(() => oldValue = null);
            EnqueueCallback(() => newValue = null);
            EnqueueCallback(() => propertyChanged = string.Empty);

            // Change preferred similarity measure
            EnqueueCallback(() => attribute.SetPreferredSimilarityMeasure(typeof(LevenshteinDistanceStringSimilarityMeasure)));
            EnqueueConditional(() => propertyChanged != string.Empty);
            EnqueueCallback(() => Assert.AreEqual<string>("PreferredSimilarityMeasure", propertyChanged));
            EnqueueCallback(() => Assert.AreEqual<string>(string.Empty, (string)oldValue));
            EnqueueCallback(() => Assert.AreEqual<string>(typeof(LevenshteinDistanceStringSimilarityMeasure).FullName, (string)newValue));
            EnqueueCallback(() => oldValue = null);
            EnqueueCallback(() => newValue = null);
            EnqueueCallback(() => propertyChanged = string.Empty);

            EnqueueTestComplete();
        }

        [TestMethod]
        [Tag("Attribute")]
        [Description("Ensures the SetPreferredSimilarityMeasure method accepts only ISimilarityMeasure objects")]
        public void TestSetSimilarityMethodAcceptsOnlyISimilarityMeasureObjects()
        {
            bool exceptionThrown = false;
            Attribute attribute = new Attribute("Name");

            try
            {
                attribute.SetPreferredSimilarityMeasure(typeof(string));
            }
            catch (System.ArgumentException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
        }

        [TestMethod]
        [Tag("Attribute")]
        [Description("Validates functionality of the SetPreferredSimilarityMeasure method")]
        public void TestSettingSimilarityMeasure()
        {
            Attribute attribute = new Attribute("Name");
            ISimilarityMeasure measure = new AlphabeticalSimilarityMeasure();

            attribute.SetPreferredSimilarityMeasure(typeof(AlphabeticalSimilarityMeasure));

            Assert.AreEqual(attribute.PreferredSimilarityMeasure, typeof(AlphabeticalSimilarityMeasure).FullName);
        }

        [TestMethod]
        [Tag("Attribute")]
        [Description("Ensures the name of the Attribute is returned when ToString() is called")]
        public void TestToStringReturnsNameOfAttribute()
        {
            string expected = "SnagL";
            Attribute attribute = new Attribute(expected);

            Assert.AreEqual(expected, attribute.ToString());
            Assert.AreEqual(attribute.Name, attribute.ToString());
        }

        [TestMethod]
        [Tag("Attribute")]
        [Description("Ensures that two attributes with the same properties, but different references are equal")]
        public void TestEqualityOfAttributes()
        {
            Attribute attribute1 = new Attribute("SnagL");
            Attribute attribute2 = new Attribute("SnagL");

            Assert.AreEqual<Attribute>(attribute1, attribute2);
        }

        [TestMethod]
        [Tag("Attribute")]
        [Description("Ensures that two attributes with different properties are not equal")]
        public void TestInequalityOfAttributes()
        {
            Attribute attribute1 = new Attribute("SnagL1");
            Attribute attribute2 = new Attribute("SnagL2");

            Assert.AreNotEqual<Attribute>(attribute1, attribute2);
        }

        [TestMethod]
        [Tag("Attribute")]
        [Description("Ensures that two attributes with different properties are not equal using equality operator")]
        public void TestInequalityOfAttributesWithEqualityOperator()
        {
            Attribute attribute1 = new Attribute("SnagL1");
            Attribute attribute2 = new Attribute("SnagL2");

            Assert.IsFalse(attribute1 == attribute2);
        }
    }
}