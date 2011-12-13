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
    using System.Windows;
    using System.Windows.Media;
    using Berico.SnagL.Infrastructure.Data.Attributes;
    using Berico.SnagL.Infrastructure.Data.Mapping;
    using Berico.SnagL.Infrastructure.Graph;
    using Berico.SnagL.Model;
    using Berico.SnagL.Model.Attributes;
    using Berico.SnagL.UI;

    /// <summary>
    /// Provides conversion methods for converting GraphMapData to
    /// and from GraphComponents
    /// </summary>
    public static class MappingExtensions
    {
        /// <summary>
        /// Converts a GraphMapData instance, with data from an import operation,
        /// to a Graph (GraphComponents)
        /// </summary>
        /// <param name="graph">The mapping data to be imported into the Graph</param>
        /// <param name="graphComponents">The Graph that data is being imported into</param>
        /// <param name="creationType">The specified CreationType</param>
        public static void ImportGraph(this GraphMapData graph, GraphComponents graphComponents, CreationType creationType)
        {
            // Ensure that valid mapping data was provided
            if (graph == null)
            {
                throw new ArgumentNullException("graph", "No mapping data was provided");
            }

            graphComponents.NodeType = NodeTypes.Text;
            foreach (NodeMapData objNode in graph.GetNodes())
            {
                if (objNode is IconNodeMapData)
                {
                    graphComponents.NodeType = NodeTypes.Icon;
                    break;
                }
            }

            // TODO edgedefault?

            // Loop over the node mapping objects
            foreach (NodeMapData objNode in graph.GetNodes())
            {
                AddNode(graphComponents, creationType, objNode);
            }

            // Edges
            foreach (EdgeMapData objEdge in graph.GetEdges())
            {
                AddEdge(graphComponents, creationType, objEdge);
            }
        }

        /// <summary>
        /// Adds the specificed node
        /// </summary>
        /// <param name="graphComponents">The Graph that data is being imported into</param>
        /// <param name="creationType">The specified CreationType</param>
        /// <param name="objNode">Node to be added</param>
        public static void AddNode(GraphComponents graphComponents, CreationType creationType, NodeMapData objNode)
        {
            // Create new node
            Node uiNode = new Node(objNode.Id);
            uiNode.SourceMechanism = creationType;

            // TODO as NodeMapData types expands, this needs to be adjusted
            NodeTypes uiNodeType = NodeTypes.Simple;
            if (objNode is IconNodeMapData)
            {
                uiNodeType = NodeTypes.Icon;
            }
            else if (objNode is TextNodeMapData)
            {
                uiNodeType = NodeTypes.Text;
            }

            NodeViewModelBase uiNodeVM = NodeViewModelBase.GetNodeViewModel(uiNodeType, uiNode, graphComponents.Scope);

            // Properties
            if (uiNodeType == NodeTypes.Icon)
            {
                IconNodeMapData objIconNode = (IconNodeMapData)objNode;
                if (objIconNode.ImageSource != null)
                {
                    ((IconNodeViewModel)uiNodeVM).ImageSource = objIconNode.ImageSource.ToString();
                }
            }

            uiNodeVM.Description = objNode.Description;
            uiNodeVM.DisplayValue = objNode.Label;
            uiNodeVM.Width = objNode.Dimension.Width;
            uiNodeVM.Height = objNode.Dimension.Height;
            uiNodeVM.Position = objNode.Position;
            uiNodeVM.IsHidden = objNode.IsHidden;

            SolidColorBrush uiBackgroundColorBrush = new SolidColorBrush(objNode.BackgroundColor);
            uiNodeVM.BackgroundColor = uiBackgroundColorBrush;

            SolidColorBrush uiSelectionColorBrush = new SolidColorBrush(objNode.SelectionColor);
            uiNodeVM.SelectionColor = uiSelectionColorBrush;

            if (uiNodeVM.Height == 0)
            {
                uiNodeVM.Height = 45;
            }

            if (uiNodeVM.Width == 0)
            {
                uiNodeVM.Width = 45;
            }

            // Add the node to the graph
            graphComponents.AddNodeViewModel(uiNodeVM);

            // Attributes
            foreach (KeyValuePair<string, AttributeMapData> objNodeAttrKVP in objNode.Attributes)
            {
                Attributes.Attribute uiNodeAttribute = new Attributes.Attribute(objNodeAttrKVP.Value.Name);
                AttributeValue uiNodeAttributeValue = new AttributeValue(objNodeAttrKVP.Value.Value);

                uiNode.Attributes.Add(uiNodeAttribute.Name, uiNodeAttributeValue);
                GlobalAttributeCollection.GetInstance(graphComponents.Scope).Add(uiNodeAttribute, uiNodeAttributeValue);

                uiNodeAttribute.SemanticType = objNodeAttrKVP.Value.SemanticType;
                uiNodeAttribute.PreferredSimilarityMeasure = objNodeAttrKVP.Value.SimilarityMeasure;
                uiNodeAttribute.Visible = !objNodeAttrKVP.Value.IsHidden;
            }
        }

        /// <summary>
        /// Adds the specificed edge
        /// </summary>
        /// <param name="graphComponents">The Graph that data is being imported into</param>
        /// <param name="creationType">The specified CreationType</param>
        /// <param name="objEdge">Edge to be added</param>
        public static void AddEdge(GraphComponents graphComponents, CreationType creationType, EdgeMapData objEdge)
        {
            INode uiSourceNode = graphComponents.Data.GetNode(objEdge.Source);
            if (uiSourceNode == null && creationType == CreationType.Imported)
            {
                throw new Exception("Missing Source Node");
            }
            else if (uiSourceNode == null)// && creationType == CreationType.Live
            {
                uiSourceNode = new GhostNode(objEdge.Source);
            }

            INode uiTargetNode = graphComponents.Data.GetNode(objEdge.Target);
            if (uiTargetNode == null && creationType == CreationType.Imported)
            {
                throw new Exception("Missing Target Node");
            }
            else if (uiTargetNode == null)// && creationType == CreationType.Live
            {
                uiTargetNode = new GhostNode(objEdge.Target);
            }

            if (string.IsNullOrEmpty(objEdge.Label) && objEdge.Attributes.Count == 0)
            {
                Berico.SnagL.Model.Edge uiEdge = new Berico.SnagL.Model.Edge(uiSourceNode, uiTargetNode);
                uiEdge.SourceMechanism = creationType;

                // Properties
                uiEdge.Type = objEdge.Type;

                // the EdgeViewModel must be created after uiEdge has had a Type specified
                IEdgeViewModel uiEdgeVM = EdgeViewModelBase.GetEdgeViewModel(uiEdge, graphComponents.Scope);
                graphComponents.AddEdgeViewModel(uiEdgeVM);
            }
            else
            {
                DataEdge uiEdge = new DataEdge(uiSourceNode, uiTargetNode);
                uiEdge.SourceMechanism = creationType;

                // Properties
                uiEdge.Type = objEdge.Type;
                uiEdge.DisplayValue = objEdge.Label;

                // the EdgeViewModel must be created after uiEdge has had a Type specified
                IEdgeViewModel uiEdgeVM = EdgeViewModelBase.GetEdgeViewModel(uiEdge, graphComponents.Scope);
                graphComponents.AddEdgeViewModel(uiEdgeVM);

                uiEdgeVM.Thickness = objEdge.Thickness;
                uiEdgeVM.Color = new SolidColorBrush(objEdge.Color);
                uiEdgeVM.EdgeLine.Text = objEdge.Label;
                uiEdgeVM.EdgeLine.LabelTextUnderline = objEdge.IsLabelTextUnderlined;
                uiEdgeVM.EdgeLine.LabelBackgroundColor = new SolidColorBrush(objEdge.LabelBackgroundColor);
                uiEdgeVM.EdgeLine.LabelForegroundColor = new SolidColorBrush(objEdge.LabelForegroundColor);
                uiEdgeVM.EdgeLine.LabelFontStyle = objEdge.LabelFontStyle;
                uiEdgeVM.EdgeLine.LabelFontWeight = objEdge.LabelFontWeight;
                if (objEdge.LabelFont != null)
                {
                    uiEdgeVM.EdgeLine.LabelFont = objEdge.LabelFont;
                }

                // Attributes
                foreach (KeyValuePair<string, AttributeMapData> objEdgeAttrKVP in objEdge.Attributes)
                {
                    Attributes.Attribute uiEdgeAttribute = new Attributes.Attribute(objEdgeAttrKVP.Value.Name);
                    AttributeValue uiEdgeAttributeValue = new AttributeValue(objEdgeAttrKVP.Value.Value);

                    uiEdge.Attributes.Add(uiEdgeAttribute.Name, uiEdgeAttributeValue);
                    //GlobalAttributeCollection.GetInstance(graphComponents.Scope).Add(uiEdgeAttribute, uiEdgeAttributeValue);

                    uiEdgeAttribute.SemanticType = objEdgeAttrKVP.Value.SemanticType;
                    uiEdgeAttribute.PreferredSimilarityMeasure = objEdgeAttrKVP.Value.SimilarityMeasure;
                    uiEdgeAttribute.Visible = !objEdgeAttrKVP.Value.IsHidden;
                }
            }
        }

        /// <summary>
        /// Converts the provided GraphComponents instance to a GraphMapData
        /// instance that can be exported to a target file format
        /// </summary>
        /// <param name="graphComponents">The graph to be exported</param>
        /// <returns>A GraphMapData instance ready to be exported to the target format</returns>
        public static GraphMapData ExportGraph(this GraphComponents graphComponents)
        {
            GraphMapData graph = new GraphMapData();

            // Nodes
            IEnumerable<INodeShape> uiNodeViewModels = graphComponents.GetNodeViewModels();
            foreach (NodeViewModelBase uiNodeVM in uiNodeViewModels)
            {
                NodeMapData objNode = GetNode(uiNodeVM);
                graph.Add(objNode);
            }

            // Edges
            IEnumerable<IEdgeViewModel> edgeViewModels = graphComponents.GetEdgeViewModels();
            foreach (EdgeViewModelBase uiEdge in edgeViewModels)
            {
                EdgeMapData objEdge = GetEdge(uiEdge);
                graph.Add(objEdge);
            }

            return graph;
        }

        public static NodeMapData GetNode(NodeViewModelBase uiNodeVM)
        {
            NodeMapData objNode;
            if (uiNodeVM.GetType().Equals(typeof(IconNodeViewModel)))
            {
                objNode = new IconNodeMapData(uiNodeVM.ParentNode.ID);

                // Property
                IconNodeViewModel iconNodeVM = (IconNodeViewModel)uiNodeVM;
                if (iconNodeVM.ImageSource != null)
                {
                    ((IconNodeMapData)objNode).ImageSource = new System.Uri(iconNodeVM.ImageSource, UriKind.Relative);
                }
            }
            else
            {
                objNode = new TextNodeMapData(uiNodeVM.ParentNode.ID);
            }

            // Properties
            objNode.Description = uiNodeVM.Description;
            objNode.Label = uiNodeVM.DisplayValue;
            Size dimension = new Size(uiNodeVM.Width, uiNodeVM.Height);
            objNode.Dimension = dimension;
            objNode.Position = uiNodeVM.Position;
            objNode.IsHidden = uiNodeVM.IsHidden;
            objNode.BackgroundColor = uiNodeVM.BackgroundColor.Color;
            objNode.SelectionColor = uiNodeVM.SelectionColor.Color;

            // Attributes
            foreach (KeyValuePair<string, AttributeValue> uiNodeVMAttrKVP in uiNodeVM.ParentNode.Attributes)
            {
                Attributes.Attribute uiNodeVMAttribute = GlobalAttributeCollection.GetInstance(uiNodeVM.Scope).GetAttribute(uiNodeVMAttrKVP.Key);

                AttributeMapData objNodeAttribute = new AttributeMapData(uiNodeVMAttrKVP.Key, uiNodeVMAttrKVP.Value.Value);
                objNode.Attributes.Add(objNodeAttribute.Name, objNodeAttribute);

                objNodeAttribute.SemanticType = uiNodeVMAttribute.SemanticType;
                objNodeAttribute.SimilarityMeasure = uiNodeVMAttribute.PreferredSimilarityMeasure;
                objNodeAttribute.IsHidden = !uiNodeVMAttribute.Visible;
            }

            return objNode;
        }

        private static EdgeMapData GetEdge(EdgeViewModelBase uiEdge)
        {
            EdgeMapData objEdge = new EdgeMapData(uiEdge.ParentEdge.Source.ID, uiEdge.ParentEdge.Target.ID);

            // Properties
            objEdge.Type = uiEdge.ParentEdge.Type;
            objEdge.IsLabelTextUnderlined = uiEdge.EdgeLine.LabelTextUnderline;
            objEdge.Thickness = uiEdge.Thickness;
            objEdge.Color = ((SolidColorBrush)uiEdge.Color).Color;
            objEdge.LabelBackgroundColor = ((SolidColorBrush)uiEdge.LabelBackgroundColor).Color;
            objEdge.LabelForegroundColor = ((SolidColorBrush)uiEdge.LabelForegroundColor).Color;
            objEdge.LabelFontStyle = uiEdge.LabelFontStyle;
            objEdge.LabelFontWeight = uiEdge.LabelFontWeight;
            objEdge.LabelFont = uiEdge.LabelFont;

            if (uiEdge.ParentEdge.GetType().Equals(typeof(DataEdge)))
            {
                DataEdge dataEdge = (DataEdge)uiEdge.ParentEdge;
                objEdge.Label = dataEdge.DisplayValue;

                // Attributes
                foreach (KeyValuePair<string, AttributeValue> uiDataEdgeAttrKVP in dataEdge.Attributes)
                {
                    //Attributes.Attribute uiEdgeAttribute = GlobalAttributeCollection.GetInstance(scope).GetAttribute(uiDataEdgeAttrKVP.Key);

                    AttributeMapData objEdgeAttribute = new AttributeMapData(uiDataEdgeAttrKVP.Key, uiDataEdgeAttrKVP.Value.Value);
                    objEdge.Attributes.Add(objEdgeAttribute.Name, objEdgeAttribute);

                    //objEdgeAttribute.SemanticType = uiEdgeAttribute.SemanticType;
                    //objEdgeAttribute.SimilarityMeasure = uiEdgeAttribute.PreferredSimilarityMeasure;
                    //objEdgeAttribute.IsHidden = !uiEdgeAttribute.Visible;
                }
            }

            return objEdge;
        }
    }
}
