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
    ///  This class represents data related to the applications
    ///  client.  This is used by the LogEntry class.  Not all
    ///  properties are available for a Silverlight application.
    /// </summary>
    public class ClientData
    {
        /// <summary>
        /// Gets or sets the name of the log
        /// </summary>
        public string LogName { get; set; }

        /// <summary>
        /// Gets or sets the current user name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the name of the user's machine
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// Gets or sets the current Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the user's IP address
        /// </summary>
        public string IPAddress { get; set; }

        public ClientData() { }

        public override string ToString()
        {
            return string.Format("Log Name: {0}, Username: {1}, Machine Name: {2}, Url: {3}, IP Address: {4}", LogName, UserName, MachineName, Url, IPAddress);
        }
    }
}