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
using System.Collections.Generic;
using Berico.Common;

namespace Berico.SnagL.Infrastructure.Similarity
{
    [PartMetadata("ID", "SimilarityMeasureExtension"), Export(typeof(ISimilarityMeasure))]
    public class DoubleMetaphoneSimilarityMeasure : SimilarityMeasureBase
    {
        private const SemanticType ASSIGNED_SEMANTIC_TYPES = SemanticType.GeneralString | SemanticType.Name | SemanticType.EmailAddress;

        /// <summary>
        /// Creates a new instance of the DoubleMetaphoneSimilarityMeasure class
        /// </summary>
        public DoubleMetaphoneSimilarityMeasure() : base("Double Metaphone (Sounds-Like)", ASSIGNED_SEMANTIC_TYPES, "Measures attribute similarity based on how similar the values sound.") { }

        /// <summary>
        /// Calculates distance using a double metaphone (soundex) to compare
        /// each word in a string.
        /// </summary>
        /// <param name="sourceValue">The source value</param>
        /// <param name="targetValue">The target value</param>
        /// <returns></returns>
        public override double? CalculateDistance(string sourceValue, string targetValue)
        {
            HashSet<string> targetKeys = GetPhoneticKeys(targetValue);

            string primary = string.Empty;
            string alternate = string.Empty;
            double matches = 0;
            int totalWordCount = sourceValue.SplitWords().Length + targetValue.SplitWords().Length;

            // Loop over all the words in the provided string
            foreach (string word in sourceValue.SplitWords())
            {
                // Clear the key variables
                primary = string.Empty;
                alternate = string.Empty;

                DoubleMetaphone.doubleMetaphone(word, ref primary, ref alternate);

                if (!string.IsNullOrEmpty(primary) && targetKeys.Contains(primary))
                    matches++;
                else if (!string.IsNullOrEmpty(alternate))
                    if (targetKeys.Contains(alternate))
                        matches++;
            }

            return 1 - (2 * matches / totalWordCount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private HashSet<string> GetPhoneticKeys(string value)
        {
            string primary = string.Empty;
            string alternate = string.Empty;

            HashSet<string> values = new HashSet<string>();

            if (string.IsNullOrEmpty(value))
                return values;

            // Loop over all the words in the provided string
            foreach (string word in value.SplitWords())
            {
                // Clear the key variables
                primary = string.Empty;
                alternate = string.Empty;

                DoubleMetaphone.doubleMetaphone(word, ref primary, ref alternate);

                if (!string.IsNullOrEmpty(primary))
                    values.Add(primary);

                if (!string.IsNullOrEmpty(alternate))
                    values.Add(alternate);
            }

            return values;
        }

    }
}