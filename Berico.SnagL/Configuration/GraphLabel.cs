//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using System.Xml.Serialization;

namespace Berico.SnagL.Infrastructure.Configuration
{
    /// <summary>
    /// The GraphLabel element
    /// </summary>
    [XmlType(AnonymousType = true, TypeName = "graphLabel")]
    public class GraphLabel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the background color
        /// </summary>
        [XmlAttribute(AttributeName = "background")]
        public string Background
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the foreground color
        /// </summary>
        [XmlAttribute(AttributeName = "foreground")]
        public string Foreground
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the text displayed
        /// </summary>
        [XmlAttribute(AttributeName = "text")]
        public string Text
        {
            get;
            set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the ConfigurationGraphLabel class
        /// </summary>
        public GraphLabel()
        {
        }

        #endregion
    }
}
