﻿//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using System.Windows.Media;
using System.Windows.Shapes;
using Berico.SnagL.Infrastructure.Graph;
using Berico.SnagL.Model;
using Berico.SnagL.UI;

namespace Berico.SnagL.UI
{
    /// <summary>
    /// Provides the code for a standard (normal) edge view
    /// </summary>
    public class StandardEdgeViewModel : EdgeViewModelBase
    {

        /// <summary>
        /// Create a new instance of the Berico.LinkAnalysis.ViewModel.StandardEdgeViewModel
        /// class using the provided parent edge
        /// </summary>
        /// <param name="parentEdge">The edge data represented by the view and viewmodel</param>
        /// <param name="scope">Identifies the scope of this edge view model</param>
        public StandardEdgeViewModel(IEdge parentEdge, string scope) : base(parentEdge, scope) { }

        /// <summary>
        /// Performs initialization
        /// </summary>
        protected override void Initialize()
        {
            // Specify the style for the edge line
            EdgeLine edgeLine = new EdgeLine(ParentEdge.Type)
            {
                Opacity = 1,
                Color = new SolidColorBrush(Colors.Black),
                Thickness = 2
            };

            this.EdgeLine = edgeLine;
        }

    }
}