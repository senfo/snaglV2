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
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Xml.Serialization;
using Berico.SnagL.Infrastructure.Logging;
using Newtonsoft.Json;

namespace Berico.SnagL.Infrastructure.Configuration
{
    /// <summary>
    /// This class is responsible for managing the configuration settings for
    /// the application.
    /// </summary>
    public class ConfigurationManager
    {
        #region Public Constants

        /// <summary>
        /// Specifies the delimiter for parameters that need to be delimited
        /// </summary>
        public static readonly char PARAMETER_DELIMITER = ';';

        /// <summary>
        /// The extensions key
        /// </summary>
        public static readonly string PARAMETER_EXTENSIONS_PATH = "extensionsPath";

        /// <summary>
        /// The external resources key
        /// </summary>
        public static readonly string PARAMETER_EXTERNAL_RESOURCES_PATH = "externalResources";

        /// <summary>
        /// The graph label key
        /// </summary>
        public static readonly string PARAMETER_GRAPH_LABEL = "graphLabel";

        /// <summary>
        /// The applicationMode key
        /// </summary>
        public static readonly string PARAMETER_APPLICATION_MODE = "applicationMode";

        /// <summary>
        /// The live key
        /// </summary>
        public static readonly string PARAMETER_LIVE = "autostartLive";

        /// <summary>
        /// The log provider key
        /// </summary>
        public static readonly string PARAMETER_LOGGER_PROVIDER = "loggerProvider";

        /// <summary>
        /// The log level key
        /// </summary>
        public static readonly string PARAMETER_LOGGER_LEVEL = "loggerLevel"; 

        /// <summary>
        /// The preferences provider key
        /// </summary>
        public static readonly string PARAMETER_PREFERENCES_PROVIDER = "preferencesProvider";

        /// <summary>
        /// Indicates whether the tool bar is hidden or not
        /// </summary>
        public static readonly string PARAMETER_GRAPH_ISTOOLBARHIDDEN = "isToolbarHidden";

        /// <summary>
        /// Indicates whether the tool panel is hidden or not
        /// </summary>
        public static readonly string PARAMETER_GRAPH_ISTOOLPANELHIDDEN = "isToolPanelHidden";

        /// <summary>
        /// The path to the default resource file
        /// </summary>
        public static readonly string RESOURCE_DEFAULT_CONFIGURATION_FILE = "Resources/Configuration/DefaultConfiguration.xml";

        #endregion

        #region Fields

        /// <summary>
        /// Stores the default configuration settings
        /// </summary>
        private Configuration _configuration;

        /// <summary>
        /// Stores a reference to the logger
        /// </summary>
        private Logger _logger = Logger.GetLogger(typeof(ConfigurationManager));

        #endregion

        #region Properties

        /// <summary>
        /// Gets a reference to the ConfigurationManger singleton instance
        /// </summary>
        public static ConfigurationManager Instance
        {
            get
            {
                return NestedConfigurationManager.instance;
            }
        }

        /// <summary>
        /// Gets a reference to the current configuration value
        /// </summary>
        public Configuration CurrentConfig
        {
            get
            {
                return _configuration;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Prevents an instance of the ConfigurationManager class from being instantiated
        /// </summary>
        private ConfigurationManager()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes the configuration manager
        /// </summary>
        /// <param name="parameters">A dictionary of key/value pairs representing configuration
        /// settings for the application</param>
        public void Initialize(IDictionary<string, string> parameters)
        {
            Configuration internalConfig = ParseConfigurationData();
            Configuration markupConfig;

            //_logger.WriteLogEntry(LogLevel.INFO, "Storing configuration parameters", null, null);

            // Copy the provided initParams to our internal dictionary
            if (parameters.Count > 0)
            {
                markupConfig = ParseConfigurationData(parameters);
            }
            else
            {
                markupConfig = null;
            }

            // Merge the configuration values
            _configuration = MergeProperties(typeof(Configuration), internalConfig, markupConfig) as Configuration;
        }

        /// <summary>
        /// Merges the public property values from the two parameters.
        /// Values from typeB override values from TypeA.
        /// </summary>
        /// <param name="paramA">Object which contains public properties</param>
        /// <param name="paramB">Object which contains public properties</param>
        /// <returns>The property values from paramB merged with values from paramA.</returns>
        public static object MergeProperties(Type type, object paramA, object paramB)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;

            if (paramA == null && paramB == null)
            {
                throw new InvalidOperationException("Unable to obtain property information from either parameter");
            }

            if (paramA == null && paramB != null)
            {
                return paramB;
            }

            if (paramB == null && paramA != null)
            {
                return paramA;
            }

            if ((paramA.GetType() != type) || (paramB.GetType() != type))
            {
                throw new InvalidOperationException("One or more parameters do not match the specified object type");
            }

            ConstructorInfo constructor = type.GetConstructor(new Type[] { });
            object merged = constructor.Invoke(new object[] { });
            PropertyInfo[] infoA = paramA.GetType().GetProperties(flags);
            PropertyInfo[] infoB = paramB.GetType().GetProperties(flags);

            for (int x = 0; x < infoB.Length; x++)
            {
                object valueA = infoA[x].GetValue(paramA, null);
                object valueB = infoB[x].GetValue(paramB, null);

                if (valueB != null)
                {
                    infoB[x].SetValue(merged, valueB, null);
                }
                else
                {
                    infoA[x].SetValue(merged, valueA, null);
                }
            }

            return merged;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parses the specified XML data
        /// </summary>
        /// <returns>An IDictionary containing the configuration key/value pairs</returns>
        private static Configuration ParseConfigurationData()
        {
            Configuration configuration;
            XmlSerializer serializer = new XmlSerializer(typeof(Configuration));

            using (Stream stream = Application.GetResourceStream(new Uri(RESOURCE_DEFAULT_CONFIGURATION_FILE, UriKind.Relative)).Stream)
            {
                configuration = (Configuration)serializer.Deserialize(stream);
            }

            return configuration;
        }

        /// <summary>
        /// Extracts the configuration data from the supplied dictionary
        /// </summary>
        /// <param name="parameters">A dictionary object containing the startup parameters</param>
        /// <returns>A Configuration object containing the parsed configuration values</returns>
        private Configuration ParseConfigurationData(IDictionary<string, string> parameters)
        {
            string[] values;
            Configuration configuration = new Configuration();

            // Extensions
            if (parameters.ContainsKey(PARAMETER_EXTENSIONS_PATH))
            {
                values = parameters[PARAMETER_EXTENSIONS_PATH].Split(PARAMETER_DELIMITER);

                foreach (string str in values)
                {
                    configuration.Extensions.Add(new Extension
                        {
                            Path = str
                        });
                }
            }

            // External resources
            if (parameters.ContainsKey(PARAMETER_EXTERNAL_RESOURCES_PATH))
            {
                string value = parameters[PARAMETER_EXTERNAL_RESOURCES_PATH].Replace(";", ","); // Replace semicolons with commas to make it proper JSON

                configuration.ExternalResources = JsonConvert.DeserializeObject<Collection<ExternalResource>>(value);
            }

            // Graph Label
            if (parameters.ContainsKey(PARAMETER_GRAPH_LABEL))
            {
                string value = parameters[PARAMETER_GRAPH_LABEL].Replace(";", ","); // Replace semicolons with commas to make it proper JSON

                configuration.GraphLabel = JsonConvert.DeserializeObject<GraphLabel>(value);
            }

            // Application Mode
            if (parameters.ContainsKey(PARAMETER_APPLICATION_MODE))
            {
                configuration.ApplicationMode = (ApplicationMode)Enum.Parse(typeof(ApplicationMode), parameters[PARAMETER_APPLICATION_MODE], true);
            }
            else
            {
                configuration.ApplicationMode = ApplicationMode.Evaluation;
            }

            // Live
            if (parameters.ContainsKey(PARAMETER_LIVE))
            {
                if (configuration.LivePreferences == null)
                {
                    configuration.LivePreferences = new Live();
                }

                configuration.LivePreferences.AutoStart = Boolean.Parse(parameters[PARAMETER_LIVE]);
            }

            // LoggerProvider
            if (parameters.ContainsKey(PARAMETER_LOGGER_LEVEL) && parameters.ContainsKey(PARAMETER_LOGGER_PROVIDER))
            {
                configuration.LoggerProvider = new LoggerProvider
                {
                    Level = (LoggerLevel)Enum.Parse(typeof(LoggerLevel), parameters[PARAMETER_LOGGER_LEVEL], true),
                    Provider = parameters[PARAMETER_PREFERENCES_PROVIDER]
                };
            }

            // PreferencesProvider
            if (parameters.ContainsKey(PARAMETER_PREFERENCES_PROVIDER))
            {
                configuration.PreferencesProvider = new PreferencesProvider
                {
                    Provider = parameters[PARAMETER_PREFERENCES_PROVIDER]
                };
            }

            // IsToolbarHidden
            if (parameters.ContainsKey(PARAMETER_GRAPH_ISTOOLBARHIDDEN))
            {
                configuration.IsToolbarHidden = bool.Parse(parameters[PARAMETER_GRAPH_ISTOOLBARHIDDEN]);
            }
            else
                configuration.IsToolbarHidden = false;

            // IsToolPanelHidden
            if (parameters.ContainsKey(PARAMETER_GRAPH_ISTOOLPANELHIDDEN))
            {
                configuration.IsToolPanelHidden = bool.Parse(parameters[PARAMETER_GRAPH_ISTOOLPANELHIDDEN]);
            }
            else
                configuration.IsToolPanelHidden = false;

            return configuration;
        }

        #endregion

        #region Nested Instance

        /// <summary>
        /// Implements the singleton pattern to provide access to the ConfigurationManager class
        /// </summary>
        private class NestedConfigurationManager
        {
            /// <summary>
            /// Stores the singleton instance of the ConfigurationManager class
            /// </summary>
            internal static readonly ConfigurationManager instance = new ConfigurationManager();

            /// <summary>
            /// Initializes a new instance of the class
            /// </summary>
            static NestedConfigurationManager()
            {
            }
        }

        #endregion
    }
}