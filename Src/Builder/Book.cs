using System;

namespace Bible
{
	public class Book
	{
		// Fields
		private readonly Bible _bible;
		private readonly BookName _book;
		private readonly ChapterCollection _chapters;
		private BookPostscript _postscript;
		
		// Properties
		public Bible Bible
		{
			get { return _bible; }
		}
		
		public BookName BookId
		{
			get { return _book; }
		}
		
		public String Name
		{
			get { return _book.ToString(); }
		}
		
		public ChapterCollection Chapters
		{
			get { return _chapters; }
		}
		
		public BookPostscript Postscript
		{
			get { return _postscript; }
		}
		
		// Constructor
		public Book(Bible bible, BookName bookId)
		{
			if(bible == null)
				throw new ArgumentNullException("bible");
			if((Int32) bookId < 0 || (Int32) bookId > 65)
				throw new ArgumentOutOfRangeException("bookId");
			
			
			_bible = bible;
			_book = bookId;
			_chapters = new ChapterCollection();
		}
	
		// Methods
		public void SetPostscript(BookPostscript postscript)
		{
			if(postscript == null)
				throw new ArgumentNullException("postscript");
			
			_postscript = postscript;
		}
	}
	
	public class BookCollection
	{
		// Fields
		private readonly Book[] _books;
		private readonly Bible _bible;
		
		// Properties
		public Int32 Count
		{
			get { return 66; }
		}

		public Book this[BookName bookId]
		{
			get
			{
				// Assertions
				Int32 idx = (Int32) bookId;
				if(idx < 0 || idx > 65)
					throw new ArgumentNullException("bookId");
				
				// Create new, if necessary
				if(_books[idx] == null)
					_books[idx] = new Book(_bible, bookId);
				
				// Return the book object
				return _books[idx];
			}
		}
		
		// Constructor
		public BookCollection(Bible bible)
		{
			if(bible == null) throw new ArgumentNullException("bible");
			
			_bible = bible;
			_books = new Book[66]; // We are not Popish here!
		}
	}
}
