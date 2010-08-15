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
using Bible.File;

namespace Bible
{
    public class BibleWriter
    {
        #region Misc

        private BibleWriter()
        {
        }

        #endregion

        public static void Write(Bible bible, WordIndex idx, BinaryWriter wr)
        {
            Console.WriteLine("Writing bible...");
            CalcStats(bible, idx);
            var header = new HeaderWriter();
            WritePos(wr, "Header Begin");
            header.WriteHeader(wr);

            WritePos(wr, "Case Sensitive Begin");
            WriteWordIndex(WordsCaseSensitive, WordsRevCaseSensitive, wr);
            WritePos(wr, "  Word Data");
            WriteWordData(WordsCaseSensitive, wr);
            WritePos(wr, "  Verse Refs");
            WriteVerseRefs(WordsCaseSensitive, wr);

            WritePos(wr, "Case Sensitive Begin");
            WriteWordIndex(WordsCaseInsensitive, WordsRevCaseInsensitive, wr);
            WritePos(wr, "  Word Data");
            WriteWordData(WordsCaseInsensitive, wr);
            WritePos(wr, "  Verse Refs");
            WriteVerseRefs(WordsCaseInsensitive, wr);

            WritePos(wr, "Book Index Begin");
            WriteBookIndex(bible, wr);
            WritePos(wr, "Chapter Index Begin");
            WriteChapIndex(bible, wr);
            WritePos(wr, "Verse Index Begin");
            WriteVerseIndex(bible, wr);

            WritePos(wr, "Verse Data Begin");
            WriteVerseData(bible, wr);

            WritePos(wr, "Extra Markup Begin");
            WriteExtraMarkup(bible, wr);

            WritePos(wr, "Word Pos Index Begin");
            WordPosIndex.Write(wr);
            WritePos(wr, "Italic Pos Index Begin");
            ItalicsPosIndex.Write(wr);

            WritePos(wr, "End of File");
        }

        private static void WritePos(BinaryWriter wr, string msg)
        {
            Console.WriteLine("    {0:D8} {1}", wr.BaseStream.Position, msg);
        }

        #region Statistics

        private static WordObject[] s_CIwords;
        private static WordObject[] s_CIwordsRev;
        private static WordObject[] s_CSwords;
        private static WordObject[] s_CSwordsRev;
        private static Int32 s_CIwordDataSize;
        private static Int32 s_CIverseRefElements;
        private static Int32 s_CSwordDataSize;
        private static Int32 s_CSverseRefElements;

        private static Int32 s_chapIndexElements;
        private static Int32 s_verseIndexElements;
        private static Int32 s_verseDataSize;
        private static Int32 s_extraMarkupSize;
        private static readonly WordPosIndex s_wordPosIndex = new WordPosIndex();
        private static readonly ItalicPosIndex s_italicPosIndex = new ItalicPosIndex();

        public static Int32 ExtraMarkupSize
        {
            get { return s_extraMarkupSize; }
        }

        public static WordObject[] WordsCaseSensitive
        {
            get { return s_CSwords; }
        }

        public static WordObject[] WordsRevCaseSensitive
        {
            get { return s_CSwordsRev; }
        }

        public static WordObject[] WordsCaseInsensitive
        {
            get { return s_CIwords; }
        }

        public static WordObject[] WordsRevCaseInsensitive
        {
            get { return s_CIwordsRev; }
        }

        public static Int32 WordDataSizeCaseSensitive
        {
            get { return s_CSwordDataSize; }
        }

        public static Int32 WordDataSizeCaseInsensitive
        {
            get { return s_CIwordDataSize; }
        }

        public static Int32 VerseRefElementsCaseSensitive
        {
            get { return s_CSverseRefElements; }
        }

        public static Int32 VerseRefElementsCaseInsensitive
        {
            get { return s_CIverseRefElements; }
        }

        public static Int32 ChapIndexElements
        {
            get { return s_chapIndexElements; }
        }

        public static Int32 VerseIndexElements
        {
            get { return s_verseIndexElements; }
        }

        public static Int32 VerseDataSize
        {
            get { return s_verseDataSize; }
        }

        public static WordPosIndex WordPosIndex
        {
            get { return s_wordPosIndex; }
        }

        public static ItalicPosIndex ItalicsPosIndex
        {
            get { return s_italicPosIndex; }
        }

        private static void CalcStats(Bible bible, WordIndex idx)
        {
            Int32 maxVerseSize = 0;

            // Get the words
            s_CSwords = idx.CaseSensitiveWords.GetAllWords();
            s_CSwordsRev = idx.CaseSensitiveWords.GetAllWordsRev(s_CSwords);
            s_CIwords = idx.CaseInsensitiveWords.GetAllWords();
            s_CIwordsRev = idx.CaseInsensitiveWords.GetAllWordsRev(s_CIwords);

            using (StreamWriter wr = System.IO.File.CreateText("c:\\words.txt"))
            {
                foreach (WordObject w in s_CIwords)
                {
                    wr.WriteLine(w.Word);
                }
            }

            // Calc word data size & verse ref elements for Case Sensitive words
            s_CSwordDataSize = 0;
            s_CSverseRefElements = 0;
            foreach (WordObject t in WordsCaseSensitive)
            {
                s_CSwordDataSize += t.Word.Length;
                s_CSverseRefElements += t.VerseRefCount;
            }

            // Calc word data size & verse ref elements for Case Sensitive words
            s_CIwordDataSize = 0;
            s_CIverseRefElements = 0;
            foreach (WordObject t in WordsCaseInsensitive)
            {
                s_CIwordDataSize += t.Word.Length;
                s_CIverseRefElements += t.VerseRefCount;
            }

            // Calc num chapters, num verses, and verse size
            s_chapIndexElements = 0;
            s_verseIndexElements = 0;
            s_verseDataSize = 0;
            for (Int32 i = 0; i < bible.Books.Count; ++i)
            {
                Book book = bible.Books[(BookName) i];
                s_chapIndexElements += book.Chapters.Count;

                for (Int32 j = 0; j < book.Chapters.Count; ++j)
                {
                    Chapter ch = book.Chapters[j];
                    s_verseIndexElements += ch.Verses.Count;
                    if (ch.Preface != null)
                    {
                        ++s_verseIndexElements;
                        ch.Preface.ProcessItalicIndex(s_italicPosIndex);
                        ch.Preface.ProcessWordPosIndex(s_wordPosIndex);
                        maxVerseSize = Math.Max(maxVerseSize, ch.Preface.NonVerseData.Length);
                        s_verseDataSize += ch.Preface.NonVerseData.Length;
                    }

                    for (Int32 k = 0; k < ch.Verses.Count; ++k)
                    {
                        Verse v = ch.Verses[k];
                        v.ProcessItalicIndex(s_italicPosIndex);
                        v.ProcessWordPosIndex(s_wordPosIndex);
                        maxVerseSize = Math.Max(maxVerseSize, v.VerseData.Length);
                        s_verseDataSize += v.VerseData.Length;
                    }
                }

                if (book.Postscript != null)
                {
                    ++s_verseIndexElements;
                    book.Postscript.ProcessItalicIndex(s_italicPosIndex);
                    book.Postscript.ProcessWordPosIndex(s_wordPosIndex);
                    maxVerseSize = Math.Max(maxVerseSize, book.Postscript.NonVerseData.Length);
                    s_verseDataSize += book.Postscript.NonVerseData.Length;
                }
            }

            // Calc extra markup size
            s_extraMarkupSize = checked((Int32) bible.BibleMarkup.Length);
            Console.WriteLine("Extra markup size: {0}", s_extraMarkupSize);
            Console.WriteLine("Max verse data size: {0}", maxVerseSize);
            Console.WriteLine("Max words per verse: {0}", s_wordPosIndex.MaxWordsPerVerse);
            Console.WriteLine("Max italics per verse: {0}", s_italicPosIndex.MaxItalicsPerVerse);
        }

        #endregion

        #region WriteWordIndex

        private static void WriteWordIndex(IEnumerable<WordObject> idx, IEnumerable<WordObject> rev, BinaryWriter wr)
        {
            foreach (WordObject t in idx)
            {
                wr.Write(checked((Byte) t.Word.Length));
                wr.Write(checked((Int16) t.VerseRefCount));
            }

            foreach (WordObject t in rev)
            {
                wr.Write(checked((Int16) t.Index));
            }
        }

        #endregion

        #region WriteWordData

        private static void WriteWordData(IEnumerable<WordObject> idx, BinaryWriter wr)
        {
            Encoding enc = Encoding.ASCII;
            foreach (WordObject t in idx)
            {
                wr.Write(enc.GetBytes(t.Word));
            }
        }

        #endregion

        #region WriteVerseRefs

        private static void WriteVerseRefs(IEnumerable<WordObject> idx, BinaryWriter wr)
        {
            foreach (WordObject t in idx)
            {
                Int32[] refs = t.VerseRefs;
                if (t.Word == "thou") Console.Write("");
                foreach (int t1 in refs)
                {
                    wr.Write(checked((Int16) t1));
                }
            }
        }

        #endregion

        #region WriteBookIndex

        private static void WriteBookIndex(Bible bible, BinaryWriter wr)
        {
            for (Int32 i = 0; i < bible.Books.Count; ++i)
            {
                Book book = bible.Books[(BookName) i];
                wr.Write(checked((Byte) book.Chapters.Count));
            }
        }

        #endregion

        #region WriteChapIndex

        private static void WriteChapIndex(Bible bible, BinaryWriter wr)
        {
            for (Int32 i = 0; i < bible.Books.Count; ++i)
            {
                Book book = bible.Books[(BookName) i];
                for (Int32 j = 0; j < book.Chapters.Count; ++j)
                {
                    Chapter ch = book.Chapters[j];
                    Int32 verseCount = ch.Verses.Count;
                    ChapterExtra extra = ChapterExtra.None;
                    if (ch.Preface != null)
                    {
                        extra |= ChapterExtra.HasDesc;
                        ++verseCount;
                    }
                    if (j == book.Chapters.Count - 1 && book.Postscript != null)
                    {
                        extra |= ChapterExtra.HasPostscript;
                        ++verseCount;
                    }

                    wr.Write(checked((Byte) verseCount));
                    wr.Write((Byte) extra);
                }
            }
        }

        #endregion

        #region WriteVerseIndex

        private static void WriteVerseIndex(Bible bible, BinaryWriter wr)
        {
            Int32 id = 0;
            for (Int32 i = 0; i < bible.Books.Count; ++i)
            {
                Book book = bible.Books[(BookName) i];
                for (Int32 j = 0; j < book.Chapters.Count; ++j)
                {
                    Chapter ch = book.Chapters[j];
                    if (ch.Preface != null)
                    {
                        ch.Preface.WriteVerseIndexData(wr);
                        if (ch.Preface.NonVerseId != id++) throw new Exception("SanityCheck.");
                    }
                    for (Int32 k = 0; k < ch.Verses.Count; ++k)
                    {
                        Verse v = ch.Verses[k];
                        v.WriteVerseIndexData(wr);
                        if (v.VerseId != id++) throw new Exception("SanityCheck.");
                    }
                }
                if (book.Postscript != null)
                {
                    book.Postscript.WriteVerseIndexData(wr);
                    if (book.Postscript.NonVerseId != id++) throw new Exception("SanityCheck.");
                }
            }
        }

        #endregion

        #region WriteVerseData

        private static void WriteVerseData(Bible bible, BinaryWriter wr)
        {
            Encoding enc = Encoding.ASCII;
            for (Int32 i = 0; i < bible.Books.Count; ++i)
            {
                Book book = bible.Books[(BookName) i];
                for (Int32 j = 0; j < book.Chapters.Count; ++j)
                {
                    Chapter ch = book.Chapters[j];
                    if (ch.Preface != null)
                        wr.Write(enc.GetBytes(ch.Preface.NonVerseData));
                    for (Int32 k = 0; k < ch.Verses.Count; ++k)
                    {
                        wr.Write(enc.GetBytes(ch.Verses[k].VerseData));
                    }
                }
                if (book.Postscript != null)
                    wr.Write(enc.GetBytes(book.Postscript.NonVerseData));
            }
        }

        #endregion

        #region WriteExtraMarkup

        private static void WriteExtraMarkup(Bible bible, BinaryWriter wr)
        {
            // Reset stream
            Stream input = bible.BibleMarkup;
            input.Seek(0, SeekOrigin.Begin);

            // Copy data
            var buff = new Byte[1024];
            while (true)
            {
                Int32 bytesRead = input.Read(buff, 0, 1024);
                if (bytesRead <= 0) break;
                wr.Write(buff, 0, bytesRead);
            }
        }

        #endregion
    }

    public class HeaderWriter : Header
    {
        #region WriteHeader

        public void WriteHeader(BinaryWriter wr)
        {
            // Calc fields
            _wordCsIndexStart = HEADER_SIZE;
            _wordCsIndexElements = BibleWriter.WordsCaseSensitive.Length;
            _wordCsIndexRevStart = WordIndexCaseSensitiveStart +
                                   WordIndexCaseSensitiveElements*WORD_INDEX_ROW_SIZE;
            _wordCsIndexRevElements = BibleWriter.WordsRevCaseSensitive.Length;
            _wordCsDataStart = WordIndexCaseSensitiveRevStart +
                               WordIndexCaseSensitiveRevElements*WORD_INDEX_REV_ROW_SIZE;
            _wordCsDataSize = BibleWriter.WordDataSizeCaseSensitive;
            _verseCsRefStart = WordDataCaseSensitiveStart +
                               WordDataCaseSensitiveSize;
            _verseCsRefElements = BibleWriter.VerseRefElementsCaseSensitive;

            _wordCiIndexStart = VerseRefCaseSensitiveStart +
                                VerseRefCaseSensitiveElements*VERSEREF_INDEX_ROW_SIZE;
            _wordCiIndexElements = BibleWriter.WordsCaseInsensitive.Length;
            _wordCiIndexRevStart = WordIndexCaseInsensitiveStart +
                                   WordIndexCaseInsensitiveElements*WORD_INDEX_ROW_SIZE;
            _wordCiIndexRevElements = BibleWriter.WordsRevCaseInsensitive.Length;
            _wordCiDataStart = WordIndexCaseInsensitiveRevStart +
                               WordIndexCaseInsensitiveRevElements*WORD_INDEX_REV_ROW_SIZE;
            _wordCiDataSize = BibleWriter.WordDataSizeCaseInsensitive;
            _verseCiRefStart = WordDataCaseInsensitiveStart +
                               WordDataCaseInsensitiveSize;
            _verseCiRefElements = BibleWriter.VerseRefElementsCaseInsensitive;

            _bookIndexStart = VerseRefCaseInsensitiveStart +
                              VerseRefCaseInsensitiveElements*VERSEREF_INDEX_ROW_SIZE;
            _bookIndexElements = 66;
            _chapterIndexStart = BookIndexStart + BookIndexElements*BOOK_INDEX_ROW_SIZE;
            _chapterIndexElements = BibleWriter.ChapIndexElements;
            _verseIndexStart = ChapterIndexStart + ChapterIndexElements*CHAP_INDEX_ROW_SIZE;
            _verseIndexElements = BibleWriter.VerseIndexElements;
            _verseDataStart = VerseIndexStart + VerseIndexElements*VERSE_INDEX_ROW_SIZE;
            _verseDataSize = BibleWriter.VerseDataSize;
            _extraMarkupStart = VerseDataStart + VerseDataSize;
            _extraMarkupSize = BibleWriter.ExtraMarkupSize;
            _wordPosIndexStart = ExtraMarkupStart + ExtraMarkupSize;
            _wordPosIndexElements = BibleWriter.WordPosIndex.Count;
            _italicsPosIndexStart = WordPosIndexStart + WordPosIndexElements*WORDPOS_INDEX_ROW_SIZE;
            _italicsPosIndexElements = BibleWriter.ItalicsPosIndex.Count;

            // Write File Id
            wr.Write(Encoding.ASCII.GetBytes(FILE_ID));

            // Write various fields
            wr.Write(WordIndexCaseSensitiveStart);
            wr.Write(WordIndexCaseSensitiveElements);
            wr.Write(WordIndexCaseSensitiveRevStart);
            wr.Write(WordIndexCaseSensitiveRevElements);
            wr.Write(WordDataCaseSensitiveStart);
            wr.Write(WordDataCaseSensitiveSize);
            wr.Write(VerseRefCaseSensitiveStart);
            wr.Write(VerseRefCaseSensitiveElements);

            wr.Write(WordIndexCaseInsensitiveStart);
            wr.Write(WordIndexCaseInsensitiveElements);
            wr.Write(WordIndexCaseInsensitiveRevStart);
            wr.Write(WordIndexCaseInsensitiveRevElements);
            wr.Write(WordDataCaseInsensitiveStart);
            wr.Write(WordDataCaseInsensitiveSize);
            wr.Write(VerseRefCaseInsensitiveStart);
            wr.Write(VerseRefCaseInsensitiveElements);

            wr.Write(BookIndexStart);
            wr.Write(BookIndexElements);
            wr.Write(ChapterIndexStart);
            wr.Write(ChapterIndexElements);
            wr.Write(VerseIndexStart);
            wr.Write(VerseIndexElements);
            wr.Write(VerseDataStart);
            wr.Write(VerseDataSize);
            wr.Write(ExtraMarkupStart);
            wr.Write(ExtraMarkupSize);
            wr.Write(WordPosIndexStart);
            wr.Write(WordPosIndexElements);
            wr.Write(ItalicsPosIndexStart);
            wr.Write(ItalicsPosIndexElements);
        }

        #endregion
    }
}