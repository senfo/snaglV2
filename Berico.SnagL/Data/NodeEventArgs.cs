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
    /// Represents arguments for events related to nodes
    /// </summary>
    public class NodeEventArgs
    {
        /// <summary>
        /// Gets the node that fired the event
        /// </summary>
        public Node AffectedNode { get; private set; }

        /// <summary>
        /// Gets the arguments for the original property changed
        /// event.  Depending on the event itself, this may
        /// be null.
        /// </summary>
        public Common.Events.PropertyChangedEventArgs<string> PropertyChangedArguments { get; private set; }

        /// <summary>
        /// Creates a new instance of the Berico.SnagL.Events.
        /// NodeEventArgs class using the provided Node,
        /// PropertyChangedEventArgs and scope.
        /// </summary>
        /// <param name="_node">The Node that fired the event</param>
        /// <param name="_args">The arguments related to the initial event</param>
        /// <param name="_sourceID">The ID for graph that this object belongs to</param>
        public NodeEventArgs(Node _node, Common.Events.PropertyChangedEventArgs<string> _propChangedArgs)
        {
            AffectedNode = _node;
            PropertyChangedArguments = _propChangedArgs;
        }

    }
}