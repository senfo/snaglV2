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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Berico.SnagL.UI
{
    /// <summary>
    /// Specifies the state of a Node
    /// </summary>
    public enum NodeStates
    {
        /// <summary>
        /// Indicates that the node is in a 
        /// normal state
        /// </summary>
        Normal,
        /// <summary>
        /// Indicates that the node has been
        /// selected
        /// </summary>
        Selected,
        /// <summary>
        /// Indicates that the nod has been 
        /// unselected
        /// </summary>
        Unselected
    }
}