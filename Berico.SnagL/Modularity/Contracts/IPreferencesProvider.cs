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
    /// Provides the contract for all preference providers.  A preference
    /// provider is responsible for providing the means to save and load
    /// preference (user settings).  This is primarily used by MEF for
    /// importing and exporting.  
    /// </summary>
    public interface IPreferencesProvider
    {
        /// <summary>
        /// Gets the value for the specified field.  If the field does not exist
        /// it is created using the defaultValue and the defaultValue is returned.
        /// </summary>
        /// <param name="key">The name of the field whose value is being requested</param>
        /// <param name="defaultValue">The default value to return in the event that the field doesn't exist</param>
        /// <returns>The value of the specified field or the defaultValue, if the field does not exist</returns>
        string GetPreference(string key, string defaultValue);

        /// <summary>
        /// Sets the specified preference field with the specified value.
        /// </summary>
        /// <param name="key">The name of the field to be set</param>
        /// <param name="value">The value to be assigned to the field</param>
        void SetPreference(string key, string value);
    }
}