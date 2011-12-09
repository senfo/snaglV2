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
using System.Text;

namespace Berico.SnagL.Infrastructure.Logging
{
    /// <summary>
    /// This class represents data related to an exception.
    /// This is used by the LogEntry class.  Not all properties
    /// are available for a Silverlight application.
    /// </summary>
    public class ExceptionData
    {
        /// <summary>
        /// Gets or sets the type name
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Gets or sets the exception message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the location in the source
        /// where the exception occurred
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the stack trace for the exception
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        /// Gets or sets the help link for this exception
        /// </summary>
        public string HelpLink { get; set; }

        /// <summary>
        /// Gets or sets
        /// </summary>
        public ExceptionData InnerException { get; set; }

        /// <summary>
        /// Initializes a new instance of Berico.LinkAnalysis.SnagL.
        /// Logging.ExceptionData
        /// </summary>
        public ExceptionData() { }

        /// <summary>
        /// Initializes a new instance of Berico.LinkAnalysis.SnagL.
        /// Logging.ExceptionData
        /// </summary>
        /// <param name="exception">An instance of the Exception that this class
        /// is based off of</param>
        public ExceptionData(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException("exception", "No exception value was provided");

            TypeName = exception.GetType().AssemblyQualifiedName;
            Message = exception.Message;
            StackTrace = exception.StackTrace;

            if (exception.InnerException!=null)
                InnerException = new ExceptionData(exception.InnerException);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            do
            {
                sb.AppendLine(string.Format("{0}  Message: {1}{2}StackTrace: {3}", TypeName, Message, Environment.NewLine, StackTrace));
            } while (InnerException != null);

            return sb.ToString();
        }

    }
}