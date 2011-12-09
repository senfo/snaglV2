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
using System.ComponentModel.Composition;
using System.Windows.Browser;
using Berico.SnagL.Infrastructure.Modularity;
using Berico.SnagL.Infrastructure.Modularity.Contracts;

namespace Berico.SnagL.Infrastructure.Logging
{
    /// <summary>
    /// This singleton is the central management point for all loggers used
    /// in the application.  Typically, each class has it's own logger but
    /// they are all instantiated and tracked by this class.  The manager
    /// relies on a provider that physically represents the log itself and
    /// how to write to it.  The Provider is a MEF extension.
    /// </summary>
    public class LoggerManager
    {
        private static LoggerManager instance;
        private static object syncRoot = new object();
        private static readonly Dictionary<string, Logger> logRepository = new Dictionary<string, Logger>();

        /// <summary>
        /// Gets or sets the provider that represents the actual log
        /// itself and provides the means to write to it.  Only a single
        /// provider is used and MEF is responsible for instantiating it.
        /// </summary>
        [Import(typeof(ILoggerProvider), AllowRecomposition=true, AllowDefault=true)]
        public ILoggerProvider LoggerProvider { get; set; }

        //TODO:  MAKE MORE GENERIC TO PUT IN TO COMMON NAMESPACE

        private LoggerManager() { }

        /// <summary>
        /// Gets the instance of the LoggerManager class
        /// </summary>
        public static LoggerManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new LoggerManager();
                            instance.Initialize();
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Initializes the LoggerManager class
        /// </summary>
        private void Initialize()
        {
            // Tell MEF to compose the parts for this class and
            // set the default level
            ExtensionManager.ComposeParts(this);
            Level = LogLevel.DEBUG;
        }

        /// <summary>
        /// Gets or sets the level that should be used by all loggers
        /// </summary>
        public LogLevel Level { get; set; }

        /// <summary>
        /// This method actually creates and returns a new logger instance
        /// </summary>
        /// <param name="caller">Represents the Type that created the logger.</param>
        /// <returns></returns>
        public static Logger GetLogger(Type caller)
        {
            Logger logger = null;

            // Generate a unique name for the logger (based on the callers Type)
            string logName = FormatLogName(caller);

            lock (syncRoot)
            {
                // Check if the logger (with the specified name) was
                // already created
                if (logRepository.TryGetValue(logName, out logger))
                    return logger;

                // Create the new logger and add it to the internal
                // repository
                logger = new Logger(caller, logName);
                logRepository[logName] = logger;
            }

            return logger;
        }

        /// <summary>
        /// This method actually creates and returns a new logger instance
        /// </summary>
        /// <param name="name">Represents </param>
        /// <returns>Returns a new logger instance</returns>
        public static Logger GetLogger(string name)
        {
            Logger logger = null;

            lock (syncRoot)
            {
                // Check if the logger (with the specified name) was
                // already created
                if (logRepository.TryGetValue(name, out logger))
                    return logger;

                // Create the new logger and add it to the internal
                // repository
                logger = new Logger(name);
                logRepository[name] = logger;
            }

            return logger;
        }

        /// <summary>
        /// This method generates a unique log namebased on the provided
        /// Type class.
        /// </summary>
        /// <param name="_callerType">The Type that created the logger</param>
        /// <returns>A unique name for the log</returns>
        private static string FormatLogName(Type _callerType)
        {
            return string.Format("{0}:{1}", _callerType.FullName, FormatUri());
        }

        /// <summary>
        /// This method generates a string based off of the URI of this application.
        /// This information is used as part of the logger's unique name.
        /// </summary>
        /// <returns>A string containing a formatted representation of the URI of this application</returns>
        private static string FormatUri()
        {
            string result = string.Empty; ;
            Uri uri = HtmlPage.Document.DocumentUri;

            if (uri.Host.ToLower() == "localhost" || uri.Host.Contains("127.0.0."))
                result = uri.LocalPath;
            else
                if (uri.Port == 80 || uri.Port == 443)
                    result = string.Format("{0}://{1}{2}", uri.Scheme, uri.Host, uri.LocalPath);
                else
                    result = string.Format("{0}://{1}:{2}{3}", uri.Scheme, uri.Host, uri.Port, uri.LocalPath);

            return result.ToLower();
        }

    }
}