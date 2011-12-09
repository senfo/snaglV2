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
    /// 
    /// </summary>
    [PartMetadata("ID", "SimilarityMeasureExtension"), Export(typeof(ISimilarityMeasure))]
    public class LevenshteinDistanceStringSimilarityMeasure : SimilarityMeasureBase
    {
        private const SemanticType ASSIGNED_SEMANTIC_TYPES = SemanticType.GeneralString | SemanticType.Date | SemanticType.Name | SemanticType.EmailAddress | SemanticType.PhoneNumber | SemanticType.Coordinates;

        /// <summary>
        /// Creates a new instance of the LevenshteinDistanceStringSimilarityMeasure class
        /// </summary>
        public LevenshteinDistanceStringSimilarityMeasure() : base("Edit Distance (Levenshtein)", ASSIGNED_SEMANTIC_TYPES, "Measures attribute similarity based on how many edits it would take to convert one value into the other.") { }

        /// <summary>
        /// Calculates distance based on the Levenshtein distance measurement.  This determines
        /// the number of edits required to transform one string into the other.
        /// </summary>
        /// <param name="sourceValue">The source value</param>
        /// <param name="targetValue">The target value</param>
        /// <returns>a value representing the edit distance between the two provided values</returns>
        public override double? CalculateDistance(string sourceValue, string targetValue)
        {
            
            // Validate parameters
            if (sourceValue == null)
                return null;

            if (targetValue == null)
                return null;

            // Get the length of both strings.  If either is 0, return the
            // length of the other since that represents the number of edits
            // (insertions) that would be required.
            int n = sourceValue.Length;
            int m = targetValue.Length;

            if (n == 0) return m;
            if (m == 0) return n;

            // Rather thatn maintain an entire matric (which would require O(n*m) space),
            // just store the current RowDefinition and the next RowDefinition, each of
            // which has a length m+1, so just O(m) space
            int currentRow = 0;
            int nextRow = 1;
            int[][] rows = new int[][] { new int[m + 1], new int[m + 1] };

            // Initialize the current row
            for (int j = 0; j <= m; ++j)
                rows[currentRow][j] = j;

            // For each virtual row (since we only have physical storage for two)
            for (int i = 1; i <= n; ++i)
            {
                // Fill in the values in the row
                rows[nextRow][0] = i;

                for (int j = 1; j <= m; ++j)
                {
                    int dist1 = rows[currentRow][j] + 1;
                    int dist2 = rows[nextRow][j - 1] + 1;
                    int dist3 = rows[currentRow][j - 1] + (sourceValue[i - 1].Equals(targetValue[j - 1]) ? 0 : 1);

                    rows[nextRow][j] = Math.Min(dist1, Math.Min(dist2, dist3));
                }

                // Swap the current and next rows
                if (currentRow == 0)
                {
                    currentRow = 1;
                    nextRow = 0;
                }
                else
                {
                    currentRow = 0;
                    nextRow = 1;
                }
            }

            // Return the computed edit distance
            return rows[currentRow][m]; 

        }
    }
}