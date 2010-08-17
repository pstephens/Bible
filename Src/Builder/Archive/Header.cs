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

namespace Bible.File
{
    public enum ChapterExtra : byte
    {
        None = 0,
        HasDesc = 1,
        HasPostscript = 2
    }

    public class Header
    {
        // File version History:
        // 1.3.0.0 - Changed verse data encoding. Removed Word Pos Index
        // 1.2.0.0 - Added separate indexes for case sensitive and 
        //           case insensitive words.

        #region Constants
        public const String FILE_ID = "AvBible 1.3.0.0 "; // 16 Bytes
        public const Int32 HEADER_SIZE = 136;
        public const Int32 WORD_INDEX_ROW_SIZE = 3; // WordSize : Byte, VerseRefs : Short
        public const Int32 WORD_INDEX_REV_ROW_SIZE = 2; // Index into WORD_INDEX : Short
        public const Int32 VERSEREF_INDEX_ROW_SIZE = 2; // VerseRef : Short
        public const Int32 BOOK_INDEX_ROW_SIZE = 1; // NumChapters : Byte
        public const Int32 CHAP_INDEX_ROW_SIZE = 2; // NumVerses : Byte, BitFlag--HasDesc--HasPostcript
        public const Int32 VERSE_INDEX_ROW_SIZE = 3; // VerseSize : Bits10, WordPosSize : Bits7, ItalicPosSize : Bits4
        public const Int32 ITALIC_INDEX_ROW_SIZE = 2; // ItalicStart : Bits10, ItalicLen : Bits6
        public const Int32 WORDPOS_INDEX_ROW_SIZE = 2; // WordLen : Bits5, PrevChars : Bits3, PrePunct : Bits2, PostPunct : Bits2, VerseStart : Bits1
        #endregion

        #region Fields
        protected Int32 _wordCsIndexStart;
        protected Int32 _wordCsIndexElements;
        protected Int32 _wordCsIndexRevStart;
        protected Int32 _wordCsIndexRevElements;
        protected Int32 _wordCsDataStart;
        protected Int32 _wordCsDataSize;
        protected Int32 _verseCsRefStart;
        protected Int32 _verseCsRefElements;
        protected Int32 _wordCiIndexStart;
        protected Int32 _wordCiIndexElements;
        protected Int32 _wordCiIndexRevStart;
        protected Int32 _wordCiIndexRevElements;
        protected Int32 _wordCiDataStart;
        protected Int32 _wordCiDataSize;
        protected Int32 _verseCiRefStart;
        protected Int32 _verseCiRefElements;
        protected Int32 _bookIndexStart;
        protected Int32 _bookIndexElements;
        protected Int32 _chapterIndexStart;
        protected Int32 _chapterIndexElements;
        protected Int32 _verseIndexStart;
        protected Int32 _verseIndexElements;
        protected Int32 _verseDataStart;
        protected Int32 _verseDataSize;
        protected Int32 _extraMarkupStart;
        protected Int32 _extraMarkupSize;
        protected Int32 _wordPosIndexStart;
        protected Int32 _wordPosIndexElements;
        protected Int32 _italicsPosIndexStart;
        protected Int32 _italicsPosIndexElements;
        #endregion

        #region Properties
        public Int32 WordIndexCaseSensitiveStart
        {
            get { return _wordCsIndexStart; }
        }

        public Int32 WordIndexCaseSensitiveElements
        {
            get { return _wordCsIndexElements; }
        }

        public Int32 WordIndexCaseSensitiveRevStart
        {
            get { return _wordCsIndexRevStart; }
        }

        public Int32 WordIndexCaseSensitiveRevElements
        {
            get { return _wordCsIndexRevElements; }
        }

        public Int32 WordDataCaseSensitiveStart
        {
            get { return _wordCsDataStart; }
        }

        public Int32 WordDataCaseSensitiveSize
        {
            get { return _wordCsDataSize; }
        }

        public Int32 VerseRefCaseSensitiveStart
        {
            get { return _verseCsRefStart; }
        }

        public Int32 VerseRefCaseSensitiveElements
        {
            get { return _verseCsRefElements; }
        }

        public Int32 WordIndexCaseInsensitiveStart
        {
            get { return _wordCiIndexStart; }
        }

        public Int32 WordIndexCaseInsensitiveElements
        {
            get { return _wordCiIndexElements; }
        }

        public Int32 WordIndexCaseInsensitiveRevStart
        {
            get { return _wordCiIndexRevStart; }
        }

        public Int32 WordIndexCaseInsensitiveRevElements
        {
            get { return _wordCiIndexRevElements; }
        }

        public Int32 WordDataCaseInsensitiveStart
        {
            get { return _wordCiDataStart; }
        }

        public Int32 WordDataCaseInsensitiveSize
        {
            get { return _wordCiDataSize; }
        }

        public Int32 VerseRefCaseInsensitiveStart
        {
            get { return _verseCiRefStart; }
        }

        public Int32 VerseRefCaseInsensitiveElements
        {
            get { return _verseCiRefElements; }
        }

        public Int32 BookIndexStart
        {
            get { return _bookIndexStart; }
        }

        public Int32 BookIndexElements
        {
            get { return _bookIndexElements; }
        }

        public Int32 ChapterIndexStart
        {
            get { return _chapterIndexStart; }
        }

        public Int32 ChapterIndexElements
        {
            get { return _chapterIndexElements; }
        }

        public Int32 VerseIndexStart
        {
            get { return _verseIndexStart; }
        }

        public Int32 VerseIndexElements
        {
            get { return _verseIndexElements; }
        }

        public Int32 VerseDataStart
        {
            get { return _verseDataStart; }
        }

        public Int32 VerseDataSize
        {
            get { return _verseDataSize; }
        }

        public Int32 ExtraMarkupStart
        {
            get { return _extraMarkupStart; }
        }

        public Int32 ExtraMarkupSize
        {
            get { return _extraMarkupSize; }
        }

        public Int32 WordPosIndexStart
        {
            get { return _wordPosIndexStart; }
        }

        public Int32 WordPosIndexElements
        {
            get { return _wordPosIndexElements; }
        }

        public Int32 ItalicsPosIndexStart
        {
            get { return _italicsPosIndexStart; }
        }

        public Int32 ItalicsPosIndexElements
        {
            get { return _italicsPosIndexElements; }
        }
        #endregion
    }
}