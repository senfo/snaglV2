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
    using Berico.SnagL.Model;

    public class EdgeMapData
    {
        /// <summary>
        /// Origination point of the Edge
        /// </summary>
        public string Source { get; private set; }

        /// <summary>
        /// Destination point of the Edge
        /// </summary>
        public string Target { get; private set; }

        /// <summary>
        /// Specifies the direction of the arrow for the edge
        /// </summary>
        public EdgeType Type { get; set; }



        // properties

        /// <summary>
        /// The color of the Edge
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// A number indicating the thickness of the edge
        /// </summary>
        public double Thickness { get; set; }

        /// <summary>
        /// The value to be displayed on the edge
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Label’s background color.  If not specified, the default is white.
        /// </summary>
        public Color LabelBackgroundColor { get; set; }

        /// <summary>
        /// Label’s foreground color.  If not specified, the default is black.
        /// </summary>
        public Color LabelForegroundColor { get; set; }

        /// <summary>
        /// The name of the font used for the edge’s label.  This can be any font that exists on the client machine.
        /// </summary>
        public FontFamily LabelFont { get; set; }

        /// <summary>
        /// This can be either Italic or Normal.  The default is Normal.
        /// </summary>
        public FontStyle LabelFontStyle { get; set; }

        /// <summary>
        /// Set this to true if the text on the label should be underlined
        /// </summary>
        public bool IsLabelTextUnderlined { get; set; }

        /// <summary>
        /// This can be one of the following values { Black, Bold, ExtraBlack, ExtraBold, ExtraLight, Light, Medium, Normal, SemiBold, Thin }
        /// Default is Normal
        /// </summary>
        public FontWeight LabelFontWeight { get; set; }

        /// <summary>
        /// See Clustering
        /// </summary>
        public double Weight { get; set; }

        // attributes
        public IDictionary<string, AttributeMapData> Attributes { get; set; }

        /// <summary>
        /// Link between two nodes
        /// </summary>
        /// <param name="source">Origination point of the Edge</param>
        /// <param name="target">Destination point of the Edge</param>
        public EdgeMapData(string source, string target)
        {
            Source = source;
            Target = target;

            Type = EdgeType.Undirected;

            Thickness = 1D;

            Color = Colors.Black;
            LabelBackgroundColor = Colors.Transparent;
            LabelForegroundColor = Colors.Black;
            LabelFontStyle = FontStyles.Normal;
            LabelFontWeight = FontWeights.Normal;

            Attributes = new Dictionary<string, AttributeMapData>(0);
        }
    }
}
