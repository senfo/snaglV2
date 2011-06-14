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

namespace Berico.Common.Events
{
    /// <summary>
    /// Provides the template for notifying clients that some
    /// property has changed.  This custom version of the interface
    /// uses a generic version of PropertyChangedEventArgs that 
    /// includes the old and new values of the property that was
    /// changed.
    /// </summary>
    /// <typeparam name="T">The type for the property that changed</typeparam>
    public interface INotifyPropertyChanged<T>
    {
        /// <summary>
        /// The event fired when a property changes.  This custom version uses a
        /// generic version of PropertyChangedEventArgs which includes the new
        /// and old values of the property that changed.
        /// </summary>
        event EventHandler<PropertyChangedEventArgs<T>> PropertyChanged;
    }
}