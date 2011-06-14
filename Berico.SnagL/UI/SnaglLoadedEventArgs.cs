//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

namespace Berico.SnagL.UI
{
    /// <summary>
    /// Represents arguments for the SnaglLoadedEvent
    /// </summary>
    public class SnaglLoadedEventArgs
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value that indicates whether or not external resources were loaded
        /// </summary>
        public bool ExternalResourcesLoaded
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value that indicates whether or not the graph has loaded
        /// </summary>
        public bool GraphLoaded
        {
            get;
            set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the Berico.SnagL.Events.
        /// DataLoadedEventArgs class
        /// </summary>
        public SnaglLoadedEventArgs()
        {
            //TODO: UPDATE TO INCLUDE ADDITIONAL PROPERTIES (SUCH AS NUMBER OF NODES AND EDGES LOADED)            
        }

        #endregion
    }
}