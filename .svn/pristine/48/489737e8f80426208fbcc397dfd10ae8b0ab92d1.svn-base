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
    /// Measures the similarity between two date/time values
    /// </summary>
    [PartMetadata("ID", "SimilarityMeasureExtension"), Export(typeof(ISimilarityMeasure))]
    public class DateTimeSimilarityMeasure : SimilarityMeasureBase, IComparer
    {
        private const SemanticType ASSIGNED_SEMANTIC_TYPES = SemanticType.GeneralString | SemanticType.Date;

        /// <summary>
        /// Creates a new instance of the DateTimeSimilarityMeasure class
        /// </summary>
        public DateTimeSimilarityMeasure() : base("Date/Time", ASSIGNED_SEMANTIC_TYPES, "Measures attribute similarity based on how the values compare chronologically.") { }

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

            DateTime sourceDateTime;
            DateTime targetDateTime;

            // Parse the string date/time values to an actual date and time instance
            if (DateTime.TryParse(sourceValue, out sourceDateTime) && DateTime.TryParse(targetValue, out targetDateTime))
            {
                // Convert the date and time values to a double and subtract them
                // TODO:  TEST USING TICKS INSTEAD OF CONVERTING DATE AND TIME TO MINUTES
                //return Math.Abs(DateTimeToDouble(value1) - DateTimeToDouble(value2));
                return Math.Abs(sourceDateTime.Ticks - targetDateTime.Ticks);
            }
            else
                return null;
        }

        /// <summary>
        /// Converts the provided date and time to a number
        /// </summary>
        /// <param name="dateTime">The date/time value to be converted</param>
        /// <returns>a numerical representation of the date and time</returns>
        private double DateTimeToDouble(DateTime dateTime)
        {
            // converts the date and time to minutes and returns it
            return dateTime.Minute + dateTime.Hour * 60 + dateTime.Day * 1440 + dateTime.Month * 43200 + dateTime.Year * 43200 * 12;
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

                // Convert our incoming values to strings
                string sourceValue = x as string;
                string targetValue = y as string;

                DateTime sourceDateTime;
                DateTime targetDateTime;

                // Attempt to parse the provided values to DateTime instances
                bool sourceParseSuccess = DateTime.TryParse(sourceValue as string, out sourceDateTime);
                bool targetParseSuccess = DateTime.TryParse(targetValue as string, out targetDateTime);

                // Validate the parsing results
                if (sourceParseSuccess && targetParseSuccess)
                    return sourceDateTime.CompareTo(targetDateTime);
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

    }
}