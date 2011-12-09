using System;

namespace Berico.SnagL.Infrastructure
{
    /// <summary>
    /// Extends EventArgs to provide information when a XAP file has finished being loaded
    /// </summary>
    public class XapLoadedEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets a reference to the object created when the XAP was loaded
        /// </summary>
        public object Instance
        {
            get;
            private set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Prevents an instance of the XapLoadedEventArgs class from being instantiated
        /// </summary>
        private XapLoadedEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the XapLoadedEventArgs class
        /// </summary>
        /// <param name="instance">The object that was initialized by the XAP loader</param>
        public XapLoadedEventArgs(object instance)
            : base()
        {
            Instance = instance;
        }

        #endregion
    }
}
