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
    /// The LoggerProvider element
    /// </summary>
    [XmlType(AnonymousType = true, TypeName = "loggerProvider")]
    public class LoggerProvider
    {
        #region Properties

        /// <summary>
        /// Gets or sets the logger level
        /// </summary>
        [XmlAttribute(AttributeName = "level")]
        public LoggerLevel Level
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the logger provider
        /// </summary>
        [XmlAttribute(AttributeName = "provider")]
        public string Provider
        {
            get;
            set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the ConfigurationLoggerProvider class
        /// </summary>
        public LoggerProvider()
        {
        }

        #endregion
    }
}
