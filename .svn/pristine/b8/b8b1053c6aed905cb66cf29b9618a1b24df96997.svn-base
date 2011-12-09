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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Berico.Common.Collections;
using Berico.Common.Events;
using Newtonsoft.Json;

namespace Berico.SnagL.Model.Attributes
{
    /// <summary>
    /// Represents a collection of attributes.  This collection stores
    /// the attribute's name and its value.  The GlobalAttributeCollection
    /// is reponsible for maintaining additional information on the 
    /// attributes themselves.
    /// </summary>
    /// <see cref="Berico.LinkAnalysis.Model.Attribute"/>
    public class AttributeCollection : IEnumerable<KeyValuePair<string, AttributeValue>>, INotifyCollectionChanged
    {

        private KeyedDictionaryEntryCollection<string> attributes;

        /// <summary>
        /// Custom event that indicates a property in an attribute's value, stored
        /// in the collection, has changed.
        /// </summary>
        public event EventHandler<PropertyChangedEventArgs<object>> AttributeValuePropertyChanged;

        /// <summary>
        /// Initializes a new instance of the Berico.LinkAnalysis.Model.AttributeCollection with
        /// default values.
        /// </summary>
        public AttributeCollection()
        {
            attributes =  new KeyedDictionaryEntryCollection<string>();
        }

        /// <summary>
        /// Add a new attribute (and value) to the collection
        /// </summary>
        /// <param name="attributeName">The name of the attribute that is being added</param>
        /// <param name="attributeValue">The AttributeValue that is being added</param>
        /// <exception cref="System.ArgumentException">Thrown if the provided key already exists</exception> 
        /// <exception cref="System.ArgumentNullException">Thrown if the provided key is null</exception> 
        public void Add(string attributeName, AttributeValue attributeValue)
        {
            // Try and add the attribute
            if (AddAttribute(attributeName, attributeValue))
            {
                DictionaryEntry entry;
                int index = GetIndexAndEntry(attributeName, out entry);

                // Fire the CollectionChanged event
                if (index > -1)
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new KeyValuePair<string, AttributeValue>(entry.Key as string, entry.Value as AttributeValue), index));
                    
                    // Add PropertyChanged handler for the attribute value
                    attributeValue.PropertyChanged += new EventHandler<PropertyChangedEventArgs<object>>(AttributeValue_PropertyChanged);
                }
                else
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        /// <summary>
        /// Removes the specified item from the collection
        /// </summary>
        /// <param name="attributeName">The key of the item that should be removed</param>
        /// <returns>true if the item was successfully remove; otherwise false (false
        /// is also returned if the provided key does not exist in the collection)</returns>
        public bool Remove(string attributeName)
        {
            bool result = false;

            DictionaryEntry entry;
            int index = GetIndexAndEntry(attributeName, out entry);

            // Try and remove the attribute
            result = RemoveAttribute(attributeName);
            if (result)
            {
                // Fire the CollectionChanged event
                if (index > -1)
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new KeyValuePair<string, AttributeValue>(entry.Key as string, entry.Value as AttributeValue), index));
                    (entry.Value as AttributeValue).PropertyChanged -= new EventHandler<PropertyChangedEventArgs<object>>(AttributeValue_PropertyChanged);
                }
            }

            return result;
        }

        /// <summary>
        /// Updates the specified attribute with the specified value
        /// </summary>
        /// <param name="attributeName">The name of the attribute being updated</param>
        /// <param name="attributeValue">The new attribute value</param>
        public void Update(string attributeName, AttributeValue attributeValue)
        {

            // Validate parameters
            if (string.IsNullOrEmpty(attributeName))
                throw new ArgumentNullException("attributeName", "No attribute name was provided");

            // Check if the provided item exists
            bool exists = this.attributes.Contains(attributeName);

            // If the item already exists, then we're done (no change to make)
            if (exists && attributeValue.Equals((AttributeValue)this.attributes[attributeName].Value))
                return;

            if (exists)
            {
                DictionaryEntry oldEntry;
                int oldIndex = GetIndexAndEntry(attributeName, out oldEntry);

                // Update the item in the collection
                this.attributes[oldIndex] = new DictionaryEntry(attributeName, attributeValue);

                if (oldIndex > -1)
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, new KeyValuePair<string, AttributeValue>(attributeName, attributeValue), new KeyValuePair<string, AttributeValue>(oldEntry.Key as string, oldEntry.Value as AttributeValue), oldIndex));

                    // Add PropertyChanged handler for the attribute value
                    attributeValue.PropertyChanged += new EventHandler<PropertyChangedEventArgs<object>>(AttributeValue_PropertyChanged);

                    // Remove the handler for the old AttributeValue
                    (oldEntry.Value as AttributeValue).PropertyChanged -= new EventHandler<PropertyChangedEventArgs<object>>(AttributeValue_PropertyChanged);
                }
            }
        }

        /// <summary>
        /// Handles the PropertyChanged event for each AttributeValue in the collection
        /// </summary>
        /// <param name="sender">The AttributeValue instance that triggered the event</param>
        /// <param name="e">The arguments for the event</param>
        private void AttributeValue_PropertyChanged(object sender, PropertyChangedEventArgs<object> e)
        {
            AttributeValue attributeValue = sender as AttributeValue;

            // Throws the custom AttributePropertyChanged event
            if (AttributeValuePropertyChanged != null && attributeValue != null)
            {
                AttributeValuePropertyChanged(this, e);
            }
        }

        /// <summary>
        /// Determines whether an Attribute, with the specified name, exists in the collection 
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <returns>true if the attribute exists in the collection; otherwise false</returns>
        public bool ContainsAttribute(string attributeName)
        {
            return this.attributes.Contains(attributeName);
        }

        /// <summary>
        /// Returns the attribute value in the collection based on the specified
        /// attribute name
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <returns>the desired AttributeValue instance if it exists; otherwise null</returns>
        public AttributeValue GetAttributeValue(string attributeName)
        {
            return this[attributeName];
        }

        /// <summary>
        /// Gets the attribute value in the collection based on the attribute's name
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <returns>an AttributeValue instance for the given attribute name; otherwise null</returns>
        public AttributeValue this[string attributeName]
        {
            get
            {
                AttributeValue attributeValue = null;

                // Check if the attribute exists
                bool result = this.attributes.Contains(attributeName);

                if (result)
                {
                    // Get the Attribute Value
                    attributeValue = (AttributeValue)this.attributes[attributeName].Value;
                }

                return attributeValue;
            }
        }

        /// <summary>
        /// Gets a collection of the keys stored in the collection
        /// </summary>
        public IEnumerable<string> Attributes
        {
            get { return this.attributes.Keys; }
        }

        /// <summary>
        /// Gets a collection of the values stored in the collection
        /// </summary>
        public IEnumerable<AttributeValue> AttributeValues
        {
            get
            {
                if (this.attributes != null && this.attributes.Count > 0)
                    return this.attributes.Values.Select(entry => entry.Value as AttributeValue);
                else
                    return new List<AttributeValue>();
            }
        }

        /// <summary>
        /// Gets the number of attributes in the collection
        /// </summary>
        public int Count
        {
            get { return this.attributes.Count; }
        }

        /// <summary>
        /// Removes all the keys and values from the collection
        /// </summary>
        public void Clear()
        {
            // Check if there is anything to be cleared
            if (Count > 0)
            {
                // Clear the collection
                attributes.Clear();

                // Throw the CollectionChanged event
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        /// <summary>
        /// Generates a JSON version of this collection
        /// </summary>
        /// <returns>a JSON representation of this collection</returns>
        public string ToJSON()
        {
            List<KeyValuePair<string, string>> attributes = new List<KeyValuePair<string, string>>();

            // Loop over the attributes and save them as keyvalue pairs
            foreach (string attribute in this.Attributes)
            {
                attributes.Add(new KeyValuePair<string, string>(attribute, this[attribute].Value));
            }

            // Convert our keyvalue pair collection to JSON
            return JsonConvert.SerializeObject(attributes, Formatting.None); 
        }

        /// <summary>
        /// Removes the attribute with the provided attribute name
        /// </summary>
        /// <param name="atributeName">The name of the Attribute that should be removed</param>
        /// <returns>true if the item was removed; otherwise false</returns>
        private bool RemoveAttribute(string attributeName)
        {
            bool result = false;

            // Ensure that the attribute provided actually exists
            if (this.attributes.Contains(attributeName))
            {
                // Get the attribute value
                AttributeValue attributeValue = this[attributeName];

                // Remove the PropertyChanged handler for the attribute value
                attributeValue.PropertyChanged -= new EventHandler<PropertyChangedEventArgs<object>>(AttributeValue_PropertyChanged);

                // Now remove the attribute itself
                result = attributes.Remove(attributeName);
            }

            return result;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <exception cref="System.ArgumentException">Thrown if the provided attribute name already exists</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if the provided attribute name is null</exception> 
        /// <returns></returns>
        private bool AddAttribute(string attributeName, AttributeValue value)
        {
            // Validate parameters
            if (string.IsNullOrEmpty(attributeName))
                throw new ArgumentNullException("attributeName", "No attribute name was provided");

            // Add the attribute to the underlying collection
            this.attributes.Add(new DictionaryEntry(attributeName, value));

            return true;
        }

        /// <summary>
        /// Returns the index and entry in the collection for the item
        /// with the provided attribute name
        /// </summary>
        /// <param name="attributeName">The name of the attribute to return the information for</param>
        /// <param name="entry"></param>
        /// <returns>the index of the entry with the given key; -1 if no entry is found</returns>
        private int GetIndexAndEntry(string attributeName, out DictionaryEntry entry)
        {
            entry = new DictionaryEntry();
            int index = -1;

            // Check if the attribute exists in the internal collection
            if (this.attributes.Contains(attributeName))
            {
                entry = this.attributes[attributeName];
                index = this.attributes.IndexOf(entry);
            }

            return index;
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (CollectionChanged != null)
                CollectionChanged(this, args);
        }

        #region IEnumerable<KeyValuePair<string,AttributeValue>> Members

            public IEnumerator<KeyValuePair<string, AttributeValue>> GetEnumerator()
            {
                foreach (DictionaryEntry entry in new List<DictionaryEntry>(this.attributes))
                {
                    yield return new KeyValuePair<string, AttributeValue>(entry.Key as string, entry.Value as AttributeValue);
                }
            }

        #endregion

        #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return attributes.GetEnumerator();
            }

        #endregion

        #region INotifyCollectionChanged Members

            public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion
    }
}