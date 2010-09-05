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
using Builder.Model;
using NUnit.Framework;

namespace Builder.UnitTests.Model
{
    [TestFixture]
    public class NavigationExtensionMethodsTests
    {
        [Test]
        public void AllVerses_should_throw_if_bible_object_is_null()
        {
            IBible bible = null;

            Assert.Throws<ArgumentNullException>(() => bible.AllVerses().ToArray());
        }

        [Test]
        public void AllVerses_should_return_verses_in_bible_order()
        {
            var bible = CreateBibleData();

            var verses = bible.AllVerses();

            Assert.That(verses.Select(verse => verse.Text).ToArray(),
                Is.EqualTo(new [] {"Pre Verse 1", "Verse 1", "Post Verse 1", 
                "Chapter 2 Verse 1",
                "Job Chapter 1 Verse 1"}));
        }

        private static IBible CreateBibleData()
        {
            IBible bible = new Bible();

            IBook book = new Book(bible, BookName.Job);
            bible.Books.Add(BookName.Job, book);

            IChapter chapter = new Chapter(book, 2);
            book.Chapters.Add(chapter);
            chapter.Verses.Add(new Verse("Job Chapter 1 Verse 1", chapter, 4, VerseFlags.Normal));
            
            book = new Book(bible, BookName.Exodus);
            bible.Books.Add(BookName.Exodus, book);
            
            chapter = new Chapter(book, 0);
            book.Chapters.Add(chapter);
            chapter.Verses.Add(new Verse("Pre Verse 1", chapter, 0, VerseFlags.PreVerseData));
            chapter.Verses.Add(new Verse("Verse 1", chapter, 1, VerseFlags.Normal));
            chapter.Verses.Add(new Verse("Post Verse 1", chapter, 2, VerseFlags.PostVerseData));

            chapter = new Chapter(book, 1);
            book.Chapters.Add(chapter);
            chapter.Verses.Add(new Verse("Chapter 2 Verse 1", chapter, 3, VerseFlags.Normal));

            return bible;
        }
    }
}