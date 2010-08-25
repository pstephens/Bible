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
    }
}