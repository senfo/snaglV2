using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace SnagL.FacebookHostDemo.GraphML
{
    /// <summary>
    /// Contains properties that represent a GraphML graph
    /// </summary>
    [Serializable]
    public class Graph
    {
        #region Properties

        /// <summary>
        /// Gets or sets the graph ID
        /// </summary>
        [XmlAttribute("id")]
        public string GraphId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the default edge type
        /// </summary>
        [XmlAttribute("edgedefault")]
        public EdgeType EdgeDefault
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of nodes
        /// </summary>
        [XmlAttribute(AttributeName = "nodeType", Namespace = "http://graph.bericotechnologies.com/xmlns")]
        public NodeType NodeType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a reference to a collection of Node objects
        /// </summary>
        [XmlElement("node")]
        public Collection<Node> Nodes
        {
            get;
            set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Graph class
        /// </summary>
        public Graph()
        {
            Nodes = new Collection<Node>();
        }

        #endregion
    }
}
