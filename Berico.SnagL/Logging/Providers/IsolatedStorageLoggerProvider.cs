//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using System.ComponentModel.Composition;
using System.IO;
using System.IO.IsolatedStorage;
using Berico.SnagL.Infrastructure.Modularity.Contracts;

namespace Berico.SnagL.Infrastructure.Logging
{
    /// This class represents a Loggerprovider which provides logging functionality
    /// for the LoggerManager.  This class is a MEF plugin (export) that is imported
    /// by the LoggerManager and used to write log entries.
    /// 
    /// This provider writes log entries to the user's isolated storage area.
    [PartCreationPolicy(CreationPolicy.Shared)] 
    [PartMetadata("IsDefault", "True")]
    [PartMetadata("ID", "Logger.Provider.IsolatedStorage"), Export(typeof(ILoggerProvider))]
    public class IsolatedStorageLoggerProvider : ILoggerProvider
    {
        //TODO:  REQUEST ADDITIONAL ISOLATED STORAGE SPACE

        private IsolatedStorageFile isoStoreFile = null;

        /// <summary>
        /// Initializes a new instance of the Berico.LinkAnalysis.SnagL.Logging
        /// .Providers class.
        /// </summary>
        public IsolatedStorageLoggerProvider()
        {
            //TODO:  HANDLE THE CASE WHERE ISOLATED STORAGE IS DISABLED

            // Obtain a reference to the user's isolated storage
            if (IsolatedStorageFile.IsEnabled)
                isoStoreFile = IsolatedStorageFile.GetUserStoreForApplication();
        }

        #region ILoggerProvider Members

            /// <summary>
            /// Writes the provided message to the log
            /// </summary>
            /// <param name="logMessage">The message to write to the log</param>
            public void Write(string logMessage)
            {
                if (isoStoreFile != null)
                {
                    // Open the log file
                    using (IsolatedStorageFileStream fs = new IsolatedStorageFileStream("SnagL.Log", System.IO.FileMode.Append, isoStoreFile))
                    {
                        // Open a write to the log
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            // Write the message(s) to the log
                            sw.WriteLine(logMessage);

                            sw.Flush();
                            sw.Close();
                        }
                    }
                }

            }

        #endregion
    }
}