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

namespace Builder.UnitTests
{
    [TestFixture]
    public class ValidatingDictionaryTests
    {
        private static ValidatingDictionary<int, string> CreateDictionaryUnderTest(
            IDictionary<int, string> dictionary = null)
        {
            return new ValidatingDictionary<int, string>(
                val => val != null && val.Contains("35"), dictionary);
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
        public void Add_invalid_item_should_throw()
        {
            var dict = CreateDictionaryUnderTest();

            Assert.Throws<ArgumentException>(
                () => dict.Add(5, "Doesn't contain number thirty-five"));
        }
    }
}