﻿#region Copyright Notice

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

using NUnit.Framework;

namespace Builder.UnitTests
{
    [TestFixture]
    public class VerseTests
    {
        [Test]
        public void Verse_Text_should_return_text()
        {
            const string text = "Some verse data.";
            
            var verse = new Verse(text) as IVerse;

            Assert.That(verse.Text, Is.EqualTo(text));
        }

        [Test]
        public void Verse_Chapter_should_return_injected_Chapter()
        {
            IChapter chapter = null; //  MockRepository.GenerateStub<IChapter>();

            var verse = new Verse("Content") as IVerse;

            Assert.That(verse.Chapter, Is.SameAs(chapter));
        }
    }
}
