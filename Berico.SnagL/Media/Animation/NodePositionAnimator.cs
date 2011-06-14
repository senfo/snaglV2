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
using System.Windows;
using Berico.SnagL.Infrastructure.Graph;

namespace Berico.SnagL.Infrastructure.Media.Animation
{
    /// <summary>
    /// Animates the movement of nodes from one postion to another
    /// </summary>
    public class NodePositionAnimator : AnimatorBase
    {

        private NodeViewModelBase targetNode = null;
        private Point targetPosition = new Point();
        private double incrementalX = 0;
        private double incrementalY = 0;

        /// <summary>
        /// Creates a new instance of the NodePositionAnimator class using
        /// the provided target node and target position
        /// </summary>
        /// <param name="_targetNode">The node being animated</param>
        /// <param name="_targetPosition">The position the node should be
        /// moved to</param>
        public NodePositionAnimator(NodeViewModelBase _targetNode, Point _targetPosition)
            : this(_targetNode, _targetPosition, 0, 0) { }

        /// <summary>
        /// Creates a new instance of the NodePositionAnimator class using
        /// the provided target node and target position
        /// </summary>
        /// <param name="_targetNode">The node being animated</param>
        /// <param name="_targetPosition">The position the node should be
        /// moved to</param>
        /// <param name="_duration">The duration of the animation</param>
        public NodePositionAnimator(NodeViewModelBase _targetNode, Point _targetPosition, int _duration)
            : this(_targetNode, _targetPosition, _duration, 0) { }

        /// <summary>
        /// Creates a new instance of the NodePositionAnimator class using
        /// the provided target node and target position
        /// </summary>
        /// <param name="_targetNode">The node being animated</param>
        /// <param name="_targetPosition">The position the node should be
        /// moved to</param>
        /// <param name="_duration">The duration of the animation</param>
        /// <param name="_frameRate">The frame rate of the animation</param>
        public NodePositionAnimator(NodeViewModelBase _targetNode, Point _targetPosition, int _duration, int _frameRate)
            : base(_duration, _frameRate)
        {
            this.targetNode = _targetNode;
            this.targetPosition = _targetPosition;
        }

        /// <summary>
        /// Initializes the class
        /// </summary>
        protected override void Initialize()
        {
            // Setup the incremental values
            incrementalX = (targetPosition.X - this.targetNode.Position.X) / TotalFrames;
            incrementalY = (targetPosition.Y - this.targetNode.Position.Y) / TotalFrames;
        }

        /// <summary>
        /// Positions the nodes for this single frame
        /// </summary>
        /// <param name="currentFrame">The frame to be animated</param>
        protected override void AnimateFrame(int currentFrame)
        {
            // Update the nodes position by the incremental amount
            double newX = this.targetNode.Position.X + incrementalX;
            double newY = this.targetNode.Position.Y + incrementalY;

            this.targetNode.Position = new Point(newX, newY);
        }

        /// <summary>
        /// Fires the completed event
        /// </summary>
        /// <param name="e">The arguments for the event</param>
        protected override void OnCompleted(EventArgs e)
        {
            base.OnCompleted(e);

            // Ensure that the node position is what it should be
            this.targetNode.Position = this.targetPosition;
        }
    }
}