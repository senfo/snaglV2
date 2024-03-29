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
using System.Collections.Specialized;
using System.IO;
using System.Windows;
using System.Windows.Resources;
using Berico.SnagL.Infrastructure.Data;
using Berico.SnagL.Infrastructure.Data.Attributes;
using Berico.SnagL.Infrastructure.Similarity;
using Berico.SnagL.Model;
using Berico.SnagL.Model.Attributes;

namespace Berico.SnagL.Infrastructure.Graph
{
    /// <summary>
    /// This class is for demonstration purposes only and will eventually
    /// be removed (or hidden) from the actual application.  It is used
    /// to randomly generate a graph (nodes, edges and attributes).  This
    /// may not be needed once importing is in place.
    /// </summary>
    public class GraphGenerator
    {
        private const int DEFAULT_NODE_COUNT = 100;
        private const int DEFAULT_EDGE_COUNT = 50;
        private const string DATA_FILE_PATH = "/Berico.SnagL;component/Resources/FakeData.csv";
        private List<List<Tuple<string, string>>> data = new List<List<Tuple<string, string>>>();
        private Random rand = new System.Random();
        private GraphData generatedGraph = null;

        public GraphGenerator()
        {
            LoadSourceData();
        }

        /// <summary>
        /// An external file (courtesy of FakeData.com) contains a large amount
        /// of fake data.  We use this file to generate our nodes and attributes.
        /// This method is responsible for reading in the data from that file.
        /// </summary>
        private void LoadSourceData()
        {
            StreamResourceInfo sri = Application.GetResourceStream(new Uri(DATA_FILE_PATH, UriKind.Relative));
            
            using (StreamReader dataFileStream = new StreamReader(sri.Stream))
            {
                string line;
                string[] header = new string[0];
                string[] row = new string[0];
                List<Tuple<string, string>> tuples = new List<Tuple<string, string>>();

                bool firstRow = true;
                while ((line = dataFileStream.ReadLine()) != null)
                {
                    if (firstRow)
                    {
                        header = line.Split(',');
                        firstRow = false;
                    }
                    else
                    {
                        row = line.Split(',');
                        tuples = new List<Tuple<string, string>>();

                        for (int i = 0; i <= row.GetUpperBound(0); i++)
                        {
                            Tuple<string, string> fieldData = Tuple.Create<string, string>(header[i], row[i]);
                            tuples.Add(fieldData);
                        }

                        this.data.Add(tuples);
                    }
                    
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numberOfNodes"></param>
        /// <param name="numberOfEdges"></param>
        /// <returns></returns>
        public GraphData GenerateGraph(string scope, int numberOfNodes, int numberOfEdges)
        {
            generatedGraph = new GraphData(scope);

            List<Node> nodes = GenerateNodes(numberOfNodes);
            List<IEdge> edges = GenerateEdges(numberOfEdges, nodes);

            generatedGraph.AddNodes(nodes);
            generatedGraph.AddEdges(edges);

            return generatedGraph;
        }

        private List<Node> GenerateNodes(int nodeCount)
        {
            List<Node> newNodes = new List<Node>();

            for (int i = 1; i <= nodeCount; i++)
            {
                Node newNode = new Node("Node" + i);

                //newNode.Attributes.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Attributes_CollectionChanged);
                GenerateAttributes(newNode);
                //newNode.Attributes.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Attributes_CollectionChanged);

                newNodes.Add(newNode);
            }

            return newNodes;
        }

        //private void Attributes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    // We shouldn't be doing anything but add actions here
        //    if (e.NewItems != null)
        //    {
        //        foreach (KeyValuePair<string, AttributeValue> data in e.NewItems)
        //        {
        //            // Add the attribute (and value) to the global collection
        //            GlobalAttributeCollection.GetInstance(generatedGraph.Scope).Update(data.Key, data.Value, null);
        //        }
        //    }
        //}

        private List<IEdge> GenerateEdges(int edgeCount, List<Node> nodes)
        {
            List<IEdge> newEdges = new List<IEdge>();

            int sourceNodeIndex = -1;
            int targetNodeIndex = -1;

            for (int i = 1; i <= edgeCount; i++)
            {
                // Keep looping as long as the source and target indexes are equal
                while (sourceNodeIndex == targetNodeIndex)
                {
                    // Randomly pick an index for a source and target node
                    sourceNodeIndex = rand.Next(0, nodes.Count - 1);
                    targetNodeIndex = rand.Next(0, nodes.Count - 1);
                }

                // Create an edge based on the randomly selected source
                // and target node indexes
                newEdges.Add(new Edge(nodes[sourceNodeIndex], nodes[targetNodeIndex]));

                // Reset the index variables
                sourceNodeIndex = -1;
                targetNodeIndex = -1;
            }

            return newEdges;

        }

        private void GenerateAttributes(Node newNode)
        {

            // Randomly select a record from the sample data
            int recordIndex = rand.Next(0, data.Count - 1);

            string name = string.Empty;

            foreach (Tuple<string, string> fieldData in this.data[recordIndex])
            {
                Data.Attributes.Attribute attribute = new Data.Attributes.Attribute(fieldData.Item1);
                AttributeValue attributeValue = null;
                
                switch (fieldData.Item1)
                {
                    case ("GivenName"):
                        attribute.SetPreferredSimilarityMeasure(typeof(DoubleMetaphoneSimilarityMeasure));
                        attribute.SemanticType = SemanticType.Name;
                        name = fieldData.Item2;
                        break;
                    case ("MiddleInitial"):
                        attribute.SetPreferredSimilarityMeasure(typeof(DoubleMetaphoneSimilarityMeasure));
                        attribute.SemanticType = SemanticType.Name;
                        name = name + " " + fieldData.Item2;
                        break;
                    case ("Surname"):
                        attribute.SetPreferredSimilarityMeasure(typeof(DoubleMetaphoneSimilarityMeasure));
                        attribute.SemanticType = SemanticType.Name;
                        name = name + ". " + fieldData.Item2;

                        attribute = new Data.Attributes.Attribute("Full Name");
                        attributeValue = new AttributeValue(name, name);

                        // Add the attribute and it's value to the provided node
                        UpdateAttributeCollection(newNode, attribute, attributeValue);
                        break;
                    case ("Gender"):
                        attribute.SetPreferredSimilarityMeasure(typeof(ExactMatchSimilarityMeasure));
                        attribute.SemanticType = SemanticType.GeneralString;
                        attributeValue = new AttributeValue(fieldData.Item2, fieldData.Item2);

                        // Add the attribute and it's value to the provided node
                        UpdateAttributeCollection(newNode, attribute, attributeValue);
                        break;
                    case ("EmailAddress"):
                        attribute.SetPreferredSimilarityMeasure(typeof(EmailDomainSimilarityMeasure));
                        attribute.SemanticType = SemanticType.EmailAddress;
                        attributeValue = new AttributeValue(fieldData.Item2, fieldData.Item2);

                        // Add the attribute and it's value to the provided node
                        UpdateAttributeCollection(newNode, attribute, attributeValue);
                        break;
                    case ("TelephoneNumber"):
                        attribute.SetPreferredSimilarityMeasure(typeof(LevenshteinDistanceStringSimilarityMeasure));
                        attribute.SemanticType = SemanticType.Number;
                        attributeValue = new AttributeValue(fieldData.Item2, fieldData.Item2);

                        // Add the attribute and it's value to the provided node
                        UpdateAttributeCollection(newNode, attribute, attributeValue);
                        break;
                    case ("Birthday"):
                        attribute.SetPreferredSimilarityMeasure(typeof(DateTimeSimilarityMeasure));
                        attribute.SemanticType = SemanticType.Date;
                        attributeValue = new AttributeValue(fieldData.Item2, fieldData.Item2);

                        // Add the attribute and it's value to the provided node
                        UpdateAttributeCollection(newNode, attribute, attributeValue);
                        break;
                    case ("Occupation"):
                        attribute.SetPreferredSimilarityMeasure(typeof(DoubleMetaphoneSimilarityMeasure));
                        attribute.SemanticType = SemanticType.GeneralString;
                        attributeValue = new AttributeValue(fieldData.Item2, fieldData.Item2);

                        // Add the attribute and it's value to the provided node
                        UpdateAttributeCollection(newNode, attribute, attributeValue);
                        break;
                    default:
                        attribute.SetPreferredSimilarityMeasure(typeof(LevenshteinDistanceStringSimilarityMeasure));
                        attribute.SemanticType = SemanticType.GeneralString;
                        attributeValue = new AttributeValue(fieldData.Item2, fieldData.Item2);

                        // Add the attribute and it's value to the provided node
                        UpdateAttributeCollection(newNode, attribute, attributeValue);
                        break;
                }
            }

            // Make the node's display name the name field
            newNode.DisplayValue = name;
            newNode.Description = "Generated by internal generator";

            // Remove the item we used
            this.data.RemoveAt(recordIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetNode"></param>
        /// <param name="attribute"></param>
        /// <param name="attributeValue"></param>
        private void UpdateAttributeCollection(Node targetNode, Data.Attributes.Attribute attribute, AttributeValue attributeValue)
        {
            // Add the attribute name and value to the nodes attribute collection
            targetNode.Attributes.Add(attribute.Name, attributeValue);

            // Update the global attribute collection
            GlobalAttributeCollection.GetInstance(generatedGraph.Scope).Add(attribute, attributeValue);
        }


    }
}