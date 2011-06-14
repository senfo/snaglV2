using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace SnagL.FacebookHostDemo.GraphML
{
    /// <summary>
    /// Contains properties that represent a GraphML node element
    /// </summary>
    public class Node
    {
        #region Properties

        /// <summary>
        /// Gets or sets the node ID
        /// </summary>
        [XmlAttribute("id")]
        public string NodeId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a reference to a collection of Data objects
        /// </summary>
        [XmlElement("data")]
        public Collection<Data> Data
        {
            get;
            set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Node class
        /// </summary>
        public Node()
        {
            Data = new Collection<Data>();
        }

        #endregion
    }
}
