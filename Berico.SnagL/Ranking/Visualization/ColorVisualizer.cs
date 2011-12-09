//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using System.Collections.Generic;
using System.Windows.Media;
using Berico.SnagL.Infrastructure.Graph;

namespace Berico.SnagL.Infrastructure.Ranking.Visualization
{
    /// <summary>
    /// Visualizes the importance of a node by altering
    /// it's color
    /// </summary>
    public class ColorVisualizer : IVisualizer
    {
        private double _range = double.NaN;
        private readonly Color _lowerColor = Colors.Blue;
        private readonly Color _upperColor = Colors.Red;
        private readonly Dictionary<string, Color> _storedColors = new Dictionary<string, Color>();
        private Color _delta;

        /// <summary>
        /// Initializes a new instance of ColorVisualizer class
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public ColorVisualizer(double min, double max)
        {
            _range = max - min;
            _delta = new Color
                {
                    R = (byte)(_upperColor.R - _lowerColor.R),
                    G = (byte)(_upperColor.G - _lowerColor.G),
                    B = (byte)(_upperColor.B - _lowerColor.B)
                };
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset(double min, double max)
        {
            _range = max - min;
            _delta = new Color
            {
                R = (byte)(_upperColor.R - _lowerColor.R),
                G = (byte)(_upperColor.G - _lowerColor.G),
                B = (byte)(_upperColor.B - _lowerColor.B)
            };
        }

        public void Clear()
        {
            _storedColors.Clear();
        }

        /// <summary>
        /// Visualizes the target node's importance by altering it's color
        /// </summary>
        /// <param name="target">The node that is the target for this visualization</param>
        /// <param name="importance">A value that indicates how important the target
        ///   is.  This value affects the target visualization.</param>
        public void Visualize(NodeViewModelBase target, double importance)
        {
            // Compute the color that should be used
            Color computedColor = new Color
            {
                A = 255,
                R = (byte)(_delta.R * importance / _range + _lowerColor.R),
                G = (byte)(_delta.G * importance / _range + _lowerColor.G),
                B = (byte)(_delta.B * importance / _range + _lowerColor.B)
            };

            // Check if the current background color is Transparent
            //if (target.BackgroundColor.Color != Colors.Transparent)
            //{
                // Check if the color for this node has already
                // been saved
                if (!_storedColors.ContainsKey(target.ParentNode.ID))
                {
                    // Save the color for this node
                    _storedColors.Add(target.ParentNode.ID, target.BackgroundColor.Color);
                }
            //}

            // Set the node's background color
            target.BackgroundColor = new SolidColorBrush(computedColor);
        }

        /// <summary>
        /// Removes the color visualization from the target node
        /// </summary>
        /// <param name="target">The node whose visualization is to be removed</param>
        public void ClearVisualization(NodeViewModelBase target)
        {
            target.BackgroundColor = _storedColors.ContainsKey(target.ParentNode.ID) ? new SolidColorBrush(_storedColors[target.ParentNode.ID]) : new SolidColorBrush(Colors.Transparent);
        }
    }
}
