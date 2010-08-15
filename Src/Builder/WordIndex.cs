using System;
using System.Collections.Generic;
using System.Globalization;

namespace Bible
{
    public class WordIndex
    {
        private readonly Bible _bible;
        private readonly WordHashtable _tableCI;
        private readonly WordHashtable _tableCS;

        public WordIndex(Bible bible)
        {
            if (bible == null)
                throw new ArgumentNullException("bible");

            _bible = bible;
            _tableCS = new WordHashtable(false);
            _tableCI = new WordHashtable(true);
        }

        public WordHashtable CaseSensitiveWords
        {
            get { return _tableCS; }
        }

        public WordHashtable CaseInsensitiveWords
        {
            get { return _tableCI; }
        }

        public Bible Bible
        {
            get { return _bible; }
        }

        // Constructor

        // Methods
        public void ProcessBible()
        {
            for (Int32 i = 0; i < 66; ++i)
                ProcessBook(_bible.Books[(BookName) i]);
        }

        // Implementation
        private void ProcessBook(Book book)
        {
            for (Int32 i = 0; i < book.Chapters.Count; ++i)
                ProcessChapter(book.Chapters[i]);
            if (book.Postscript != null)
                ProcessString(book.Postscript.NonVerseData,
                              book.Postscript.NonVerseId);
        }

        private void ProcessChapter(Chapter chapter)
        {
            if (chapter.Preface != null)
                ProcessString(chapter.Preface.NonVerseData,
                              chapter.Preface.NonVerseId);
            for (Int32 i = 0; i < chapter.Verses.Count; ++i)
                ProcessVerse(chapter.Verses[i]);
        }

        private void ProcessVerse(Verse verse)
        {
            ProcessString(verse.VerseData, verse.VerseId);
        }

        private void ProcessString(String data, Int32 verseRef)
        {
            Int32 len = 0, i, start = 0;
            for (i = 0; i < data.Length; ++i)
            {
                Char ch = data[i];

                // Skip over whitespace or punctuation
                if (IsPunctOrWhite(ch))
                {
                    if (len > 0)
                    {
                        ProcessWord(data, start, len, verseRef);
                        len = 0;
                    }
                    continue;
                }

                if (IsHyphen(ch))
                {
                    // Is the next char a word char?
                    if (len > 0 && i + 1 < data.Length && IsWordChar(data[i + 1]))
                    {
                        ++len;
                        continue;
                    }
                    if (len > 0)
                    {
                        ProcessWord(data, start, len, verseRef);
                        len = 0;
                    }
                    continue;
                }

                // Process word
                if (IsWordChar(ch))
                {
                    if (len == 0) start = i;
                    ++len;
                    continue;
                }

                // Otherwise we have a bad character
                throw new Exception("Bad character '" + ch + "' in verse.");
            }
            if (len > 0)
                ProcessWord(data, start, len, verseRef);
        }

        private static Boolean IsHyphen(Char ch)
        {
            return ch == '-';
        }

        private static Boolean IsWordChar(Char ch)
        {
            return (ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z') || ch == '\'';
        }

        private static Boolean IsPunctOrWhite(Char ch)
        {
            return ch == ' ' || ch == ':' || ch == ';' || ch == '.' || ch == '?' ||
                   ch == ',' || ch == '[' || ch == ']' || ch == '(' || ch == ')' ||
                   ch == '!';
        }

        private void ProcessWord(String data, Int32 start, Int32 len, Int32 verseRef)
        {
            _tableCS.AddVerseRef(data, start, len, verseRef);
            _tableCI.AddVerseRef(data, start, len, verseRef);
        }

        public void Shrink()
        {
            _tableCI.Shrink();
            _tableCS.Shrink();
        }
    }

    public class WordHashtable
    {
        private readonly Boolean _caseInsensitive;
        private readonly StringComparer _comparer;
        private readonly Dictionary<String, WordObject> _words;

        // Constructor
        public WordHashtable(Boolean caseInsensitive)
        {
            _caseInsensitive = caseInsensitive;
            _comparer = new WordObject.ForwardComparer(_caseInsensitive);
            _words = new Dictionary<string, WordObject>(
                _comparer);
        }

        public Int32 Count
        {
            get { return _words.Count; }
        }

        // Methods
        public void AddVerseRef(String sentence, Int32 start, Int32 len, Int32 verseRef)
        {
            if (len <= 0)
                throw new ArgumentException("The word's length must be greater than 0.");
            WordObject obj = GetWordObject(sentence, start, len);
            obj.AddVerseRef(verseRef);
        }

        public WordObject[] GetAllWords()
        {
            // Get all the words
            var words = new WordObject[_words.Count];
            _words.Values.CopyTo(words, 0);

            // Now sort the words
            Array.Sort(words, new WordObject.WordObjectComparer(
                                  _caseInsensitive, false));

            for (Int32 j = 0; j < words.Length; ++j)
            {
                words[j].Index = j;
            }

            return words;
        }

        public WordObject[] GetAllWordsRev(WordObject[] forwardSorted)
        {
            var words = new WordObject[forwardSorted.Length];
            Array.Copy(forwardSorted, words, forwardSorted.Length);
            Array.Sort(words, new WordObject.WordObjectComparer(
                                  _caseInsensitive, true));
            return words;
        }

        private WordObject GetWordObject(String sentence, Int32 start, Int32 len)
        {
            // Get the word String
            String word = sentence.Substring(start, len);

            WordObject obj;
            if (!_words.TryGetValue(word, out obj))
            {
                if (_caseInsensitive)
                    word = word.ToUpper(CultureInfo.InvariantCulture);
                obj = new WordObject(word);
                _words.Add(word, obj);
            }
            return obj;
        }

        public void Shrink()
        {
            foreach (WordObject word in _words.Values)
            {
                word.SortAndShrink();
            }
        }
    }

    public class WordObject
    {
        private static readonly Byte[] StdCollation = new Byte[128];
        private static readonly Byte[] CaseInsensitiveCollation = new Byte[128];

        // Fields
        private readonly String _data;
        private readonly List<Int32> _verseRefs;

        static WordObject()
        {
            Byte std = 0, ci = 0;

            for (Int32 k = 0; k < 26; ++k)
            {
                StdCollation['A' + k] = ++std;
                StdCollation['a' + k] = ++std;
                CaseInsensitiveCollation['A' + k] = ++ci;
                CaseInsensitiveCollation['a' + k] = ci;
            }
            StdCollation['\''] = ++std;
            CaseInsensitiveCollation['\''] = ++ci;

            StdCollation['-'] = ++std;
            CaseInsensitiveCollation['-'] = ++ci;
        }

        public WordObject(String word)
        {
            if (word == null) throw new ArgumentNullException("word");

            _verseRefs = new List<Int32>();
            _data = word;
        }

        // Properties
        public String Word
        {
            get { return _data; }
        }

        public Int32 Index { get; set; }

        public Int32 VerseRefCount
        {
            get { return _verseRefs.Count; }
        }

        public Int32[] VerseRefs
        {
            get { return _verseRefs.ToArray(); }
        }

        public Int32 this[Int32 i]
        {
            get { return _verseRefs[i]; }
        }

        // Constructor

        public void SortAndShrink()
        {
            if (_verseRefs.Count <= 1)
            {
                Shrink();
                return;
            }

            // First sort the array
            _verseRefs.Sort();

            // And then keep only unique values
            Int32 srcI, dstI;
            Int32 dst = _verseRefs[0];
            for (srcI = 1, dstI = 0; srcI < _verseRefs.Count; ++srcI)
            {
                Int32 src = _verseRefs[srcI];
                if (src != dst)
                {
                    dst = src;
                    _verseRefs[++dstI] = src;
                }
            }

            ++dstI;
            if (dstI < _verseRefs.Count)
            {
                _verseRefs.RemoveRange(dstI, _verseRefs.Count - dstI);
            }

            // And Shrink
            Shrink();
        }

        public void AddVerseRef(Int32 verseRef)
        {
            _verseRefs.Add(verseRef);
        }

        private void Shrink()
        {
            _verseRefs.TrimExcess();
        }

        #region Nested type: ForwardComparer

        public class ForwardComparer : StringComparer
        {
            private readonly StringComparer _cmp;
            private readonly Byte[] _collation;

            public ForwardComparer(Boolean caseInsensitive)
            {
                _cmp = caseInsensitive
                           ? OrdinalIgnoreCase
                           : Ordinal;
                _collation = caseInsensitive
                                 ? CaseInsensitiveCollation
                                 : StdCollation;
            }

            public override int Compare(string x, string y)
            {
                if (ReferenceEquals(x, y)) return 0;
                if (x == null) return -1;
                if (y == null) return 1;

                Int32 i = 0, j = 0;
                for (;;)
                {
                    if (i >= x.Length)
                    {
                        if (j >= y.Length) return 0;
                        return -1;
                    }
                    if (j >= y.Length) return 1;

                    Byte c1 = _collation[x[i++]];
                    Byte c2 = _collation[y[j++]];
                    if (c1 < c2) return -1;
                    if (c1 > c2) return 1;
                }
            }

            public override bool Equals(string x, string y)
            {
                return Compare(x, y) == 0;
            }

            public override int GetHashCode(string obj)
            {
                return _cmp.GetHashCode(obj);
            }
        }

        #endregion

        #region Nested type: RevComparer

        public class RevComparer : StringComparer
        {
            private readonly StringComparer _cmp;
            private readonly Byte[] _collation;

            public RevComparer(Boolean caseInsensitive)
            {
                _cmp = caseInsensitive
                           ? OrdinalIgnoreCase
                           : Ordinal;
                _collation = caseInsensitive
                                 ? CaseInsensitiveCollation
                                 : StdCollation;
            }

            public override int Compare(string x, string y)
            {
                if (ReferenceEquals(x, y)) return 0;
                if (x == null) return -1;
                if (y == null) return 1;

                Int32 i = x.Length - 1, j = y.Length - 1;
                for (;;)
                {
                    if (i < 0)
                    {
                        if (j < 0) return 0;
                        return -1;
                    }
                    if (j < 0) return 1;

                    Byte c1 = _collation[x[i--]];
                    Byte c2 = _collation[y[j--]];
                    if (c1 < c2) return -1;
                    if (c1 > c2) return 1;
                }
            }

            public override bool Equals(string x, string y)
            {
                return Compare(x, y) == 0;
            }

            public override int GetHashCode(string obj)
            {
                return _cmp.GetHashCode(obj);
            }
        }

        #endregion

        #region Nested type: WordObjectComparer

        public class WordObjectComparer : IComparer<WordObject>
        {
            private readonly StringComparer _cmp;

            public WordObjectComparer(Boolean caseInsensitive,
                                      Boolean rev)
            {
                if (rev)
                    _cmp = new RevComparer(caseInsensitive);
                else
                    _cmp = new ForwardComparer(caseInsensitive);
            }

            #region IComparer<WordObject> Members

            public int Compare(WordObject x, WordObject y)
            {
                if (ReferenceEquals(x, y)) return 0;
                if (x == null) return -1;
                if (y == null) return 1;
                return _cmp.Compare(x._data, y._data);
            }

            #endregion
        }

        #endregion
    }
}