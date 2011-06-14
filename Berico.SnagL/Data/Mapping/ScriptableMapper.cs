//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

namespace Berico.SnagL.Infrastructure.Data.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;
    using Berico.SnagL.Infrastructure.Data.Attributes;
    using Berico.SnagL.Infrastructure.Data.Mapping.JS;
    using Berico.SnagL.Model;

    public static class ScriptableMapper
    {
        public static NodeMapData GetNode(ScriptableNodeMapData scriptableNode)
        {
            NodeMapData node;

            if (scriptableNode is ScriptableIconNodeMapData)
            {
                ScriptableIconNodeMapData scriptableIconNode = (ScriptableIconNodeMapData)scriptableNode;

                IconNodeMapData iconNode = new IconNodeMapData(scriptableIconNode.Id);
                if (!String.IsNullOrEmpty(scriptableIconNode.ImageSource))
                {
                    Uri imageSourceUri = new Uri(scriptableIconNode.ImageSource);
                    iconNode.ImageSource = imageSourceUri;
                }

                node = iconNode;
            }
            else
            {
                node = new TextNodeMapData(scriptableNode.Id);
            }

            // Properties
            node.Description = scriptableNode.Description;
            node.Label = scriptableNode.Label;
            node.Dimension = GetDimension(scriptableNode.Dimension);
            node.Position = GetPosition(scriptableNode.Position);
            node.IsHidden = scriptableNode.IsHidden;
            node.BackgroundColor = GetColor(scriptableNode.BackgroundColor);
            node.SelectionColor = GetColor(scriptableNode.SelectionColor);

            // Attributes
            foreach (KeyValuePair<string, ScriptableAttributeMapData> kvp in scriptableNode.Attributes)
            {
                ScriptableAttributeMapData scriptableAttribute = kvp.Value;

                AttributeMapData attribute = new AttributeMapData(scriptableAttribute.Name, scriptableAttribute.Value);
                attribute.SemanticType = (SemanticType)scriptableAttribute.SemanticType;
                attribute.SimilarityMeasure = scriptableAttribute.SimilarityMeasure;
                attribute.IsHidden = scriptableAttribute.IsHidden;

                node.Attributes.Add(kvp.Key, attribute);
            }

            return node;
        }

        /// <summary>
        /// Maps a <see cref="ScriptableNodeMapData"/> object from a <see cref="NodeMapData"/> object
        /// </summary>
        /// <param name="node">The <see cref="NodeMapData"/> object to map from</param>
        /// <returns>A <see cref="ScriptableNodeMapData"/> object mapped from from a <see cref="NodeMapData"/> object</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is null</exception>
        public static ScriptableNodeMapData GetNode(NodeMapData node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            ScriptableNodeMapData scriptableNode = new ScriptableNodeMapData
            {
                BackgroundColor = node.BackgroundColor.ToString(),
                Description = node.Description,
                Dimension = new ScriptableSize(node.Dimension.Height, node.Dimension.Width),
                Id = node.Id,
                IsHidden = node.IsHidden,
                Label = node.Label,
                Position = new ScriptablePoint(node.Position.X, node.Position.Y),
                SelectionColor = node.SelectionColor.ToString()
            };

            foreach (KeyValuePair<string, AttributeMapData> kvp in node.Attributes)
            {
                AttributeMapData attributeMapData = kvp.Value;
                ScriptableAttributeMapData scriptableAttributeData = new ScriptableAttributeMapData
                {
                    IsHidden = attributeMapData.IsHidden,
                    Name = attributeMapData.Name,
                    SemanticType = (int)attributeMapData.SemanticType,
                    SimilarityMeasure = attributeMapData.SimilarityMeasure,
                    Value = attributeMapData.Value
                };

                scriptableNode.Attributes.Add(kvp.Key, scriptableAttributeData);
            }

            return scriptableNode;
        }

        public static EdgeMapData GetEdge(ScriptableEdgeMapData scriptableEdge)
        {
            EdgeMapData edge = new EdgeMapData(scriptableEdge.Source, scriptableEdge.Target);

            // Properties
            edge.Type = (EdgeType)Enum.Parse(typeof(EdgeType), scriptableEdge.Type, false);
            edge.Thickness = scriptableEdge.Thickness;
            edge.Color = GetColor(scriptableEdge.Color);
            edge.Label = scriptableEdge.Label;
            edge.IsLabelTextUnderlined = scriptableEdge.IsLabelTextUnderlined;
            edge.LabelBackgroundColor = GetColor(scriptableEdge.LabelBackgroundColor);
            edge.LabelForegroundColor = GetColor(scriptableEdge.LabelForegroundColor);
            edge.LabelFontStyle = GetFontStyle(scriptableEdge.LabelFontStyle);
            edge.LabelFontWeight = GetFontWeight(scriptableEdge.LabelFontWeight);
            if (!String.IsNullOrEmpty(scriptableEdge.LabelFont))
            {
                FontFamily fontFamily = new FontFamily(scriptableEdge.LabelFont);
                edge.LabelFont = fontFamily;
            }
            edge.Weight = scriptableEdge.Weight;

            // Attributes
            foreach (KeyValuePair<string, ScriptableAttributeMapData> kvp in scriptableEdge.Attributes)
            {
                ScriptableAttributeMapData scriptableAttribute = kvp.Value;

                AttributeMapData attribute = new AttributeMapData(scriptableAttribute.Name, scriptableAttribute.Value);
                attribute.SemanticType = (SemanticType)scriptableAttribute.SemanticType;
                attribute.SimilarityMeasure = scriptableAttribute.SimilarityMeasure;
                attribute.IsHidden = scriptableAttribute.IsHidden;

                edge.Attributes.Add(kvp.Key, attribute);
            }

            return edge;
        }

        private static Size GetDimension(ScriptableSize scriptableDimension)
        {
            Size dimension = new Size(scriptableDimension.Width, scriptableDimension.Height);
            return dimension;
        }

        private static Point GetPosition(ScriptablePoint scriptablePosition)
        {
            Point position = new Point(scriptablePosition.X, scriptablePosition.Y);
            return position;
        }

        private static Color GetColor(string scriptableColor)
        {
            Color color = Colors.Transparent;

            if (!String.IsNullOrEmpty(scriptableColor))
            {
                scriptableColor = scriptableColor.Replace("#", String.Empty);

                string aStr = scriptableColor.Substring(0, 2);
                string rStr = scriptableColor.Substring(2, 2);
                string gStr = scriptableColor.Substring(4, 2);
                string bStr = scriptableColor.Substring(6, 2);

                byte a = Convert.ToByte(aStr, 16);
                byte r = Convert.ToByte(rStr, 16);
                byte g = Convert.ToByte(gStr, 16);
                byte b = Convert.ToByte(bStr, 16);

                color = Color.FromArgb(a, r, g, b);
            }

            return color;
        }

        private static FontStyle GetFontStyle(string scriptableFontStyle)
        {
            FontStyle fontStyle;

            switch (scriptableFontStyle)
            {
                case "Italic":
                    fontStyle = FontStyles.Italic;
                    break;

                case "Normal":
                    fontStyle = FontStyles.Normal;
                    break;

                default:
                    throw new ArgumentException("'" + scriptableFontStyle + "'  is not a valid FontStyle");
            }

            return fontStyle;
        }

        private static FontWeight GetFontWeight(string scriptableFontWeight)
        {
            FontWeight fontWeight;

            switch (scriptableFontWeight)
            {
                case "Black":
                    fontWeight = FontWeights.Black;
                    break;

                case "Bold":
                    fontWeight = FontWeights.Bold;
                    break;

                case "ExtraBlack":
                    fontWeight = FontWeights.ExtraBlack;
                    break;

                case "ExtraBold":
                    fontWeight = FontWeights.ExtraBold;
                    break;

                case "ExtraLight":
                    fontWeight = FontWeights.ExtraLight;
                    break;

                case "Light":
                    fontWeight = FontWeights.Light;
                    break;

                case "Medium":
                    fontWeight = FontWeights.Medium;
                    break;

                case "Normal":
                    fontWeight = FontWeights.Normal;
                    break;

                case "SemiBold":
                    fontWeight = FontWeights.SemiBold;
                    break;

                case "Thin":
                    fontWeight = FontWeights.Thin;
                    break;

                default:
                    throw new ArgumentException("'" + scriptableFontWeight + "'  is not a valid FontWeight");
            }

            return fontWeight;
        }
    }
}
