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
using System.Diagnostics;

namespace Berico.SnagL.Infrastructure.Tests
{
    [TestClass]
    public class DoubleMetaphoneSimilarityMeasureTests : SilverlightTest
    {
        DoubleMetaphoneSimilarityMeasure simMeasure = new DoubleMetaphoneSimilarityMeasure();

        [TestMethod]
        [Tag("SimilarityMeasures")]
        [Description("Test similar values")]
        public void TestShortDistances()
        {
            double? result = 0.0;

            result = simMeasure.CalculateDistance("Todd Herman", "Todd Hermon");
            Debug.WriteLine("{0} compared to {1} => {2}", "Todd Herman", "Todd Hermon", result.Value);
            Assert.AreEqual<double>(0, result.Value);
            result = simMeasure.CalculateDistance("Todd Herman", "Todd Herman");
            Debug.WriteLine("{0} compared to {1} => {2}", "Todd Herman", "Todd Herman", result.Value);
            Assert.AreEqual<double>(0, result.Value);
            result = simMeasure.CalculateDistance("Todd Herman", "Todd Hermen");
            Debug.WriteLine("{0} compared to {1} => {2}", "Todd Herman", "Todd Hermen", result.Value);
            Assert.AreEqual<double>(0, result.Value);
            result = simMeasure.CalculateDistance("Todd Herman", "Todd Harmon");
            Debug.WriteLine("{0} compared to {1} => {2}", "Todd Herman", "Todd Harmon", result.Value);
            Assert.AreEqual<double>(0, result.Value);
            result = simMeasure.CalculateDistance("Todd Herman", "Tadd Harmon");
            Debug.WriteLine("{0} compared to {1} => {2}", "Todd Herman", "Todd Hermon", result.Value);
            Assert.AreEqual<double>(0, result.Value);
        }

        [TestMethod]
        [Tag("SimilarityMeasures")]
        [Description("Test less similar values")]
        public void TestMediumDistances()
        {
            double? result = 0.0;

            result = simMeasure.CalculateDistance("Todd Austin Herman", "Todd Herman");
            Debug.WriteLine("{0} compared to {1} => {2}", "Todd Austin Herman", "Todd Herman", result.Value);
            Assert.IsTrue(result < 1 && result > 0);
            result = simMeasure.CalculateDistance("Todd Herman", "Tad Williams");
            Debug.WriteLine("{0} compared to {1} => {2}", "Todd Herman", "Tad Williams", result.Value);
            Assert.IsTrue(result < 1 && result > 0);
            result = simMeasure.CalculateDistance("Todd Herman", "Hermin");
            Debug.WriteLine("{0} compared to {1} => {2}", "Todd Herman", "Hermin", result.Value);
            Assert.IsTrue(result < 1 && result > 0);
            result = simMeasure.CalculateDistance("Todd Herman", "Dan Herman");
            Debug.WriteLine("{0} compared to {1} => {2}", "Todd Herman", "Dan Herman", result.Value);
            Assert.IsTrue(result < 1 && result > 0);
            result = simMeasure.CalculateDistance("Todd Herman", "Todd Robert Herman");
            Debug.WriteLine("{0} compared to {1} => {2}", "Todd Herman", "Tad Hermin", result.Value);
            Assert.IsTrue(result < 1 && result > 0);
        }

        [TestMethod]
        [Tag("SimilarityMeasures")]
        [Description("Test very dissimilar values")]
        public void TestLongDistances()
        {
            double? result = 0.0;

            result = simMeasure.CalculateDistance("Todd Herman", "William Adama");
            Debug.WriteLine("{0} compared to {1} => {2}", "Todd Herman", "William Adama", result.Value);
            Assert.AreEqual<double>(1, result.Value);
            result = simMeasure.CalculateDistance("Todd Herman", "Inigo Montoya");
            Debug.WriteLine("{0} compared to {1} => {2}", "Todd Herman", "Inigo Montoya", result.Value);
            Assert.AreEqual<double>(1, result.Value);
            result = simMeasure.CalculateDistance("Todd Herman", "James Kirk");
            Debug.WriteLine("{0} compared to {1} => {2}", "Todd Herman", "James Kirk", result.Value);
            Assert.AreEqual<double>(1, result.Value);
            result = simMeasure.CalculateDistance("Todd Herman", "Malcolm Reynolds");
            Debug.WriteLine("{0} compared to {1} => {2}", "Todd Herman", "Malcolm Reynolds", result.Value);
            Assert.AreEqual<double>(1, result.Value);
            result = simMeasure.CalculateDistance("Todd Herman", "Rick Deckard");
            Debug.WriteLine("{0} compared to {1} => {2}", "Todd Herman", "Rick Deckard", result.Value);
            Assert.AreEqual<double>(1, result.Value);
        }
    }
}