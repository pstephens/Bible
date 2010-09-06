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
using System.Collections.Generic;
using Builder.Model;

namespace Builder.Services
{
    public class TokenToVerseMap : IService<IBible>
    {
        private IBible bible;
        private Dictionary<string, TokenVerseFrequency> map;

        public IBible Related
        {
            get { return bible; }
            set
            {
                bible = value;
                map = null;
            }
        }

        public IEnumerable<TokenVerseFrequency> TokenFrequency()
        {
            if (map == null)
                map = BuildMap();

            return map.Values;
        }

        public bool TryGetTokenFrequency(Token token, out TokenVerseFrequency tokenFrequency)
        {
            if(map == null)
                map = BuildMap();

            return map.TryGetValue(token, out tokenFrequency);
        }

        private Dictionary<string, TokenVerseFrequency> BuildMap()
        {
            if(bible == null)
                throw new InvalidOperationException();

            var m = new Dictionary<string, TokenVerseFrequency>();

            foreach (var verse in bible.AllVerses())
                AccumulateTokens(m, verse, verse.GetService<VerseTokens>().Tokens());

            return m;
        }

        private static void AccumulateTokens(Dictionary<string, TokenVerseFrequency> m, IVerse verse, IEnumerable<Token> tokens)
        {
            foreach (var token in tokens)
            {
                TokenVerseFrequency tokenVerseFrequency;
                if (!m.TryGetValue(token.TokenString, out tokenVerseFrequency))
                {
                    tokenVerseFrequency = new TokenVerseFrequency(token);
                    m.Add(token, tokenVerseFrequency);
                }
                tokenVerseFrequency.Increment(verse);
            }
        }
    }
}