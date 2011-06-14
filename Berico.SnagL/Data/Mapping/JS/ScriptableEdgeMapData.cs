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
    public class ScriptableEdgeMapData
    {
        /// <summary>
        /// Origination point of the Edge
        /// </summary>
        [ScriptableMember()]
        public string Source { get; set; }

        /// <summary>
        /// Destination point of the Edge
        /// </summary>
        [ScriptableMember()]
        public string Target { get; set; }

        /// <summary>
        /// Specifies the direction of the arrow for the edge
        /// </summary>
        [ScriptableMember()]
        public string Type { get; set; }



        // properties

        /// <summary>
        /// The color of the Edge
        /// </summary>
        [ScriptableMember()]
        public string Color { get; set; }

        /// <summary>
        /// A number indicating the thickness of the edge
        /// </summary>
        [ScriptableMember()]
        public double Thickness { get; set; }

        /// <summary>
        /// The value to be displayed on the edge
        /// </summary>
        [ScriptableMember()]
        public string Label { get; set; }

        /// <summary>
        /// Label’s background color.  If not specified, the default is white.
        /// </summary>
        [ScriptableMember()]
        public string LabelBackgroundColor { get; set; }

        /// <summary>
        /// Label’s foreground color.  If not specified, the default is black.
        /// </summary>
        [ScriptableMember()]
        public string LabelForegroundColor { get; set; }

        /// <summary>
        /// The name of the font used for the edge’s label.  This can be any font that exists on the client machine.
        /// </summary>
        [ScriptableMember()]
        public string LabelFont { get; set; }

        /// <summary>
        /// This can be either Italic or Normal.  The default is Normal.
        /// </summary>
        [ScriptableMember()]
        public string LabelFontStyle { get; set; }

        /// <summary>
        /// Set this to true if the text on the label should be underlined
        /// </summary>
        [ScriptableMember()]
        public bool IsLabelTextUnderlined { get; set; }

        /// <summary>
        /// This can be one of the following values { Black, Bold, ExtraBlack, ExtraBold, ExtraLight, Light, Medium, Normal, SemiBold, Thin }
        /// Default is Normal
        /// </summary>
        [ScriptableMember()]
        public string LabelFontWeight { get; set; }

        /// <summary>
        /// See Clustering
        /// </summary>
        [ScriptableMember()]
        public double Weight { get; set; }

        // attributes
        [ScriptableMember()]
        public Dictionary<string, ScriptableAttributeMapData> Attributes { get; set; }

        /// <summary>
        /// Link between two nodes
        /// </summary>
        public ScriptableEdgeMapData()
        {
            Type = "Undirected";

            Thickness = 1D;

            Color = "#FF000000";// black
            LabelBackgroundColor = "#00FFFFFF";// transparent
            LabelForegroundColor = "#FF000000"; // black
            LabelFontStyle = "Normal";
            LabelFontWeight = "Normal";

            Attributes = new Dictionary<string, ScriptableAttributeMapData>(0);
        }
    }
}
