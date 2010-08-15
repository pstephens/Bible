using System;
using System.IO;

namespace Bible
{
	public class Bible
	{
		// Fields
		private readonly BookCollection _books;
		private readonly NonVerseCollection _nonVerses;
		private Int32 _nextChapterId;
		private Int32 _nextVerseId;
		private Int32 _chapterCount;
		private Int32 _verseCount;
		private Stream _bibleMarkup;
		
		// Properties
		public BookCollection Books
		{
			get { return _books; }
		}
		
		public NonVerseCollection NonVerses
		{
			get { return _nonVerses; }
		}

		public Int32 NonVerseCount
		{
			get { return _nonVerses.Count; }
		}

		public Int32 VerseCount
		{
			get { return _verseCount; }
		}

		public Int32 ChapterCount
		{
			get { return _chapterCount; }
		}

		public Stream BibleMarkup
		{
			get { return _bibleMarkup; }
		}
		
		// Constructor
		public Bible()
		{
			_nextChapterId = 0;
			_nextVerseId = 0;
			_chapterCount = 0;
			_verseCount = 0;
			_books = new BookCollection(this);
			_nonVerses = new NonVerseCollection();
		}
		
		// Methods
		public Int32 GetNextChapterId()
		{
			return _nextChapterId++;
		}
		
		public Int32 GetNextVerseId()
		{
			return _nextVerseId++;
		}

		public void SetBibleMarkup(Stream str)
		{
			_bibleMarkup = str;
		}
	}
}
