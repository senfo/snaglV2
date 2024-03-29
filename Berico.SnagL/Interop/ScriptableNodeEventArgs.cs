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
using System.Windows.Browser;
using Berico.SnagL.Infrastructure.Graph.Events;

namespace Berico.SnagL.Infrastructure.Interop
{
    /// <summary>
    /// Represents Node event arguments for scriptable events
    /// </summary>
    public class ScriptableNodeEventArgs : EventArgs
    {
        /// <summary>
        /// Prevents an instance of the <see cref="ScriptableNodeEventArgs"/> class from being instantiated
        /// </summary>
        private ScriptableNodeEventArgs()
        {
        }

        /// <summary>
        /// Creates a new instance of the ScriptableNodeEventArgs based
        /// on the event arguments provided
        /// </summary>
        /// <param name="originalArgs">The original event arguments</param>
        /// <returns>a configured ScriptableNodeEventArgs instance</returns>
        public static ScriptableNodeEventArgs Create(NodeViewModelEventArgs originalArgs)
        {
            ScriptableNodeEventArgs args = new ScriptableNodeEventArgs();

            args.Id = originalArgs.NodeViewModel.ParentNode.ID;
            args.X = originalArgs.NodeViewModel.Position.X;
            args.Y = originalArgs.NodeViewModel.Position.Y;
            args.Visible = !originalArgs.NodeViewModel.IsHidden;
            args.SourceMechanism = Enum.GetName(typeof(Model.CreationType), originalArgs.NodeViewModel.ParentNode.SourceMechanism);


            // Ensure that there are attributes available before trying
            // to get a string to represent them
            if (originalArgs.NodeViewModel.ParentNode.Attributes == null || originalArgs.NodeViewModel.ParentNode.Attributes.Count == 0)
                args.Attributes = string.Empty;
            else
                args.Attributes = originalArgs.NodeViewModel.ParentNode.Attributes.ToJSON();

            return args;
        }

        /// <summary>
        /// Gets the ID of the target node
        /// </summary>
        [ScriptableMember]
        public string Id { get; private set; }

        /// <summary>
        /// Gets  the X position of the node
        /// </summary>
        [ScriptableMember]
        public double X { get; private set; }

        /// <summary>
        /// Gets the Y position of the node
        /// </summary>
        [ScriptableMember]
        public double Y { get; private set; }

        /// <summary>
        /// Gets whether the node is visible or not
        /// </summary>
        [ScriptableMember]
        public bool Visible { get; private set; }

        /// <summary>
        /// Gets the mechanism that created the node
        /// </summary>
        [ScriptableMember]
        public string SourceMechanism { get; private set; }

        /// <summary>
        /// Gets a Json string representing the nodes attributes
        /// </summary>
        [ScriptableMember]
        public string Attributes { get; private set; }
    }
}