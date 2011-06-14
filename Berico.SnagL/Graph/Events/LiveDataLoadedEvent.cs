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
    /// Represents the event that occurs when live data is loaded into the graph
    /// </summary>
    public class LiveDataLoadedEvent : CompositePresentationEvent<DataLoadedEventArgs>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LiveDataLoadedEvent"/> event
        /// </summary>
        public LiveDataLoadedEvent()
        {

        }
    }
}