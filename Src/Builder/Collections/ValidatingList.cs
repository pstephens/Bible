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
    public class ValidatingList<T> : IList<T>
    {
        private const string ValidationMessage = "Failed input validation.";

        private List<T> InnerList { get; set; }
        private Predicate<T> Validator { get; set; }

        public ValidatingList(Predicate<T> validator)
        {
            Validator = validator;
            InnerList = new List<T>();
        }

        public ValidatingList(Predicate<T> validator, IEnumerable<T> items)
            : this(validator)
        {
            foreach(var item in items)
                Add(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return InnerList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            Validate(item);
            InnerList.Add(item);
        }

        public void Clear()
        {
            InnerList.Clear();
        }

        public bool Contains(T item)
        {
            return InnerList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            InnerList.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return InnerList.Remove(item);
        }

        public int Count
        {
            get { return InnerList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public int IndexOf(T item)
        {
            return InnerList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            Validate(item);
            InnerList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            InnerList.RemoveAt(index);
        }

        public T this[int index]
        {
            get { return InnerList[index]; }
            set
            {
                Validate(value);
                InnerList[index] = value;
            }
        }

        private void Validate(T item)
        {
            if(!Validator(item))
                throw new ArgumentException(ValidationMessage, "item");
        }
    }
}