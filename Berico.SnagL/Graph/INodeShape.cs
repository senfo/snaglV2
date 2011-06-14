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