using System;
using System.Collections;
using System.Collections.Generic;

namespace Builder
{
    public class ValidatingDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private Dictionary<TKey, TValue> InnerDictionary { get; set; }
        private Predicate<TValue> Validator { get; set; }

        public ValidatingDictionary(Predicate<TValue> validator, 
            IDictionary<TKey, TValue> dictionary = null,
            IEqualityComparer<TKey> comparer = null)
        {
            Validator = validator;
            InnerDictionary = new Dictionary<TKey, TValue>(
                comparer ?? EqualityComparer<TKey>.Default);
            // TODO: populate from source if available.
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool ContainsKey(TKey key)
        {
            throw new NotImplementedException();
        }

        public void Add(TKey key, TValue value)
        {
            Validate(value);
            InnerDictionary.Add(key, value);
        }

        private void Validate(TValue value)
        {
            if(!Validator(value))
                throw new ArgumentException("Invalid value.", "value");
        }

        public bool Remove(TKey key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            throw new NotImplementedException();
        }

        public TValue this[TKey key]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public ICollection<TKey> Keys
        {
            get { throw new NotImplementedException(); }
        }

        public ICollection<TValue> Values
        {
            get { return InnerDictionary.Values; }
        }
    }
}