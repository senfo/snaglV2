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

namespace Berico.SnagL.Infrastructure.Graph
{
    /// <summary>
    /// Defines the contract for a class that imports live data
    /// </summary>
    interface ILiveData
    {
        /// <summary>
        /// Occurs when an import of live data has completed
        /// </summary>
        event EventHandler<EventArgs> LiveDataEnded;

        /// <summary>
        /// Occurs when an import of live data is started
        /// </summary>
        event EventHandler<EventArgs> LiveDataStarted;

        /// <summary>
        /// Loads live data onto the graph
        /// </summary>
        /// <param name="xmlData">XML data to containing information to be placed on the graph</param>
        void LoadLiveData(string xmlData);
    }
}