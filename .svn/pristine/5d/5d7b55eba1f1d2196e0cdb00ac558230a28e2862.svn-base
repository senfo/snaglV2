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
using System.IO.IsolatedStorage;
using Berico.SnagL.Infrastructure.Modularity.Contracts;

namespace Berico.SnagL.Infrastructure.Preferences
{
    /// <summary>
    /// This class provides a concrete preference provider that uses
    /// IsolatedStorage to store user settings.
    /// </summary>
    [PartMetadata("IsDefault", "True")]
    [PartMetadata("ID", "Preference.Provider.IsolatedStorage")]
    [Export(typeof(IPreferencesProvider))]
    public class IsolatedStoragePreferencesProvider : IPreferencesProvider
    {
        //TODO: HANDLE ISOLATEDSTORAGE EXCEPTIONS
        private IsolatedStorageSettings isoSettings = IsolatedStorageSettings.ApplicationSettings;

        #region IPreferencesProvider Members

            /// <summary>
            /// Gets the value for the specified field.  If the field does not exist
            /// it is created using the defaultValue and the defaultValue is returned.
            /// </summary>
            /// <param name="key">The name of the field whose value is being requested</param>
            /// <param name="defaultValue">The default value to return in the event that the field doesn't exist</param>
            /// <returns>The value of the specified field or the defaultValue, if the field does not exist</returns>
            public string GetPreference(string key, string defaultValue)
            {
                string value = null;

                // Validate the provided key
                if (string.IsNullOrEmpty(key))
                {
                    // Add the new setting with the provided default value
                    SetPreference(key, defaultValue);
                    return defaultValue;
                }

                // Try and get the setting from Isolated Storage
                if (isoSettings.TryGetValue(key, out value))
                    return value;
                else
                {
                    // Add the new setting with the provided default value
                    SetPreference(key, defaultValue);
                    return defaultValue;
                }

            }

            /// <summary>
            /// Sets the specified preference field with the specified value.
            /// </summary>
            /// <param name="key">The name of the field to be set</param>
            /// <param name="value">The value to be assigned to the field</param>
            public void SetPreference(string key, string value)
            {
                // Set the field with the provided value
                isoSettings[key] = value;
            }

        #endregion
    }
}