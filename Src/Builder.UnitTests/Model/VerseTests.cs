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
    public class VerseTests
    {
        private static IVerse CreateVerseUnderTest(string verseData = "Content", 
            IChapter chapter = null, int id = 0, VerseFlags flags = VerseFlags.Normal)
        {
            return new Verse(verseData, chapter ?? new ChapterStub(), id, flags);
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
            Assert.Throws<ArgumentNullException>(() => new Verse("Data", null, 0, VerseFlags.Normal));
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

        [TestCase(5, 6, Result = false)]
        [TestCase(5, 5, Result = true)]
        public bool Exercise_GetHashCode(int id1, int id2)
        {
            var verse1 = CreateVerseUnderTest(id: id1);
            var verse2 = CreateVerseUnderTest(id: id2);

            var hash1 = verse1.GetHashCode();
            var hash2 = verse2.GetHashCode();

            return hash1 == hash2;
        }

        [TestCase(5, 7, Result = false)]
        [TestCase(5, 5, Result = true)]
        public bool Exercise_Equals(int id1, int id2)
        {
            var verse1 = CreateVerseUnderTest(id: id1);
            var verse2 = CreateVerseUnderTest(id: id2);

            return verse1.Equals(verse2);
        }

        [TestCase(5, 7, Result = false)]
        [TestCase(5, 5, Result = true)]
        public bool Exercise_EqualsObject(int id1, int id2)
        {
            object verse1 = CreateVerseUnderTest(id: id1);
            object verse2 = CreateVerseUnderTest(id: id2);

            return verse1.Equals(verse2);
        }

        [Test]
        public void Equals_to_null_IVerse_should_be_false()
        {
            var verse = CreateVerseUnderTest(id: 5);

            Assert.That(verse.Equals(null), Is.False);
        }

        [Test]
        public void EqualsObject_to_wrong_type_should_be_false()
        {
            object verse = CreateVerseUnderTest(id: 5);

            Assert.That(verse.Equals("not_a_verse"), Is.False);
        }

        [TestCase(VerseFlags.Normal, Result=false)]
        [TestCase(VerseFlags.PreVerseData, Result=true)]
        [TestCase(VerseFlags.PostVerseData, Result=false)]
        public bool Exercise_IsPreVerse(VerseFlags flags)
        {
            var verse = CreateVerseUnderTest(flags: flags);
            return verse.IsPreVerse;
        }

        [TestCase(VerseFlags.Normal, Result = false)]
        [TestCase(VerseFlags.PostVerseData, Result = true)]
        [TestCase(VerseFlags.PreVerseData, Result = false)]
        public bool Exercise_IsPostVerse(VerseFlags flags)
        {
            var verse = CreateVerseUnderTest(flags: flags);
            return verse.IsPostVerse;
        }
    }
}
