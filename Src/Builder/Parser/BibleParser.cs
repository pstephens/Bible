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
            else
            {
                throw new InvalidOperationException(
                    string.Format("Invalid input line: '{0}'", line));
            }
        }

        private void ParseBook(string line)
        {
            var book = line.Substring(2);
            BookName name;
            Enum.TryParse(book, true, out name);

            currentBook = new Book(bible, name);
            bible.Books.Add(name, currentBook);
        }

        private void ParseVerse(string line)
        {
            var spacePos = line.IndexOf(' ');
            if(spacePos < 0)
                throw new InvalidOperationException(
                    string.Format("Invalid verse line: Missing space: {0}", line));
            var verseRef = ParseVerseRef(line.Substring(0, spacePos));
            var verseData = line.Substring(spacePos + 1);

            SetVerseData(verseRef, verseData);
        }

        private void SetVerseData(VerseRef verseRef, string verseData)
        {
            while(currentBook.Chapters.Count < verseRef.ChapterIndex)
                currentBook.Chapters.Add(new Chapter(currentBook, nextChapterId++));
            var curChapter = currentBook.Chapters[verseRef.ChapterIndex - 1];

            if(curChapter.Verses.Count != verseRef.VerseIndex - 1)
                throw new InvalidOperationException(
                    string.Format("Verse index out of sequence at line {0}.", currentLine));

            curChapter.Verses.Add(
                new Verse(verseData, curChapter, nextVerseId++));
        }

        private static VerseRef ParseVerseRef(string verseRefString)
        {
            var colonPos = verseRefString.IndexOf(":");
            if(colonPos < 0)
                throw new InvalidOperationException(
                    string.Format("Invalid verse ref: Missing colon: {0}", verseRefString));
            var chapterIndex = int.Parse(verseRefString.Substring(0, colonPos));
            var verseIndex = int.Parse(verseRefString.Substring(colonPos + 1));
            return new VerseRef { ChapterIndex = chapterIndex, VerseIndex = verseIndex};
        }
    }
}