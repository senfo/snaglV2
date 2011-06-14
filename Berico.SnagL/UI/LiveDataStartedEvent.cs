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
using Microsoft.Practices.Prism.Events;

namespace Berico.SnagL.UI
{
    /// <summary>
    /// Represents the event that occurs when live data is started
    /// </summary>
    public class LiveDataStartedEvent : CompositePresentationEvent<EventArgs>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LiveDataStartedEvent"/> class
        /// </summary>
        public LiveDataStartedEvent()
        {
        }
    }
}