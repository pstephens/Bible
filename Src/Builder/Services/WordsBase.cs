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
using System.Linq;
using Builder.Model;

namespace Builder.Services
{
    public abstract class WordsBase
    {
        private readonly IEqualityComparer<string> comparer;
        private IBible bible;
        private HashSet<string> words;

        protected WordsBase(IEqualityComparer<string> comparer)
        {
            this.comparer = comparer;
        }

        public IBible Related
        {
            get { return bible; }
            set { 
                bible = value;
                words = null;
            }
        }

        public IEnumerable<string> Words()
        {
            if (words == null)
                BuildWords();

            return words;
        }

        private void BuildWords()
        {
            if (bible == null)
                throw new InvalidOperationException();

            words = new HashSet<string>(
                bible.GetService<ITokenToVerseMap>().TokenFrequency()
                    .Where(tf => tf.Token.IsWord)
                    .Select(tf => (string) tf.Token),
                comparer);
        }
    }
}