using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace SnagL.FacebookHostDemo.GraphML
{
    /// <summary>
    /// Contains properties that represent a GraphML document
    /// </summary>
    [XmlRoot(Namespace ="http://graphml.graphdrawing.org/xmlns", ElementName = "graphml")]
    public class GraphML
    {
        #region Properties

        /// <summary>
        /// Gets a reference to a collection of Key elements
        /// </summary>
        [XmlElement("key")]
        public Collection<Key> Keys
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a reference to a GraphML graph
        /// </summary>
        [XmlElement("graph", Namespace = "")]
        public Graph Graph
        {
            get;
            set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Intializes a new instance of the GraphML class
        /// </summary>
        public GraphML()
        {
            Keys = new Collection<Key>();
            Graph = new Graph();
        }

        #endregion
    }
}
