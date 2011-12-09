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
using Berico.SnagL.Model.Attributes;
using System.Collections.Specialized;

namespace Berico.SnagL.Infrastructure.Data.Attributes
{
    /// <summary>
    /// Provides argument data for attribute related events
    /// </summary>
    public class AttributeEventArgs : EventArgs
    {
        private Attribute attribute = null;
        private AttributeValue newValue = null;
        private AttributeValue oldValue = null;
        private NotifyCollectionChangedAction action;

        public AttributeEventArgs(Attribute _attribute, AttributeValue _newValue, NotifyCollectionChangedAction _action)
            : this(_attribute, _newValue, null, _action) { }

        public AttributeEventArgs(Attribute _attribute, AttributeValue _newValue, AttributeValue _oldValue, NotifyCollectionChangedAction _action)
            : base()
        {
            this.attribute = _attribute;
            this.newValue = _newValue;
            this.oldValue = _oldValue;
            this.action = _action;
        }

        /// <summary>
        /// Gets the attribute information
        /// </summary>
        public Attribute Attribute
        {
            get { return this.attribute; }
        }

        /// <summary>
        /// Gets the new value for the attribute
        /// </summary>
        public AttributeValue NewValue
        {
            get { return this.newValue; }
        }

        /// <summary>
        /// Gets the old value for the attribute
        /// </summary>
        public AttributeValue OldValue
        {
            get { return this.oldValue; }
        }

        /// <summary>
        /// Gets the action that triggered the parent event
        /// </summary>
        public NotifyCollectionChangedAction Action
        {
            get { return this.action; }
        }
    }

}