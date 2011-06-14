using System;
using System.Xml.Serialization;

namespace SnagL.FacebookHostDemo.GraphML
{
    /// <summary>
    /// Contains properties that represent a single datum element for a Node object
    /// </summary>
    [Serializable]
    public class Data
    {
        #region Properties

        /// <summary>
        /// Gets or sets the key
        /// </summary>
        [XmlAttribute("key")]
        public string Key
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the inner text for the data
        /// </summary>
        [XmlText]
        public string InnerText
        {
            get;
            set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Intializes a new instance of the Data class
        /// </summary>
        public Data()
        {
        }

        #endregion
    }
}
