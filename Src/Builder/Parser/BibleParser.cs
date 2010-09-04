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
using System.IO;
using System.Text.RegularExpressions;
using Builder.Model;

namespace Builder.Parser
{
    public class BibleParser : IBibleParser
    {
        private IBible bible;
        private IBook currentBook;
        private int nextChapterId;
        private int nextVerseId;
        private int currentLine;

        public IBible Parse(Stream input)
        {
            bible = new Bible();

            using (var reader = new StreamReader(input))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    currentLine++;
                    ParseLine(line);
                }
            }

            return bible;
        }

        private void ParseLine(string line)
        {
            if(line.Length <= 0)
                return;

            if (char.IsDigit(line[0]))
                ParseVerse(line);
            else if (line.StartsWith("B:"))
                ParseBook(line);
            else if (line.StartsWith("Pre "))
                ParsePreVerse(line);
            else if (line.StartsWith("Post:"))
                ParsePostVerse(line);
            else
                throw ParseException("Invalid input.");
        }

        private void ParseBook(string line)
        {
            var book = line.Substring(2).Trim();
            BookName name;
            if (!Enum.TryParse(book, true, out name))
                throw ParseException("Invalid book name '{0}'.", book);

            if (bible.Books.ContainsKey(name))
                throw ParseException("Duplicate book names '{0}'.", book);

            currentBook = new Book(bible, name);
            bible.Books.Add(name, currentBook);
        }

        private void ParsePreVerse(string line)
        {
            var spacePos = line.IndexOf(' ');
            var colonPos = line.IndexOf(':');
            var numberCharLength = colonPos - spacePos - 1;

            if (spacePos < 0 || colonPos < 0 || numberCharLength < 0)
                throw ParseException("Invalid chapter ref format.");

            var chapter = ParseChapterRef(line.Substring(spacePos + 1, numberCharLength));
            var verseData = line.Substring(colonPos + 1);

            SetPreVerseData(chapter, verseData);
        }

        private VerseRef ParseChapterRef(string chapterRefString)
        {
            int chapter;
            if(!int.TryParse(chapterRefString, out chapter))
                throw ParseException("Incorrect chapter number format.");
            if (chapter < 1)
                throw ParseException("Chapter reference must be >= 1.");
            return new VerseRef { ChapterIndex = chapter, VerseIndex = 0};
        }

        private void SetPreVerseData(VerseRef chapterRef, string verseData)
        {
            if (currentBook == null)
                throw ParseException("Input file must start with a book reference.");

            var chapter = GetCurrentChapter(chapterRef);
            if (chapter.Verses.Count > 0)
                throw ParseException("Pre-verse data must be the first verse in the chapter.");

            chapter.Verses.Add(
                new Verse(CleanupVerseData(verseData), chapter, nextVerseId++, 
                    VerseFlags.PreVerseData));
        }

        private void ParsePostVerse(string line)
        {
            SetPostVerse(CleanupVerseData(line.Substring("Post:".Length)));
        }

        private void SetPostVerse(string verseData)
        {
            if (currentBook == null)
                throw ParseException("Input file must start with a book reference.");

            if(currentBook.Chapters.Count <= 0)
                throw ParseException("Current book must already contain a chapter.");

            var chapter = currentBook.Chapters[currentBook.Chapters.Count - 1];
            if (HasPostVerseData(chapter))
                throw ParseException("Only one post-verse is allowed per chapter.");

            chapter.Verses.Add(
                new Verse(verseData, chapter, nextVerseId++, VerseFlags.PostVerseData));
        }

        private static bool HasPostVerseData(IChapter chapter)
        {
            return chapter.Verses.Count > 0 && chapter.Verses[chapter.Verses.Count - 1].IsPostVerse;
        }

        private void ParseVerse(string line)
        {
            var spacePos = line.IndexOf(' ');
            if (spacePos < 0)
                throw ParseException("Invalid verse line: missing space.");
            var verseRef = ParseVerseRef(line.Substring(0, spacePos));
            var verseData = line.Substring(spacePos + 1);

            SetVerseData(verseRef, verseData);
        }

        private void SetVerseData(VerseRef verseRef, string verseData)
        {
            if (currentBook == null)
                throw ParseException("Input file must start with a book reference.");

            var curChapter = GetCurrentChapter(verseRef);

            var expectedCount = verseRef.VerseIndex + (HasPreVerseData(curChapter) ? 0 : -1);

            if(curChapter.Verses.Count != expectedCount)
                throw ParseException("Verse index out of sequence.");

            if (HasPostVerseData(curChapter))
                throw ParseException("Verse data must not come after post data.");

            curChapter.Verses.Add(
                new Verse(CleanupVerseData(verseData), curChapter, nextVerseId++, VerseFlags.Normal));
        }

        private static bool HasPreVerseData(IChapter chapter)
        {
            return chapter.Verses.Count > 0 && chapter.Verses[0].IsPreVerse;
        }

        private IChapter GetCurrentChapter(VerseRef verseRef)
        {
            IChapter curChapter;
            if (verseRef.ChapterIndex == currentBook.Chapters.Count)
            {
                curChapter = currentBook.Chapters[verseRef.ChapterIndex - 1];
            }
            else if (verseRef.ChapterIndex == currentBook.Chapters.Count + 1)
            {
                curChapter = new Chapter(currentBook, nextChapterId++);
                currentBook.Chapters.Add(curChapter);
            }
            else
            {
                throw ParseException("Chapter index must be sequential.");
            }
            return curChapter;
        }

        private static string CleanupVerseData(string verseData)
        {
            return
                Regex.Replace(verseData, "[ ][ ]+", " ")
                    .Trim();
        }

        private VerseRef ParseVerseRef(string verseRefString)
        {
            var colonPos = verseRefString.IndexOf(":");
            if(colonPos < 0)
                throw ParseException("Invalid verse ref: Missing colon: {0}", verseRefString);
            
            var chapterStr = verseRefString.Substring(0, colonPos);
            int chapterIndex;
            if(!int.TryParse(chapterStr, out chapterIndex))
                throw ParseException("Invalid verse ref: Incorrect chapter number format: '{0}'.",
                                     chapterStr);

            if (chapterIndex < 1)
                throw ParseException("The chapter index must be >= 1.");

            var verseStr = verseRefString.Substring(colonPos + 1);
            int verseIndex;
            if (!int.TryParse(verseStr, out verseIndex))
                throw ParseException("Invalid verse ref: Incorrect verse number format: '{0}'.",
                                     verseStr);

            return new VerseRef { ChapterIndex = chapterIndex, VerseIndex = verseIndex};
        }

        private Exception ParseException(string message, params object[] parameters)
        {
            throw new ParseException(currentLine, 
                                     string.Format("Line {0}: ", currentLine) +
                                     string.Format(message, parameters));
        }
    }


}