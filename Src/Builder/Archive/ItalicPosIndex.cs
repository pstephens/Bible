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
using System.IO;
using System.Text;

namespace Builder.Archive
{
    public struct ItalicPos
    {
        public Int16 Start;
        public Int16 Stop;

        public ItalicPos(Int16 start, Int16 stop)
        {
            Start = start;
            Stop = stop;
        }
    }

    public class ItalicPosIndex
    {
        private readonly List<ItalicPos> _list = new List<ItalicPos>();

        public Int32 MaxItalicsPerVerse;

        public ItalicPos this[Int32 i]
        {
            get { return _list[i]; }
        }

        public Int32 Count
        {
            get { return _list.Count; }
        }

        public String ProcessVerse(String verseData)
        {
            Int32 state = 0;
            Int16 i, j, jbegin;
            var b = new StringBuilder();
            Int32 beginPos = _list.Count;
            for (i = 0, j = 0, jbegin = 0; i < verseData.Length; ++i)
            {
                switch (verseData[i])
                {
                    case '[':
                        if (state == 1) throw new Exception();
                        state = 1;
                        jbegin = j;
                        break;
                    case ']':
                        if (state == 0) throw new Exception();
                        state = 0;
                        _list.Add(new ItalicPos(jbegin, j));
                        break;
                    default:
                        ++j;
                        b.Append(verseData[i]);
                        break;
                }
            }
            if (state != 0) throw new Exception();
            MaxItalicsPerVerse = Math.Max(MaxItalicsPerVerse, _list.Count - beginPos);
            return b.ToString();
        }

        public void Write(BinaryWriter wr)
        {
            foreach (ItalicPos p in _list)
            {
                var accum =
                    (Int16)
                    ((p.Start & 0x3FF) |
                     (((p.Stop - p.Start) & 0x3F) << 10));
                wr.Write(accum);
            }
        }
    }
}