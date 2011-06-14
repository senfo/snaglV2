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

namespace Berico.Common.Diagnostics
{
    /// <summary>
    /// This class mimics the StopWatch class available in
    /// the full .NET framework.  It provides a simple means
    /// to determine how much time has ellapsed.
    /// </summary>
    public class StopWatch
    {
        /// <summary>
        /// Gets the frequency of the timer as the number of ticks per second
        /// </summary>
        public static readonly long Frequency = TimeSpan.TicksPerSecond;

        private DateTime? StartUtc { get; set; }
        private DateTime? EndUtc { get; set; }

        /// <summary>
        /// Gets  whether or not the Stopwatch timer
        /// is running
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Gets the total elapsed time measured by the
        /// current instance
        /// </summary>
        public TimeSpan Elapsed
        {
            get
            {
                if (!this.StartUtc.HasValue)
                    return TimeSpan.Zero;

                if (!this.EndUtc.HasValue)
                    return (DateTime.UtcNow - this.StartUtc.Value);

                return (this.EndUtc.Value - this.StartUtc.Value);
            }
        }

        /// <summary>
        /// Gets the total elapsed time measured by the
        /// current instance, in timer ticks
        /// </summary>
        public long ElapsedTicks
        {
            get { return this.Elapsed.Ticks; }
        }

        /// <summary>
        /// Gets the total elapsed time measured by the
        /// current instance, in milliseconds
        /// </summary>
        public long ElapsedMilliseconds
        {
            get { return this.ElapsedTicks / TimeSpan.TicksPerMillisecond; }
        }

        /// <summary>
        /// Gets the current number of ticks in the timer mechanism
        /// </summary>
        /// <returns>a long representing the tick counter value
        /// of the underlying timer mechanism</returns>
        public static long GetTimeStamp()
        {
            return DateTime.UtcNow.Ticks;
        }

        /// <summary>
        /// Stops time interval measurement and resets the elapsed
        /// time to zero
        /// </summary>
        public void Reset()
        {
            // ensure that the stop watch is stopped
            Stop();

            // Rest the variables
            this.EndUtc = null;
            this.StartUtc = null;
        }

        /// <summary>
        /// Stops time interval measurement, resets the elapsed time
        /// to zero and starts measureing elasped time
        /// </summary>
        public void Restart()
        {
            // Reset the stopwatch
            Reset();

            // Start the stopwatch
            Start();
        }

        /// <summary>
        /// Starts, or resumes, measuring elapsed time for an interval
        /// </summary>
        public void Start()
        {
            if (this.IsRunning)
                return;

            if ((this.StartUtc.HasValue) && (this.EndUtc.HasValue))
            {
                // Resume time from previous state
                this.StartUtc = this.StartUtc.Value + (DateTime.UtcNow - this.EndUtc.Value);
            }
            else
            {
                // Start a new time-interval
                this.StartUtc = DateTime.UtcNow;
            }

            // Set variables
            this.IsRunning = true;
            this.EndUtc = null;
        }

        /// <summary>
        /// Stops measuring elapsed time for an interval
        /// </summary>
        public void Stop()
        {
            if (this.IsRunning)
            {
                this.IsRunning = false;
                this.EndUtc = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Initializes a new Stopwatch instance, sets the elapsed time
        /// property to zero and starts measuring elapsed time
        /// </summary>
        public static StopWatch StartNew()
        {
            StopWatch stopWatch = new StopWatch();
            stopWatch.Start();

            return stopWatch;
        }

    }
}