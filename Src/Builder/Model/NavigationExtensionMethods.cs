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

namespace Builder.Model
{
    public static class NavigationExtensionMethods
    {
        public static IEnumerable<IVerse> AllVerses(this IBible bible)
        {
            if(bible == null)
                throw new ArgumentNullException("bible");

            var allBooks = bible.Books.Values.OrderBy(book => book.Id);
            var allChapters = allBooks.SelectMany(book => book.Chapters);
            var allVerses = allChapters.SelectMany(chapter => chapter.Verses);

            return allVerses;
        }
    }
}