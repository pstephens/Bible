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
using System.Linq;
using System.Text;
using Builder.Model;
using Builder.Parser;
using Builder.Services;
using NUnit.Framework;

namespace Builder.UnitTests.Services
{
    [TestFixture]
    public class TokenToVerseMapTests
    {
        [Test]
        public void Tokens_should_return_Token_list_given_valid_bible()
        {
            var bible = CreateBible();

            var service = bible.GetService<TokenToVerseMap>();
            var tokens = service.TokenFrequency()
                .OrderBy(tf => tf.Token.TokenString, StringComparer.InvariantCulture)
                .Select(tf => tf.Token)
                .ToArray();

            Assert.That(tokens, Is.EqualTo(new Token[] 
                { " ", "!", ".", "a", "First", "Isn't", "Second", "Third", "verse", "Verse", "VERSE" }));
        }

        [Test]
        public void Tokens_should_return_Token_list_with_proper_frequency()
        {
            var bible = CreateBible();

            var service = bible.GetService<TokenToVerseMap>();
            var freq =
                service.TokenFrequency()
                    .Select(tf => tf.RelatedVerses().Sum(rv => rv.Frequency))
                    .OrderByDescending(f => f)
                    .ToArray();

            Assert.That(freq, Is.EqualTo(new [] { 5, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1}));
        }

        [Test]
        public void Token_frequency_should_be_calculated_correctly_per_verse()
        {
            var bible = CreateBible("B:Job\r\n1:1 word word word.\r\n1:2 word word with some other words.");
            var chapter = bible.Books[BookName.Job].Chapters[0];
            var verse1 = chapter.Verses[0];
            var verse2 = chapter.Verses[1];

            var service = bible.GetService<TokenToVerseMap>();
            TokenVerseFrequency tokenVerseFrequency;
            Assert.That(service.TryGetTokenFrequency("word", out tokenVerseFrequency), Is.True);

            var relatedVerses = tokenVerseFrequency.RelatedVerses()
                .OrderBy(rv => rv.Frequency)
                .ToArray();

            Assert.That(relatedVerses.Length, Is.EqualTo(2));
            Assert.That(relatedVerses[0].Frequency, Is.EqualTo(2));
            Assert.That(relatedVerses[0].Verse, Is.SameAs(verse2));
            Assert.That(relatedVerses[1].Frequency, Is.EqualTo(3));
            Assert.That(relatedVerses[1].Verse, Is.SameAs(verse1));
        }

        [Test]
        public void Tokens_with_related_not_set_should_throw()
        {
            var service = new TokenToVerseMap();

            Assert.Throws<InvalidOperationException>(() => service.TokenFrequency().ToArray());
        }

        private static IBible CreateBible(string bibleText = 
            "B:Job\r\n1:1 First verse\r\n1:2 Second VERSE!" + 
            "\r\n2:1 Third Verse.\r\n2:2 Isn't a verse")
        {
            var parser = new BibleParser();
            var encodedString = Encoding.ASCII.GetBytes(bibleText);
            var str = new MemoryStream(encodedString);
            return parser.Parse(str);
        }
    }
}