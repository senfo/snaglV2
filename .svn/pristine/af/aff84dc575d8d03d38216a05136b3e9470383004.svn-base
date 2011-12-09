//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

namespace Berico.SnagL.Infrastructure.Modularity.Contracts
{
    /// <summary>
    /// Provides the contract for all logger providers.  A logger
    /// provider represents a physical log of some type and the mechanism
    /// to write to it.  This is primarily used by MEF for importing and
    /// exporting.
    /// </summary>
    public interface ILoggerProvider
    {
        /// <summary>
        /// Writes the provided message to the log
        /// </summary>
        /// <param name="logMessage">The message to write to the log</param>
        void Write(string logMessage);
    }
}