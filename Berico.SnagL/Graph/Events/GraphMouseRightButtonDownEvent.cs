//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using Microsoft.Practices.Prism.Events;
using System;
using System.Windows.Input;

namespace Berico.SnagL.Infrastructure.Graph.Events
{
    /// <summary>
    /// Represents the mouse right button down event for the graph
    /// </summary>
    public class GraphMouseRightButtonDownEvent : CompositePresentationEvent<MouseButtonEventArgs>
    { }
}