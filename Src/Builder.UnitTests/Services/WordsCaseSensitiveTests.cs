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
    public class WordsCaseSensitiveTests
    {
        [Test]
        public void Words_should_return_unique_case_sensitive_word_list()
        {
            var bible = CreateBible();

            var wordsCaseSensitive = bible.GetService<IWordsCaseSensitive>();
            var words = wordsCaseSensitive.Words().ToArray();

            Assert.That(words, Is.EqualTo(new[] { "a", "First", "Isn't", "Second", "Third", "verse", "Verse", "VERSE", }));
        }
        
        [TestCase("Second", Result = 3)]
        [TestCase("second", Result = -1)]
        [TestCase("a", Result = 0)]
        [TestCase("b", Result = -1)]
        [TestCase("z", Result = -1)]
        public int Exercise_IndexOf(string word)
        {
            var bible = CreateBible();

            var service = bible.GetService<IWordsCaseSensitive>();
            return service.IndexOf(word);
        }

        [Test]
        public void Words_with_related_not_set_should_throw()
        {
            var service = new WordsCaseSensitive();

            Assert.Throws<InvalidOperationException>(() => service.Words().ToArray());
        }

        private static IBible CreateBible(string bibleText =
            "B:Job\r\n1:1 First verse\r\n1:2 Second VERSE!\r\n2:1 Third Verse.\r\n2:2 Isn't a verse")
        {
            var parser = new BibleParser();
            var encodedString = Encoding.ASCII.GetBytes(bibleText);
            var str = new MemoryStream(encodedString);
            return parser.Parse(str);
        }
    }
}