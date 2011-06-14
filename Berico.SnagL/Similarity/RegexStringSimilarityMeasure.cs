//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using System.Text.RegularExpressions;
using Berico.SnagL.Infrastructure.Data.Attributes;
using Berico.SnagL.Infrastructure.Modularity.Contracts;

namespace Berico.SnagL.Infrastructure.Similarity
{
    /// <summary>
    /// Measures the similarity between a portion of two strings.  For example,
    /// comparing the domain portion of two email addresses.  This class
    /// can not be instantiated directly.
    /// </summary>
    public abstract class RegexStringSimilarityMeasure : SimilarityMeasureBase
    {

        private string expression = string.Empty;

        /// <summary>
        /// Creates a new instance of the RegexStringSimilarityMeasure class
        /// </summary>
        /// <param name="name">The name to be assigned to this similarity measure</param>
        /// <param name="regex">A regular expression to be used for comparing only
        /// a specific portion of the strings</param>
        public RegexStringSimilarityMeasure(string _name, string _regex, SemanticType _type, string _description )
            : base(_name, _type, _description)
        {
            this.expression = _regex;
        }

        /// <summary>
        /// Calculates distance between a portion of two strings
        /// </summary>
        /// <param name="sourceValue">The source value</param>
        /// <param name="targetValue">The target value</param>
        /// <returns>a value representing the edit distance between the two provided values</returns>
        public override double? CalculateDistance(string sourceValue, string targetValue)
        {
            string sourcePart = GetStringPart(sourceValue);
            string targetPart = GetStringPart(targetValue);

            // Ensure we got two parts from the provided strings
            if (string.IsNullOrEmpty(sourcePart) || string.IsNullOrEmpty(targetPart))
                return null;

            // Use the Levenshtein Distance similarity measure to 
            // get the distance for the returned strings
            return new LevenshteinDistanceStringSimilarityMeasure().CalculateDistance(sourcePart, targetPart);
        }

        /// <summary>
        /// Executes a regular expression to limit the portion of
        /// the string that is being compared by the similarity
        /// measure
        /// </summary>
        /// <param name="value">The string to be analyzed</param>
        /// <returns>a string containg the desired portion of the provided
        /// string; otherwise null</returns>
        private string GetStringPart(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            // Attempt to match the regular expression
            Match match = Regex.Match(value, this.expression);

            // If the match was successfull, return the matched value
            if (match.Success)
                return match.Groups[1].Value;
            else
                return null;
        }
    }
}