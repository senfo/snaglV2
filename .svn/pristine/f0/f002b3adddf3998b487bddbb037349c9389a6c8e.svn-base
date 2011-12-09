//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using System.ComponentModel.Composition;
using Berico.SnagL.Infrastructure.Data.Attributes;
using Berico.SnagL.Infrastructure.Modularity.Contracts;

namespace Berico.SnagL.Infrastructure.Similarity
{
    /// <summary>
    /// Represents an RegexStringSimilarityMeasure that measures string
    /// similarity based on the user portion of an email address.
    /// </summary>
    [PartMetadata("ID", "SimilarityMeasureExtension"), Export(typeof(ISimilarityMeasure))]
    public class EmailUserSimilarityMeasure : RegexStringSimilarityMeasure
    {
        private const SemanticType ASSIGNED_SEMANTIC_TYPES = SemanticType.GeneralString | SemanticType.EmailAddress;

        /// <summary>
        /// Creates a new instance of the EmailUserSimilarityMeasure class
        /// using the provided name and regualr expression.
        /// </summary>
        public EmailUserSimilarityMeasure() : base("Email User", "([^@]+)@.+", ASSIGNED_SEMANTIC_TYPES, "Measures the similarity of email addresses based on how the user portion's (left of the '@') compare.") { }
    }
}