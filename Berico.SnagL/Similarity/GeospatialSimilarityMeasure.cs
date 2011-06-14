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
using Berico.Common;
using Berico.SnagL.Infrastructure.Data.Attributes;
using Berico.SnagL.Infrastructure.Modularity.Contracts;

namespace Berico.SnagL.Infrastructure.Similarity
{

    /// <summary>
    /// Measures the similarity between two geospatil (such as longitude
    /// and latitude) values
    /// </summary>
    [PartMetadata("ID", "SimilarityMeasureExtension"), Export(typeof(ISimilarityMeasure))]    
    public class GeospatialSimilarityMeasure : SimilarityMeasureBase
    {
        private const SemanticType ASSIGNED_SEMANTIC_TYPES = SemanticType.GeneralString | SemanticType.Coordinates;

        /// <summary>
        /// Creates a new instance of the GeospatialSimilarityMeasure class
        /// </summary>
        public GeospatialSimilarityMeasure() : base("Geo-Spatial", ASSIGNED_SEMANTIC_TYPES, "Measures attribute similarity based on how similar the values spatially.") { }

        public override double? CalculateDistance(string sourceValue, string targetValue)
        {
            GeoCoordinate sourceCoordinates = null;
            GeoCoordinate targetCoordinates = null;

            // Attempt to parse the source value into coordinates
            if (!GeoCoordinate.TryParse(sourceValue, out sourceCoordinates))
                return null;

            // Attempt to parse the target value into coordinates
            if (!GeoCoordinate.TryParse(targetValue, out targetCoordinates))
                return null;

            return sourceCoordinates.GetDistanceTo(targetCoordinates);
        }

    }
}