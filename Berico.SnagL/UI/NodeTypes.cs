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
    /// Specifies the Node type
    /// </summary>
    public enum NodeTypes
    {
        /// <summary>
        /// Indicates a simple node that is represented as
        /// a circle with no label
        /// </summary>
        Simple,
        /// <summary>
        /// Indicates a simple node that is represented as
        /// a small label
        /// </summary>
        Text,
        /// <summary>
        /// Indicates a node that is represented as an icon
        /// with a label
        /// </summary>
        Icon
    }
}