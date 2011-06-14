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
using System.Windows.Threading;

namespace Berico.SnagL.Infrastructure.Media.Animation
{
    /// <summary>
    /// Represents the base clase for animations
    /// </summary>
    public abstract class AnimatorBase
    {
        private const int DEFAULT_DURATION = 1000;
        private const int DEFAULT_FRAMERATE = 50;

        private int frameRate;
        private int duration;
        private int numFrames = 0;
        private int currentFrame = 0;
        private DateTime startTime = DateTime.MaxValue;
        private DispatcherTimer timer;

        /// <summary>
        /// Creates a new instance of the AnimatorBase class
        /// </summary>
        public AnimatorBase()
        {
            this.duration = DEFAULT_DURATION;
            this.frameRate = DEFAULT_FRAMERATE;
        }

        /// <summary>
        /// Creates a new instance of the AnimatorBase class using
        /// the provided duration
        /// </summary>
        /// <param name="_duration">The duration for the animation</param>
        public AnimatorBase(int _duration) : this(_duration, DEFAULT_FRAMERATE) { }

        /// <summary>
        /// Creates a new instance of the AnimatorBase class using
        /// the provided duration and framerate
        /// </summary>
        /// <param name="_duration">The duration for the animation</param>
        /// <param name="_frameRate">The framerate for the animation</param>
        public AnimatorBase(int _duration, int _frameRate)
        {
            this.duration = _duration > 0 ? _duration : DEFAULT_DURATION;
            this.frameRate = _frameRate > 0 ? _frameRate : DEFAULT_FRAMERATE;
        }

        /// <summary>
        /// Gets or sets the duration of the animation
        /// </summary>
        public int Duration
        {
            get { return duration; }
            set { duration = value; }
        }

        /// <summary>
        /// Gets the total number of frames in the animation
        /// </summary>
        protected int TotalFrames
        {
            get { return numFrames; }
        }

        //TODO: REPLACE WEITH EVENTAGGREGATOR EVENTS
        public event EventHandler Completed;
        public event EventHandler FrameAnimated;

        /// <summary>
        /// Starts the animation
        /// </summary>
        public void Begin()
        {
            this.startTime = DateTime.Now;
            numFrames = duration / frameRate;
            Initialize();
            
            // Start up the time
            this.timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, frameRate);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        /// <summary>
        /// Handles the main animation's timer tick event
        /// </summary>
        /// <param name="sender">The object that fired the event</param>
        /// <param name="e">The arguments for the event</param>
        private void timer_Tick(object sender, EventArgs e)
        {
            // Check how long we have been running.  We should
            // not run passed the specified duration.
            if (currentFrame >= numFrames)
            {
                Stop();
                return;
            }

            // Animated a frame
            AnimateFrame(currentFrame);
            currentFrame += 1;

            // Fire the frame animated event
            OnFrameAnimated(EventArgs.Empty);
        }

        /// <summary>
        /// Initializtion routine which must be implemented
        /// by a parent class
        /// </summary>
        protected abstract void Initialize();

        /// <summary>
        /// Provides the code to animate a frame, which must be
        /// implemented by a parent class
        /// </summary>
        /// <param name="currentFrame">The current frame number</param>
        protected abstract void AnimateFrame(int currentFrame);

        /// <summary>
        /// Stops the animation
        /// </summary>
        protected virtual void Stop()
        {
            this.timer.Stop();

            //TODO: SHOULD WE FIRE STOPPED INSTEAD OF COMPLETED?
            OnCompleted(EventArgs.Empty);
        }

        /// <summary>
        /// Fires the FrameAnimated event
        /// </summary>
        /// <param name="e">The arguments for the event</param>
        protected virtual void OnFrameAnimated(EventArgs e)
        {
            if (FrameAnimated != null)
            {
                FrameAnimated(this, e);
            }
        }

        /// <summary>
        /// Fires the Completed event
        /// </summary>
        /// <param name="e">The arguments for the event</param>
        protected virtual void OnCompleted(EventArgs e)
        {
            if (Completed != null)
            {
                Completed(this, e);
            }
        }
    }
}