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

using System.IO;

namespace BibleLib.Reader
{
    public struct HeaderRec
    {
        public const int RecSize = 10;

        public BibleTableId TableId { get; set; }
        public HeaderFlags Flags { get; set; }
        public int Size { get; set; }
        public int CompressedSize { get; set; }

        // Not serialized
        public int StartByteIndex { get; set; }

        public static HeaderRec ReadFrom(BinaryReader r, int startByteIndex)
        {
            var rec = new HeaderRec
                       {
                           TableId = (BibleTableId) r.ReadByte(),
                           Flags = (HeaderFlags) r.ReadByte(),
                           Size = r.ReadInt32(),
                           CompressedSize = r.ReadInt32(),
                           StartByteIndex = startByteIndex
                       };

            if(rec.CompressedSize < 0 || rec.Size < 0)
                throw new BibleFormatException("Invalid size in header record.");

            return rec;
        }
    }
}