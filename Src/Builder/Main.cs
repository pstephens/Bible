using System;

namespace Bible
{
	class MainClass
	{
		public static void Main()
		{
			BibleAccum bible = new BibleAccum(@"c:\src\Bible\Normalized\Kjv3.txt", 
				@"c:\src\Bible\Temp\", @"c:\src\Bible\Binary\Av.bible", false);
			
			bible.Parse();
			bible.Process();
			bible.Write();

			DisplayStats(bible.Bible, bible.WordIndex);

			Console.ReadLine();
		}

		public static void DisplayStats(Bible bible, WordIndex idx)
		{
			// Calc some stats
			Int32 minVerseRef = -1, maxVerseRef = -1,
				minVerseCount = -1, maxVerseCount = -1,
				minChapCount = -1, maxChapCount = -1,
				minVerseSize = -1, maxVerseSize = -1;
			for(Int32 i = 0; i < bible.Books.Count; ++i)
			{
				Book b = bible.Books[(BookName) i];
				if(minChapCount == -1 || minChapCount > b.Chapters.Count)
					minChapCount = b.Chapters.Count;
				if(maxChapCount == -1 || maxChapCount < b.Chapters.Count)
					maxChapCount = b.Chapters.Count;

				for(Int32 j = 0; j < b.Chapters.Count; ++j)
				{
					Chapter ch = b.Chapters[j];
					if(minVerseCount == -1 || minVerseCount > ch.Verses.Count)
						minVerseCount = ch.Verses.Count;
					if(maxVerseCount == -1 || maxVerseCount < ch.Verses.Count)
						maxVerseCount = ch.Verses.Count;

					for(Int32 k = 0; k < ch.Verses.Count; ++k)
					{
						Verse v = ch.Verses[k];
						if(minVerseSize == -1 || minVerseSize > v.VerseData.Length)
							minVerseSize = v.VerseData.Length;
						if(maxVerseSize == -1 || maxVerseSize < v.VerseData.Length)
							maxVerseSize = v.VerseData.Length;
					}
				}
			}

			// Calc some word stats
			Int32 verseRefTot = 0, wordData = 0;
			WordObject[] words = idx.CaseSensitiveWords.GetAllWords();
			for(Int32 i = 0; i < words.Length; ++i)
			{
				if(minVerseRef == -1 || minVerseRef > words[i].VerseRefCount)
					minVerseRef = words[i].VerseRefCount;
				if(maxVerseRef == -1 || maxVerseRef < words[i].VerseRefCount)
					maxVerseRef = words[i].VerseRefCount;
				verseRefTot += words[i].VerseRefCount;
				wordData += words[i].Word.Length;
			}
			Console.WriteLine("CaseSensitiveIndex:");
			Console.WriteLine(" Words: {0}", words.Length.ToString());
			Console.WriteLine(" Verse References: {0}", verseRefTot.ToString());
			Console.WriteLine(" Total word bytes: {0}", wordData.ToString());
			Console.WriteLine(" Max verse references per word: {0}", maxVerseRef.ToString());
			Console.WriteLine(" Min verse references per word: {0}", minVerseRef.ToString());

			verseRefTot = 0; wordData = 0;
			minVerseRef = -1; maxVerseRef = -1;
			words = idx.CaseInsensitiveWords.GetAllWords();
			for(Int32 i = 0; i < words.Length; ++i)
			{
				if(minVerseRef == -1 || minVerseRef > words[i].VerseRefCount)
					minVerseRef = words[i].VerseRefCount;
				if(maxVerseRef == -1 || maxVerseRef < words[i].VerseRefCount)
					maxVerseRef = words[i].VerseRefCount;
				verseRefTot += words[i].VerseRefCount;
				wordData += words[i].Word.Length;
			}
			Console.WriteLine("CaseInsensitiveIndex:");
			Console.WriteLine(" Words: {0}", words.Length.ToString());
			Console.WriteLine(" Verse References: {0}", verseRefTot.ToString());
			Console.WriteLine(" Total word bytes: {0}", wordData.ToString());
			Console.WriteLine(" Max verse references per word: {0}", maxVerseRef.ToString());
			Console.WriteLine(" Min verse references per word: {0}", minVerseRef.ToString());

			// Print stats
			Console.WriteLine("Max chapters per book: {0}", maxChapCount.ToString());
			Console.WriteLine("Min chapters per book: {0}", minChapCount.ToString());
			Console.WriteLine("Max verses per chapter: {0}", maxVerseCount.ToString());
			Console.WriteLine("Min verses per chapter: {0}", minVerseCount.ToString());
			Console.WriteLine("Max verse length: {0}", maxVerseSize.ToString());
			Console.WriteLine("Min verse length: {0}", minVerseSize.ToString());
		}
	}
}
