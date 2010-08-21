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
using Builder.UnitTests.HandMocks;
using NUnit.Framework;

namespace Builder.UnitTests
{
    [TestFixture]
    public class VerseTests
    {
        private static IVerse CreateVerseUnderTest(string verseData = "Content", 
            IChapter chapter = null, int id = 0, int index = 0)
        {
            return new Verse(verseData, chapter ?? new ChapterStub(), id, index);
        }

        [Test]
        public void Verse_Text_should_return_text()
        {
            const string text = "Some verse data.";
            
            var verse = CreateVerseUnderTest(text);

            Assert.That(verse.Text, Is.EqualTo(text));
        }

        [TestCase(null)]
        [TestCase("")]
        public void Verse_Text_with_null_or_empty_should_throw(string text)
        {
            Assert.Throws<ArgumentNullException>(() => CreateVerseUnderTest(text));
        }

        [Test]
        public void Verse_Chapter_should_return_injected_Chapter()
        {
            IChapter ch = new ChapterStub();

            var verse = CreateVerseUnderTest(chapter: ch);

            Assert.That(verse.Chapter, Is.SameAs(ch));
        }

        [Test]
        public void Verse_Chapter_with_null_should_throw()
        {
            Assert.Throws<ArgumentNullException>(() => new Verse("Data", null, 0, 0));
        }

        [Test]
        public void Verse_Id_should_return_injected_Id()
        {
            var verse = CreateVerseUnderTest(id: 25);

            Assert.That(verse.Id, Is.EqualTo(25));
        }

        [Test]
        public void Verse_Id_with_less_than_zero_should_throw()
        {
            Assert.Throws<ArgumentException>(
                () => CreateVerseUnderTest(id: -1));
        }

        [Test]
        public void Verse_Index_should_return_injected_Index()
        {
            var verse = CreateVerseUnderTest(index: 15);

            Assert.That(verse.Index, Is.EqualTo(15));
        }

        [Test]
        public void Verse_Index_with_less_than_zero_should_throw()
        {
            Assert.Throws<ArgumentException>(
                () => CreateVerseUnderTest(index: -1));
        }
    }
}
