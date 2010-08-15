// Launches normalization routines

using System;

namespace Normalize
{
	public enum Books
	{
		None = -1,
		Genesis = 0,
		Exodus = 1,
		Leviticus = 2,
		Numbers = 3,
		Deuteronomy = 4,
		Joshua = 5,
		Judges = 6,
		Ruth = 7,
		Samuel1 = 8,
		Samuel2 = 9,
		Kings1 = 10,
		Kings2 = 11,
		Chronicles1 = 12,
		Chronicles2 = 13,
		Ezra = 14,
		Nehemiah = 15,
		Esther = 16,
		Job = 17,
		Psalms = 18,
		Proverbs = 19,
		Ecclesiastes = 20,
		SongOfSolomon = 21,
		Isaiah = 22,
		Jeremiah = 23,
		Lamentations = 24,
		Ezekiel = 25,
		Daniel = 26,
		Hosea = 27,
		Joel = 28,
		Amos = 29,
		Obadiah = 30,
		Jonah = 31,
		Micah = 32,
		Nahum = 33,
		Habakkuk = 34,
		Zephaniah = 35,
		Haggai = 36,
		Zechariah = 37,
		Malachi = 38,
		Matthew = 39,
		Mark = 40,
		Luke = 41,
		John = 42,
		Acts = 43,
		Romans = 44,
		Corinthians1 = 45,
		Corinthians2 = 46,
		Galatians = 47,
		Ephesians = 48,
		Philippians = 49,
		Colossians = 50,
		Thessalonians1 = 51,
		Thessalonians2 = 52,
		Timothy1 = 53,
		Timothy2 = 54,
		Titus = 55,
		Philemon = 56,
		Hebrews = 57,
		James = 58,
		Peter1 = 59,
		Peter2 = 60,
		John1 = 61,
		John2 = 62,
		John3 = 63,
		Jude = 64,
		Revelation = 65
	}
	
	public class MainClass
	{
	    // Properties
	    public static bool OutputItalics { get; private set; }

	    public static bool OutputParaMarks { get; private set; }

	    public static bool OutputChapterNotes { get; private set; }

	    public static Int32 Main()
		{
			OutputItalics = true;
			OutputParaMarks = false;
			OutputChapterNotes = true;
			
			// NO ITALICS, NO PRE/POST TEXT
			RootKjv1769.Parse(
				@"input-Kjv\kjv-1769.txt",
				@"..\Normalized\Kjv1.txt");
			// ITALICS IN SQUARE BRACKETS, PRE/POST IN DOUBLE ANGLE BRACKETS
			Staggs.Parse(
				@"input-Kjv\www.staggs.pair.com-kjbp\kjv.txt",
				@"..\Normalized\Kjv2.txt");
			// ITALICS IN SQUARE BRACKETS, PRE/POST IN SINGLE ANGLE BRACKETS
			BfOrg.Parse(
				@"input-Kjv\www.bf.org\av-1769\",
				@"..\Normalized\Kjv3.txt");
			// NO ITALICS, NO PRE/POST TEXT
			Gutenberg.Parse(
				@"input-Kjv\sailor.gutenberg.org\kjv10-edited.txt",
				@"..\Normalized\Kjv4.txt");

			return 1;
		}
	}
}
