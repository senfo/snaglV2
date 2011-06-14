//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Berico.Common.Collections
{
    /// <summary>
    /// Represents a keyed collection of DictionaryEntry items.  This collection behaves similar
    /// to a Dictionary&lt;&gt; class but provides additional functionality.
    /// </summary>
    /// <typeparam name="TKey">The type that represents the keys in the collection</typeparam>
    public class KeyedDictionaryEntryCollection<TKey> : KeyedCollection<TKey, DictionaryEntry> where TKey : class
    {
        /// <summary>
        /// Returns the key for the provided DictionaryEntry
        /// </summary>
        /// <param name="item">A DictionaryEntry</param>
        /// <returns>the key for the provided item</returns>
        protected override TKey GetKeyForItem(DictionaryEntry item)
        {
            return item.Key as TKey;
        }

        /// <summary>
        /// Gets a collection of the keys contained in the internal dictionary
        /// </summary>
        public ICollection<TKey> Keys
        {
            get
            {
                if (this.Dictionary != null)
                {
                    return this.Dictionary.Keys;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets a collection of the values contained in the internal dictionary
        /// </summary>
        public ICollection<DictionaryEntry> Values
        {
            get { return this.Dictionary.Values; }
        }
    }
}