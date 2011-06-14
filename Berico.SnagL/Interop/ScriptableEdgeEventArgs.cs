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
using System.Collections.Generic;
using System.Windows.Browser;
using Berico.SnagL.Infrastructure.Graph.Events;
using Newtonsoft.Json;

namespace Berico.SnagL.Infrastructure.Interop
{
    /// <summary>
    /// Represents Edge event arguments for scriptable events
    /// </summary>
    public class ScriptableEdgeEventArgs : EventArgs
    {
        /// <summary>
        ///  Creates a new instance of ScriptableEdgeEventArgs
        /// </summary>
        private ScriptableEdgeEventArgs()
        { }

        /// <summary>
        /// Creates a new instance of the ScriptableEdgeEventArgs based
        /// on the event arguments provided
        /// </summary>
        /// <param name="originalArgs">The original event arguments</param>
        /// <returns>a configured ScriptableEdgeEventArgs instance</returns>
        public static ScriptableEdgeEventArgs Create(EdgeViewModelEventArgs originalArgs)
        {
            ScriptableEdgeEventArgs args = new ScriptableEdgeEventArgs();

            args.SourceId = originalArgs.EdgeViewModel.ParentEdge.Source.ID;
            args.TargetId = originalArgs.EdgeViewModel.ParentEdge.Target.ID;
            args.Visible = !originalArgs.EdgeViewModel.IsHidden;

            // Determine if the edge is a data egde
            if (originalArgs.EdgeViewModel.ParentEdge is Model.DataEdge)
                args.Attributes = (originalArgs.EdgeViewModel.ParentEdge as Model.DataEdge).Attributes.ToJSON();
            else
                args.Attributes = string.Empty;

            return args;
        }

        /// <summary>
        /// Gets the id of the target edge's source node
        /// </summary>
        [ScriptableMember] 
        public string SourceId { get; private set; }

        /// <summary>
        /// Gets  the id of the target edge's target node
        /// </summary>
        [ScriptableMember]
        public string TargetId { get; private set; }

        /// <summary>
        /// Gets whether the edge is visible or not
        /// </summary>
        [ScriptableMember]
        public bool Visible { get; private set; }

        /// <summary>
        /// Gets a Json string representing the edge's attributes
        /// </summary>
        [ScriptableMember]
        public string Attributes { get; private set; }

    }
}