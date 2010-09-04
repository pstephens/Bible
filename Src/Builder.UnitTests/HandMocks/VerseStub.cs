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
using Builder.Model;

namespace Builder.UnitTests.HandMocks
{
    public class VerseStub : ServiceStub<IVerse>, IVerse
    {
        public string Text { get; set; }
        public IChapter Chapter { get; set; }
        public int Id { get; set; }
        public bool IsPreVerse { get; set; }
        public bool IsPostVerse { get; set; }

        public bool Equals(IVerse other)
        {
            if (ReferenceEquals(other, null)) return false;
            return other.Id == Id;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IVerse);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}