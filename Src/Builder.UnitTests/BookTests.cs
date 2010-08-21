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
using Builder.UnitTests.HandMocks;
using NUnit.Framework;

namespace Builder.UnitTests
{
    [TestFixture]
    public class BookTests
    {
        private static Book CreateBookUnderTest(IBible bible = null, BookName id = BookName.None)
        {
            return new Book(bible ?? new BibleStub(), id);
        }

        [Test]
        public void Bible_should_return_injected_Bible()
        {
            var bible = new BibleStub();
            var book = CreateBookUnderTest(bible);

            Assert.That(book.Bible, Is.SameAs(bible));
        }

        [Test]
        public void Book_with_null_bible_should_throw()
        {
            Assert.Throws<ArgumentNullException>(() => new Book(null, BookName.None));
        }

        [Test]
        public void Id_should_return_injected_id()
        {
            var book = CreateBookUnderTest(id: BookName.Zephaniah);

            Assert.That(book.Id, Is.EqualTo(BookName.Zephaniah));
        }

        [Test]
        public void Chapters_should_return_IList_on_new_Book()
        {
            var book = CreateBookUnderTest();

            Assert.That(book.Chapters, Is.Not.Null);
        }

        [Test]
        public void Chapters_should_return_chapter_objects_added_to_collection()
        {
            var book = CreateBookUnderTest();
            var chapter1 = new ChapterStub {Id = 5};
            var chapter2 = new ChapterStub {Id = 10};

            book.Chapters.Add(chapter1);
            book.Chapters.Add(chapter2);

            Assert.That(book.Chapters.Select(ch => ch.Id).ToArray(),
                Is.EquivalentTo(new[] {5, 10}));
        }

        [Test]
        public void Chapters_Add_null_should_throw()
        {
            var book = CreateBookUnderTest();

            Assert.Throws<ArgumentException>(
                () => book.Chapters.Add(null));
        }
    }
}