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
using System.IO;

namespace Builder.Archive
{
	public class Bible
	{
		// Fields
		private readonly BookCollection _books;
		private readonly NonVerseCollection _nonVerses;
		private Int32 _nextChapterId;
		private Int32 _nextVerseId;
		private readonly Int32 _chapterCount;
		private readonly Int32 _verseCount;
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
