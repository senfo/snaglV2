//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using Berico.SnagL.Model;

namespace Berico.SnagL.Infrastructure.Data
{
    /// <summary>
    /// Represents arguments for events related to edges
    /// </summary>
    /// <typeparam name="T">Indicates the type used by the
    /// PropertyChanedEventArgs class</typeparam>
    public class EdgeEventArgs
    {
        /// <summary>
        /// Gets the edge that fired the event
        /// </summary>
        public IEdge AffectedEdge { get; private set; }

        /// <summary>
        /// Gets the arguments for the original property changed
        /// event.  Depending on the event itself, this may
        /// be null.
        /// </summary>
        public Common.Events.PropertyChangedEventArgs<object> PropertyChangedArguments { get; private set; }

        /// <summary>
        /// Creates a new instance of the Berico.SnagL.Events.
        /// EdgeEventArgs class using the provided IEdge,
        /// PropertyChangedEventArgs and scope.
        /// </summary>
        /// <param name="_node">The IEdge that fired the event</param>
        /// <param name="_args">The arguments related to the initial event</param>
        /// <param name="_sourceID">The ID for graph that this object belongs to</param>
        public EdgeEventArgs(IEdge _edge, Common.Events.PropertyChangedEventArgs<object> _propChangedArgs)
        {
            AffectedEdge = _edge;
            PropertyChangedArguments = _propChangedArgs;
        }

    }
}