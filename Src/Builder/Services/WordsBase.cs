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
    public abstract class WordsBase : IWordsBase
    {
        private readonly StringComparer comparer;
        private IBible bible;
        private List<string> words;

        protected WordsBase(StringComparer comparer)
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

        public IList<string> Words()
        {
            return words ?? (words = BuildWords());
        }

        public int IndexOf(string word)
        {
            if(words == null) words = BuildWords();

            var index = words.BinarySearch(word, comparer);
            return index < 0 ? -1 : index;
        }

        private List<string> BuildWords()
        {
            if (bible == null)
                throw new InvalidOperationException();

            var uniqueWords = new HashSet<string>(
                bible.GetService<ITokenToVerseMap>().TokenFrequency()
                    .Where(tf => tf.Token.IsWord)
                    .Select(tf => (string) tf.Token),
                comparer);
            var sortedWords = uniqueWords.OrderBy(word => word, comparer);
            return sortedWords.ToList();
        }
    }
}