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
    /// The resource element
    /// </summary>
    [XmlType(TypeName = "resource")]
    public class ExternalResource
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the assembly
        /// </summary>
        [XmlAttribute(AttributeName = "assembly")]
        public string Assembly
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the path
        /// </summary>
        [XmlAttribute(AttributeName = "path")]
        public string Path
        {
            get;
            set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the ConfigurationResource class
        /// </summary>
        public ExternalResource()
        {
        }

        #endregion
    }
}
