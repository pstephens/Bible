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

// Store information about chapter prefaces and book postscripts

using System;
using System.Collections;
using System.IO;

namespace Builder.Archive
{
    public abstract class NonVerse
    {
        // Fields
        private readonly Int32 _nonVerseId;
        private String _data;
        private Int32 _italicPosIndex;
        private Int32 _italicPosLen;
        private Int32 _wordPosIndex;
        private Int32 _wordPosLen;

        protected NonVerse(Int32 nonVerseId, String data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            _nonVerseId = nonVerseId;
            _data = data;
        }

        // Properties
        public abstract Bible Bible { get; }

        public Int32 NonVerseId
        {
            get { return _nonVerseId; }
        }

        public String NonVerseData
        {
            get { return _data; }
        }

        public Int32 WordPosIndex
        {
            get { return _wordPosIndex; }
            set { _wordPosIndex = value; }
        }

        public Int32 ItalicPosIndex
        {
            get { return _italicPosIndex; }
            set { _italicPosIndex = value; }
        }

        public void ProcessItalicIndex(ItalicPosIndex index)
        {
            _italicPosIndex = index.Count;
            _data = index.ProcessVerse(_data);
            _italicPosLen = index.Count - _italicPosIndex;
        }

        public void ProcessWordPosIndex(WordPosIndex index)
        {
            _wordPosIndex = index.Count;
            index.ProcessVerse(_data);
            _wordPosLen = index.Count - _wordPosIndex;
        }

        public void WriteVerseIndexData(BinaryWriter wr)
        {
            Int32 accum = _data.Length;
            accum |= _wordPosLen << 10;
            accum |= _italicPosLen << 17;
            wr.Write((Byte) ((accum & 0x0000FF)));
            wr.Write((Byte) ((accum & 0x00FF00) >> 8));
            wr.Write((Byte) ((accum & 0xFF0000) >> 16));
        }

        // Constructor
    }

    public class ChapterPreface : NonVerse
    {
        // Fields
        private Chapter _chapter;

        public ChapterPreface(Int32 nonVerseId, String data)
            : base(nonVerseId, data)
        {
        }

        // Properties
        public override Bible Bible
        {
            get { return _chapter.Book.Bible; }
        }

        public Book Book
        {
            get { return _chapter.Book; }
        }

        public Chapter Chapter
        {
            get { return _chapter; }
        }

        // Constructor

        internal void SetChapter(Chapter chap)
        {
            if (chap == null)
                throw new ArgumentNullException("chap");

            _chapter = chap;
        }
    }

    public class BookPostscript : NonVerse
    {
        // Fields
        private readonly Book _book;

        public BookPostscript(Book book, Int32 nonVerseId, String data)
            : base(nonVerseId, data)
        {
            if (book == null)
                throw new ArgumentNullException("book");

            _book = book;
        }

        // Properties
        public override Bible Bible
        {
            get { return _book.Bible; }
        }

        public Book Book
        {
            get { return _book; }
        }

        // Constructor
    }

    public class NonVerseCollection
    {
        // Fields
        private readonly ArrayList _data;

        public NonVerseCollection()
        {
            _data = new ArrayList();
        }

        // Properties
        public NonVerse this[Int32 i]
        {
            get { return (NonVerse) _data[i]; }
        }

        public Int32 Count
        {
            get { return _data.Count; }
        }

        // Constructor

        // Methods
        public void Add(NonVerse nonverse)
        {
            if (nonverse == null)
                throw new ArgumentNullException("nonverse");

            _data.Add(nonverse);
        }

        public void Shrink()
        {
            _data.TrimToSize();
        }
    }
}