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
using Berico.SnagL.Infrastructure.Graph.Events;

namespace Berico.SnagL.Infrastructure.Data.Formats
{
    /// <summary>
    /// Represents the event that data is being imported into the graph
    /// </summary>
    public class DataImportingEvent : CompositePresentationEvent<DataLoadedEventArgs>
    { }
}