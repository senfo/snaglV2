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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using Berico.Common;
using Berico.SnagL.Infrastructure.Data.Attributes;
using Berico.SnagL.Infrastructure.Modularity;
using Berico.SnagL.Infrastructure.Modularity.Contracts;
using Berico.SnagL.Infrastructure.Similarity;
using Berico.SnagL.Model;

namespace Berico.SnagL.Infrastructure.Clustering
{
    /// <summary>
    /// Responsible for managing similarity measuring (and their
    /// execution and caching) across all attributes in the graph.
    /// 
    /// Since this class is a singleton, you must use the Instance
    /// property to retrieve an instance of the class.
    /// </summary>
    public class AttributeSimilarityManager : IPartImportsSatisfiedNotification
    {
        private static AttributeSimilarityManager instance;
        private static object syncRoot = new object();
        private ObservableCollection<string> managedAttributes = new ObservableCollection<string>();
        private Dictionary<string, int> attributeToMeasure = new Dictionary<string, int>();
        private Dictionary<string, double> attributeWeights = new Dictionary<string, double>();
        private Dictionary<Tuple<string, string>, double> distancesCache = null;
        private Dictionary<Tuple<string, string>, double> meanCache = new Dictionary<Tuple<string, string>, double>();
        private Dictionary<Tuple<string, string>, double> sdCache = new Dictionary<Tuple<string, string>, double>();
        private List<string> exactMatchAttributes = new List<string>();
        private string scope = string.Empty;
        List<AttributeSimilarityDescriptor> similarityDescriptors;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_scope"></param>
        private AttributeSimilarityManager(string _scope) { this.scope = _scope; }

        [ImportMany(typeof(ISimilarityMeasure), AllowRecomposition = true)]
        public List<ISimilarityMeasure> SimilarityMeasures { get; set; }

        /// <summary>
        /// Indicates that the ManagedAttributes collection has changed
        /// </summary>
        public event EventHandler<NotifyCollectionChangedEventArgs> ManagedAttributesChanged;

        /// <summary>
        /// Gets the instance of the AttributeSimilarityManager class
        /// </summary>
        public static AttributeSimilarityManager Instance
        {
            get
            {
                // Check if the instance is null
                if (instance == null)
                {
                    // If we are here then the Instance property was called
                    // before the InitialSetup method.
                    throw new InvalidOperationException("The AttributeSimilarityManager class must be initialized (using the InitialSetup method) before the Instance property can be used.");
                }

                return instance;
            }
        }

        /// <summary>
        /// Gets a reference to the similarity descriptors used during clustering
        /// </summary>
        public IEnumerable<AttributeSimilarityDescriptor> SimilarityDescriptors
        {
            get
            {
                return similarityDescriptors;
            }
        }

        /// <summary>
        /// Performs the initial setup of the AttributeSimilarityManager class.  This method
        /// must be called once before the Instance property can be used.
        /// </summary>
        /// <param name="_scope">The scope of the data that will be used by this class</param>
        public static void InitialSetup(string _scope)
        {
            // Validate the parameter
            if (string.IsNullOrEmpty(_scope))
                throw new System.ArgumentNullException("_scope", "An invalid scope was provided");

            lock (syncRoot)
            {
                // Ensure that this can only be called once
                if (instance == null)
                {
                    instance = new AttributeSimilarityManager(_scope);
                    instance.Initialize();
                }
            }
        }

        //TODO: POTENTIALLY USE GLOBAL LIST INSTEAD

        /// <summary>
        /// Gets an enumerable list of attributes managed by
        /// this class
        /// </summary>
        public IEnumerable<string> ManagedAttributes
        {
            get { return this.managedAttributes; }
        }

        /// <summary>
        /// Retrieves the weighted value for the provided attribute.  A weighted value
        /// indicates that attributes importance and is used during clustering.  The value
        /// returned is scaled (which causes all weights to add up to 1).
        /// </summary>
        /// <param name="attributeName">The attribute to retrieve the weight for</param>
        /// <returns>the scaled weight value for the specified attribute; otherwise null</returns>
        public double GetAttributeWeight(string attributeName)
        {
            // Validate the provided attributename
            if (string.IsNullOrEmpty(attributeName))
                throw new ArgumentNullException("AttributeName", "No valid attribute name was provided");

            // Check if a weight for the attribute exists
            if (this.attributeWeights.ContainsKey(attributeName))
            {
                // Return the scaled weight
                return this.attributeWeights[attributeName] / this.attributeWeights.Sum(attributeWeight => attributeWeight.Value);
            }
            else
                return 0;

        }

        /// <summary>
        /// Clear the current set of similarity descriptors
        /// </summary>
        public void ClearDescriptors()
        {
            this.similarityDescriptors.Clear();
        }

        /// <summary>
        /// Sets the list of descriptors to be used
        /// </summary>
        /// <param name="descriptors">The list of descriptors to use</param>
        public void SetDescriptors(List<AttributeSimilarityDescriptor> descriptors)
        {
            this.similarityDescriptors = descriptors;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetAttributesWithWeights()
        {
            return new List<string>(this.attributeWeights.Keys);
        }

        /// <summary>
        /// Assigns a weighted valued to the provided attribute.  A weighted value
        /// indicates the attributes importance and is used during clustering.
        /// </summary>
        /// <param name="attributeName">The attribute to assign a weight value too</param>
        /// <param name="weightValue">The weight value for the specified attribute</param>
        public void SetAttributeWeight(string attributeName, double weightValue)
        {
            // Validate the provided attributename
            if (string.IsNullOrEmpty(attributeName))
                throw new ArgumentNullException("AttributeName", "No valid attribute name was provided");

            // If the weight is 0, then the item should be removed
            if (weightValue == 0)
            {
                // Make sure that the attribute name provided exists
                if (this.attributeWeights.ContainsKey(attributeName))
                {
                    // Remove the item (since 0 is turning of the weight)
                    this.attributeWeights.Remove(attributeName);
                }
            }
            else
            {
                // Store the new weight value
                this.attributeWeights[attributeName] = weightValue;
            }
        }

        /// <summary>
        /// Gets whether there are any weights currently assigned
        /// </summary>
        public bool HasAssignedWeights
        {
            get
            {
                return (this.attributeWeights.Count > 0);
            }
        }

        /// <summary>
        /// Initialize an instance of the AttributeSimilarityManager class
        /// </summary>
        private void Initialize()
        {
            ExtensionManager.ComposeParts(this);

            // Add the attributes currently in the Global Attribute
            // Collection to our list.
            foreach (Data.Attributes.Attribute attribute in GlobalAttributeCollection.GetInstance(this.scope).GetAttributes())
            {
                if (attribute.Visible)
                    this.managedAttributes.Add(attribute.Name);
            }

            this.managedAttributes.CollectionChanged += new NotifyCollectionChangedEventHandler(managedAttributes_CollectionChanged);
            this.distancesCache = new Dictionary<Tuple<string, string>, double>();

            // Setup a listener for the AttributeListUpdated event
            GlobalAttributeCollection.GetInstance(this.scope).AttributeListUpdated += new EventHandler<AttributeEventArgs>(GlobalAttributeCollection_AttributeListUpdated);
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributeName"></param>
        public void AddExactMatchAttribute(string attributeName)
        {
            if (!exactMatchAttributes.Contains(attributeName))
                exactMatchAttributes.Add(attributeName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributeName"></param>
        public void RemoveExactMatchAttribute(string attributeName)
        {
            if (exactMatchAttributes.Contains(attributeName))
                exactMatchAttributes.Remove(attributeName);
        }

        /// <summary>
        /// Handles the CollectionChanged event for the collection of managed attributes
        /// </summary>
        /// <param name="sender">The collection that fired the event</param>
        /// <param name="e">The arguments for the event</param>
        private void managedAttributes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnManagedAttributesChanged(e);
        }

        /// <summary>
        /// Handles the AttributeListUpdated event, which is fired anytime
        /// the GlobalAttributeCollection changes.
        /// </summary>
        /// <param name="sender">The object that fired the event</param>
        /// <param name="e">The attributes associated with the event</param>
        private void GlobalAttributeCollection_AttributeListUpdated(object sender, AttributeEventArgs e)
        {
            // We are only concerned about Visible attributes
            if (e.Attribute != null && !e.Attribute.Visible)
                return;

            // Ensure changes to the global attribute collection 
            // are reflected by the internal attribute list
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                this.managedAttributes.Add(e.Attribute.Name);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                this.managedAttributes.Remove(e.Attribute.Name);
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                // Not something we need to handle here
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                // The global collection was cleared so we need to clear
                // the local managed list
                this.managedAttributes.Clear();
            }
            else
            {
                // We shouldn't get here unless the action was incorrectly
                // specified for the event
                throw new ArgumentException("No valid action was specified for the AttributeListUpdated event", "Action");
            }

            // If the global collection has changed in anyway, then
            // we need to invalidate the similarity caches for the
            // affected attribute
            if (e.Attribute != null)
                InvalidateCache(e.Attribute.Name);
            else
                InvalidateCache();

        }

        /// <summary>
        /// Returns the similarity measure for the specified Attribute
        /// </summary>
        /// <param name="attribute">The Attribute</param>
        /// <returns>the default similarity measure for the specified attribute</returns>
        public ISimilarityMeasure GetDefaultSimilarityMeasure(Data.Attributes.Attribute attribute)
        {
            return GetDefaultSimilarityMeasure(attribute.Name);
        }

        /// <summary>
        /// Returns an instance of the default SimilarityMeasure for
        /// the provided attribute.  This method does not check the
        /// cache because the cache contains the currently associated
        /// Similarity Measure, which is not neccessairly the default.
        /// </summary>
        /// <param name="attributeName">The name of the targetAttribute</param>
        /// <returns>the default ISimilarityMeasure that should be used for the provided attribute</returns>
        public ISimilarityMeasure GetDefaultSimilarityMeasure(string attributeName)
        {

            // Validate parameter
            if (string.IsNullOrEmpty(attributeName))
                throw new ArgumentNullException("AttributeName", "No valid attribute name was provided");

            ISimilarityMeasure defaultMeasure = null;

            // Attempt to get the default similairy measure using the assigned
            // preffered similarity measure
            defaultMeasure = GetPrefferedSimilarityMeasure(attributeName);

            // If we have a good measure, we can return it
            if (defaultMeasure != null)
                return defaultMeasure;

            // No preffered similarity measure is set so we will determine
            // the default by analyzing the type of data stored in the attribute
            string firstValue = GlobalAttributeCollection.GetInstance(this.scope).GetAttributeValues(attributeName).FirstOrDefault();
            string secondValue = GlobalAttributeCollection.GetInstance(this.scope).GetAttributeValues(attributeName).LastOrDefault();

            if (string.IsNullOrEmpty(firstValue) || string.IsNullOrEmpty(secondValue))
                return GetSimilarityMeasureInstance(typeof(LevenshteinDistanceStringSimilarityMeasure).FullName);

            // Determine the default by analyzing data stored in the specified
            // attribute
            defaultMeasure = DetermineSimilarityMeasure(attributeName, firstValue, secondValue);

            return defaultMeasure;
        }

        /// <summary>
        /// Returns the preferred similarity measure using the provided attribute
        /// name
        /// </summary>
        /// <param name="attributeName">The name of the target attribute</param>
        /// <returns>an instance of the Preffered Similarity Measure; otherwise null</returns>
        private ISimilarityMeasure GetPrefferedSimilarityMeasure(string attributeName)
        {
            // Retrieve the Attribute instance from the GlobalAttributeCollection
            Data.Attributes.Attribute attribute = GlobalAttributeCollection.GetInstance(this.scope).GetAttribute(attributeName);

            if (attribute == null)
                return null;
            else
                return GetPrefferedSimilarityMeasure(attribute);
        }

        /// <summary>
        /// Returns the preferred similairy using the provided attribute
        /// </summary>
        /// <param name="attribute">The target attribute</param>
        /// <returns>an instance of the preffered Similarity Measure; otherwise null</returns>
        private ISimilarityMeasure GetPrefferedSimilarityMeasure(Data.Attributes.Attribute attribute)
        {
            // Check if a preffered similarity measure has been set for
            // the given attribute
            if (string.IsNullOrEmpty(attribute.PreferredSimilarityMeasure))
                return null;

            // Return the MEF maintained instant of the prefered similarity measure
            return GetSimilarityMeasureInstance(attribute.PreferredSimilarityMeasure);
        }

        /// <summary>
        /// Returns the similarity between the two specified nodes.  The similarity
        /// calculation is based on how similar all the attribute values are between
        /// both nodes.
        /// </summary>
        /// <param name="sourceNode">The source Node</param>
        /// <param name="targetNode">The target Node</param>
        /// <returns>the calculated similarity for the provided nodes</returns>
        public double ComputeNodeSimilarity(Node sourceNode, Node targetNode)
        {
            return ComputeNodeSimilarity(sourceNode, targetNode, false);
        }

        /// <summary>
        /// Returns the similarity between the two specified nodes.  The similarity
        /// calculation is based on how similar all the attribute values are between
        /// both nodes.
        /// </summary>
        /// <param name="sourceNode">The source Node</param>
        /// <param name="targetNode">The target Node</param>
        /// <param name="useExactMatchOverrideList">Indicates if the exact match override list should be used</param>/// 
        /// <returns>the calculated similarity for the provided nodes</returns>
        public double ComputeNodeSimilarity(Node sourceNode, Node targetNode, bool useExactMatchOverrideList)
        {
            // Validate the paramters
            if (sourceNode == null)
                throw new ArgumentNullException("SourceNode", "No valid source node was provided");
            if (targetNode == null)
                throw new ArgumentNullException("TargetNode", "No valid target node was provided");

            double totalSimilarity = 0;

            foreach (AttributeSimilarityDescriptor descriptor in this.similarityDescriptors)
            {
                Model.Attributes.AttributeValue attributeValue = null;
                string sourceValue = string.Empty;
                string targetValue = string.Empty;

                // Get the source value
                attributeValue = sourceNode.Attributes.GetAttributeValue(descriptor.AttributeName);
                if (attributeValue != null)
                    sourceValue = attributeValue.Value;

                // Get the target value
                attributeValue = targetNode.Attributes.GetAttributeValue(descriptor.AttributeName);
                if (attributeValue != null)
                    targetValue = attributeValue.Value;

                // Make sure we have a value for both attributes being compared
                if (!string.IsNullOrEmpty(sourceValue) && !string.IsNullOrEmpty(targetValue))
                {
                    // Compute the similarity between the two values, for this attribute
                    double? similarity = ComputeAttributeSimilarity(descriptor.AttributeName, sourceValue, targetValue, descriptor.SimilarityMeasure);

                    // Save off the maximum similarity
                    if (similarity != null)
                    {
                        totalSimilarity += descriptor.Weight * similarity.Value;
                    }
                }
            }

            return totalSimilarity;
        }

        /// <summary>
        /// Returns the similarity of the two specified values for the given
        /// attribute name.  This method uses various methods to determine the
        /// most appropriate Similarity Measure that should be used.
        /// </summary>
        /// <param name="attributeName">The name of the attribute to measure similarity for</param>
        /// <param name="sourceValue">The source value for similarity measurement</param>
        /// <param name="targetValue">The target value for similarity measurement</param>
        /// <returns>the calculated similarity for the given attribute</returns>
        public double? ComputeAttributeSimilarity(string attributeName, string sourceValue, string targetValue)
        {
            ISimilarityMeasure measure = null;

            // Check if we have already identified an appropriate 
            // Similarity Instance
            if (attributeToMeasure.ContainsKey(attributeName))
            {
                // Get the pre-determined similarity measure
                measure = SimilarityMeasures[attributeToMeasure[attributeName]];
            }
            else
            {
                // Try and get the perferred Similarity Measure
                measure = GetPrefferedSimilarityMeasure(attributeName);

                // If we couldn't get a preferred Similarity Measure, try and
                // determine the most appropriate one to use
                if (measure == null)
                {
                    measure = DetermineSimilarityMeasure(attributeName, sourceValue, targetValue);
                }

                // We are going to cache the determined Similarity Measure
                this.attributeToMeasure.Add(attributeName, SimilarityMeasures.IndexOf(measure));
            }

            // Call the main method to get the similarity value
            return ComputeAttributeSimilarity(attributeName, sourceValue, targetValue, measure);
        }

        /// <summary>
        /// Returns the similarity of the two specified values for the given
        /// attribute name.  This method uses various methods to determine the
        /// most appropriate Similarity Measure that should be used.
        /// </summary>
        /// <param name="attributeName">The name of the attribute to measure similarity for</param>
        /// <param name="sourceValue">The source value for similarity measurement</param>
        /// <param name="targetValue">The target value for similarity measurement</param>
        /// <param name="measure"></param>
        /// <returns>the calculated similarity for the given attribute</returns>
        public double? ComputeAttributeSimilarity(string attributeName, string sourceValue, string targetValue, ISimilarityMeasure measure)
        {
            // Execute the similarity measure and return the results
            return measure.MeasureSimilarity(attributeName, sourceValue, targetValue);
        }

        /// <summary>
        /// Returns a collection of all Similarity Measures that are valid for
        /// the specified attribute
        /// </summary>
        /// <param name="attributeName">The attribute name to retrieve valid Similarity
        /// Measures for</param>
        /// <returns>a collection of all Similarity Measures that are valid
        /// for the specified attribute</returns>
        public ICollection<ISimilarityMeasure> GetValidSimilarityMeasures(string attributeName)
        {
            // Validate the parameter
            if (string.IsNullOrEmpty(attributeName))
                throw new ArgumentNullException("AttributeName", "No valid attribute name was provided");

            List<ISimilarityMeasure> validMeasures = new List<ISimilarityMeasure>();

            // Retrieve the Attribute instance from the GlobalAttributeCollection
            Data.Attributes.Attribute attribute = GlobalAttributeCollection.GetInstance(this.scope).GetAttribute(attributeName);

            // Make sure that the SimilairyMeasures collection has values
            //if (this.SimilarityMeasures.IsValueCreated)
            //{
                // Loop over the MEF maintained similarity collection
                foreach (ISimilarityMeasure currentMeasure in SimilarityMeasures)
                {
                    SimilarityMeasureBase measure = currentMeasure as SimilarityMeasureBase;

                    // Check if current measure is valid based on allowed
                    // semantic types
                    if (measure.SemanticTypes.HasFlag(attribute.SemanticType))
                        validMeasures.Add(measure);
                    else
                    {
                        // If we are here, appropriate semantic types not matching so 
                        // something was setup wrong initially

                        //TODO:  HANDLE THIS
                    }
                }
            //}

            return validMeasures;
        }

        /// <summary>
        /// Returns the mean for all distances (over all values) for the given
        /// attributes and using the specified similarity measure. This method
        /// attempts to use the cached value.  If one doesn't exist, it is calculated.
        /// </summary>
        /// <param name="attributeName">The attribute to compute the standard
        /// deviation for</param>
        /// <param name="measure">The similarity measure to be used</param>
        /// <returns>the mean for all distances over all attribute
        /// values for the given attribute and similarity measure</returns>
        public double GetDistanceMean(string attributeName, ISimilarityMeasure measure)
        {
            Tuple<string, string> tuple = Tuple.Create(attributeName, measure.ToString());
            
            // Ensure that the mean distance value isn't already computed
            if (!this.meanCache.ContainsKey(tuple))
            {
                // Calculate the mean distance
                this.meanCache[tuple] = CalculateDistanceMean(attributeName, measure);
            }

            // Returns the cached mean value
            return this.meanCache[tuple];
        }

        /// <summary>
        /// Returns the standard deviation for all distances (over all values) for the
        /// given attribute and using the specified similarity measure.  This method
        /// attempts to use the cached value.  If one doesn't exist, it is calculated.
        /// </summary>
        /// <param name="attributeName">The attribute to compute the standard
        /// deviation for</param>
        /// <param name="measure">The similarity measure to be used</param>
        /// <returns>the standard deviation for all distances over all attribute
        /// values for the given attribute and similarity measure</returns>
        public double GetDistanceStandardDeviation(string attributeName, ISimilarityMeasure measure)
        {
            Tuple<string, string> tuple = Tuple.Create(attributeName, measure.ToString());
            
            // Ensure that the standard deviation distance value isn't already computed
            if (!this.sdCache.ContainsKey(tuple))
            {
                // Calculate the distance standard deviation
                this.sdCache[tuple] = CalculateDistanceSD(attributeName, measure);
            }

            // Returns the cached standard deviation value
            return this.sdCache[tuple];
        }

        /// <summary>
        /// Calculates the mean for all distances (over all values) for the given
        /// attributes and using the specified similarity measure
        /// </summary>
        /// <param name="attributeName">The attribute to compute the standard
        /// deviation for</param>
        /// <param name="measure">The similarity measure to be used</param>
        /// <returns>the mean for all distances over all attribute
        /// values for the given attribute and similarity measure</returns>
        private double CalculateDistanceMean(string attributeName, ISimilarityMeasure measure)
        {
            // Get all the distances
            List<Tuple<double, int>> distanceValues = CalculateDistances(attributeName, measure);

            if (distanceValues == null || distanceValues.Count == 0)
                return 0;

            // Calculate and return the mean value
            return distanceValues.Mean();
        }

        /// <summary>
        /// Calulates the standard deviation for all distances (over all values) for the
        /// given attribute and using the specified similarity measure
        /// </summary>
        /// <param name="attributeName">The attribute to compute the standard
        /// deviation for</param>
        /// <param name="measure">The similarity measure to be used</param>
        /// <returns>the standard deviation for all distances over all attribute
        /// values for the given attribute and similarity measure</returns>
        private double CalculateDistanceSD(string attributeName, ISimilarityMeasure measure)
        {
            // Get all the distances
            List<Tuple<double, int>> distanceValues = CalculateDistances(attributeName, measure);

            if (distanceValues == null || distanceValues.Count ==0)
                return 0;

            // To compute SD, we need the mean
            double mean = GetDistanceMean(attributeName, measure);

            // Calculate and return the standard deviation
            return distanceValues.StandardDeviation(mean);
        }

        /// <summary>
        /// Returns a list of tuples that contain the calculated distances
        /// and the frequency of those distances
        /// </summary>
        /// <param name="attributeName">The name of the attribute that
        /// distances are being calculated for</param>
        /// <param name="measure">The similarity measure to be used</param>
        /// <returns>a collection of distances and the number of times
        /// that those distances occur</returns>
        private List<Tuple<double, int>> CalculateDistances(string attributeName, ISimilarityMeasure measure)
        {
            if (!GlobalAttributeCollection.GetInstance(this.scope).ContainsAttribute(attributeName))
                return null;

            List<Tuple<double, int>> distances = new List<Tuple<double, int>>();
            double frequencyTotal = 0;
            int nodeCount = 0;

            // Get the values for the attribute
            List<string> attributeValues = new List<string>(GlobalAttributeCollection.GetInstance(this.scope).GetAttributeValues(attributeName));

            // Loop over all the attribute values
            for (int i = 0; i <= attributeValues.Count - 1; i++)
            {
                // Get the frequency at which the source attribute value occurrs
                int sourceFrequency = GlobalAttributeCollection.GetInstance(this.scope).GetFrequency(attributeName, attributeValues[i]);

                nodeCount += sourceFrequency;

                // Compare the current attribute value to all other
                // attribute values
                for (int j = 0; j < i; j++)
                {
                    // Compute the distance for the two values
                    double? distance = measure.CalculateDistance(attributeValues[i], attributeValues[j]);

                    if (distance != null)
                    {
                        // Get the frequency at which the target attribute value occurrs
                        int targetFrequency = GlobalAttributeCollection.GetInstance(this.scope).GetFrequency(attributeName, attributeValues[j]);

                        distances.Add(Tuple.Create<double, int>(distance.Value, sourceFrequency * targetFrequency));

                        // Keep a running total of the frequencies
                        frequencyTotal += sourceFrequency * targetFrequency;
                     }
                }
            }

            // Since we only loop over unique attribute values we never make
            // the comparisons against nodes where the values would be the 
            // same.  We need to determine if this case has occurred and insert
            // the appropriate number of zero distance items.

            // Use binomial function to determine the number of possible combinations
            //double combinations = (MathUtils.LogFactorial(nodeCount) / (2 * (MathUtils.LogFactorial(nodeCount - 2))));
            double combinations = Math.Exp(MathUtils.LogFactorial(nodeCount) - MathUtils.LogFactorial(2) - MathUtils.LogFactorial(nodeCount - 2));

            // Add in all the zero distance items that we need
            for (int i = 1; i <= combinations - frequencyTotal; i++)
            {
                distances.Add(Tuple.Create<double, int>(0, 1));
            }

            //foreach (Tuple<double, int> distanceCount in distances)
            //{
            //    System.Diagnostics.Debug.WriteLine("[{0},{1}]", distanceCount.Item1, distanceCount.Item2);
            //}

            return distances;
        }

        /// <summary>
        /// Returns the appropriate similarity measure for the provided attributes and values
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="sourceValue">The source value</param>
        /// <param name="targetValue">The target value</param>
        /// <returns>the appropriate similarity measure for the given attribute and values</returns>
        private ISimilarityMeasure DetermineSimilarityMeasure(string attributeName, string sourceValue, String targetValue)
        {
            ISimilarityMeasure measure = null;

            // Determine if we have already done the work of determing
            // the default similarity measure to be used
            if (attributeToMeasure.ContainsKey(attributeName))
            {
                // If the key was found, then we have already determined
                // the default similarity measure and can retrieve and
                // return that
                return SimilarityMeasures[attributeToMeasure[attributeName]];
            }
            else
            {
                // Determine and retrieve the instance of the appropriate
                // Similarity Measure for the given parameters
                if (IsNumber(sourceValue, targetValue))
                    measure = GetSimilarityMeasureInstance(typeof(NumericSimilarityMeasure).FullName);
                else if (IsDate(sourceValue,targetValue))
                    measure = GetSimilarityMeasureInstance(typeof(DateTimeSimilarityMeasure).FullName);
                else if (GeoCoordinate.IsValid(sourceValue) && GeoCoordinate.IsValid(targetValue))
                    measure = GetSimilarityMeasureInstance(typeof(GeospatialSimilarityMeasure).FullName);
                else
                    measure = GetSimilarityMeasureInstance(typeof(LevenshteinDistanceStringSimilarityMeasure).FullName);
            }

            //if (measure != null)
            //{
                // Cache the attribute name and corresponding index to similarity measure instance
                //this.attributeToMeasure.Add(attributeName, SimilarityMeasures.Value.IndexOf(measure));
            //}

            return measure;
        }

        /// <summary>
        /// Returns an ISimilarityMeasure from the MEF maintained collection based
        /// on the specified FQN
        /// </summary>
        /// <param name="attributeType">A string containing the FQN of an ISimilarityMeasure class</param>
        /// <returns>an ISimilarityMeasure isntance maintained by MEF; otherwise null</returns>
        public ISimilarityMeasure GetSimilarityMeasureInstance(string attributeType)
        {
            // Get the type for the preferred similarity measure
            Type type = Type.GetType(attributeType);

            //if (SimilarityMeasures.IsValueCreated)
            //{
                ISimilarityMeasure measureFound = this.SimilarityMeasures.FirstOrDefault(measure => measure.GetType() == type);

                // Ensure that the found Similarity Measure isn't null
                if (measureFound == null)
                    return null;

                // Return the retrieved instance of the preferred similarity measure
                return measureFound;
            //}
            //else
            //    return null;
        }

        /// <summary>
        /// Returns whether the provided values are dates
        /// </summary>
        /// <param name="sourceValue">The source value</param>
        /// <param name="targetValue">The target value</param>
        /// <returns>returns true if both values are dates; otherwise false</returns>
        private bool IsDate(string sourceValue, string targetValue)
        {
            DateTime outTest;

            // Try to parse the values into dates
            if (DateTime.TryParse(sourceValue, out outTest) && DateTime.TryParse(targetValue, out outTest))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Returns whether the provided values are numbers
        /// </summary>
        /// <param name="sourceValue">The source value</param>
        /// <param name="targetValue">The target value</param>
        /// <returns>true if both values are a number; otherwise false</returns>
        private bool IsNumber(string sourceValue, string targetValue)
        {
            int outInt;
            double outDouble;

            // Check if the values are integers
            if (int.TryParse(sourceValue, out outInt) && int.TryParse(targetValue, out outInt))
                return true;

            // Check if the values are doubles
            if (double.TryParse(sourceValue, out outDouble) && double.TryParse(targetValue, out outDouble))
                return true;

            return false;
        }

        /// <summary>
        /// Fires the ManagedAttributesChanged event
        /// </summary>
        /// <param name="args">The arguments for the event</param>
        public virtual void OnManagedAttributesChanged(NotifyCollectionChangedEventArgs args)
        {
            if (ManagedAttributesChanged != null)
            {
                ManagedAttributesChanged(this, args);
            }
        }

        /// <summary>
        /// Returns the maximum distance for all values for the provided attribute
        /// using the provided ISimilarityMeasure
        /// </summary>
        /// <param name="attributeName">The name of the attribute to retrieve the maximum distance for</param>
        /// <param name="measure">The ISimilarityMeasure to use to calculate the distance</param>
        /// <returns>the maximum distance value for all values for the given attribute</returns>
        public double GetMaxDistance(string attributeName, ISimilarityMeasure measure)
        {
            Tuple<string, string> tuple = Tuple.Create(attributeName, measure.ToString());

            // Ensure that the maximum distance isn't already cached
            if (!this.distancesCache.ContainsKey(tuple))
            {
                // Get the maximum distance calculation, for the provided
                // attribute and measure, and cache it.
                this.distancesCache[tuple] = CalculateMaxDistance(attributeName, measure);
            }

            return this.distancesCache[tuple];
        }

        /// <summary>
        /// Calculates the maximum distance for all values for the provided attribute
        /// name using the provided ISimilarityMeasure
        /// </summary>
        /// <param name="attributeName">The attribute name to retrieve the maximum distance for</param>
        /// <param name="measure">The ISimilarityMeasure to use to calculate the distance</param>
        /// <returns>the maximum distance value for all values for the given attribute</returns>
        private double CalculateMaxDistance(string attributeName, ISimilarityMeasure measure)
        {
            // Get the Attribute instance based on the provided attribute name
            Data.Attributes.Attribute attributeFound = GlobalAttributeCollection.GetInstance(this.scope).GetAttributes().Where(attribute => attribute.Name == attributeName).FirstOrDefault();

            return CalculateMaxDistance(attributeFound, measure);
        }

        /// <summary>
        /// Calculates the maximum distance for all values for the provided attribute
        /// using the provided ISimilarityMeasure
        /// </summary>
        /// <param name="attribute">The attribute to retrieve the maximum distance for</param>
        /// <param name="measure">The ISimilarityMeasure to use to calculate the distance</param>
        /// <returns>the maximum distance value for all values for the given attribute</returns>
        private double CalculateMaxDistance(Data.Attributes.Attribute attribute, ISimilarityMeasure measure)
        {
            // Ensure that the global collection contains the attribute
            if (!GlobalAttributeCollection.GetInstance(this.scope).ContainsAttribute(attribute))
                return 0;

            double maxDistance = double.MinValue;

            List<string> values = new List<string>(GlobalAttributeCollection.GetInstance(this.scope).GetAttributeValues(attribute));

            //TODO:  IS THERE A MORE EFFICIENT METHOD FOR HANDLING THIS??
            for (int i = 0; i < values.Count; i++)
            {
                // get the value for this item
                string a = values[i];

                // Compare the current item to all other items in the list
                for (int j = 0; j < i; j++)
                {
                    // get the value for this item
                    string b = values[j];
                    double? distance = measure.CalculateDistance(a, b);

                    if (distance != null)
                        maxDistance = Math.Max(maxDistance, (double)distance);

                }
            }

            return maxDistance;
        }

        /// <summary>
        /// Removes any cache entries for the given attribute
        /// </summary>
        private void InvalidateCache(string attributeName)
        {
            // If the collection was changed, we must invalidate the measures
            // cache for the given attribute
            foreach (Tuple<string, string> tuple in new List<Tuple<string, string>>(this.distancesCache.Keys))
            {
                // Check if this cache item is for the specified attribute
                if (tuple.Item1 == attributeName)
                {
                    // Remove the item from the cache
                    this.distancesCache.Remove(tuple);
                    this.meanCache.Remove(tuple);
                    this.sdCache.Remove(tuple);
                }
            }
        }

        private void InvalidateCache()
        {
            // Remove the item from the cache
            this.distancesCache.Clear();
            this.meanCache.Clear();
            this.sdCache.Clear();
        }

        #region IPartImportsSatisfiedNotification Members

            public void OnImportsSatisfied()
            {
                //throw new NotImplementedException();
            }

        #endregion
    }
}