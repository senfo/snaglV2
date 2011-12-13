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
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;
    using Berico.Common;

    public class NodeMapData
    {
        /// <summary>
        /// how this node is identified/referenced
        /// all nodes on a graph will have a unique id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// indicates whether the node is visible on the graph
        /// </summary>
        public bool IsHidden { get; set; }

        /// <summary>
        /// The color used to show a node is selected
        /// </summary>
        public Color SelectionColor { get; set; }

        /// <summary>
        /// The color of the background of the node.  This can be used to highlight a node.
        /// </summary>
        public Color BackgroundColor { get; set; }

        /// <summary>
        /// The value to be displayed under the node
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// A description of the node.  This isn’t currently used by the application itself but can be used to indicate the mechanism by which a node was created.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The X and Y position for the location of the node.  If all nodes have no position or the same position, the graph will be layed out initially.
        /// </summary>
        public Point Position { get; set; }

        /// <summary>
        /// The Width and Height of the node (this could effect how the node appears on the graph)
        /// </summary>
        public Size Dimension { get; set; }

        // Attributes
        public IDictionary<string, AttributeMapData> Attributes;

        /// <summary>
        /// Vertex
        /// </summary>
        /// <param name="id">how this node is identified/referenced
        /// all nodes on a graph will have a unique id</param>
        public NodeMapData(string id)
        {
            Id = id;

            BackgroundColor = Colors.Transparent;
            Dimension = new Size();
            Position = new Point();
            Attributes = new Dictionary<string, AttributeMapData>(0);
            SelectionColor = Conversion.HexColorToBrush("#FF4A75A9").Color; // Assign a default color
        }
    }
}
