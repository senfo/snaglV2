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

namespace Berico.SnagL.Infrastructure.Graph.Events
{
    /// <summary>
    /// Represents the mouse left button up event for nodes
    /// </summary>
    public class NodeMouseLeftButtonUpEvent : CompositePresentationEvent<NodeViewModelMouseEventArgs<System.Windows.Input.MouseButtonEventArgs>>
    { }
}