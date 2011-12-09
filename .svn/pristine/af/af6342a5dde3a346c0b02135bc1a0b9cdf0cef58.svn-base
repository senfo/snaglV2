using System;
using System.Xml.Serialization;

namespace SnagL.FacebookHostDemo.GraphML
{
    /// <summary>
    /// Contains properties that represent a GraphML key
    /// </summary>
    [Serializable]
    public class Key
    {
        #region Properties

        /// <summary>
        /// Gets or sets the key ID
        /// </summary>
        [XmlAttribute("id")]
        public string KeyId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the targeted type (for)
        /// </summary>
        [XmlAttribute("for")]
        public string Scope
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the target attribute (attr.name)
        /// </summary>
        [XmlAttribute("attr.name")]
        public string Target
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the data type (attr.type)
        /// </summary>
        [XmlAttribute("attr.type")]
        public string DataType
        {
            get;
            set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Key class
        /// </summary>
        public Key()
        {
        }

        #endregion
    }
}
