using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bible
{
	public struct WordPos
	{
		public Int32 Start;
		public Int32 Stop;
		public Int32 PrePunct;
		public Int32 PostPunct;
		public Int32 PrevChars;
		public Boolean NewVerse;

		public WordPos(Int32 start, Int32 stop, Int32 prePunct, Int32 postPunct, Int32 prevChars, Boolean newVerse)
		{
			Start = start;
			Stop = stop;
			PrePunct = prePunct;
			PostPunct = postPunct;
			PrevChars = prevChars;
			NewVerse = newVerse;
		}
	}


	public class WordPosIndex
	{
		public static Boolean IsWordChar(Char ch)
		{
			return ch == '\'' ||
				(ch >= 'A' && ch <= 'Z') ||
				(ch >= 'a' && ch <= 'z');
		}

		public static Boolean IsSpace(Char ch)
		{
			return ch == ' ';
		}

		public static Boolean IsHyphen(Char ch)
		{
			return ch == '-';
		}

		private List<WordPos> _list = new List<WordPos>();

		public Int32 MaxWordsPerVerse = 0;

		public WordPos this[Int32 i]
		{
			get { return _list[i]; }
		}

		public Int32 Count
		{
			get { return _list.Count; }
		}

		public Int32 GetPrePunct(String verse, Int32 start)
		{
			Int32 accum = 0;
			for(--start; 
				start >= 0 && !IsWordChar(verse[start]) && !IsSpace(verse[start]); 
				++accum, --start) ;
			return accum;
		}

		public Int32 GetPostPunct(String verse, Int32 stop)
		{
			Int32 accum = 0;
			for(;
				stop < verse.Length && !IsWordChar(verse[stop]) && !IsSpace(verse[stop]);
				++accum, ++stop) ;
			return accum;
		}

		public void ProcessVerse(String verseData)
		{
			Int32 state = 0;
			Int32 wordBegin = 0;
			Int32 beginPos = _list.Count;
			Int32 prevStop = 0;
			Boolean newVerse = true;

			for(Int32 i = 0; ; ++i)
			{
				if(i == verseData.Length)
				{
					if(state == 1 && i > wordBegin)
						_list.Add(new WordPos(wordBegin, i, 
							GetPrePunct(verseData, wordBegin), 
							GetPostPunct(verseData, i),
							wordBegin - prevStop, newVerse));
					MaxWordsPerVerse = Math.Max(MaxWordsPerVerse, _list.Count - beginPos);
					return;
				}
				else
				{
					Boolean iswordchar = IsWordChar(verseData[i]);
					Boolean ishyphen = IsHyphen(verseData[i]);
					switch(state)
					{
						case 0:
							if(iswordchar)
							{
								wordBegin = i;
								state = 1;
							}
							break;
						case 1:
							if(ishyphen)
							{
								// Look ahead 1 char
								if(i + 1 < verseData.Length && IsWordChar(verseData[i + 1]))
									break;
							}


							if(!iswordchar)
							{
								if(i > wordBegin)
								{
									_list.Add(new WordPos(wordBegin, i,
										GetPrePunct(verseData, wordBegin),
										GetPostPunct(verseData, i),
										wordBegin - prevStop, newVerse));
									newVerse = false;
									prevStop = i;
								}
								else
									throw new Exception();

								state = 0;
							}
							break;
					}
				}
			}
		}

		public void Write(BinaryWriter wr)
		{
			foreach(WordPos p in _list)
			{
				Int16 accum =
					(Int16)((((p.Stop - p.Start) & 0x1F) << 0) |
					(((p.PrevChars) & 0x07) << 5) |
					(((p.PrePunct) & 0x03) << 8) |
					(((p.PostPunct) & 0x03) << 10) |
					(((p.NewVerse ? 1 : 0) & 0x01) << 12));
				wr.Write(accum);
			}
		}
	}
}
