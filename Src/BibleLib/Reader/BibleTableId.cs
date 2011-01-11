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

namespace BibleLib.Reader
{
    public enum BibleTableId : byte
    {
        Header  = 0,
        
        Words_CS_Alpha = 10,
        Words_CS_Freq = 11,
        Words_CS_VerseRef = 12,
        Words_CS_String_Blob = 13,
        Words_CI_Alpha = 14,
        Words_CI_WordRef = 15,

        Books = 20,
        Chapters = 21,
        Verses = 22,

        VerseWords_Uncompressed_Blob = 30,
        VerseWords_Int16Compressed_Blob = 31,
    }
}