using System;
using System.Collections;
using System.IO;

namespace Bible
{
    public class Verse
    {
        // Fields
        private readonly Chapter _chap;
        private readonly Int32 _verseId; // verse number in whole Bible (zero based)
        private readonly Int32 _verseNum; // verse number in current chapter (zero base)
        private Int32 _italicPosIndex;
        private Int32 _italicPosLen;
        private String _verse;
        private Int32 _wordPosIndex;
        private Int32 _wordPosLen;

        public Verse(Int32 verseId, Int32 verseNum, String verse, Chapter chap)
        {
            _verseId = verseId;
            _verseNum = verseNum;
            _verse = verse;
            _chap = chap;
        }

        // Properties
        public Bible Bible
        {
            get { return _chap.Book.Bible; }
        }

        public Book Book
        {
            get { return _chap.Book; }
        }

        public Int32 VerseId
        {
            get { return _verseId; }
        }

        public Int32 VerseNum
        {
            get { return _verseNum; }
        }

        public String VerseData
        {
            get { return _verse; }
        }

        public Chapter Chapter
        {
            get { return _chap; }
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
            _verse = index.ProcessVerse(_verse);
            _italicPosLen = index.Count - _italicPosIndex;
        }

        public void ProcessWordPosIndex(WordPosIndex index)
        {
            _wordPosIndex = index.Count;
            index.ProcessVerse(_verse);
            _wordPosLen = index.Count - _wordPosIndex;
        }

        public void WriteVerseIndexData(BinaryWriter wr)
        {
            Int32 accum = _verse.Length;
            accum |= _wordPosLen << 10;
            accum |= _italicPosLen << 17;
            wr.Write((Byte) ((accum & 0x0000FF)));
            wr.Write((Byte) ((accum & 0x00FF00) >> 8));
            wr.Write((Byte) ((accum & 0xFF0000) >> 16));
        }

        // Constructor
    }

    public class VerseCollection
    {
        // Fields
        private readonly ArrayList _data;

        public VerseCollection()
        {
            _data = new ArrayList();
        }

        // Properties
        public Verse this[Int32 i]
        {
            get { return (Verse) _data[i]; }
        }

        public Int32 Count
        {
            get { return _data.Count; }
        }

        // Constructor

        // Methods
        public void Add(Verse verse)
        {
            if (verse == null)
                throw new ArgumentNullException("verse");

            _data.Add(verse);
        }

        public void Shrink()
        {
            _data.TrimToSize();
        }
    }

    public class AllVersesCollection
    {
        private readonly Verse[] _verses;

        public AllVersesCollection(Bible bible)
        {
            _verses = new Verse[bible.VerseCount];

            // Accumulate the verses
            Int32 book;
            Int32 i;
            Book b;
            Chapter ch;
            for (book = 0, i = 0; book < bible.Books.Count; ++book)
            {
                b = bible.Books[(BookName) book];
                Int32 chapter;
                for (chapter = 0; chapter < b.Chapters.Count; ++chapter)
                {
                    ch = b.Chapters[chapter];
                    Int32 verse;
                    for (verse = 0; verse < ch.Verses.Count; ++verse)
                    {
                        _verses[i++] = ch.Verses[verse];
                    }
                }
            }
        }

        public Verse this[Int32 idx]
        {
            get { return _verses[idx]; }
        }

        public Int32 Count
        {
            get { return _verses.Length; }
        }
    }
}