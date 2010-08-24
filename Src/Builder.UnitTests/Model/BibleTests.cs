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
using Builder.Model;
using Builder.UnitTests.HandMocks;
using NUnit.Framework;

namespace Builder.UnitTests.Model
{
    [TestFixture]
    public class BibleTests
    {
        [Test]
        public void Books_collection_should_allow_setting_an_item()
        {
            var bible = new Bible();
            var book = new BookStub();

            bible.Books[BookName.Genesis] = book;

            Assert.That(bible.Books[BookName.Genesis], Is.SameAs(book));
        }

        [Test]
        public void Setting_null_value_into_Books_collection_should_throw()
        {
            var bible = new Bible();

            Assert.Throws<ArgumentException>(() => bible.Books[BookName.Genesis] = null);
        }

        [Test]
        public void Adding_null_value_to_Books_collection_should_throw()
        {
            var bible = new Bible();
            IBook book = null;

            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentException>(() => bible.Books.Add(BookName.Genesis, book));
        }
    }
}