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
using System.Text;

namespace Normalize
{
    public class RootKjv1769
    {
        private const Int32 fileBuffSize = 65536;
        private const Int32 memoryBuffSize = 8192;
        private static FileStream s_in;
        private static FileStream s_out;
        private static Byte[] s_inBuff;
        private static Byte[] s_outBuff;
        private static Int32 s_inPos;
        private static Int32 s_inMax;
        private static Int32 s_outPos;
        private static Int32 s_bookCount;
        private static Int32 s_verseCount;

        private static readonly String[] s_books =
            {
                "Genesis",
                "Exodus",
                "Leviticus",
                "Numbers",
                "Deuteronomy",
                "Joshua",
                "Judges",
                "Ruth",
                "I Samuel",
                "II Samuel",
                "I Kings",
                "II Kings",
                "I Chronicles",
                "II Chronicles",
                "Ezra",
                "Nehemiah",
                "Esther",
                "Job",
                "Psalms",
                "Proverbs",
                "Ecclesiastes",
                "Song of Solomon",
                "Isaiah",
                "Jeremiah",
                "Lamentations",
                "Ezekiel",
                "Daniel",
                "Hosea",
                "Joel",
                "Amos",
                "Obadiah",
                "Jonah",
                "Micah",
                "Nahum",
                "Habakkuk",
                "Zephaniah",
                "Haggai",
                "Zechariah",
                "Malachi",
                "Matthew",
                "Mark",
                "Luke",
                "John",
                "Acts",
                "Romans",
                "I Corinthians",
                "II Corinthians",
                "Galatians",
                "Ephesians",
                "Philippians",
                "Colossians",
                "I Thessalonians",
                "II Thessalonians",
                "I Timothy",
                "II Timothy",
                "Titus",
                "Philemon",
                "Hebrews",
                "James",
                "I Peter",
                "II Peter",
                "I John",
                "II John",
                "III John",
                "Jude",
                "Revelation"
            };

        private static readonly ASCIIEncoding ASCII = new ASCIIEncoding();

        // File format:
        // Newlines delimited with chr(10)
        // Each verse is on it's own line
        // Each book starts with the string "BOOK:Name" where
        //   name is the name of the book.
        // Each verse starts with X:Y where X is the chapter
        //   and Y is the verse.
        // No italics or paragraph markers are embedded.

        public static void Parse(String path, String output)
        {
            // Open the files
            using (var fileIn = new FileStream(path,
                                               FileMode.Open,
                                               FileAccess.Read,
                                               FileShare.None,
                                               fileBuffSize,
                                               false))
            using (var fileOut = new FileStream(output,
                                                FileMode.Create,
                                                FileAccess.Write,
                                                FileShare.None,
                                                fileBuffSize,
                                                false))
            {
                Console.WriteLine("Parsing: {0}", path);
                try
                {
                    s_in = fileIn;
                    s_out = fileOut;
                    s_inBuff = new Byte[memoryBuffSize];
                    s_outBuff = new Byte[memoryBuffSize];
                    s_outPos = 0;

                    // Prime the pump
                    if (!ReadData())
                        return;

                    // Process data
                    Process();


                    // Write last bit of data
                    WriteData();

                    // Success
                    Console.WriteLine("Success-->");
                }
                finally
                {
                    // Cleanup
                    s_inBuff = null;
                    s_outBuff = null;
                    s_in = null;
                    s_out = null;

                    // Print out statistics
                    Console.WriteLine("Books: {0}", s_bookCount);
                    Console.WriteLine("Verses: {0}", s_verseCount);
                }
            }
        }

        private static Boolean ReadData()
        {
            s_inPos = 0;
            s_inMax = s_in.Read(s_inBuff, 0, memoryBuffSize);
            return s_inMax != 0;
        }

        private static void WriteData()
        {
            if (s_outPos > 0)
            {
                s_out.Write(s_outBuff, 0, s_outPos);
                s_outPos = 0;
            }
        }

        private static void Process()
        {
            while (true)
            {
                if (!ProcessLine()) break;
            }
        }

        private static Byte ReadByte()
        {
            if (s_inPos >= s_inMax)
            {
                if (!ReadData())
                    return 0;
            }

            return s_inBuff[s_inPos++];
        }

        private static void WriteByte(Byte b)
        {
            s_outBuff[s_outPos++] = b;
            if (s_outPos >= memoryBuffSize)
                WriteData();
        }

        private static void WriteEOL()
        {
            WriteByte(13);
            WriteByte(10);
        }

        private static Boolean CopyToEOL()
        {
            Boolean ws_accum = false;
            while (true)
            {
                Byte b = ReadByte();
                if (b == 0) return false;
                if (b == 10) return true;
                if (b == 32 || b == 9)
                {
                    ws_accum = true;
                    continue;
                }
                if (ws_accum)
                {
                    WriteByte(32);
                    ws_accum = false;
                }
                WriteByte(b);
            }
        }

        private static Boolean CopyBookName()
        {
            WriteByte(66);
            WriteByte(58);

            Boolean ws_accum = false;
            var buff = new Byte[32];
            Int32 pos = 0;
            Boolean ret;
            while (true)
            {
                Byte b = ReadByte();
                if (b == 0)
                {
                    ret = false;
                    break;
                }
                if (b == 10)
                {
                    ret = true;
                    break;
                }
                if (b == 32 || b == 9)
                {
                    ws_accum = true;
                    continue;
                }
                if (ws_accum)
                {
                    buff[pos++] = 32;
                    ws_accum = false;
                }
                buff[pos++] = b;
            }

            // Convert byte array to string
            String str = ASCII.GetString(buff, 0, pos);
            str = String.Intern(str);

            // Is it a match?
            for (int i = 0; i < s_books.Length; ++i)
            {
                if (ReferenceEquals(str, s_books[i]))
                {
                    var book = (Books) i;
                    str = book.ToString();
                    i = ASCII.GetBytes(str, 0, str.Length, buff, 0);
                    for (int j = 0; j < i; ++j)
                    {
                        WriteByte(buff[j]);
                    }
                    return ret;
                }
            }

            throw new Exception("Book not found: " + str);
        }

        private static Boolean ProcessLine()
        {
            // Read to first nonwhite
            Byte b;
            while (true)
            {
                if ((b = ReadByte()) == 0) return false;
                if (b != 32) break;
            }

            if (b == 66)
            {
                // Found new book
                ++s_bookCount;
                // discard chaff
                ReadByte();
                ReadByte();
                ReadByte();
                ReadByte();
                // copy book marker
                if (CopyBookName())
                    WriteEOL();
                else
                    return false;
            }
            if (b >= 48 && b <= 57)
            {
                // Found verse
                ++s_verseCount;
                WriteByte(b);
                if (CopyToEOL())
                    WriteEOL();
                else
                    return false;
            }

            return true;
        }
    }
}