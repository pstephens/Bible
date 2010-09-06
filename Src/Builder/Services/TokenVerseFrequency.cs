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

using System.Collections.Generic;
using System.Linq;
using Builder.Model;

namespace Builder.Services
{
    public class TokenVerseFrequency
    {
        private readonly Dictionary<IVerse, int> verses = new Dictionary<IVerse, int>();

        public Token Token { get; private set; }

        public TokenVerseFrequency(Token token)
        {
            Token = token;
        }

        public void Increment(IVerse verse)
        {
            if (verses.ContainsKey(verse))
                verses[verse]++;
            else
                verses.Add(verse, 1);
        }

        public IEnumerable<VerseFrequency> RelatedVerses()
        {
            return verses.Select(kvp => new VerseFrequency(kvp.Key, kvp.Value));
        }
    }
}