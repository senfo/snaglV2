//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Berico.SnagL.Infrastructure.Configuration
{
    /// <summary>
    /// Used to specify the logger level
    /// </summary>
    [DataContract(Name = "loggerLevelType")]
    public enum LoggerLevel
    {
        /// <summary>
        /// Debug level
        /// </summary>
        [XmlEnum(Name = "DEBUG")]
        Debug,

        /// <summary>
        /// Release level
        /// </summary>
        [XmlEnum(Name = "RELEASE")]
        Release,
    }
}
