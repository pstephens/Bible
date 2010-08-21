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
    public class ChapterTests
    {
        private static Chapter CreateChapterUnderTest(BookStub book = null, 
                                                      int id = 0, int index = 0)
        {
            return new Chapter(book ?? new BookStub(), id, index);
        }

        [Test]
        public void Book_should_return_injected_Book()
        {
            var book = new BookStub();
            var chapter = CreateChapterUnderTest(book);

            Assert.That(chapter.Book, Is.SameAs(book));
        }

        [Test]
        public void Book_with_null_should_throw()
        {
            Assert.Throws<ArgumentNullException>(
                () => new Chapter(null, 0, 0));
        }

        [Test]
        public void Id_should_return_injected_id()
        {
            var chapter = CreateChapterUnderTest(id: 25);

            Assert.That(chapter.Id, Is.EqualTo(25));
        }

        [Test]
        public void Id_with_negative_should_throw()
        {
            Assert.Throws<ArgumentException>(
                () => CreateChapterUnderTest(id: -1));
        }

        [Test]
        public void Index_should_return_injected_index()
        {
            var chapter = CreateChapterUnderTest(index: 15);

            Assert.That(chapter.Index, Is.EqualTo(15));
        }

        [Test]
        public void Index_with_negative_should_throw()
        {
            Assert.Throws<ArgumentException>(
                () => CreateChapterUnderTest(index: -1));
        }

        [Test]
        public void Verses_should_not_be_null_on_new_Chapter()
        {
            var chapter = CreateChapterUnderTest();

            var verses = chapter.Verses;

            Assert.That(verses, Is.Not.Null);
        }

        [Test]
        public void Should_be_able_to_Add_to_the_Verses_collection()
        {
            var chapter = CreateChapterUnderTest();
            var verse1 = new VerseStub { Text="In"};
            var verse2 = new VerseStub { Text="the"};

            chapter.Verses.Add(verse1);
            chapter.Verses.Add(verse2);
            
            Assert.That(chapter.Verses.Select(v => v.Text).ToArray(),
                Is.EquivalentTo(new[] {"In", "the"}));
        }

        [Test]
        public void Verses_Add_with_null_IVerse_should_throw()
        {
            var chapter = CreateChapterUnderTest();

            Assert.Throws<ArgumentException>(() => chapter.Verses.Add(null));
        }
    }
}