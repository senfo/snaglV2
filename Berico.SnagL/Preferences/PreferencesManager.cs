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
using Berico.SnagL.Infrastructure.Modularity.Contracts;
using Berico.SnagL.Infrastructure.Modularity;

namespace Berico.SnagL.Infrastructure.Preferences
{

    /// <summary>
    /// This singleton is responsible for managing all preferences for the
    /// application.  Preferences represent user settings which are
    /// used to save and restore the state of the application.  The manager
    /// relies on a provider to actually perform the saving and loading.
    /// The Provider is a MEF extension.
    /// </summary>
    public class PreferencesManager
    {
        private static PreferencesManager instance;
        private static object syncRoot = new object();

        /// <summary>
        /// Hidden constructor.  Only here for MEF
        /// purposes.
        /// </summary>
        private PreferencesManager() { }

        /// <summary>
        /// Gets or sets the provider that the class uses to actually
        /// save and load the user's preferences.  Only a single provider
        /// is used and MEF is responsible for instantiating it.
        /// </summary>
        [Import(typeof(IPreferencesProvider), AllowRecomposition = true)]
        public IPreferencesProvider Provider { get; set; }

        /// <summary>
        /// Gets the instance of the PreferencesManager class
        /// </summary>
        public static PreferencesManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            // Create the instance of the PreferencesManager
                            instance = new PreferencesManager();
                            instance.Initialize();
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Initializes the PreferencesManager class
        /// </summary>
        private void Initialize()
        {
            // Tell MEF to compose the parts for this class
            ExtensionManager.ComposeParts(this);
        }

        /// <summary>
        /// Sets the provided field with the provided value
        /// </summary>
        /// <param name="name">The name of the preference to set</param>
        /// <param name="value">The value for the field</param>
        public void SetPreference(string name, string value)
        {
            // Call the SetPreference method provided by the Provider
            Provider.SetPreference(name, value);
        }

        /// <summary>
        /// Gets the value for the specified field
        /// </summary>
        /// <param name="name">The name of the perference whose value to return</param>
        /// <param name="defaultValue">The default value for the preference</param>
        /// <returns>The value for the specified field</returns>
        public string GetPreference(string name, string defaultValue)
        {
            // Call the GetPreference method provided by the Provider
            return Provider.GetPreference(name, defaultValue);
        }
    }
}