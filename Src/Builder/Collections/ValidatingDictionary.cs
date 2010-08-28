#region Copyright Notice

/* Copyright 2009-2010 Peter Stephens

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */
#endregion

using System;
using System.Collections;
using System.Collections.Generic;

namespace Builder.Collections
{
    public class ValidatingDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private IDictionary<TKey, TValue> InnerDictionary { get; set; }
        private Predicate<TValue> Validator { get; set; }

        public ValidatingDictionary(Predicate<TValue> validator, 
            IEnumerable<KeyValuePair<TKey, TValue>> dictionary = null,
            IEqualityComparer<TKey> comparer = null)
        {
            Validator = validator;
            InnerDictionary = new Dictionary<TKey, TValue>(
                comparer ?? EqualityComparer<TKey>.Default);
            
            if (dictionary == null) return;

            foreach (var val in dictionary)
            {
                Validate(val.Value);
                InnerDictionary.Add(val.Key, val.Value);
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return InnerDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            InnerDictionary.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return InnerDictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            InnerDictionary.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return InnerDictionary.Remove(item);
        }

        public int Count
        {
            get { return InnerDictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool ContainsKey(TKey key)
        {
            return InnerDictionary.ContainsKey(key);
        }

        public void Add(TKey key, TValue value)
        {
            Validate(value);
            InnerDictionary.Add(key, value);
        }

        public bool Remove(TKey key)
        {
            return InnerDictionary.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return InnerDictionary.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get { return InnerDictionary[key]; }
            set
            {
                Validate(value);
                InnerDictionary[key] = value;
            }
        }

        public ICollection<TKey> Keys
        {
            get { return InnerDictionary.Keys; }
        }

        public ICollection<TValue> Values
        {
            get { return InnerDictionary.Values; }
        }

        private void Validate(TValue value)
        {
            if(!Validator(value))
                throw new ArgumentException("Invalid value.", "value");
        }
    }
}