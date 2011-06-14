//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using System.ComponentModel;

namespace Berico.Common.Events
{
    /// <summary>
    /// An enhanced version of the PropertyChangedEventArgs that includes
    /// the new and old values invloved in the event
    /// </summary>
    /// <typeparam name="T">The type of data that has been changed</typeparam>
    public class PropertyChangedEventArgs<T> : PropertyChangedEventArgs
    {
        /// <summary>
        /// Creates a new instance of Berico.Common.Events.PropertyChangedEventArgs
        /// that uses the included property name and new and old values
        /// </summary>
        /// <param name="propertyName">The name of the property that was changed</param>
        /// <param name="newValue">The new property value</param>
        /// <param name="oldValue">The old property value</param>
        public PropertyChangedEventArgs(string propertyName, T newValue, T oldValue)
            : base(propertyName)
        {
            this.NewValue = newValue;
            this.OldValue = oldValue;
        }

        /// <summary>
        /// Gets or sets the property's new value
        /// </summary>
        public T NewValue { get; set; }

        /// <summary>
        /// Gets or sets the property's old value
        /// </summary>
        public T OldValue { get; set; }
    }
}