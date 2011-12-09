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
using System.Collections;
using System.ComponentModel.Composition;
using Berico.SnagL.Infrastructure.Data.Attributes;
using Berico.SnagL.Infrastructure.Modularity.Contracts;

namespace Berico.SnagL.Infrastructure.Similarity
{
    /// <summary>
    /// Measures the similarity between two numerical values
    /// </summary>
    [PartMetadata("ID", "SimilarityMeasureExtension"), Export(typeof(ISimilarityMeasure))]
    public class NumericSimilarityMeasure : SimilarityMeasureBase, IComparer
    {
        private const SemanticType ASSIGNED_SEMANTIC_TYPES = SemanticType.Number | SemanticType.GeneralString | SemanticType.PhoneNumber | SemanticType.Age;

        /// <summary>
        /// Creates a new instance of the AlphabeticalSimilarityMeasure class
        /// </summary>
        public NumericSimilarityMeasure() : base("Numerical", ASSIGNED_SEMANTIC_TYPES, "Measures attribute similarity based on how the values compare numerically.") { }

        #region IComparer Members

            public int Compare(object x, object y)
            {
                if (x == null)
                    return -1;

                // Convert our incoming values to strings
                string sourceValue = x as string;
                string targetValue = y as string;

                double sourceDouble;
                double targetDouble;

                // Attempt to parse the provided values to DateTime instances
                bool sourceParseSuccess = TryParseNumber(sourceValue as string, out sourceDouble);
                bool targetParseSuccess = TryParseNumber(targetValue as string, out targetDouble);

                // Validate the parsing results
                if (sourceParseSuccess && targetParseSuccess)
                    return sourceDouble.CompareTo(targetDouble);
                else if (sourceParseSuccess && !targetParseSuccess)
                    return 1;
                else if (!sourceParseSuccess && targetParseSuccess)
                    return -1;
                else
                {
                    // Attempt to compare the values as string
                    return sourceValue.CompareTo(targetValue);
                }
            }

        #endregion

        /// <summary>
        /// Attempts to create a double from the provided string
        /// </summary>
        /// <param name="value">The value to be parsed</param>
        /// <param name="number">The output of the parsing operation</param>
        /// <returns>true if the parsing succeeded; otherwise false</returns>
        private bool TryParseNumber(string value, out double number)
        {
            // Validate parameter
            if (string.IsNullOrEmpty(value))
            {
                number = 0;
                return true;
            }

            //TODO:  CLEANUP BELOW SHOULD BE CONFIGURABLE

            // Remove extraneous symbols from the number
            string cleanValue = value.Trim().Replace("$", "").Replace(",", "").Replace("%", "");

            // Parse and return the value
            return double.TryParse(cleanValue, out number);
        }

        /// <summary>
        /// Calculates the distance between the two provided numerical values
        /// </summary>
        /// <param name="sourceValue">The soruce value</param>
        /// <param name="targetValue">The target value</param>
        /// <returns>a value representing the distance between the two provided values</returns>
        public override double? CalculateDistance(string sourceValue, string targetValue)
        {

            // Validate parameters
            if (sourceValue == null || targetValue == null) return null;

            double sourceDouble;
            double targetDouble;

            if (TryParseNumber(sourceValue, out sourceDouble) && TryParseNumber(targetValue, out targetDouble))
                return Math.Abs(sourceDouble - targetDouble);
            else
                return null;

        }

    }
}