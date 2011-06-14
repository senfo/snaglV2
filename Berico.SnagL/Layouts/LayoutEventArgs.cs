//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using System.Windows.Input;
using GalaSoft.MvvmLight;
using System;
using Berico.SnagL.Infrastructure.Events;
using Berico.SnagL.Infrastructure.Graph;
using Berico.SnagL.Infrastructure.Layouts;

namespace Berico.SnagL.Infrastructure.Layouts
{
    /// <summary>
    /// Represents arguments for events related to layouts
    /// </summary>
    public class LayoutEventArgs
    {
        /// <summary>
        /// Gets the name of the layout algorithm used to lay out the graph
        /// </summary>
        public string LayoutName
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new instance of the Berico.SnagL.Events.
        /// LayoutEventArgs class using the provided layout type
        /// and scope
        /// </summary>
        /// <param name="layoutName">The type of layout used to lay out the graph</param>
        /// <param name="_scope">The scope for the layout</param>
        public LayoutEventArgs(string layoutName)
        {
            LayoutName = layoutName;
        }
    }
}