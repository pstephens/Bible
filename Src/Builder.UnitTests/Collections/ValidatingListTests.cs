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
using System.Linq;
using Builder.Collections;
using NUnit.Framework;

namespace Builder.UnitTests.Collections
{
    [TestFixture]
    public class ValidatingListTests
    {
        private static ValidatingList<int> CreateListUnderTest()
        {
            return new ValidatingList<int>(item => item >= 5 && item <= 10);
        }

        private static ValidatingList<int> CreateListUnderTest(params int[] items)
        {
            return new ValidatingList<int>(item => item >= 5 && item <= 10, items);
        }

        [Test]
        public void Should_be_able_to_Add_8()
        {
            var list = CreateListUnderTest();

            list.Add(8);

            Assert.That(list[0], Is.EqualTo(8));
        }

        [Test]
        public void Add_12_should_throw()
        {
            var list = CreateListUnderTest();

            Assert.Throws<ArgumentException>(
                () => list.Add(12));
        }

        [Test]
        public void Setting_index_0_to_5_should_set()
        {
            var list = CreateListUnderTest();
            list.Add(8);

            list[0] = 5;

            Assert.That(list[0], Is.EqualTo(5));
        }

        [Test]
        public void Setting_index_0_to_2_should_throw()
        {
            var list = CreateListUnderTest();
            list.Add(8);

            Assert.Throws<ArgumentException>(() => list[0] = 2);
        }

        [Test]
        public void Insert_6_at_0_should_insert_new_item()
        {
            var list = CreateListUnderTest();
            list.Add(8);

            list.Insert(0, 6);

            Assert.That(list[0], Is.EqualTo(6));
            Assert.That(list[1], Is.EqualTo(8));
        }

        [Test]
        public void Insert_3_at_0_should_throw()
        {
            var list = CreateListUnderTest();
            list.Add(8);

            Assert.Throws<ArgumentException>(
                () => list.Insert(0, 3));
        }

        [Test] 
        public void Count_with_3_items_should_return_3()
        {
            var list = CreateListUnderTest();
            list.Add(5);
            list.Add(6);
            list.Add(7);

            Assert.That(list.Count, Is.EqualTo(3));
        }

        [Test]
        public void CopyTo_with_3_items_should_copy_3_items()
        {
            var list = CreateListUnderTest();
            list.Add(5);
            list.Add(6);
            list.Add(7);
            var outputArray = new int[3];

            list.CopyTo(outputArray, 0);

            Assert.That(outputArray, Is.EquivalentTo(new[] {5, 6, 7}));
        }

        [Test]
        public void GetEnumerator_should_enumerate_through_items()
        {
            var list = CreateListUnderTest();
            list.Add(5);
            list.Add(6);
            list.Add(7);

            Assert.That(list.ToArray(),
                        Is.EquivalentTo(new[] {5, 6, 7}));
        }

        [Test]
        public void Constructor_with_items_should_prepopulate_list()
        {
            var list = new ValidatingList<int>(item => item >= 5 && item <= 10, 
                new[] {5, 6, 7});

            Assert.That(list.ToArray(), Is.EquivalentTo(new[] {5, 6, 7}));
        }

        [Test]
        public void Constructor_with_invalid_items_should_throw()
        {
            Assert.Throws<ArgumentException>(() =>
                                             new ValidatingList<int>(item => item >= 5 && item <= 10,
                                                                     new[] {1, 5, 15}));
        }

        [Test]
        public void Clear_should_remove_items()
        {
            var list = CreateListUnderTest(5, 6, 7);
            Assert.That(list.Count, Is.EqualTo(3));

            list.Clear();

            Assert.That(list.Count, Is.EqualTo(0));
        }

        [TestCase(6, Result=true)]
        [TestCase(3, Result=false)]
        [TestCase(5, Result=true)]
        public bool Exercise_Contains(int itemToTest)
        {
            var list = CreateListUnderTest(5, 6, 7);

            return list.Contains(itemToTest);
        }

        [TestCase(6, true, Result=new[] {5, 7})]
        [TestCase(10, false, Result=new[] {5, 6, 7})]
        public int[] Exercise_Remove(int itemToRemove, bool removeResults)
        {
            var list = CreateListUnderTest(5, 6, 7);

            Assert.That(list.Remove(itemToRemove), Is.EqualTo(removeResults));

            return list.ToArray();
        }

        [Test]
        public void IsReadOnly_should_return_false()
        {
            var list = CreateListUnderTest();

            Assert.That(list.IsReadOnly, Is.False);
        }

        [TestCase(5, Result=0)]
        [TestCase(7, Result=2)]
        [TestCase(10, Result=-1)]
        public int Exercise_IndexOf(int itemToFind)
        {
            var list = CreateListUnderTest(5, 6, 7);

            return list.IndexOf(itemToFind);
        }

        [Test]
        public void RemoveAt_2_should_remove_7()
        {
            var list = CreateListUnderTest(5, 6, 7);

            list.RemoveAt(2);

            Assert.That(list.ToArray(), Is.EquivalentTo(new[] {5, 6}));
        }
    }
}