﻿//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using System;
using Berico.Common;
using Berico.SnagL.Infrastructure.Clustering;
using Berico.SnagL.Infrastructure.Data.Attributes;

namespace Berico.SnagL.Infrastructure.Modularity.Contracts
{
    /// <summary>
    /// Base class for all Similarity Measures.  A Similarity Measure is a
    /// class that is capable of measuring how similar a set of values are.
    /// This class is abstract and must be inherited from and the CalculateDistance
    /// method must be implemented.
    /// </summary>
    public abstract class SimilarityMeasureBase : ISimilarityMeasure
    {
        private readonly string _name;
        private readonly string _description = "No description provided";
        private readonly SemanticType _semanticTypes = SemanticType.Unknown;

        /// <summary>
        /// Creates a new instance of the SimilarityMeasureBase class
        /// </summary>
        /// <param name="name">Specifies the name</param>
        /// <param name="semanticTypes">Specifies the <see cref="SemanticType"/></param>
        /// <param name="description">Specifies the description</param>
        protected SimilarityMeasureBase(string name, SemanticType semanticTypes, string description)
        {
            // Validate the provided argument
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name", "No valid name was provided for this similarity measure");

            // Set the similarity measure's name and description
            _name = name;
            _description = description;

            // Set the assigned semantic types for this similarity measure
            _semanticTypes = semanticTypes;
        }

        /// <summary>
        /// Gets all of the SemanticTypes associated with this measure
        /// </summary>
        public SemanticType SemanticTypes
        {
            get { return _semanticTypes; }
        }

        #region ISimilarityMeasure Members
                
        /// <summary>
        /// Gets the name of this Similarity Measure
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the description of this Similarity Measure
        /// </summary>
        public string Description
        {
            get { return _description; }
        }

        /// <summary>
        /// Calculates how similar the two provided values are for the
        /// provided attribute
        /// </summary>
        /// <param name="attribute">The attribute to compare</param>
        /// <param name="value1">The first value for similarity comparison</param>
        /// <param name="value2">The second value for similarity comparison</param>
        /// <returns>typically 1 for a match and 0 for no match at all; any
        /// other value indicates some similarity</returns>
        public virtual double? MeasureSimilarity(string attribute, string value1, string value2)
        {
            // Calculate the distance between the two values
            double? distance = CalculateDistance(value1, value2);

            // Ensure that we were able to calculate the distance
            if (distance != null)
            {
                // Get the maximum distance for all values (for all nodes) for
                // the provided attribute
                // ******** LINEAR SCALING ***************
                //double maxDistance = GlobalAttributeCollection.Instance.GetMaxDistance(attribute, this);
                    
                //if (maxDistance == 0)
                //    return 1;

                //return 1 - (distance / maxDistance);
                double mean = AttributeSimilarityManager.Instance.GetDistanceMean(attribute, this);
                double sd = AttributeSimilarityManager.Instance.GetDistanceStandardDeviation(attribute, this);

                //System.Diagnostics.Debug.WriteLine("     Distance:            {0}", distance);
                //System.Diagnostics.Debug.WriteLine("     Mean:                {0}", mean);
                //System.Diagnostics.Debug.WriteLine("     Standard Deviation:  {0}", sd);

                // If the standard deviation is 0, we don't want to cluster on this attribute
                // because it indicates that all the values are the same (no variance)
                if (sd == 0)
                    return 0;

                return 1 - MathUtils.NormalCdf(distance.Value, mean, sd);
            }

            return null;
        }

        /// <summary>
        /// Calculates the distance between the two provided values
        /// </summary>
        /// <param name="value1">The first value for similarity comparison</param>
        /// <param name="value2">The second value for similarity comparison</param>
        /// <returns>a number indicating how far apart the two values are</returns>
        public abstract double? CalculateDistance(string value1, string value2);

        #endregion
    }
}