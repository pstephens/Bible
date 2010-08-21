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

namespace Builder
{
    public class Verse : ServiceProvider<IVerse>, IVerse
    {
        public Verse(string verseData, IChapter chapter, int id, int index)
        {
            if(string.IsNullOrEmpty(verseData)) throw new ArgumentNullException("verseData");
            if(chapter == null) throw new ArgumentNullException("chapter");
            if(id < 0) throw new ArgumentException("Must be greater than zero.", "id");
            if (index < 0) throw new ArgumentException("Must be greater than zero.", "index");

            Text = verseData;
            Chapter = chapter;
            Index = index;
            Id = id;
        }

        public string Text { get; private set; }

        public IChapter Chapter { get; private set; }

        public int Index { get; private set; }

        public int Id { get; private set; }
    }
}