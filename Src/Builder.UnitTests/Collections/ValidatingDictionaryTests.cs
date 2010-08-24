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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Builder.UnitTests.Collections
{
    [TestFixture]
    public class ValidatingDictionaryTests
    {
        private static ValidatingDictionary<int, string> CreateDictionaryUnderTest(
            IEnumerable<KeyValuePair<int, string>> dictionary = null)
        {
            return new ValidatingDictionary<int, string>(
                val => val != null && val.Contains("35"), dictionary);
        }

        public static ValidatingDictionary<int, string> CreateDictionaryUnderTest(
            params KeyValuePair<int, string>[] items)
        {
            return CreateDictionaryUnderTest(
                items.ToDictionary(item => item.Key, item => item.Value));
        }

        private static ValidatingDictionary<int, string> CreateTwoItemDictionary()
        {
            return CreateDictionaryUnderTest(
                new KeyValuePair<int, string>(5, "35A"),
                new KeyValuePair<int, string>(10, "35B"));
        }

        [Test]
        public void Add_2_items_should_retain_two_items()
        {
            var dict = CreateDictionaryUnderTest();

            dict.Add(5, "Some 35");
            dict.Add(3, "Value 35");

            Assert.That(dict.Values.ToArray(),
                Is.EquivalentTo(new[] {"Some 35", "Value 35"}));
        }

        [Test]
        public void Add_2_items_should_retain_two_items_v2()
        {
            var dict = CreateDictionaryUnderTest();

            dict.Add(new KeyValuePair<int, string>(5, "Some 35"));
            dict.Add(new KeyValuePair<int, string>(3, "Value 35"));

            Assert.That(dict.Values.ToArray(),
                Is.EquivalentTo(new[] {"Some 35", "Value 35"}));
        }

        [Test]
        public void Add_invalid_item_should_throw()
        {
            var dict = CreateDictionaryUnderTest();

            Assert.Throws<ArgumentException>(
                () => dict.Add(5, "Doesn't contain number thirty-five"));
        }

        [Test]
        public void Add_invalid_item_should_throw_v2()
        {
            var dict = CreateDictionaryUnderTest();

            Assert.Throws<ArgumentException>(
                () => dict.Add(new KeyValuePair<int, string>(5, "Doesn't contain thirty-five")));
        }

        [Test]
        public void Add_multiple_items_should_retain_items()
        {
            var dict = CreateDictionaryUnderTest(
                new KeyValuePair<int, string>(1, "Some 35"),
                new KeyValuePair<int, string>(2, "Other 35"));

            Assert.That(dict.Values.ToArray(),
                Is.EquivalentTo(new[] {"Some 35", "Other 35"}));
        }

        [Test]
        public void Add_multiple_invalid_items_should_throw()
        {
            Assert.Throws<ArgumentException>(
                () => CreateDictionaryUnderTest(
                    new KeyValuePair<int, string>(2, "Invalid Value 1"),
                    new KeyValuePair<int, string>(5, "Invalid Value 2")));
        }

        [Test]
        public void Values_collection_should_be_read_only()
        {
            var dict = CreateDictionaryUnderTest();

            Assert.That(dict.Values.IsReadOnly, Is.True);
        }

        [Test]
        public void Values_collection_should_return_values()
        {
            var dict = CreateDictionaryUnderTest(
                new KeyValuePair<int, string>(3, "35 A"),
                new KeyValuePair<int, string>(5, "35 B"));

            Assert.That(dict.Values.ToArray(),
                Is.EquivalentTo(new[] {"35 A", "35 B"}));
        }

        [Test]
        public void Keys_collection_should_be_read_only()
        {
            var dict = CreateDictionaryUnderTest(
                new KeyValuePair<int, string>(1, "35 A"));

            Assert.That(dict.Keys.IsReadOnly, Is.True);
        }

        [Test]
        public void Keys_collection_should_return_keys()
        {
            var dict = CreateTwoItemDictionary();

            Assert.That(dict.Keys.ToArray(),
                Is.EquivalentTo(new[] {5, 10}));
        }

        [Test]
        public void Get_indexer_should_return_value()
        {
            var dict = CreateTwoItemDictionary();

            var val = dict[5];

            Assert.That(val, Is.EqualTo("35A"));
        }

        [Test]
        public void Get_indexer_using_invalid_key_should_throw()
        {
            var dict = CreateTwoItemDictionary();

            Assert.Throws<KeyNotFoundException>(
                () => Assert.That(dict[101], Is.EqualTo(null)));
        }

        [Test]
        public void Set_indexer_should_store_value()
        {
            var dict = CreateTwoItemDictionary();

            dict[15] = "35C";

            Assert.That(dict.Keys.ToArray(),
                Is.EquivalentTo(new[] {5, 10, 15}));
        }

        [Test]
        public void Set_indexer_with_invalid_value_should_throw()
        {
            var dict = CreateTwoItemDictionary();

            Assert.Throws<ArgumentException>(() => dict[15] = "C");
        }

        [Test]
        public void GetEnumerator_should_return_list_of_items()
        {
            var dict = CreateTwoItemDictionary();

            var items = new string[2];
            var j = 0;
            foreach (var item in dict)
            {
                items[j++] = item.Value;
            }

            Assert.That(items,
                Is.EquivalentTo(new[] {"35A", "35B"}));
        }

        [Test]
        public void Count_should_return_item_count()
        {
            var dict = CreateTwoItemDictionary();

            Assert.That(dict.Count, Is.EqualTo(2));
        }

        [Test]
        public void Clear_should_remove_all_items()
        {
            var dict = CreateTwoItemDictionary();
            Assert.That(dict.Count, Is.EqualTo(2));

            dict.Clear();

            Assert.That(dict.Count, Is.EqualTo(0));
        }

        [Test]
        public void ContainsKey_with_valid_key_should_return_true()
        {
            var dict = CreateTwoItemDictionary();

            Assert.That(dict.ContainsKey(5), Is.True);
        }

        [Test]
        public void ContainsKey_with_invalid_key_should_return_false()
        {
            var dict = CreateTwoItemDictionary();

            Assert.That(dict.ContainsKey(42), Is.False);
        }

        [Test]
        public void Contains_with_valid_KVP_should_return_true()
        {
            var dict = CreateTwoItemDictionary();

            Assert.That(dict.Contains(new KeyValuePair<int, string>(5, "35A")),
                        Is.True);
        }

        [Test]
        public void Contains_with_invalid_KVP_should_return_false()
        {
            var dict = CreateTwoItemDictionary();

            Assert.That(dict.Contains(new KeyValuePair<int, string>(12, "35C")),
                Is.False);
        }

        [Test]
        public void CopyTo_should_copy_values()
        {
            var dict = CreateTwoItemDictionary();

            var copy = new KeyValuePair<int, string>[2];
            dict.CopyTo(copy, 0);

            Assert.That(copy.Select(v => v.Value).ToArray(),
                Is.EquivalentTo(new[] { "35A", "35B"}));
        }

        [Test]
        public void Remove_should_remove_an_item()
        {
            var dict = CreateTwoItemDictionary();

            var result = dict.Remove(5);

            Assert.That(result, Is.True);
            Assert.That(dict.Keys.ToArray(),
                Is.EquivalentTo(new[] {10}));
        }

        [Test]
        public void Remove_with_invalid_item_should_not_remove_anything()
        {
            var dict = CreateTwoItemDictionary();

            var result = dict.Remove(42);

            Assert.That(result, Is.False);
            Assert.That(dict.Keys.ToArray(),
                Is.EquivalentTo(new[] {5, 10}));
        }

        [Test]
        public void Remove_KeyValuePair_should_remove_an_item()
        {
            var dict = CreateTwoItemDictionary();

            var result = dict.Remove(new KeyValuePair<int, string>(5, "35A"));

            Assert.That(result, Is.True);
            Assert.That(dict.Keys.ToArray(),
                Is.EquivalentTo(new[] {10}));
        }

        [Test]
        public void Remove_KeyValuePair_with_item_not_in_dictionary_should_not_remove_anything()
        {
            var dict = CreateTwoItemDictionary();

            var result = dict.Remove(new KeyValuePair<int, string>(5, "35C"));

            Assert.That(result, Is.False);
            Assert.That(dict.Keys.ToArray(),
                        Is.EquivalentTo(new[] {5, 10}));
        }

        [Test]
        public void IsReadOnly_should_return_false()
        {
            var dict = CreateDictionaryUnderTest();

            Assert.That(dict.IsReadOnly, Is.False);
        }

        [Test]
        public void TryGetValue_should_return_value_in_dictionary_given_valid_key()
        {
            var dict = CreateTwoItemDictionary();

            string val;
            var result = dict.TryGetValue(10, out val);

            Assert.That(result, Is.True);
            Assert.That(val, Is.EqualTo("35B"));
        }

        [Test]
        public void TryGetValue_should_not_return_value_given_invalid_key()
        {
            var dict = CreateTwoItemDictionary();

            string val;
            var result = dict.TryGetValue(13, out val);

            Assert.That(result, Is.False);
            Assert.That(val, Is.Null);
        }
    }
}