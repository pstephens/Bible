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

namespace Builder
{
    public class Chapter : ServiceProvider<IChapter>, IChapter
    {
        public Chapter(IBook book, int id)
        {
            if(book == null) throw new ArgumentNullException("book");
            if (id < 0) throw new ArgumentException("Must be zero or greater.", "id");

            Book = book;
            Id = id;
            Verses = new ValidatingList<IVerse>(verse => verse != null);
        }

        public IBook Book { get; private set; }

        public int Id { get; private set; }

        public IList<IVerse> Verses { get; private set; }
        
        public bool Equals(IChapter other)
        {
            if (ReferenceEquals(other, null)) return false;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IChapter);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}