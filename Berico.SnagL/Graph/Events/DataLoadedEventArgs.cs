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
using Berico.SnagL.Model;

namespace Berico.SnagL.Infrastructure.Graph.Events
{
    /// <summary>
    /// Represents arguments for events related to loading data
    /// </summary>
    public class DataLoadedEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets the mechanism used to load the data
        /// </summary>
        public CreationType SourceMechanism
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the scope for which the data was loaded for
        /// </summary>
        public string Scope
        {
            get;
            private set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Prevents an instance of the <see cref="DataLoadedEventArgs"/> class from being instantiated
        /// </summary>
        private DataLoadedEventArgs()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataLoadedEventArgs"/> class
        /// </summary>
        /// <param name="scope">The scope of the loaded data</param>
        /// <param name="sourceMechanism">Indicates the source mechanism for the data that was loaded</param>
        public DataLoadedEventArgs(string scope, CreationType sourceMechanism)
        {
            Scope = scope;
            SourceMechanism = sourceMechanism;
        }

        #endregion

        //TODO: UPDATE TO INCLUDE ADDITIONAL PROPERTIES (SUCH AS NUMBER OF NODES AND EDGES LOADED)
    }
}