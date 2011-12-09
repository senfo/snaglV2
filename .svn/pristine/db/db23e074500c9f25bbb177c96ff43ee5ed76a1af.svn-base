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

namespace Berico.Common
{
    /// <summary>
    /// This custom attribute provides an easy way to tag a property or field
    /// as being exportable in some manner.  It contains one unnamed argument
    /// that represents a key value for the property or field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ExportablePropertyAttribute : System.Attribute
    {
        private string key = string.Empty;
        private string format = string.Empty;

        /// <summary>
        /// Creates a new instance of the ExportablePropertyAttribute
        /// class using the provided string as an unnamed argument.
        /// The argument represents the name used to identify the 
        /// property for exporting.
        /// </summary>
        /// <param name="name"></param>
        public ExportablePropertyAttribute(string _key)
        {
            this.key = _key;
        }

        /// <summary>
        /// Gets the key defined for this attribute 
        /// </summary>
        public string Key { get { return this.key; } }

        /// <summary>
        /// Gets or sets a format to be used when 
        /// exporting the properties value
        /// </summary>
        public string Format
        {
            get { return this.format; }
            set { this.format = value; }
        }

    }
}