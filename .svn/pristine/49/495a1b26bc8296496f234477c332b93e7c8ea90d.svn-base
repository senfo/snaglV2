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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Berico.Common.Events;
using Berico.SnagL.Model.Attributes;

namespace Berico.SnagL.Infrastructure.Data.Attributes
{
    /// <summary>
    /// This class contains all attributes (for all nodes) and
    /// their values.  This is used by similarity measures and
    /// clustering.
    /// 
    /// Since this class is a singleton, you must use the
    /// GetInstance method to retrieve an instance of the class.
    /// </summary>
    public class GlobalAttributeCollection
    {

        /// <summary>
        /// A custom PropertyChangedEventArgs class that includes an instance
        /// to the Attribute whose property changed.
        /// </summary>
        public class AttributePropertyChangedEventArgs : PropertyChangedEventArgs<object>
        {
            public AttributePropertyChangedEventArgs(string propertyName, object newValue, object oldValue) : base(propertyName, newValue, oldValue) { }
            public Attribute Attribute { get; set; }
        }

        private static Dictionary<string, GlobalAttributeCollection> instances;
        private static object syncRoot = new object();
        private Dictionary<Attribute, Dictionary<string, int>> attributes = null;
        
        private GlobalAttributeCollection() { }

        /// <summary>
        /// Custom event that indicates that the global attribute collection
        /// has changed in some manner
        /// </summary>
        public event EventHandler<AttributeEventArgs> AttributeListUpdated;

        /// <summary>
        /// Custom event that indicates a property in an underlying Attribute, stored
        /// in the collection, has changed.
        /// </summary>
        public event EventHandler<AttributePropertyChangedEventArgs> AttributePropertyChanged;

        /// <summary>
        /// Gets the instance of the GlobalAttributeCollection class that
        /// is assigned the specified key
        /// </summary>
        /// <param name="id">The ID of the instance for which to return.
        /// If an instance by the specified ID doesn't exist, a new
        /// instance will be created using the specified id.</param>
        public static GlobalAttributeCollection GetInstance(string id)
        {
            // Validate the provided id
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException("id", "An invalid id was provided");

            lock (syncRoot)
            {
                if (instances == null)
                {
                    // Create the instances collection
                    instances = new Dictionary<string, GlobalAttributeCollection>();

                    // Ensure that the provided key doesn't exist
                    if (!instances.ContainsKey(id))
                    {
                        // Create a new instance of the GlobalAttributeCollection
                        // and assign it to the provided key
                        instances.Add(id, new GlobalAttributeCollection());
                        instances[id].Initialize();
                    }
                }
            }

            // Ensure that the provided key doesn't exist
            if (!instances.ContainsKey(id))
            {
                // Create a new instance of the GlobalAttributeCollection
                // and assign it to the provided key
                instances.Add(id, new GlobalAttributeCollection());
                instances[id].Initialize();
            }

            // Return the default instance
            return instances[id];
        }

        /// <summary>
        /// Initializes the GlobalAttributeCollection class
        /// </summary>
        public void Initialize()
        {
            attributes = new Dictionary<Attribute, Dictionary<string, int>>();
        }

        /// <summary>
        /// Gets the list of values for the provided key
        /// </summary>
        /// <param name="key">The attribute whose values should be returned</param>
        /// <returns>the list of all values for the given attribute</returns>
        public ICollection<string> this[Attribute key]
        {
            get
            {
                Dictionary<string, int> values = new Dictionary<string, int>();

                if (this.attributes.TryGetValue(key, out values))
                    return values.Keys;
                else
                    return new List<string>();
            }
        }

        /// <summary>
        /// Gets the list of values for the provided key
        /// </summary>
        /// <param name="key">The name of the attribute whose values should be returned</param>
        /// <returns>the list of all values for the given attribute</returns>
        public ICollection<string> this[string key]
        {
            get
            {
                // Get the attribute with the given name
                Attribute attributeFound = attributes.Keys.Where(attribute => attribute.Name == key).FirstOrDefault();

                if (attributeFound != null)
                    return this[attributeFound];
                else
                    return new List<string>();
            }
        }

        /// <summary>
        /// Returns a collection of the keys (attributes) stored in the
        /// global attribute collection
        /// </summary>
        /// <returns>a collection of all the keys</returns>
        public ICollection<Attribute> GetAttributes()
        {
            return attributes.Keys;
        }

        /// <summary>
        /// Returns the stored Attribute instance for the provided attribute name
        /// </summary>
        /// <param name="attributeName">The name of the attribute to return</param>
        /// <returns>an Attribute instance; otherwise null</returns>
        public Attribute GetAttribute(string attributeName)
        {
            // Get the attribute with the given name
            Attribute attributeFound = attributes.Keys.Where(attribute => attribute.Name == attributeName).FirstOrDefault();

            // Return the attribute (or null)
            return attributeFound;
        }

        /// <summary>
        /// Determines if the provided attribute is in the collection
        /// </summary>
        /// <param name="attributeName">The name of the attribute to look for</param>
        /// <returns>true if the provided attribute name is in the collection; otherwise false</returns>
        public bool ContainsAttribute(string attributeName)
        {
            // Get the attribute with the given name
            Attribute attributeFound = attributes.Keys.Where(attribute => attribute.Name == attributeName).FirstOrDefault();

            if (attributeFound != null)
                return true;
            else
                return false;

        }

        /// <summary>
        /// Determines if the provided attribute is in the collection
        /// </summary>
        /// <param name="attribute">The Attribute to look for</param>
        /// <returns>true if the provided Attribute is in the collection; otherwise false</returns>
        public bool ContainsAttribute(Attribute attribute)
        {

            if (this.attributes.ContainsKey(attribute))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Returns a collection of values for the given attribute
        /// </summary>
        /// <param name="key">The attribute to return values for</param>
        /// <returns>a collection of values for the given attribute</returns>
        public ICollection<string> GetAttributeValues(Attribute key)
        {
            return this[key];
        }

        /// <summary>
        /// Returns a collection of values for the given attribute
        /// </summary>
        /// <param name="key">The name of the attribute to return values for</param>
        /// <returns>a collection of values for the given attribute</returns>
        public ICollection<string> GetAttributeValues(string key)
        {
            return this[key];
        }

        /// <summary>
        /// Returns all the attribute values, and their frequencies, for
        /// the provided attribute name
        /// </summary>
        /// <param name="attributeName">The name of the attribute to get the
        /// values and frequencies for</param>
        /// <returns>the collection of attribute values and their frequencies. An
        /// empty collection is returned if the specified attribute can not be found.</returns>
        public IDictionary<string, int> GetFrequenciesAndValues(string attributeName)
        {

            // Validate the key parameter
            if (attributeName == null)
                throw new ArgumentNullException("AttributeName", "No valid attribute name was provided.");

            // Get the attribute with the given name
            Attribute attributeFound = attributes.Keys.Where(attribute => attribute.Name == attributeName).FirstOrDefault();

            // Ensure that we found an actual Attribute with the given name
            if (attributeFound != null)
            {
                // Return the collection of values and their frequencies
                return this.attributes[attributeFound];
            }
            else
            {
                // The attribute wasn't found so just return an empty collection
                return new Dictionary<string, int>();
            }

        }

        /// <summary>
        /// Returns the frequency that the provided value occurs for
        /// the provided attribute
        /// </summary>
        /// <param name="attributeName">The attribute name</param>
        /// <param name="attributeValue">The attribute value</param>
        /// <returns>the frequency that </returns>
        public int GetFrequency(string attributeName, string attributeValue)
        {

            // Validate the attributeName parameter
            if (attributeName == null)
                throw new ArgumentNullException("attributeName", "No valid attribute name was provided.");

            // Validate the attributeValue parameter
            if (attributeValue == null)
                throw new ArgumentNullException("attributeValue", "No valid attribute value was provided.");

            // Get the values and their frequencies
            Dictionary<string, int> frequenciesAndValues = new Dictionary<string, int>(GetFrequenciesAndValues(attributeName));

            // Make sure that the attribute value exists
            if (frequenciesAndValues.ContainsKey(attributeValue))
            {
                // Return the frequency for the specified value
                return frequenciesAndValues[attributeValue];
            }
            else
                return 0;

        }

        /// <summary>
        /// Reset the class by clearing the internal collections
        /// </summary>
        public void Clear()
        {
            if (this.attributes.Count > 0)
            {
                this.attributes.Clear();

                // Fire the AttributeListUpdated event
                OnAttributeListUpdated(new AttributeEventArgs(null, null, null, NotifyCollectionChangedAction.Reset));
            }
        }

        /// <summary>
        /// Adds the provided attribute to the list of global attributes.
        /// If the attribute already exists, its value collection is updated.
        /// </summary>
        /// <param name="attribute">Attribute to add to the list</param>
        /// <param name="attributeValue">Attribute value associated with the attribute</param>
        /// <exception cref="System.ArgumentNullException">Thrown if the provided attribute or value is null</exception>
        public void Add(Attribute attribute, AttributeValue attributeValue)
        {
            if (attribute == null)
                throw new ArgumentNullException("attribute");
            
            if (attributeValue == null)
                throw new ArgumentNullException("attributeValue");

            // Check if the attribute already exists in the collection
            if (this.attributes.ContainsKey(attribute))
            {
                // The attribute exists so we need to update it
                Update(attribute, attributeValue);
            }
            else
            {
                // Since this is a new attribute, we need to create a new list
                // and add the new value to it
                this.attributes[attribute] = new Dictionary<string, int>() { { attributeValue.Value, 1 } };

                // Fire the AttributeListUpdated event
                OnAttributeListUpdated(new AttributeEventArgs(attribute, attributeValue, null, NotifyCollectionChangedAction.Add));

                // Wire up the property changed event
                attribute.PropertyChanged += new EventHandler<PropertyChangedEventArgs<object>>(Attribute_PropertyChanged);
            }
        }

        /// <summary>
        /// Updates the attribute list using the provided attribute name and new value
        /// </summary>
        /// <param name="attributeName">The name of the attribute that is being updated in the list</param>
        /// <param name="newValue">The new value for the attribute</param>
        public void Update(string attributeName, AttributeValue newValue)
        {
            Update(attributeName, newValue, null);
        }

        /// <summary>
        /// Updates the attribute list using the provided attribute name and values
        /// </summary>
        /// <param name="attribute">The name of the attribute that is being updated in the list</param>
        /// <param name="newValue">The new value for the attribute</param>
        /// <param name="oldValue">The old value for the attribute</param>
        public void Update(string attributeName, AttributeValue newValue, AttributeValue oldValue)
        {
            //NOTE: ATTRIBUTE INSTANCE MUST BE SPECIFIED IF NEW; STRING ONLY FOR EXISTING

            // Get the Attribute instance based on the provided attribute name
            Attribute attributeFound = attributes.Keys.Where(attribute => attribute.Name == attributeName).FirstOrDefault();

            // Update the list
            Update(attributeFound, newValue, oldValue);
        }

        /// <summary>
        /// Updates the global attribute list using the provided Attribute instance and new value
        /// </summary>
        /// <param name="attribute">The Attribute that is being updated in the list</param>
        /// <param name="newValue">The new value for the attribute</param>
        public void Update(Attribute attribute, AttributeValue newValue)
        {
            Update(attribute, newValue, null);
        }

        /// <summary>
        /// Updates the global attribute list using the provided Attribute instance and values
        /// </summary>
        /// <param name="attribute">The Attribute that is being updated in the list</param>
        /// <param name="newValue">The new value for the attribute</param>
        /// <param name="oldValue">The old value for the attribute</param>
        /// <exception cref="System.ArgumentException">Thrown if the provided attribute does not exist</exception> 
        /// <exception cref="System.ArgumentNullException">Thrown if the provided attribute or the new
        /// AttributeValue instance is null</exception>
        public void Update(Attribute attribute, AttributeValue newValue, AttributeValue oldValue)
        {

            // Validate the key parameter
            if (attribute == null)
                throw new ArgumentNullException("attribute", "No valid Attribute instance was provided.");

            if (newValue == null)
                throw new ArgumentNullException("newValue", "No valid AttributeValue instance was provided.");

            // Check if the provided attribute
            if (!this.attributes.ContainsKey(attribute))
                throw new ArgumentException("The provided attribute [" + attribute.Name + "] does not exist in the collection", "attribute");

            // Don't save the new value if it is empty
            if (String.IsNullOrEmpty(newValue.Value))
                return;

            // Check if we were provided an oldValue, which indicates
            // a value is changing
            if (oldValue != null && attributes[attribute].ContainsKey(oldValue.Value))
            {

                // Decrement the count for the value.  If it is 0
                // remove it.
                if ((attributes[attribute][oldValue.Value] - 1) <= 0)
                {
                    // Remove the oldValue from the list of values
                    attributes[attribute].Remove(oldValue.Value);
                }
                else
                {
                    // Just decrement the count of nodes that
                    // have this attribute value
                    attributes[attribute][oldValue.Value]--;
                }

                // Fire the AttributeListUpdated event
                OnAttributeListUpdated(new AttributeEventArgs(attribute, newValue, oldValue, NotifyCollectionChangedAction.Replace));

            }

            // Now we attempt to add the newValue to the collection
            if (attributes[attribute].ContainsKey(newValue.Value))
            {
                // The value is already there so we just want to 
                // increment the count
                attributes[attribute][newValue.Value]++;
            }
            else
            {
                // Add the new value
                attributes[attribute].Add(newValue.Value, 1);

                // Wire up the property changed event
                attribute.PropertyChanged += new EventHandler<PropertyChangedEventArgs<object>>(Attribute_PropertyChanged);
            }

        }

        /// <summary>
        /// Handles the PropertyChanged event for each Attribute in the collection
        /// </summary>
        /// <param name="sender">The object that initial fired the event</param>
        /// <param name="e">The arguments for the event</param>
        void Attribute_PropertyChanged(object sender, PropertyChangedEventArgs<object> e)
        {
            Attribute attribute = sender as Attribute;

            // Throws the custom AttributePropertyChanged event
            if (AttributePropertyChanged != null && attribute != null)
            {
                AttributePropertyChanged(this, new AttributePropertyChangedEventArgs(e.PropertyName, e.NewValue, e.OldValue) { Attribute = attribute });
            }
        }

        /// <summary>
        /// Remove the attribute from the global attribute list
        /// </summary>
        /// <param name="key">The name of the attribute that is being removed from the collection</param>
        public void Remove(string key)
        {
            // Get the Attribute instance based on the provided attribute name
            Attribute attributeFound = attributes.Keys.Where(attribute => attribute.Name == key).FirstOrDefault();

            // Now remove the attribute
            Remove(attributeFound);

        }

        /// <summary>
        /// Remove an attribute from the global attribute list.  This will remove all values
        /// associated with the given attribute
        /// </summary>
        /// <param name="attribute">The attribute that is being removed from the collection</param>
        public void Remove(Attribute attribute)
        {
            // Make sure that the attribute is in the global list
            if (this.attributes.ContainsKey(attribute))
            {
                // Stop handling the PropertyChanged event for this attribute
                attribute.PropertyChanged -= new EventHandler<PropertyChangedEventArgs<object>>(Attribute_PropertyChanged);

                // Remove the attribute from the global attribute list
                this.attributes.Remove(attribute);
                //InvalidateCache(attribute.Name);

                // Fire the AttributeListUpdated event
                OnAttributeListUpdated(new AttributeEventArgs(attribute, null, null, NotifyCollectionChangedAction.Remove));
            }

        }

        /// <summary>
        /// Fires the AttributeListUpdated event
        /// </summary>
        /// <param name="args">Arguments for the event</param>
        protected virtual void OnAttributeListUpdated(AttributeEventArgs args)
        {
            if (AttributeListUpdated != null)
            {
                AttributeListUpdated(this, args);
            }
        }

    }
}