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
using Berico.Common.Events;

namespace Berico.SnagL.Model.Attributes
{
    /// <summary>
    /// Represents an attribute's value that is associated with a node or edge.  An attribute's
    /// display value can differ from it's underlying value.  It can also be hidden from the user
    /// (in most cases).
    /// </summary>
    /// <exception cref="System.ArgumentNullException">Thrown in the event that a provided property value is null</exception>
    public class AttributeValue : INotifyPropertyChanged<object>
    {

        private string value;
        private string displayValue;

        /// <summary>
        /// Initializes a new instance of the Berico.LinkAnalysis.Model.AttributeValue with
        /// the provided value.
        /// </summary>
        /// <param name="_value">The underlying (raw) value</param>
        public AttributeValue(string _value) : this(_value, null) { }

        /// <summary>
        /// Initializes a new instance of Berico.LinkAnalysis.Model.AttributeValue with
        /// the value and displayValue.
        /// </summary>
        /// <param name="_value">The underlying (raw) value</param>
        /// <param name="_displayValue">The display value</param>
        /// <exception cref="System.ArgumentNullException">Thrown in the event that Value is null</exception>
        public AttributeValue(string _value, string _displayValue)
        {
            // Validate parameters
            if (String.IsNullOrEmpty(_value))
                throw new ArgumentNullException("Value", "A value must be provided for this attribute");

            this.value = _value;
            this.displayValue = _displayValue;
        }

        /// <summary>
        /// Gets or sets the actual value of this attribute
        /// </summary>
        /// <exception cref="System.ComponentModel.PropertyChanged">Thrown when the property is changed</exception> 
        /// <exception cref="System.ArgumentNullException">Thrown in the event that the provided property value is null</exception>
        public string Value
        {
            get { return this.value; }
            set
            {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException("Value", "A value must be provided for this attribute");

                if (value != this.value)
                {
                    object oldValue = this.value;
                    this.value = value;

                    NotifyPropertyChanged("Value", oldValue, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the actual text that should be displayed for this attribute, which
        /// can differ from the 
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown in the event that a provided property value is null</exception>
        public string DisplayValue
        {
            get
            {
                if (this.displayValue != null)
                    return this.displayValue;
                else
                    return this.value;
            }
            set
            {
                if (value != this.displayValue)
                {
                    object oldValue = this.displayValue;
                    this.displayValue = value;

                    NotifyPropertyChanged("DisplayValue", oldValue, value);
                }
            }
        }

        /// <summary>
        /// Fires the PropertyChanged event
        /// </summary>
        /// <param name="info">A string that contains the name of the property that has changed</param>
        /// <param name="oldValue">The old value</param>
        /// <param name="newValue">The new value</param>
        protected void NotifyPropertyChanged(string info, object oldValue, object newValue)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs<object>(info, newValue, oldValue));
        }

        #region INotifyPropertyChanged<object> Members

            public event EventHandler<PropertyChangedEventArgs<object>> PropertyChanged;

        #endregion
    }
}