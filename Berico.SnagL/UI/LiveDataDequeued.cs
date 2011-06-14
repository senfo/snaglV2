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
using Microsoft.Practices.Prism.Events;

namespace Berico.SnagL.UI
{
    /// <summary>
    /// Represents the event that occurs when live data is added to the queue
    /// </summary>
    public class LiveDataDequeuedEvent : CompositePresentationEvent<LiveDataEventArgs>
    {
        /// <summary>
        /// Initializes a new instance of the LiveDataDequeuedEvent class
        /// </summary>
        public LiveDataDequeuedEvent()
        {
        }
    }
}