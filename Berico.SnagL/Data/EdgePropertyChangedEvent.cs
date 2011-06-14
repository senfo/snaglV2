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

namespace Berico.SnagL.Infrastructure.Data
{
    /// <summary>
    /// Represents the event that at least one of an edge's properties
    /// was changed
    /// </summary>
    public class EdgePropertyChangedEvent : CompositePresentationEvent<EdgeEventArgs>
    { }
}