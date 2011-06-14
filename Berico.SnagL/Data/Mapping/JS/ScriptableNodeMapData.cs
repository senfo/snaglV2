//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

namespace Berico.SnagL.Infrastructure.Data.Mapping.JS
{
    using System.Collections.Generic;
    using System.Windows.Browser;

    [ScriptableType()]
    public class ScriptableNodeMapData
    {
        /// <summary>
        /// how this node is identified/referenced
        /// all nodes on a graph will have a unique id
        /// </summary>
        [ScriptableMember()]
        public string Id { get; set; }



        // Properties

        /// <summary>
        /// indicates whether the node is visible on the graph
        /// </summary>
        [ScriptableMember()]
        public bool IsHidden { get; set; }

        /// <summary>
        /// The color used to show a node is selected
        /// </summary>
        [ScriptableMember()]
        public string SelectionColor { get; set; }

        /// <summary>
        /// The color of the background of the node.  This can be used to highlight a node.
        /// </summary>
        [ScriptableMember()]
        public string BackgroundColor { get; set; }

        /// <summary>
        /// The value to be displayed under the node
        /// </summary>
        [ScriptableMember()]
        public string Label { get; set; }

        /// <summary>
        /// A description of the node.  This isn’t currently used by the application itself but can be used to indicate the mechanism by which a node was created.
        /// </summary>
        [ScriptableMember()]
        public string Description { get; set; }

        /// <summary>
        /// The X and Y position for the location of the node.  If all nodes have no position or the same position, the graph will be layed out initially.
        /// </summary>
        [ScriptableMember()]
        public ScriptablePoint Position { get; set; }

        /// <summary>
        /// The Width and Height of the node (this could effect how the node appears on the graph)
        /// </summary>
        [ScriptableMember()]
        public ScriptableSize Dimension { get; set; }



        // Attributes
        [ScriptableMember()]
        public Dictionary<string, ScriptableAttributeMapData> Attributes { get; set; }

        /// <summary>
        /// Vertex
        /// </summary>
        public ScriptableNodeMapData()
        {
            BackgroundColor = "#00FFFFFF";// transparent
            Dimension = new ScriptableSize();
            Position = new ScriptablePoint();
            Attributes = new Dictionary<string, ScriptableAttributeMapData>(0);
        }
    }
}
