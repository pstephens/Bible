using System;
using System.Collections;

namespace Bible
{
	public class Chapter
	{
		// Fields
		private readonly Book _book;
		private readonly Int32 _chapId;           // Zero based chapter id in the whole bible.
		private readonly Int32 _chapNum;          // Zero based chapter id in the current book.
		private readonly VerseCollection _verses;
		private readonly ChapterPreface _preface;
		
		// Properties
		public Bible Bible
		{
			get { return _book.Bible; }
		}
		
		public Book Book
		{
			get { return _book; }
		}
		
		public Int32 ChapterId
		{
			get { return _chapId; }
		}
		
		public Int32 ChapterNum
		{
			get { return _chapNum; }
		}
		
		public VerseCollection Verses
		{
			get { return _verses; }
		}
		
		public ChapterPreface Preface
		{
			get { return _preface; }
		}
		
		// Constructor
		public Chapter(Book book, Int32 chapId, Int32 chapNum, ChapterPreface preface)
		{
			if(book == null)
				throw new ArgumentNullException("book");
			
			_book = book;
			_chapId = chapId;
			_chapNum = chapNum;
			_verses = new VerseCollection();
			_preface = preface;
			if(_preface != null)
				_preface.SetChapter(this);
		}
	}
	
	public class ChapterCollection
	{
		// Fields
		private readonly ArrayList _data;
		
		// Properties
		public Chapter this[Int32 i]
		{
			get { return (Chapter) _data[i]; }
		}
		
		public Int32 Count
		{
			get { return _data.Count; }
		}
		
		// Constructor
		public ChapterCollection()
		{
			_data = new ArrayList();
		}
		
		// Methods
		public void Add(Chapter chap)
		{
			if(chap == null)
				throw new ArgumentNullException("verse");
			
			_data.Add(chap);
		}
		
		public void Shrink()
		{
			_data.TrimToSize();
		}
	}
}
