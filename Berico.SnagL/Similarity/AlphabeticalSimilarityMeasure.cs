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
    /// Measures the similarity between two values based on how different
    /// they are alphabetically.
    /// </summary>
    [PartMetadata("ID", "SimilarityMeasureExtension"), Export(typeof(ISimilarityMeasure))]
    public class AlphabeticalSimilarityMeasure : SimilarityMeasureBase, IComparer
    {
        private readonly int MIN_CHAR_VALUE = Convert.ToInt32('a');
        private const SemanticType ASSIGNED_SEMANTIC_TYPES = SemanticType.GeneralString | SemanticType.Date | SemanticType.Name | SemanticType.EmailAddress | SemanticType.PhoneNumber | SemanticType.Coordinates;
        private int baseNumber = 36;

        /// <summary>
        /// Creates a new instance of the AlphabeticalSimilarityMeasure class
        /// </summary>
        public AlphabeticalSimilarityMeasure() : base("Alphabetical", ASSIGNED_SEMANTIC_TYPES, "Measures attribute  similarity based on how the values compare alphabetically.") { }

        /// <summary>
        /// Calculates the distance between the two provided values based on
        /// alphabetically comparing individual characters with decreasing
        /// weight for letters further in.
        /// </summary>
        /// <param name="sourceValue">The soruce value</param>
        /// <param name="targetValue">The target value</param>
        /// <returns>a value representing the distance between the two provided values</returns>
        public override double? CalculateDistance(string sourceValue, string targetValue)
        {
            // Validate parameters
            if (sourceValue == null || targetValue == null) return null;

            // Convert both values to a number
            double sourceNumber = StringToCustomBase(sourceValue);
            double targetNumber = StringToCustomBase(targetValue);

            // Return the calculated distance
            return Math.Abs(sourceNumber - targetNumber);
        }

        #region IComparer Members
            
            /// <summary>
            /// Compares the two provided objects
            /// </summary>
            /// <param name="x">The first (source) value for comparison</param>
            /// <param name="y">The second (source) value for comparison</param>
            /// <returns>0 if the values are equal; -1 if the x value is less than
            /// the y value; 1 if the x value is greater than the y value</returns>
            public int Compare(object x, object y)
            {
                if (x == null)
                    return -1;

                string sourceValue = x as string;
                string targetValue = y as string;

                // Use a string comparer to compare the two values
                return String.Compare(sourceValue, targetValue, StringComparison.CurrentCultureIgnoreCase);

            }

        #endregion

        /// <summary>
        /// Convert the given string to a base 10 number.  This is done by converting 
        /// each character to a base 26 number (which ranges from from 1-26).  The
        /// first character rpresents the "ones" place, the second character the "tenths",
        /// the third the "hundredths", etc.  Once that is done, the number is converted
        /// to a base 10 number.
        /// </summary>
        /// <example>
        /// "abz" would result in [1].[2][26] which would then be converted to base 10
        /// for a final result of ~1.115384615384.
        /// </example>
        /// <param name="value">The string to convert to the custom base</param>
        /// <returns>a number representing the string</returns>
        private double StringToCustomBase(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;

            // Convert the string to lowercase
            value = value.ToLower(System.Globalization.CultureInfo.CurrentUICulture);

            double base26 = 0;

            // Loop through each character in the string
            for (int digit = 0; digit < value.Length; digit++)
            {
                if (char.IsLetterOrDigit(value[digit]))
                {
                    // Covnert the letter to a number value
                    int charNumber = LetterToNumber(value[digit]);

                    base26 += charNumber * Math.Pow(baseNumber, -digit);
                }
            }

            return base26;
        }

        /// <summary>
        /// Converts the provided character to a number
        /// </summary>
        /// <param name="c">The character that should be converted</param>
        /// <returns>a numerical representation of the provided character</returns>
        private int LetterToNumber(char c)
        {
            int intChar;
            if (char.IsDigit(c) && Int32.TryParse(c.ToString(), out intChar))
                return intChar;
            else
            {
                // Convert the base26 value into base 10
                return Convert.ToInt32(c) - MIN_CHAR_VALUE + 11;
            }
        }

    }
}