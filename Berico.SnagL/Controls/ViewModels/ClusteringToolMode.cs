﻿//-------------------------------------------------------------
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

namespace Berico.SnagL.Infrastructure.Controls
{
    /// <summary>
    /// Specifies the mode for the clustering tool
    /// </summary>
    public enum ClusteringToolMode
    {
        /// <summary>
        /// Indicates that the clustering tool is operating
        /// in thh most basic mode
        /// </summary>
        Simple,
        /// <summary>
        /// Indicates that the clustering tool is operating
        /// in its advanced mode
        /// </summary>
        Advanced
    }
}