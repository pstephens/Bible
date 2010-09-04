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
using System.Text;
using Builder.Model;
using Builder.Parser;
using NUnit.Framework;

namespace Builder.UnitTests.Parser
{
    [TestFixture]
    public class BibleParserTests
    {
        private const string TestData = @"
B:Genesis
1:1 In the beginning God created the heaven and the earth.";

        private static Stream StringToStream(string data)
        {
            var encodedString = Encoding.ASCII.GetBytes(data);
            return new MemoryStream(encodedString);
        }

        [Test]
        public void Parse_of_TestData_should_have_one_book_Genesis()
        {
            Exercise_Parse_TestData(
                bible => Assert.That(bible.Books.Count, Is.EqualTo(1)));
        }

        [Test]
        public void Parse_of_TestData_should_have_one_chapter()
        {
            Exercise_Parse_TestData(
                bible => Assert.That(bible.Books[BookName.Genesis].Chapters.Count,
                                     Is.EqualTo(1)));
        }

        [Test]
        public void Parse_of_TestData_should_have_one_verse()
        {
            Exercise_Parse_TestData(
                bible => Assert.That(bible.Books[BookName.Genesis].Chapters[0].Verses.Count,
                                     Is.EqualTo(1)));
        }

        [Test]
        public void Parse_of_TestData_should_have_verse_content_of_Gen1_1()
        {
            Exercise_Parse_TestData(
                bible => Assert.That(bible.Books[BookName.Genesis].Chapters[0].Verses[0].Text,
                                     Is.EqualTo("In the beginning God created the heaven and the earth.")));
        }

        private static void Exercise_Parse_TestData(Action<IBible> assert)
        {
            var parser = new BibleParser();
            var stream = StringToStream(TestData);

            var bible = parser.Parse(stream);

            assert(bible);
        }

        [Test]
        public void Parse_with_no_book_should_throw()
        {
            var parser = new BibleParser();
            var stream = StringToStream("1:1 In the beginning God created the heaven and the earth.");

            Assert.Throws<ParseException>(() => parser.Parse(stream));
        }

        [Test]
        public void Parse_with_invalid_book_name_should_throw()
        {
            var parser = new BibleParser();
            var stream = StringToStream("B:NotABookName");

            Assert.Throws<ParseException>(() => parser.Parse(stream));
        }

        [Test]
        public void Parse_line_with_invalid_starting_chars_should_throw()
        {
            var parser = new BibleParser();
            var stream = StringToStream("** Strange Line **");

            Assert.Throws<ParseException>(() => parser.Parse(stream));
        }

        [Test]
        public void Parse_with_duplicate_book_names_should_throw()
        {
            var parser = new BibleParser();
            var stream = StringToStream("B:Genesis\r\nB:Genesis");

            Assert.Throws<ParseException>(() => parser.Parse(stream));
        }

        [Test]
        public void Parse_with_no_data_should_return_empty_Bible()
        {
            var parser = new BibleParser();
            var stream = StringToStream("");

            var bible = parser.Parse(stream);

            Assert.That(bible.Books.Count, Is.EqualTo(0));
        }

        [Test]
        public void Parse_with_blank_lines_should_return_Bible()
        {
            var parser = new BibleParser();
            var stream = StringToStream("\r\nB:Genesis\r\n\r\n1:1 Verse 1");

            var bible = parser.Parse(stream);

            Assert.That(bible.Books[BookName.Genesis].Chapters[0].Verses[0].Text,
                        Is.EqualTo("Verse 1"));
        }

        [Test]
        public void Parse_verse_with_no_space_should_throw()
        {
            var parser = new BibleParser();
            var stream = StringToStream("B:Titus\r\n1:1NoSpaceHere.");

            Assert.Throws<ParseException>(() => parser.Parse(stream));
        }

        [Test]
        public void Parse_verse_reference_must_go_in_order()
        {
            var parser = new BibleParser();
            var stream = StringToStream("B:Job\r\n1:1 First\r\n1:3 Third?");

            Assert.Throws<ParseException>(() => parser.Parse(stream));
        }

        [TestCase("-1")]
        [TestCase("0")]
        [TestCase("2")]
        public void Parse_verse_reference_not_starting_with_1_must_throw(string verseNum)
        {
            var parser = new BibleParser();
            var stream = StringToStream("B:Job\r\n1:" + verseNum + " Zeroth verse.");

            Assert.Throws<ParseException>(() => parser.Parse(stream));
        }

        [Test]
        public void Parse_chapter_reference_must_go_in_order()
        {
            var parser = new BibleParser();
            var stream = StringToStream("B:Job\r\n1:1 First\r\n3:1 Third?");

            Assert.Throws<ParseException>(() => parser.Parse(stream));
        }

        [TestCase("-1")]
        [TestCase("0")]
        [TestCase("2")]
        public void Parse_chapter_reference_not_starting_with_1_must_throw(string chapterNum)
        {
            var parser = new BibleParser();
            var stream = StringToStream("B:Job\r\n" + chapterNum + ":1 Zeroth chapter.");

            Assert.Throws<ParseException>(() => parser.Parse(stream));
        }

        [Test]
        public void Parse_verse_reference_with_no_colon_should_throw()
        {
            var parser = new BibleParser();
            var stream = StringToStream("B:Job\r\n1*1 First");

            Assert.Throws<ParseException>(() => parser.Parse(stream));
        }

        [Test]
        public void Parse_verse_reference_with_invalid_chapter_number_should_throw()
        {
            var parser = new BibleParser();
            var stream = StringToStream("B:Job\r\n1ST:1 First");

            Assert.Throws<ParseException>(() => parser.Parse(stream));
        }

        [Test]
        public void Parse_verse_reference_with_invalid_verse_number_should_throw()
        {
            var parser = new BibleParser();
            var stream = StringToStream("B:Job\r\n1:1ST First");

            Assert.Throws<ParseException>(() => parser.Parse(stream));
        }

        [Test]
        public void Parse_verse_with_internal_spaces_should_remove_spaces()
        {
            var parser = new BibleParser();
            var stream = StringToStream("B:Job\r\n1:1   A  verse   with   internal   spaces  removed.  ");

            var bible = parser.Parse(stream);

            var bibleText = bible.Books[BookName.Job].Chapters[0].Verses[0].Text;
            Assert.That(bibleText, Is.EqualTo("A verse with internal spaces removed."));
        }

        [Test]
        public void Parse_with_Pre_Verse_should_parse()
        {
            var parser = new BibleParser();
            var stream = StringToStream("B:Job\r\nPre 1:Pre verse content.");

            var bible = parser.Parse(stream);

            Assert.That(bible.Books[BookName.Job].Chapters[0].Verses[0].IsPreVerse, 
                Is.True);
        }

        [Test]
        public void Parse_with_Pre_Verse_with_chapter_not_next_chapter_should_throw()
        {
            var parser = new BibleParser();
            var stream = StringToStream("B:Job\r\n1:1 First\r\nPre 1: Incorrect.");

            Assert.Throws<ParseException>(() => parser.Parse(stream));
        }

        [Test]
        public void Parse_with_Pre_Verse_with_next_chapter_should_parse()
        {
            var parser = new BibleParser();
            var stream = StringToStream("B:Job\r\n1:1 First\r\nPre 2: Second.");

            var bible = parser.Parse(stream);

            Assert.That(bible.Books[BookName.Job].Chapters[1].Verses[0].IsPreVerse, Is.True);
        }

        [Test]
        public void Parse_with_Pre_Verse_with_no_book_should_throw()
        {
            var parser = new BibleParser();
            var stream = StringToStream("Pre 1: First, but with no book.");

            Assert.Throws<ParseException>(() => parser.Parse(stream));
        }

        [Test]
        public void Parse_with_invalid_Pre_Verse_chapter_reference_should_throw()
        {
            var stream1 = StringToStream("B:Job\r\nPre A:Invalid chapter ref.");
            var stream2 = StringToStream("B:Job\r\nPre No chapter ref.");

            Assert.Throws<ParseException>(() => new BibleParser().Parse(stream1));
            Assert.Throws<ParseException>(() => new BibleParser().Parse(stream2));
        }

        [Test]
        public void Parse_with_Pre_Verse_chapter_ref_less_than_one_should_throw()
        {
            var stream = StringToStream("B:Job\r\nPre -1:Invalid chapter ref number.");

            Assert.Throws<ParseException>(() => new BibleParser().Parse(stream));
        }

        [Test]
        public void Parse_pre_verse_with_internal_spaces_should_remove_spaces()
        {
            var parser = new BibleParser();
            var stream = StringToStream("B:Job\r\nPre 1:   A  verse   with   internal   spaces  removed.  ");

            var bible = parser.Parse(stream);

            var bibleText = bible.Books[BookName.Job].Chapters[0].Verses[0].Text;
            Assert.That(bibleText, Is.EqualTo("A verse with internal spaces removed."));
        }

        [Test]
        public void Parse_pre_verse_data_with_additional_verses_should_parse()
        {
            var parser = new BibleParser();
            var stream = StringToStream("B:Job\r\nPre 1: Pre verse data.\r\n1:1 More verse data.");

            var bible = parser.Parse(stream);

            var chapter = bible.Books[BookName.Job].Chapters[0];
            var preVerseText = chapter.Verses[0].Text;
            var verseText = chapter.Verses[1].Text;
            Assert.That(preVerseText, Is.EqualTo("Pre verse data."));
            Assert.That(verseText, Is.EqualTo("More verse data."));
        }

        [Test]
        public void Parse_with_Post_Verse_should_parse()
        {
            var parser = new BibleParser();
            var stream = StringToStream("B:Job\r\n1:1 First\r\nPost: Post data.");

            var bible = parser.Parse(stream);

            Assert.That(bible.Books[BookName.Job].Chapters[0].Verses[1].IsPostVerse,
                        Is.True);
        }

        [Test]
        public void Parse_with_Post_Verse_without_book_should_throw()
        {
            var parser = new BibleParser();
            var stream = StringToStream("Post: Post data.");

            Assert.Throws<ParseException>(() => parser.Parse(stream));
        }

        [Test]
        public void Parse_with_Post_Verse_without_chapter_should_throw()
        {
            var parser = new BibleParser();
            var stream = StringToStream("B:Job\r\nPost: Post data.");

            Assert.Throws<ParseException>(() => parser.Parse(stream));
        }

        [Test]
        public void Parse_with_multiple_Post_Verse_in_same_chapter_should_throw()
        {
            var parser = new BibleParser();
            var stream = StringToStream("B:Job\r\n1:1 Data\r\nPost: Post\r\nPost: Post2");

            Assert.Throws<ParseException>(() => parser.Parse(stream));
        }

        [Test]
        public void Parse_with_verse_data_after_Post_Verse_should_throw()
        {
            var parser = new BibleParser();
            var stream = StringToStream("B:Job\r\n1:1 Data\r\nPost: Post\r\n1:3 More data");

            Assert.Throws<ParseException>(() => parser.Parse(stream));
        }
    }
}