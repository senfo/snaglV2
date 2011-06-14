//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using System;
using System.Windows;
using GalaSoft.MvvmLight;
using Berico.Windows.Controls;

namespace Berico.SnagL.Infrastructure.Graph.Events
{
    /// <summary>
    /// Represents arguments for general events fired by any
    /// SnagL ContextMenu
    /// </summary>
    public class ContextMenuEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetElement"></param>
        public ContextMenuEventArgs(ViewModelBase source)
        {
            Source = source;
        }

        /// <summary>
        /// 
        /// </summary>
        public ViewModelBase Source { get; private set; }


    }
}
