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
    public class WordsCaseInsensitiveTests
    {
        [Test]
        public void Words_should_return_unique_case_insensitive_word_list()
        {
            var bible = CreateBible();

            var service = bible.GetService<IWordsCaseInsensitive>();
            var words = service.Words()
                .Select(word => word.ToLower())
                .ToArray();

            Assert.That(words, Is.EqualTo(new[]{"a", "first", "isn't", "second", "third", "verse"}));
        }

        [Test]
        public void Words_with_bible_not_set_should_throw()
        {
            var service = new WordsCaseInsensitive();

            Assert.Throws<InvalidOperationException>(() => service.Words());
        }

        [TestCase("ISN'T", Result = 2)]
        [TestCase("isn't", Result = 2)]
        [TestCase("b", Result = -1)]
        [TestCase("z", Result = -1)]
        public int Exercise_IndexOf(string wordToSearchFor)
        {
            var bible = CreateBible();

            var service = bible.GetService<IWordsCaseInsensitive>();
            return service.IndexOf(wordToSearchFor);
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