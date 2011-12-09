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

namespace Berico.SnagL.UI
{
    /// <summary>
    /// Contains properties for informing subscribers about information pertaining to live data
    /// </summary>
    public class LiveDataEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets or sets the total number of items in the queue
        /// </summary>
        public int Count
        {
            get;
            set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the LiveDataEventArgs class
        /// </summary>
        public LiveDataEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the LiveDataEventArgs class
        /// </summary>
        /// <param name="count">Total number of items in the queue</param>
        public LiveDataEventArgs(int count)
        {
            Count = count;
        }

        #endregion
    }
}