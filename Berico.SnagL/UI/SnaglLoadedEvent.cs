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

namespace Berico.SnagL.UI
{
    /// <summary>
    /// Represents the event that indicates that the Snagl application has 
    /// fully loaded.  Just using the onLoaded event of the Silverlight 
    /// plug-in is not good enough because that only fires after the
    /// root visual element has loaded.  We need to ensure that the 
    /// GraphSurface object has loaded.
    /// </summary>
    public class SnaglLoadedEvent : CompositePresentationEvent<SnaglLoadedEventArgs>
    { }
}