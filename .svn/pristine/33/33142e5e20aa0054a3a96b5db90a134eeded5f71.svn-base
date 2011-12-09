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

namespace Berico.SnagL.Infrastructure.Events
{
    //TODO:  THIS SHOULD BE HANDLED BY MEF WHEN WE FULLY EMBRACE USING PRISM

    /// <summary>
    /// This is a wrapper for the EventAggregator class that is
    /// part of Prism.  This class is a singleton providing access
    /// to a default instance of the EventAggregator.
    /// </summary>
    public class SnaglEventAggregator : EventAggregator
    {
        private static SnaglEventAggregator defaultInstance;
        private static object syncRoot = new object();

        /// <summary>
        /// Gets the default instance of the SnaglEventAggregator class
        /// </summary>
        public static SnaglEventAggregator DefaultInstance
        {

            get
            {
                if (defaultInstance == null)
                {
                    lock (syncRoot)
                    {
                        if (defaultInstance == null)
                        {
                            defaultInstance = new SnaglEventAggregator();
                        }
                    }
                }
                return defaultInstance;
            }
        }

        // If different EventAggregator instances are required, they
        // can be added as properties here and maintained by this class.
    }
}