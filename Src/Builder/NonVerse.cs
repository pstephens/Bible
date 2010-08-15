// Store information about chapter prefaces and book postscripts

using System;
using System.Collections;

namespace Bible
{
	public abstract class NonVerse
	{
		// Fields
		private readonly Int32 _nonVerseId;
		private String _data;
		
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

		private Int32 _wordPosIndex;
		private Int32 _wordPosLen;
		public Int32 WordPosIndex
		{
			get { return _wordPosIndex; }
			set { _wordPosIndex = value; }
		}

		private Int32 _italicPosIndex;
		private Int32 _italicPosLen;
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

		public void WriteVerseIndexData(System.IO.BinaryWriter wr)
		{
			Int32 accum = _data.Length;
			accum |= _wordPosLen << 10;
			accum |= _italicPosLen << 17;
			wr.Write((Byte)((accum & 0x0000FF)));
			wr.Write((Byte)((accum & 0x00FF00) >> 8));
			wr.Write((Byte)((accum & 0xFF0000) >> 16));
		}
		
		// Constructor
		public NonVerse(Int32 nonVerseId, String data)
		{
			if(data == null)
				throw new ArgumentNullException("data");
		
			_nonVerseId = nonVerseId;
			_data = data;
		}
	}
	
	public class ChapterPreface : NonVerse
	{
		// Fields
		private Chapter _chapter;
		
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
		public ChapterPreface(Int32 nonVerseId, String data)
			: base(nonVerseId, data)
		{
		}

		internal void SetChapter(Chapter chap)
		{
			if(chap == null)
				throw new ArgumentNullException("chap");

			_chapter = chap;
		}
	}
	
	public class BookPostscript : NonVerse
	{
		// Fields
		private readonly Book _book;
		
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
		public BookPostscript(Book book, Int32 nonVerseId, String data)
			: base(nonVerseId, data)
		{
			if(book == null)
				throw new ArgumentNullException("book");
			
			_book = book;
		}
	}
	
	public class NonVerseCollection
	{
		// Fields
		private readonly ArrayList _data;
		
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
		public NonVerseCollection()
		{
			_data = new ArrayList();
		}
		
		// Methods
		public void Add(NonVerse nonverse)
		{
			if(nonverse == null)
				throw new ArgumentNullException("nonverse");
			
			_data.Add(nonverse);
		}
		
		public void Shrink()
		{
			_data.TrimToSize();
		}
	}
}
