//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

namespace Berico.SnagL.Infrastructure.Data.Formats
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;
    using Berico.Common;
    using Berico.SnagL.Infrastructure.Data.Mapping;
    using Berico.SnagL.Infrastructure.Logging;
    using Berico.SnagL.Model;
    using Berico.SnagL.Model.Attributes;
    using Berico.SnagL.UI;

    public class GraphMLGraphDataFormat : GraphDataFormatBase
    {
        private const string BERICO_NAMESPACE_URI = "http://graph.bericotechnologies.com/xmlns";
        private const string NODE_ATTRIBUTE_PREFIX = "node-attr-";
        private const string EDGE_ATTRIBUTE_PREFIX = "edge-attr-";
        private const string NODE_PROPERTY_PREFIX = "node-prop-";
        private const string EDGE_PROPERTY_PREFIX = "edge-prop-";
        private const string ATTRIBUTE_DESCRIPTOR_SUFFIX = "-desc";
        private const string ATTRIBUTE_VALUE_SUFFIX = "-val";

        /// <summary>
        /// Create a new instance of the GraphMLGraphDataFormat class
        /// </summary>
        public GraphMLGraphDataFormat()
        {
            Priority = 1;
            Extension = "xml";
            Description = "GraphML file";
        }

        public override bool Validate(string data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Performs the actual import for the given GraphML or SnaglML data.
        /// The Import method should be called rather than calling
        /// this method directly.
        /// </summary>
        /// <param name="data">GraphML or SnaglML</param>
        /// <param name="components">The instance of GraphComponents that
        /// is used to create nodes and edges for the graph</param>
        /// <param name="sourceMechanism">Specifies the mechanism for which objects on the graph were imported</param>
        /// <returns>true if the import was succesfull; otherwise false</returns>
        internal override GraphMapData ImportData(string data)
        {
            // TODO Should validate against XSD
            // TODO Processing should conform to GraphML defined processing rules

            GraphMapData graph = new GraphMapData();

            bool haveEncounteredGraph = false;
            using (TextReader stringReader = new StringReader(data))
            {
                XmlReaderSettings settings = new XmlReaderSettings
                {
                    CloseInput = true,
                    ConformanceLevel = ConformanceLevel.Document,
                    DtdProcessing = DtdProcessing.Parse,
                    IgnoreWhitespace = true
                };
                using (XmlReader reader = XmlReader.Create(stringReader, settings))
                {
                    // while we know the graph element must be first as per the schema the compiler does not, so these is assigned a value here
                    NodeTypes defaultNodeType = NodeTypes.Text;
                    //GraphType defaultGraphType = GraphType.Undirected;

                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.LocalName)
                            {
                                case "graph":
                                    if (!haveEncounteredGraph)
                                    {
                                        haveEncounteredGraph = true;

                                        string nodeType = reader.GetAttribute("nodeType", BERICO_NAMESPACE_URI);
                                        defaultNodeType = GetDefaultNodeType(nodeType);

                                        //string edgeDefault = reader.GetAttribute("edgedefault");
                                        //defaultGraphType = GetDefaultEdgeType(edgeDefault);
                                    }
                                    else
                                    {
                                        throw new Exception("Both multiple graphs per file and nested graphs are unsupported.");
                                    }
                                    break;

                                case "node":
                                    NodeMapData node = ReadNode(reader, defaultNodeType);
                                    graph.Add(node);
                                    break;

                                case "edge":
                                    EdgeMapData edge = ReadEdge(reader);
                                    graph.Add(edge);
                                    break;
                            }
                        }
                    }
                }
            }

            return graph;
        }

        // GraphML output is not supported, this only outputs SnaglML
        internal override string ExportData(GraphMapData graph)
        {
            string graphMLDocument = string.Empty;

            using (TextWriter stringWriter = new StringWriter())
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    CloseOutput = true,
                    ConformanceLevel = ConformanceLevel.Document,
                    Encoding = UTF8Encoding.UTF8,
                    Indent = true,
                    IndentChars = "  ",
                    NewLineChars = Environment.NewLine,
                    NewLineHandling = NewLineHandling.Replace
                };
                using (XmlWriter writer = XmlWriter.Create(stringWriter, settings))
                {
                    WriteHeader(writer, graph);
                    WriteGraphContent(writer, graph);
                    WriteFooter(writer);
                    writer.Flush();
                }

                graphMLDocument = stringWriter.ToString();
            }

            return graphMLDocument;
        }

        #region Import from GraphML and SnaglML

        private static NodeTypes GetDefaultNodeType(string defaultNodeType)
        {
            NodeTypes determinedDefaultNodeType;

            bool foundNodeType = Enum.TryParse<NodeTypes>(defaultNodeType, true, out determinedDefaultNodeType);
            if (!foundNodeType)
            {
                determinedDefaultNodeType = NodeTypes.Text;
            }

            return determinedDefaultNodeType;
        }

        /*/// <summary>
        /// Gets the default edge type (direction) to be used in the
        /// event that the 'directed' attribute of an edge is not included
        /// </summary>
        /// <param name="defaultEdgeType">The value of the edgedefault attribute</param>
        private static GraphType GetDefaultEdgeType(string defaultEdgeType)
        {
            GraphType determinedEdgeType = GraphType.Undirected;

            if (defaultEdgeType != null && defaultEdgeType.ToLower().Equals("directed"))
            {
                determinedEdgeType = GraphType.Directed;
            }

            return determinedEdgeType;
        }*/

        /// <summary>
        /// Reads an XML node from the specified XmlReader
        /// </summary>
        /// <param name="reader">Reader from which to read the node from</param>
        private NodeMapData ReadNode(XmlReader reader, NodeTypes defaultNodeType)
        {
            NodeMapData objNode;

            string nodeId = reader.GetAttribute("id");
            if (defaultNodeType == NodeTypes.Icon)
            {
                objNode = new IconNodeMapData(nodeId);
            }
            else
            {
                objNode = new TextNodeMapData(nodeId);
            }

            if (reader.ReadToDescendant("data"))
            {
                Attributes.Attribute newAttribute = null;
                AttributeValue newAttributeValue = null;

                // Loop over all data elements.  These are the attributes
                do
                {
                    // Record the attributes
                    string dataKey = reader.GetAttribute("key");
                    string dataValue = reader.ReadElementContentAsString();

                    // Determine if we are dealing with a node property
                    if (dataKey.StartsWith(NODE_PROPERTY_PREFIX))
                    {
                        string propName = dataKey.Substring(NODE_PROPERTY_PREFIX.Length);
                        switch (propName)
                        {
                            case "Description":
                                objNode.Description = dataValue;
                                break;

                            case "DisplayValue":
                                objNode.Label = dataValue;
                                break;

                            case "SelectionColor":
                                SolidColorBrush selectionColor = Conversion.HexColorToBrush(dataValue);
                                objNode.SelectionColor = selectionColor.Color;
                                break;

                            case "ImageSource":
                                ((IconNodeMapData)objNode).ImageSource = new Uri(dataValue, UriKind.RelativeOrAbsolute);
                                break;

                            case "Height":
                                double height = double.Parse(dataValue);
                                objNode.Dimension = new Size(objNode.Dimension.Width, height);
                                break;

                            case "Width":
                                double width = double.Parse(dataValue);
                                objNode.Dimension = new Size(width, objNode.Dimension.Height);
                                break;

                            case "Position":
                                string[] splitPosition = dataValue.Split(',');
                                objNode.Position = new Point(double.Parse(splitPosition[0]), double.Parse(splitPosition[1]));
                                break;

                            case "IsHidden":
                                objNode.IsHidden = bool.Parse(dataValue);
                                break;

                            case "BackgroundColor":
                                SolidColorBrush backgroundColor = Conversion.HexColorToBrush(dataValue);
                                objNode.BackgroundColor = backgroundColor.Color;
                                break;

                            default:
                                // TODO prop is for a different version of SnagL
                                break;
                        }

                        // TODO do we only want to do whats above when what is commented out below fails?
                        /*// Attempt to set the node propety
                        if ((SetExportablePropertyValue(dataKey, objNode, dataValue)))
                            _logger.WriteLogEntry(LogLevel.INFO, string.Format("The Node property [{0}] was set to '{1}'", dataKey, dataValue), null, null);
                        else
                        {
                            // The property might be for the view model so try
                            // and set the view model
                            if ((SetExportablePropertyValue(dataKey, objNode, dataValue)))
                                _logger.WriteLogEntry(LogLevel.INFO, string.Format("The NodeVM property [{0}] was set to '{1}'", dataKey, dataValue), null, null);
                            else
                                _logger.WriteLogEntry(LogLevel.ERROR, string.Format("Unable to set the property [{0}] to the specified value [{1}]", dataKey, dataValue), null, null);
                        }*/
                    }
                    else // Determine if we are dealing with an attribute
                    {
                        if (dataKey.StartsWith(NODE_ATTRIBUTE_PREFIX))
                        {
                            // Determine if we are dealing with a descriptor or a value
                            if (dataKey.EndsWith(ATTRIBUTE_DESCRIPTOR_SUFFIX))
                                newAttribute = CreateAttribute(dataValue);
                            else if (dataKey.EndsWith(ATTRIBUTE_VALUE_SUFFIX))
                            {
                                newAttributeValue = new AttributeValue(dataValue);
                            }
                        }
                        else // If we are here, we are not dealing with SnagL formatted GraphML
                        {
                            // We are dealing with an unknown data element so we 
                            // are going to treat it like a new attribute
                            // Determine if we are dealing with a descriptor or a value
                            newAttribute = new Attributes.Attribute(dataKey);
                            newAttributeValue = new AttributeValue(dataValue);
                        }

                        // Check if we have a valid Attribute and AttributeValue class
                        if (newAttribute != null && newAttributeValue != null)
                        {
                            AttributeMapData objAttribute = new AttributeMapData(newAttribute.Name, newAttributeValue.Value);
                            objNode.Attributes.Add(objAttribute.Name, objAttribute);

                            objAttribute.SemanticType = newAttribute.SemanticType;
                            objAttribute.SimilarityMeasure = newAttribute.PreferredSimilarityMeasure;
                            objAttribute.IsHidden = !newAttribute.Visible;

                            newAttribute = null;
                            newAttributeValue = null;
                        }
                    }
                } while (reader.LocalName == "data" || (string.IsNullOrEmpty(reader.LocalName) && reader.ReadToNextSibling("data")));
            }

            return objNode;
        }

        private bool SetExportablePropertyValue(string propertyName, object targetObject, string value)
        {
            propertyName = propertyName.Replace(NODE_PROPERTY_PREFIX, string.Empty);
            propertyName = propertyName.Replace(EDGE_PROPERTY_PREFIX, string.Empty);

            // Validate parameters
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName", "No valid property name was provided");
            }
            if (targetObject == null)
            {
                throw new ArgumentNullException("targetObject", "No valid target object was provided");
            }

            // Attempt to get the target property
            PropertyInfo propInfo = targetObject.GetType().GetProperty(propertyName);
            object typedValue = null;

            // Make sure we got property information
            if (propInfo != null)
            {
                if (propInfo.PropertyType.IsPrimitive || propInfo.PropertyType == typeof(string))
                {
                    // Convert the value to the appropriate type
                    typedValue = Convert.ChangeType(value, propInfo.PropertyType, CultureInfo.CurrentCulture);
                }
                else if (propInfo.PropertyType == typeof(SolidColorBrush) || propInfo.PropertyType == typeof(Brush))
                {
                    // Convert the value to a new SolidColorBrush
                    typedValue = Conversion.HexColorToBrush(value);
                }
                else if (propInfo.PropertyType == typeof(Point))
                {
                    Point point = default(System.Windows.Point);

                    // Attempt to parse the string into a Point instance
                    if (point.TryParse(value, out point))
                    {
                        // Save the new point
                        typedValue = point;
                    }
                    else
                    {
                        _logger.WriteLogEntry(LogLevel.ERROR, string.Format("The provided value [{0}] could not be parsed into a Point", value), null, null);
                        return false;
                    }
                }
                else if (propInfo.PropertyType == typeof(FontStyle))
                {
                    PropertyInfo pi = typeof(FontStyles).GetProperty(value);

                    // Ensure we were able to get the property
                    if (pi != null)
                        typedValue = pi.GetValue(null, null);
                    else
                    {
                        _logger.WriteLogEntry(LogLevel.ERROR, string.Format("The target property value [{0}] is not a valid FontStyle", value), null, null);
                        return false;
                    }
                }
                else if (propInfo.PropertyType == typeof(FontWeight))
                {
                    PropertyInfo pi = typeof(FontWeights).GetProperty(value);

                    // Ensure we were able to get the property
                    if (pi != null)
                        typedValue = pi.GetValue(null, null);
                    else
                    {
                        _logger.WriteLogEntry(LogLevel.ERROR, string.Format("The target property value [{0}] is not a valid FontWeight", value), null, null);
                        return false;
                    }
                }
                else if (propInfo.PropertyType == typeof(FontFamily))
                {
                    FontFamily ff = new FontFamily(value);

                    // If an unsupported font is specified, the fallback is
                    // Portable User Interface
                    typedValue = ff;
                }

                // Set the actual property value
                propInfo.SetValue(targetObject, typedValue, null);

                return true;
            }
            else
            {
                _logger.WriteLogEntry(LogLevel.ERROR, string.Format("The target property [{0}] does not exist in the target object [{1}]", propertyName, targetObject.ToString()), null, null);
                return false;
            }
        }

        /// <summary>
        /// Reads the edge data from the specified XmlReader
        /// </summary>
        /// <param name="reader">Reader from which to read edge data from</param>
        private static EdgeMapData ReadEdge(XmlReader reader)
        {
            string sourceId = reader.GetAttribute("source");
            string targetId = reader.GetAttribute("target");

            EdgeMapData objEdge = new EdgeMapData(sourceId, targetId);

            string directed = reader.GetAttribute("directed");
            if (directed != null)
            {
                bool isDirected = bool.Parse(directed);
                if (isDirected)
                {
                    objEdge.Type = EdgeType.Directed;
                }
            }

            // this is not used for anything
            //string edgeTypeName = reader.GetAttribute("type", BERICO_NAMESPACE_URI);

            if (reader.ReadToDescendant("data"))
            {
                do
                {
                    string dataKey = reader.GetAttribute("key");
                    string dataValue = reader.ReadElementContentAsString();

                    if (dataKey.StartsWith(EDGE_PROPERTY_PREFIX))
                    {
                        string propertyName = dataKey.Substring(EDGE_PROPERTY_PREFIX.Length);
                        switch (propertyName)
                        {
                            case "DisplayValue":
                                objEdge.Label = dataValue;
                                break;

                            case "Thickness":
                                objEdge.Thickness = double.Parse(dataValue);
                                break;

                            case "Color":
                                Color color = Conversion.HexColorToBrush(dataValue).Color;
                                objEdge.Color = color;
                                break;

                            case "LabelBackgroundColor":
                                Color labelBackgroundColor = Conversion.HexColorToBrush(dataValue).Color;
                                objEdge.LabelBackgroundColor = labelBackgroundColor;
                                break;

                            case "LabelForegroundColor":
                                Color labelForegroundColor = Conversion.HexColorToBrush(dataValue).Color;
                                objEdge.LabelForegroundColor = labelForegroundColor;
                                break;

                            case "LabelFontStyle":
                                objEdge.LabelFontStyle = (FontStyle)typeof(FontStyles).GetProperty(dataValue).GetValue(null, null);
                                break;

                            case "LabelFontWeight":
                                objEdge.LabelFontWeight = (FontWeight)typeof(FontWeights).GetProperty(dataValue).GetValue(null, null);
                                break;

                            case "LabelTextUnderline":
                                objEdge.IsLabelTextUnderlined = bool.Parse(dataValue);
                                break;
                        }
                    }
                    else if (!dataKey.EndsWith(ATTRIBUTE_DESCRIPTOR_SUFFIX))
                    {
                        string attributeName = dataKey.Substring(EDGE_ATTRIBUTE_PREFIX.Length, dataKey.Length - (EDGE_ATTRIBUTE_PREFIX.Length + ATTRIBUTE_VALUE_SUFFIX.Length));
                        AttributeMapData objAttribute = new AttributeMapData(attributeName, dataValue);
                        objEdge.Attributes.Add(objAttribute.Name, objAttribute);
                    }
                } while (reader.LocalName == "data" || (string.IsNullOrEmpty(reader.LocalName) && reader.ReadToNextSibling("data")));
            }

            return objEdge;
        }

        /// <summary>
        /// Returns a new Attribute instance based on deserializing
        /// the provided Json string
        /// </summary>
        /// <param name="attributeData">The serialized Json string</param>
        /// <returns>an Attribute instance</returns>
        private Attributes.Attribute CreateAttribute(string attributeData)
        {
            Attributes.Attribute newAttribute;

            try
            {
                // Attempt to deserialize the provided Json string
                // into an Attribute instance
                newAttribute = JSONHelper.Deserialize<Attributes.Attribute>(attributeData);
            }
            catch (SerializationException ex)
            {
                _logger.WriteLogEntry(LogLevel.ERROR, "An exception occurred while attempting to deserialize the provided Json string", ex, null);
                newAttribute = null;
            }

            return newAttribute;
        }

        #endregion

        #region Export to GML

        private static void WriteHeader(XmlWriter writer, GraphMapData graph)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("graphml", "http://graphml.graphdrawing.org/xmlns");
            writer.WriteAttributeString("xmlns", "berico", string.Empty, BERICO_NAMESPACE_URI);
            writer.WriteAttributeString("xmlns", "xsi", string.Empty, "http://www.w3.org/2001/XMLSchema-instance");
            writer.WriteAttributeString("xsi", "schemaLocation", string.Empty, "http://graphml.graphdrawing.org.xmlns/1.0/graphml.xsd");

            if (graph.GetNodes().Count > 0)
            {
                bool graphHasIconNode = false;
                foreach (NodeMapData objNode in graph.GetNodes())
                {
                    if (objNode is IconNodeMapData)
                    {
                        graphHasIconNode = true;
                        break;
                    }
                }
                if (graphHasIconNode)
                {
                    WritePropKey(writer, NODE_PROPERTY_PREFIX, "ImageSource", "node");
                }

                WritePropKey(writer, NODE_PROPERTY_PREFIX, "Description", "node");
                WritePropKey(writer, NODE_PROPERTY_PREFIX, "DisplayValue", "node");
                WritePropKey(writer, NODE_PROPERTY_PREFIX, "Width", "node");
                WritePropKey(writer, NODE_PROPERTY_PREFIX, "Height", "node");
                WritePropKey(writer, NODE_PROPERTY_PREFIX, "Position", "node");
                WritePropKey(writer, NODE_PROPERTY_PREFIX, "IsHidden", "node");
                WritePropKey(writer, NODE_PROPERTY_PREFIX, "BackgroundColor", "node");
                WritePropKey(writer, NODE_PROPERTY_PREFIX, "SelectionColor", "node");
            }

            if (graph.GetEdges().Count > 0)
            {
                WritePropKey(writer, EDGE_PROPERTY_PREFIX, "DisplayValue", "edge");
                WritePropKey(writer, EDGE_PROPERTY_PREFIX, "LabelTextUnderline", "edge");
                WritePropKey(writer, EDGE_PROPERTY_PREFIX, "Thickness", "edge");
                WritePropKey(writer, EDGE_PROPERTY_PREFIX, "Color", "edge");
                WritePropKey(writer, EDGE_PROPERTY_PREFIX, "LabelBackgroundColor", "edge");
                WritePropKey(writer, EDGE_PROPERTY_PREFIX, "LabelForegroundColor", "edge");
                WritePropKey(writer, EDGE_PROPERTY_PREFIX, "LabelFontStyle", "edge");
                WritePropKey(writer, EDGE_PROPERTY_PREFIX, "LabelFontWeight", "edge");
                WritePropKey(writer, EDGE_PROPERTY_PREFIX, "LabelFont", "edge");
            }

            ISet<string> seenAttributes = new HashSet<string>();
            foreach (NodeMapData objNode in graph.GetNodes())
            {
                foreach (KeyValuePair<string, AttributeMapData> kvp in objNode.Attributes)
                {
                    if (seenAttributes.Add(kvp.Key))
                    {
                        WriteAttrKey(writer, NODE_ATTRIBUTE_PREFIX, kvp.Value.Name, "node");
                    }
                }
            }

            seenAttributes.Clear();
            foreach (EdgeMapData objEdge in graph.GetEdges())
            {
                foreach (KeyValuePair<string, AttributeMapData> kvp in objEdge.Attributes)
                {
                    if (seenAttributes.Add(kvp.Key))
                    {
                        WriteAttrKey(writer, EDGE_ATTRIBUTE_PREFIX, kvp.Value.Name, "edge");
                    }
                }
            }
        }

        private static void WritePropKey(XmlWriter writer, string prefix, string name, string forAttr)
        {
            writer.WriteStartElement("key");
            writer.WriteAttributeString("id", prefix + name);
            writer.WriteAttributeString("for", forAttr);
            writer.WriteAttributeString("attr.name", prefix + name);
            writer.WriteAttributeString("attr.type", "string");
            writer.WriteEndElement();
        }

        private static void WriteAttrKey(XmlWriter writer, string prefix, string name, string forAttr)
        {
            // Create key for this attribute's descriptor
            writer.WriteStartElement("key");
            writer.WriteAttributeString("id", prefix + name + ATTRIBUTE_DESCRIPTOR_SUFFIX);
            writer.WriteAttributeString("for", forAttr);
            writer.WriteAttributeString("attr.name", prefix + name + ATTRIBUTE_DESCRIPTOR_SUFFIX);
            writer.WriteAttributeString("attr.type", "string");
            writer.WriteEndElement();

            // Create key for this attribute's value
            writer.WriteStartElement("key");
            writer.WriteAttributeString("id", prefix + name + ATTRIBUTE_VALUE_SUFFIX);
            writer.WriteAttributeString("for", forAttr);
            writer.WriteAttributeString("attr.name", prefix + name + ATTRIBUTE_VALUE_SUFFIX);
            writer.WriteAttributeString("attr.type", "string");
            writer.WriteEndElement();
        }

        private static void WriteGraphContent(XmlWriter writer, GraphMapData graph)
        {
            writer.WriteStartElement("graph");
            writer.WriteAttributeString("id", "snagl_export_graph");

            GraphType edgedefault = GraphType.Undirected;
            foreach (EdgeMapData objEdge in graph.GetEdges())
            {
                if (objEdge.Type == EdgeType.Directed)
                {
                    edgedefault = GraphType.Directed;
                    break;
                }
            }
            writer.WriteAttributeString("edgedefault", edgedefault.ToString());

            NodeTypes defaultNodeType = NodeTypes.Text;
            foreach (NodeMapData objNode in graph.GetNodes())
            {
                if (objNode is IconNodeMapData)
                {
                    defaultNodeType = NodeTypes.Icon;
                    break;
                }
            }
            writer.WriteAttributeString("berico", "nodeType", BERICO_NAMESPACE_URI, defaultNodeType.ToString());

            foreach (NodeMapData objNode in graph.GetNodes())
            {
                WriteNode(writer, objNode);
            }

            foreach (EdgeMapData objEdge in graph.GetEdges())
            {
                WriteEdge(writer, objEdge);
            }

            writer.WriteEndElement();
        }

        private static void WriteNode(XmlWriter writer, NodeMapData objNode)
        {
            writer.WriteStartElement("node");
            writer.WriteAttributeString("id", objNode.Id);

            if (objNode.GetType().Equals(typeof(IconNodeMapData)))
            {
                WritePropData(writer, NODE_PROPERTY_PREFIX, "ImageSource", ((IconNodeMapData)objNode).ImageSource.ToString());
            }

            WritePropData(writer, NODE_PROPERTY_PREFIX, "Description", objNode.Description);
            WritePropData(writer, NODE_PROPERTY_PREFIX, "DisplayValue", objNode.Label);
            WritePropData(writer, NODE_PROPERTY_PREFIX, "Height", objNode.Dimension.Height.ToString());
            WritePropData(writer, NODE_PROPERTY_PREFIX, "Width", objNode.Dimension.Width.ToString());
            WritePropData(writer, NODE_PROPERTY_PREFIX, "Position", objNode.Position.ToString());
            WritePropData(writer, NODE_PROPERTY_PREFIX, "IsHidden", objNode.IsHidden.ToString());

            string backgroundColor = GetSnaglStrColor(objNode.BackgroundColor);
            WritePropData(writer, NODE_PROPERTY_PREFIX, "BackgroundColor", backgroundColor);

            string selectionColor = GetSnaglStrColor(objNode.SelectionColor);
            WritePropData(writer, NODE_PROPERTY_PREFIX, "SelectionColor", selectionColor);

            foreach (KeyValuePair<string, AttributeMapData> kvp in objNode.Attributes)
            {
                string description = "{\"Name\":\"" + kvp.Value.Name + "\",\"PreferredSimilarityMeasure\":\"" + kvp.Value.SimilarityMeasure + "\",\"SemanticType\":" + (int)kvp.Value.SemanticType + ",\"Visible\":" + (!kvp.Value.IsHidden).ToString().ToLower() + "}";
                WriteAttrData(writer, NODE_ATTRIBUTE_PREFIX, kvp.Value.Name, description, kvp.Value.Value);
            }

            writer.WriteEndElement();
        }

        private static void WriteEdge(XmlWriter writer, EdgeMapData objEdge)
        {
            writer.WriteStartElement("edge");
            writer.WriteAttributeString("source", objEdge.Source);
            writer.WriteAttributeString("target", objEdge.Target);

            bool isDirected = false;
            if (objEdge.Type == EdgeType.Directed)
            {
                isDirected = true;
            }
            writer.WriteAttributeString("directed", isDirected.ToString());

            writer.WriteAttributeString("berico", "type", BERICO_NAMESPACE_URI, "Berico.SnagL.Model.Edge");

            WritePropData(writer, EDGE_PROPERTY_PREFIX, "DisplayValue", objEdge.Label);
            WritePropData(writer, EDGE_PROPERTY_PREFIX, "LabelTextUnderline", objEdge.IsLabelTextUnderlined.ToString());
            WritePropData(writer, EDGE_PROPERTY_PREFIX, "Thickness", objEdge.Thickness.ToString());
            WritePropData(writer, EDGE_PROPERTY_PREFIX, "LabelFontStyle", objEdge.LabelFontStyle.ToString());
            WritePropData(writer, EDGE_PROPERTY_PREFIX, "LabelFontWeight", objEdge.LabelFontWeight.ToString());

            if (objEdge.LabelFont != null)
            {
                WritePropData(writer, EDGE_PROPERTY_PREFIX, "LabelFont", objEdge.LabelFont.Source);
            }

            string color = GetSnaglStrColor(objEdge.Color);
            WritePropData(writer, EDGE_PROPERTY_PREFIX, "Color", color);

            string labelBackgroundColor = GetSnaglStrColor(objEdge.LabelBackgroundColor);
            WritePropData(writer, EDGE_PROPERTY_PREFIX, "LabelBackgroundColor", labelBackgroundColor);

            string labelForegroundColor = GetSnaglStrColor(objEdge.LabelForegroundColor);
            WritePropData(writer, EDGE_PROPERTY_PREFIX, "LabelForegroundColor", labelForegroundColor);

            foreach (KeyValuePair<string, AttributeMapData> kvp in objEdge.Attributes)
            {
                string description = "{\"Name\":\"" + kvp.Value.Name + "\",\"PreferredSimilarityMeasure\":\"" + kvp.Value.SimilarityMeasure + "\",\"SemanticType\":" + (int)kvp.Value.SemanticType + ",\"Visible\":" + (!kvp.Value.IsHidden).ToString().ToLower() + "}";
                WriteAttrData(writer, EDGE_ATTRIBUTE_PREFIX, kvp.Value.Name, description, kvp.Value.Value);
            }

            writer.WriteEndElement();
        }

        // TODO this should probably be moved alongside HexColorToBrush in Conversion class
        //      otherclasses currently have the String.Format duplicated
        private static string GetSnaglStrColor(Color color)
        {
            string snaglColor = String.Format("#{0:x2}{1:x2}{2:x2}{3:x2}", color.A, color.R, color.G, color.B);
            return snaglColor;
        }

        private static void WritePropData(XmlWriter writer, string prefix, string name, string value)
        {
            writer.WriteStartElement("data");
            writer.WriteAttributeString("key", prefix + name);
            writer.WriteString(value);
            writer.WriteEndElement();
        }

        private static void WriteAttrData(XmlWriter writer, string prefix, string name, string description, string value)
        {
            writer.WriteStartElement("data");
            writer.WriteAttributeString("key", prefix + name + ATTRIBUTE_DESCRIPTOR_SUFFIX);
            writer.WriteString(description);
            writer.WriteEndElement();

            writer.WriteStartElement("data");
            writer.WriteAttributeString("key", prefix + name + ATTRIBUTE_VALUE_SUFFIX);
            writer.WriteString(value);
            writer.WriteEndElement();
        }

        private static void WriteFooter(XmlWriter writer)
        {
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        #endregion
    }
}