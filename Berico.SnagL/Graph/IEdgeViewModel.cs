//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using Berico.SnagL.Model;
using System.Windows.Media;
using Berico.SnagL.UI;

namespace Berico.SnagL.Infrastructure.Graph
{
    /// <summary>
    /// Provides the definition for all EdgeViewModel classes.  An EdgeViewModel
    /// class provides the code behind for all Edge views.
    /// </summary>
    public interface IEdgeViewModel
    {

        /// <summary>
        /// Gets the parent Edge that the view and viewmodel represent
        /// </summary>
        IEdge ParentEdge { get; }

        /// <summary>
        /// Gets or sets the X property for the starting
        /// point of the line
        /// </summary>
        double X1 { get; set; }

        /// <summary>
        /// Gets or sets the Y property for the starting
        /// point of the line
        /// </summary>
        double Y1 { get; set; }

        /// <summary>
        /// Gets or sets the X property for the ending
        /// point of the line
        /// </summary>
        double X2 { get; set; }

        /// <summary>
        /// Gets or sets the Y property for the ending
        /// point of the line
        /// </summary>
        double Y2 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        double Thickness { get; set; }

        /// <summary>
        /// 
        /// </summary>
        Brush Color { get; set; }

        /// <summary>
        /// Gets or sets whether the edge is visible or not
        /// </summary>
        Visibility Visibility { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool IsHidden { get; }

        /// <summary>
        /// Gets the physical line for this edge
        /// </summary>
        IEdgeLine EdgeLine { get; set;  }

        /// <summary>
        /// Draws the edge line (and all its parts) on the provided container
        /// </summary>
        /// <param name="parentContainer">The Canvas that the edge should be drawn on</param>
        void DrawEdgeLine(Canvas parentContainer);

        /// <summary>
        /// Erases the edge line (and all its parts) from the provided container
        /// </summary>
        /// <param name="parentContainer">The Canvas that the edge should be removed from</param>
        void EraseEdgeLine(Canvas parentContainer);
    }
}