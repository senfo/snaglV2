﻿//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using System.Windows;

namespace Berico.SnagL.Infrastructure.Graph
{
    public interface INodeShape : IScopingContainer<string>
    {
        Point Position { get; set; }
        Point CenterPoint { get; }
        double Height { get; set; }
        double Width { get; set; }
        bool IsHidden { get; set; }
    }
}