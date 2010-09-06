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
using Builder.Services;
using Builder.UnitTests.HandMocks;
using NUnit.Framework;

namespace Builder.UnitTests.Services
{
    [TestFixture]
    public class VerseTokensTests
    {
        [Test]
        public void Tokens_should_return_verse_tokens()
        {
            var verse = CreateTestVerse("This is a sentence with tokens. It's also containing a contraction.");

            var verseTokens = verse.GetService<IVerseTokens>();
            var tokens = verseTokens.Tokens().Select(token => token.TokenString).ToArray();

            Assert.That(tokens, Is.EqualTo(new []
                    {
                        "This", " ", "is", " ", "a", " ", "sentence", " ", "with", " ", "tokens", ".", " ",
                        "It's", " ", "also", " ", "containing", " ", "a", " ", "contraction", "."
                    }));
        }

        [Test]
        public void Tokens_when_related_verse_not_associated_should_throw()
        {
            var verseTokens = new VerseTokens();

            Assert.Throws<InvalidOperationException>(
                () => verseTokens.Tokens().ToArray());
        }

        [Test]
        public void Tokens_should_properly_classify_token_as_word_or_nonword()
        {
            var verse = CreateTestVerse(
                "This is a sentence with tokens. It's also containing a contraction.");

            var verseTokens = verse.GetService<IVerseTokens>();
            var tokens = verseTokens.Tokens().Select(token => token.IsWord).ToArray();

            Assert.That(tokens, Is.EqualTo(new[]
                    {
                        true, false, true, false, true, false, true, false, true, false, true, false, false,
                        true, false, true, false, true, false, true, false, true, false
                    }));
        }

        [Test]
        public void Tokens_with_multiple_adjacent_nonwords_should_parse_ok()
        {
            var verse = CreateTestVerse("Punctuation:;.");

            var verseTokens = verse.GetService<IVerseTokens>();
            var tokens = verseTokens.Tokens().Select(token => token.IsWord).ToArray();

            Assert.That(tokens, Is.EqualTo(new[]
                {true, false, false, false}));
        }

        [Test]
        public void Tokens_ends_in_a_word_token_should_return_the_last_token()
        {
            var verse = CreateTestVerse("Ends in a word");

            var verseTokens = verse.GetService<IVerseTokens>();
            var tokens = verseTokens.Tokens().Select(token => token.TokenString).ToArray();

            Assert.That(tokens, Is.EqualTo(new[] {"Ends", " ", "in", " ", "a", " ", "word"}));
        }

        private static Verse CreateTestVerse(string text)
        {
            var chapterStub = new ChapterStub();
            return new Verse(text, chapterStub, 0, VerseFlags.Normal);
        }
    }
}