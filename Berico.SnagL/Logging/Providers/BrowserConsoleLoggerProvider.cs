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
using System.Windows.Browser;
using Berico.SnagL.Infrastructure.Modularity.Contracts;

namespace Berico.SnagL.Infrastructure.Logging
{
    /// <summary>
    /// This class represents a Loggerprovider which provides logging functionality
    /// for the LoggerManager.  This class is a MEF plugin (export) that is imported
    /// by the LoggerManager and used to write log entries.
    /// 
    /// This provider writes log entries to the browser console.  The console is
    /// only available to some instances of IE8 and Mozilla Firefox.  This is really
    /// only a test class and will most likely not make it into the release.
    /// </summary>
    [PartMetadata("IsDefault", "False")]
    [PartMetadata("ID", "Logger.Provider.BrowserConsole"), Export(typeof(ILoggerProvider))]
    public class BrowserConsoleLoggerProvider : ILoggerProvider
    {
        #region ILoggerProvider Members

            //TODO: TEST THIS FURTHER (DOESN'T SEEM TO WORK)

            /// <summary>
            /// Writes the provided message to the log
            /// </summary>
            /// <param name="logMessage">The message to write to the log</param>
            public void Write(string logMessage)
            {
                HtmlWindow window = HtmlPage.Window;

                // Try and determine if a browser console is available
                var isConsoleAvailable = (bool)window.Eval("typeof(console) != 'undefined' && typeof(console.log) != 'undefined'");

                if (isConsoleAvailable)
                {
                    // Crete an instance of the console
                    var console = (window.Eval("console.log") as ScriptObject);

                    if (console != null)
                    {
                        // Write the log message to the browser's console
                        console.InvokeSelf(logMessage);
                    }
                }
            }

        #endregion
    }
}