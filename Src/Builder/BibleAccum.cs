using System;
using System.Collections;
using System.IO;
using System.Text;


namespace Bible
{
	public class BibleAccum
	{
		// Fields
		private readonly String _filename;
		private readonly String _outputFilename;
		private readonly String _temppath;
		private readonly Boolean _caseInsensitive;
		
		// Properties
		public String OutputFilename
		{
			get { return _outputFilename; }
		}

		public String FileName
		{
			get { return _filename; }
		}
		
		public String TempPath
		{
			get { return _temppath; }
		}

		public Bible Bible
		{
			get { return _bible; }
		}
		
		// Constructor
		public BibleAccum(String filename, String temppath, 
				String outputPath, Boolean caseInsensitive)
		{
			if(filename == null || filename.Length <= 0)
				throw new ArgumentNullException("filename");
			if(temppath == null || temppath.Length <= 0)
				throw new ArgumentNullException("temppath");
			if(outputPath == null || outputPath.Length <= 0)
				throw new ArgumentNullException("outputPath");
			
			_filename = filename;
			_temppath = temppath;
			_caseInsensitive = caseInsensitive;
			_outputFilename = outputPath;
		}
		
		// Methods
		public void Parse()
		{
			InitAccum();
			
			// Setup input objects
			using(FileStream str = new FileStream(_filename, FileMode.Open,
					FileAccess.Read, FileShare.Read, 65536))
			using(StreamReader rd = new StreamReader(str, Encoding.ASCII))
			{
				
				// Process lines
				String dat;
				while( (dat = rd.ReadLine()) != null)
				{
					++_lineNo;
					ProcessLine(dat);
				}
			}

			// Also parse the extra markup info
			using(Stream str = System.Reflection.Assembly.GetExecutingAssembly().
					  GetManifestResourceStream("BibleMarkup.txt"))
			{
				_bible.SetBibleMarkup(ParseExtraMarkup.Parse(str));
			}
		}

		public void Process()
		{
			ProcessWordIndex();
		}
		
		public void Write()
		{
			// Write bible to disk
			using(FileStream str = new FileStream(OutputFilename, FileMode.Create,
					  FileAccess.Write, FileShare.Write, 65536))
			using(BinaryWriter wr = new BinaryWriter(str, Encoding.ASCII))
			{
                BibleWriter.Write(_bible, WordIndex, wr);
			}
		}
		
		// Implementation
		private Int32   _lineNo;
		private Bible   _bible;
		private Book    _curBook;
		private Chapter _curChapter;
		private Verse   _curVerse;
		private String  _preAccum;
		private void InitAccum()
		{
			_bible = new Bible();
			_curBook = null;
			_curChapter = null;
			_curVerse = null;
			_preAccum = null;
			_lineNo = 0;
		}

		private Exception VerseException(String msg)
		{
			return new Exception(msg + " Line: " + _lineNo.ToString());
		}

		private void ProcessLine(String line)
		{
			// What type of line is this?
			Char firstChar = line[0];
			if(firstChar >= '0' && firstChar <= '9') // New verse
			{
				ProcessVerse(line);
			}
			else if(firstChar == 'B') // New book
			{
				ProcessBook(line);
			}
			else if(String.Compare(line, 0, "Pre ", 0, 4) == 0)
			{
				ProcessPreData(line);
			}
			else if(String.Compare(line, 0, "Post:", 0, 5) == 0)
			{
				ProcessPostData(line);
			}
			else
				throw VerseException("Invalid verse data: '" + line + "'.");
		}

		private void ProcessVerse(String line)
		{
			// Assertion
			if(_curBook == null)
				throw VerseException("There is no current book.");

			// Parse the chapter & verse reference
			Int32 colonPos, spacePos, chap, verse;
			colonPos = line.IndexOf(':');
			spacePos = line.IndexOf(' ');
			if(colonPos < 0 || spacePos < 0)
				throw VerseException("Invalid chapter or verse format. (1)");
			chap = Int32.Parse(line.Substring(0, colonPos));
			verse = Int32.Parse(line.Substring(colonPos + 1, spacePos - colonPos - 1));
			if(chap <= 0 || verse <= 0)
				throw VerseException("Invalid chapter or verse format. (2)");

			// Parse out the verse info
			String verseData = line.Substring(spacePos + 1);

			// Get the current chapter object
			if(_curChapter == null)
			{
				if(chap != 1 || verse != 1)
					throw VerseException("Expected first verse and chapter of the book.");

				SetupNewCurrentChapter(chap - 1);
			}
			else
			{
				if(_curChapter.ChapterNum != chap - 1)
				{
					if(verse != 1)
						throw VerseException("Expected first verse of the chapter.");
					if(_curChapter.ChapterNum + 1 != chap - 1)
						throw VerseException("The chapter number was incremented by more than one.");

					SetupNewCurrentChapter(chap - 1);
				}
			}
			if(_preAccum != null)
				throw VerseException("Unexpected preface data.");

			// Append a new verse object
			Verse verseObj = new Verse(_bible.GetNextVerseId(), verse - 1, verseData, _curChapter);
			_curChapter.Verses.Add(verseObj);
			_curVerse = verseObj;
		}

		private void ProcessBook(String line)
		{
			String bookTitle = line.Substring(2);
			_curChapter = null;
			_curVerse = null;
			BookName bookName = (BookName) Enum.Parse(typeof(BookName), bookTitle);
			_curBook = _bible.Books[bookName];
		}

		private void ProcessPreData(String line)
		{
			Int32 colonPos = line.IndexOf(':');
			if(colonPos < 0)
				throw VerseException("Bad 'pre' data.");
			String preData = line.Substring(colonPos + 1);
			_preAccum = preData;
		}

		private void ProcessPostData(String line)
		{
			String postData = line.Substring(5);
			if(postData[0] == '[' && postData[postData.Length - 1] == ']')
				postData = postData.Substring(1, postData.Length - 2);

			BookPostscript ps = new BookPostscript(_curBook, 
				_bible.GetNextVerseId(), postData);
			_curBook.SetPostscript(ps);
			_bible.NonVerses.Add(ps);
		}

		private void SetupNewCurrentChapter(Int32 chap)
		{
			// Calculate chapter preface
			ChapterPreface preface;
			if(_preAccum != null)
			{
				preface = new ChapterPreface(_bible.GetNextVerseId(), _preAccum);
				_bible.NonVerses.Add(preface);
			}
			else
				preface = null;

			Chapter chapObj = new Chapter(
				_curBook, _bible.GetNextChapterId(), chap, preface);
			_curChapter = chapObj;
			_curBook.Chapters.Add(chapObj);
			_curVerse = null;
			_preAccum = null;
		}

		// Word Index routines
		private WordIndex _wordIndex;
		public WordIndex WordIndex
		{
			get { return _wordIndex; }
		}

		private void ProcessWordIndex()
		{
			_wordIndex = new WordIndex(_bible);
			_wordIndex.ProcessBible();
			_wordIndex.Shrink();
		}
	}
}
