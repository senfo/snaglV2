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
using System.ComponentModel.Composition;
using Berico.SnagL.Infrastructure.Data.Attributes;
using Berico.SnagL.Infrastructure.Modularity.Contracts;

namespace Berico.SnagL.Infrastructure.Similarity
{
    /// <summary>
    /// Determines if two values match exactly or not
    /// </summary>
    [PartMetadata("ID", "SimilarityMeasureExtension"), Export(typeof(ISimilarityMeasure))]
    public class ExactMatchSimilarityMeasure : SimilarityMeasureBase
    {
        private const SemanticType ASSIGNED_SEMANTIC_TYPES = SemanticType.GeneralString | SemanticType.Age | SemanticType.Number | SemanticType.Date | SemanticType.Name | SemanticType.EmailAddress | SemanticType.PhoneNumber | SemanticType.Coordinates;

        /// <summary>
        /// Creates a new instance of the ExactMatchSimilarityMeasure class
        /// </summary>
        public ExactMatchSimilarityMeasure() : base("Exact Match", ASSIGNED_SEMANTIC_TYPES, "Measures attribute similarity based whether they match exactly (ignoring case) or not.") { }

        /// <summary>
        /// Calculates how similar the two provided values are for the
        /// provided attribute
        /// </summary>
        /// <param name="attribute">This paremeter is not used for this measure</param>
        /// <param name="sourceValue">The first value for comparison</param>
        /// <param name="targetValue">The second value for comparison</param>
        /// <returns>1 if the two values match; otherwise 0</returns>
        public override double? MeasureSimilarity(string attribute, string sourceValue, string targetValue)
        {
            // We actually only want to return 1 or 0 so we subtract the
            // string comparison results from 1 to get the value we want
            return 1 - CalculateDistance(sourceValue, targetValue);
        }

        /// <summary>
        /// Compares the two provided values and returns the result
        /// </summary>
        /// <param name="value1">The first value for comparison</param>
        /// <param name="value2">The second value for comparison</param>
        /// <returns>0 if both values are the same; 1 if the first value is
        /// greater than the second; -1 if the first value is less than the
        /// second </returns>
        public override double? CalculateDistance(string sourceValue, string targetValue)
        {
            int comparison = string.Compare(sourceValue, targetValue, StringComparison.CurrentCultureIgnoreCase);

            if (comparison == 0)
                return 0;
            else
                return 1;

        }
    }
}