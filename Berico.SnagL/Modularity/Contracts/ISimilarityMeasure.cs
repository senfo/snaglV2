﻿//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

namespace Berico.SnagL.Infrastructure.Modularity.Contracts
{
    /// <summary>
    /// Represents some methodolgy for measuring how similar two objects are
    /// </summary>
    public interface ISimilarityMeasure
    {
        //TODO:  CAN THIS BE MADE GENERIC

        /// <summary>
        /// Gets the name of this Similarity Measure
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a description of this similarity measure
        /// </summary>
        string Description { get; }
        
        /// <summary>
        /// Calculates how similar the two provided values are for the
        /// provided attribute
        /// </summary>
        /// <param name="attribute">The name of the attribute that represents the values</param>
        /// <param name="value1">The first value for similarity comparison</param>
        /// <param name="value2">The second value for similarity comparison</param>
        /// <returns>a number indicating how similar the two values are in relation
        /// to all values for the given attribute</returns>
        double? MeasureSimilarity(string attribute, string value1, string value2);

        /// <summary>
        /// Calculates the distance between the two provided values
        /// </summary>
        /// <param name="value1">The first value for similarity comparison</param>
        /// <param name="value2">The second value for similarity comparison</param>
        /// <returns>a number indicating how far apart the two values are</returns>
        double? CalculateDistance(string value1, string value2);
    }
}