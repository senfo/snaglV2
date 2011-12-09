//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace Berico.SnagL.Infrastructure.Logging
{
    /// <summary>
    /// This class represents all the data in an
    /// actual log entry
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// Gets or sets the level of this log entry
        /// </summary>
        public LogLevel LogLevel { get; set; }

        /// <summary>
        /// Gets or sets the message for this log entry
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the name of the thread
        /// where the log entry was generated
        /// </summary>
        public string ThreadName { get; set; }

        /// <summary>
        /// Gets or sets the ID of the thread 
        /// where the log entry was generated
        /// </summary>
        public int ManagedThreadId { get; set; }

        /// <summary>
        /// Gets or sets the name of the type
        /// that generated the log entry
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Gets or sets the data and time at which
        /// the log entry was generated
        /// </summary>
        public DateTime OccuredAt { get; set; }

        /// <summary>
        /// Gets or sets a collection of key/value pairs containing 
        /// additional custom properties
        /// </summary>
        public IDictionary<string, object> Properties { get; set; }

        /// <summary>
        /// Gets or sets exception information for this log entry
        /// </summary>
        public ExceptionData ExceptionInfo { get; set; }

        /// <summary>
        /// Gets or sets client information for this log entry
        /// </summary>
        public ClientData ClientInfo { get; set; }

        /// <summary>
        /// Initializes a new instance of Berico.LinkAnalysis.SnagL.
        /// Logging.LogEntry
        /// </summary>
        public LogEntry() { }

        /// <summary>
        /// Returns a string that contains detailed information
        /// about this class
        /// </summary>
        /// <returns>a string representation of this class</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (Properties != null)
            {
                // Create string entries for each custom parameter
                foreach (KeyValuePair<string, object> pair in Properties)
                {
                    sb.Append(pair.Key);
                    sb.Append('=');
                    sb.Append(pair.Value);
                    sb.Append(";");
                }
            }

            // Returns a string representation of this class
            return string.Format("Message:{0}, LogLevel: {1}, ThreadName: {2}, ManagedThreadId: {3}, Properties: {4}, {5}",
                    Message, LogLevel, ThreadName, ManagedThreadId, sb, ClientInfo.ToString());

        }
    }
}