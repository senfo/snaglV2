//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Berico.SnagL.Infrastructure.Configuration
{
    /// <summary>
    /// The root configuration element
    /// </summary>
    [XmlRoot(ElementName = "configuration", IsNullable = false)]
    public class Configuration
    {
        #region Properties

        /// <summary>
        /// Gets a reference to a collection of Add objects
        /// </summary>
        [XmlArray("appSettings", IsNullable = false)]
        public Collection<ConfigurationAdd> Settings
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a reference to a collection of extensions
        /// </summary>
        [XmlArray("extensions", IsNullable = false)]
        public Collection<Extension> Extensions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the graph label
        /// </summary>
        [XmlElement("graphLabel")]
        public GraphLabel GraphLabel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the live preferences
        /// </summary>
        [XmlElement("live")]
        public Live LivePreferences
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the logger provider
        /// </summary>
        [XmlElement("loggerProvider")]
        public LoggerProvider LoggerProvider
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the preferences provider
        /// </summary>
        [XmlElement("preferencesProvider")]
        public PreferencesProvider PreferencesProvider
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a reference to a collection of resources
        /// </summary>
        [XmlArray("externalResources", IsNullable = false)]
        public Collection<ExternalResource> ExternalResources
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the theme
        /// </summary>
        [XmlElement("theme", IsNullable = false)]
        public Theme Theme
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the mnode of the application
        /// </summary>
        [XmlElement("applicationMode", typeof(ApplicationMode))]
        public ApplicationMode ApplicationMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether the toolbar is hidden
        /// </summary>
        [XmlElement("isToolbarHidden")]
        public bool IsToolbarHidden
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether the toolpanel is hidden
        /// </summary>
        [XmlElement("isToolPanelHidden")]
        public bool IsToolPanelHidden
        {
            get;
            set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Configuration class
        /// </summary>
        public Configuration()
        {
            Settings = new Collection<ConfigurationAdd>();
            Extensions = new Collection<Extension>();
            ExternalResources = new Collection<ExternalResource>();
        }

        #endregion
    }
}
