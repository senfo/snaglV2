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
using System.Windows.Browser;
using System.Collections.Generic;
using System.Text;

namespace Berico.SnagL.Infrastructure.Logging
{
    /// <summary>
    /// This class is used to generate log entries and write
    /// them to a logging provider.
    /// </summary>
    public class Logger
    {
        private Type callerType = null;
        private string name = string.Empty;
        private Uri pageUri = null;

        /// <summary>
        /// Initializes a new instance of the Berico.LinkAnalysis.SnagL.Logging
        /// class using the type that is generating the Logger and a name.
        /// This should not be called directly.  New Logger instances should
        /// be creted by calling the GetLogger method.
        /// </summary>
        /// <param name="_callerType">the class type that generated
        /// this logger</param>
        /// <param name="_name">the name for the new logger</param>
        protected internal Logger(Type _callerType, string _name)
        {
            this.callerType = _callerType;
            this.name = _name;

            //Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the Berico.LinkAnalysis.SnagL.Logging
        /// class using the provided name.  This should not be called directly.
        /// New Logger instances should be creted by calling the GetLogger
        /// method.
        /// </summary>
        /// <param name="_name">the name for the new logger</param>
        protected internal Logger(string _name)
        {
            this.name = _name;
            //Initialize();
        }

        /// <summary>
        /// Initializes this Logger instance.  This is not currently being
        /// used.
        /// </summary>
        private void Initialize()
        {
            // Not currently used
        }

        /// <summary>
        /// Creates and returns a new Berico.LinkAnalysis.SnagL.Logging instance
        /// </summary>
        /// <param name="name">A string representing the name for the
        /// Logger that will be generated</param>
        /// <returns>a new Berico.LinkAnalysis.SnagL.Logging instance</returns>
        public static Logger GetLogger(string name)
        {
            return LoggerManager.GetLogger(name);
        }

        /// <summary>
        /// Creates and returns a new Berico.LinkAnalysis.SnagL.Logging instance
        /// </summary>
        /// <param name="callerType">The type of the class that called this 
        /// method</param>
        /// <returns>a new Berico.LinkAnalysis.SnagL.Logging instance</returns>
        public static Logger GetLogger(Type callerType)
        {
            return LoggerManager.GetLogger(callerType);
        }

        /// <summary>
        /// Gets the Uri of the current page that the
        /// Logger was generated for
        /// </summary>
        public Uri PageUri
        {
            get
            {
                if (this.pageUri == null)
                {
                    this.pageUri = HtmlPage.Document.DocumentUri;
                }

                return this.pageUri;
            }
        }

        /// <summary>
        /// Gets the name assigned to the Logger
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Generates a log entry, formats it into a readable string and
        /// writes it to the configured LoggerProvider.  The LoggerProvider
        /// is pluggable and managed by the LoggerManager class.
        /// </summary>
        /// <param name="logLevel">The level of the entry to be writen</param>
        /// <param name="message">The message to be written</param>
        /// <param name="exception">The exception to be documented</param>
        /// <param name="properties">A collection of custom parameters</param>
        public void WriteLogEntry(LogLevel logLevel, string message, Exception exception, IDictionary<string, object> properties)
        {

            //TODO:  ADD ADDITIONAL WRITE METHODS

            // Do not write the log entry if the current level is 
            // less than than the configured level
            if (logLevel < LoggerManager.Instance.Level)
                return;

            ExceptionData exceptionData = null;

            // If an exception was provided, gather data from it
            if (exception != null)
                exceptionData = new ExceptionData(exception);

            // Generate client data
            ClientData clientData = new ClientData()
            {
                // We are currently limited to the client
                // information that we can collect from
                // Silverlight.
                LogName = this.name,
                Url = PageUri.ToString()
            };

            // Create the actual LogEntry using all provided parameters and
            // all generated information
            LogEntry logEntry = new LogEntry()
            {
                ExceptionInfo = exceptionData,
                ClientInfo = clientData,
                LogLevel = logLevel,
                Message = message,
                TypeName = callerType != null ? callerType.FullName : string.Empty,
                ThreadName = System.Threading.Thread.CurrentThread.Name,
                ManagedThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId,
                OccuredAt = DateTime.Now,
                Properties = properties != null ? new Dictionary<string, object>(properties) : null
            };

            // Construct the message so the provider is only
            // responsible for writing the string
            StringBuilder logMessage = new StringBuilder();

            logMessage.AppendLine(string.Format("[{0,-4}] {1:s} {2,5} <{3}> {4}", logEntry.ManagedThreadId, logEntry.OccuredAt, logEntry.LogLevel.ToString(), logEntry.TypeName, logEntry.Message));
                
            // Write any extended properties
            if (logEntry.Properties != null)
            {
                logMessage.Append(string.Format("[{0,-4}] ->", "ADDL PROPS"));
                foreach (KeyValuePair<string, object> property in logEntry.Properties)
                {
                    logMessage.Append(string.Format("{0} = {1};", property.Key, property.Value.ToString()));
                }
            }

            // Write any exception information
            if (logEntry.ExceptionInfo != null)
            {
                logMessage.Append(string.Format("[{0,-4}] ->", "EXCEPTION INFO"));
                logMessage.AppendLine(logEntry.ExceptionInfo.ToString());
            }

            // Instruct the provider to physically write the log entry
            //LoggerManager.Instance.LoggerProvider.Write(logMessage.ToString());
        }
    }
}