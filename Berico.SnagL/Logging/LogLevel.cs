//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

namespace Berico.SnagL.Infrastructure.Logging
{
    /// <summary>
    /// Specifies the log level
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Indicates a verbose log entry for debugging purposes
        /// </summary>
        DEBUG,
        /// <summary>
        /// Indicates an information log entry
        /// </summary>
        INFO,
        /// <summary>
        /// Indicates a warning log entry
        /// </summary>
        WARN,
        /// <summary>
        /// Indicates an error log entry
        /// </summary>
        ERROR,
        /// <summary>
        /// Indicates an error log entry in which the application 
        /// was forced to close
        /// </summary>
        FATAL
    }
}