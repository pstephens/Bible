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
    public class Staggs
    {
        private static FileStream s_in;
        private static FileStream s_out;
        private static Int32 s_bookCount;
        private static Int32 s_verseCount;
        private static Books s_curBook = Books.None;

        private const Int32 fileBuffSize = 65536;

        private static readonly String[] s_books =
            {
                "Ge", "Ex", "Le", "Nu", "De", "Jos", "Jg", "Ru",
                "1Sa", "2Sa", "1Ki", "2Ki", "1Ch", "2Ch", "Ezr",
                "Ne", "Es", "Job", "Ps", "Pr", "Ec", "So", "Isa",
                "Jer", "La", "Eze", "Da", "Ho", "Joe", "Am",
                "Ob", "Jon", "Mic", "Na", "Hab", "Zep", "Hag",
                "Zec", "Mal",
                "Mt", "Mr", "Lu", "Joh", "Ac", "Ro", "1Co",
                "2Co", "Ga", "Eph", "Php", "Col", "1Th", "2Th",
                "1Ti", "2Ti", "Tit", "Phm", "Heb", "Jas", "1Pe",
                "2Pe", "1Jo", "2Jo", "3Jo", "Jude", "Re"
            };

        // File format
        // Each line ends in chr(10).
        // Each line is a verse.
        // Each line begins with the prefix: Bk X:Y where
        //   Bk is the abbrievated book name, X is the 
        //   chapter, and Y is the verse.

        public static void Parse(String path, String output)
        {
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

                    Process();

                    Console.WriteLine("Success-->");
                }
                finally
                {
                    // Cleanup
                    s_in = null;
                    s_out = null;

                    // Print out statistics
                    Console.WriteLine("Books: {0}", s_bookCount);
                    Console.WriteLine("Verses: {0}", s_verseCount);
                }
            }
        }

        private static void Process()
        {
            while (true)
            {
                if (!ProcessLine()) break;
            }
        }

        private static readonly Byte[] s_bookBuff = new Byte[4];

        private static readonly TempBuff s_verserefAccum = new TempBuff();
        private static readonly TempBuff s_verseAccum = new TempBuff();
        private static readonly TempBuff s_preAccum = new TempBuff();
        private static readonly TempBuff s_postAccum = new TempBuff();
        private static Boolean s_bookPost;

        private static Int32 s_verseId;
        private static Int32 s_chapId;

        private static Boolean ProcessLine()
        {
            Int32 b;
            // Read to first non-white
            while (true)
            {
                if ((b = s_in.ReadByte()) == -1) return false;
                if (b != 32) break;
            }

            // Accumulate book name
            Int32 i = 1;
            s_bookBuff[0] = (Byte) b;
            while (true)
            {
                if ((b = s_in.ReadByte()) == -1) return false;
                if (b == 32)
                {
                    ProcessBook(i);
                    break;
                }
                s_bookBuff[i++] = (Byte) b;
            }

            s_verserefAccum.Reset();
            s_verseAccum.Reset();
            s_preAccum.Reset();
            s_postAccum.Reset();

            // Accumulate verse ref
            do
            {
                b = s_in.ReadByte();
            } while (b == 32 || b == 9);
            if (b == -1) return false;

            s_chapId = 0;
            s_verseId = 0;
            if (b < 48 || b > 57) throw new Exception("Chapter reference not found.");
            while (b >= 48 && b <= 57)
            {
                s_chapId = s_chapId*10 + b - 48;
                s_verserefAccum.WriteByte((Byte) b);
                b = s_in.ReadByte();
            }
            if (b != 58) /* : */
                throw new Exception("Verse reference not found.");
            s_verserefAccum.WriteByte((Byte) b);
            b = s_in.ReadByte();
            if (b < 48 || b > 57) throw new Exception("Verse reference not found.");
            while (b >= 48 && b <= 57)
            {
                s_verseId = s_verseId*10 + b - 48;
                s_verserefAccum.WriteByte((Byte) b);
                b = s_in.ReadByte();
            }
            s_verseCount++;

            // Accumulate data
            Boolean ret = WriteToEOL();

            if (s_verseAccum.Length <= 0) return false;
            if (MainClass.OutputChapterNotes)
            {
                if (s_preAccum.Length > 0)
                {
                    WriteEOL();
                    Byte[] buff = ASCII.GetBytes(String.Format("Pre {0}:", s_chapId));
                    s_out.Write(buff, 0, buff.Length);
                    s_preAccum.WriteToStream(s_out);
                }
            }
            WriteEOL();
            s_verserefAccum.WriteToStream(s_out);
            s_out.WriteByte(32);
            s_verseAccum.WriteToStream(s_out);
            if (MainClass.OutputChapterNotes)
            {
                if (s_postAccum.Length > 0)
                {
                    WriteEOL();
                    Byte[] buff = ASCII.GetBytes(String.Format("Post:"));
                    s_out.Write(buff, 0, buff.Length);
                    s_postAccum.WriteToStream(s_out);
                    s_bookPost = true;
                }
            }

            return ret;
        }

        private static void WriteEOL()
        {
            s_out.WriteByte(13);
            s_out.WriteByte(10);
        }

        private static Boolean WriteToEOL()
        {
            Boolean ws_accum = false;
            while (true)
            {
                Int32 b = s_in.ReadByte();
                if (b == -1) return false;
                if (b == 10) return true;
                if ((b == 91 || b == 93)
                    && (!MainClass.OutputItalics)) continue;

                if (b == 60)
                {
                    if (!ProcessPrePost()) return false;
                    continue;
                }

                if (b == 32 || b == 9)
                {
                    ws_accum = true;
                    continue;
                }
                if (ws_accum)
                {
                    ws_accum = false;
                    if (s_verseAccum.Length > 0)
                        s_verseAccum.WriteByte(32);
                }
                s_verseAccum.WriteByte((Byte) b);
            }
        }

        private static Boolean ProcessPrePost()
        {
            TempBuff tbuff =
                s_verseAccum.Length <= 0 ? s_preAccum : s_postAccum;

            Int32 b = s_in.ReadByte();
            if (b != 60) throw new Exception("Bad pre/post syntax.");
            Boolean ws_accum = false;
            while (true)
            {
                b = s_in.ReadByte();
                if (b == -1 || b == 10 || b == 60)
                    throw new Exception("Bad pre/post syntax.");
                if ((b == 91 || b == 93) &&
                    (!MainClass.OutputItalics)) continue;
                if (b == 62)
                {
                    b = s_in.ReadByte();
                    if (b != 62) throw new Exception("Bad pre/post syntax.");
                    return true;
                }
                if (b == 32 || b == 9)
                {
                    ws_accum = true;
                    continue;
                }
                if (ws_accum)
                {
                    ws_accum = false;
                    if (tbuff.Length > 0)
                        tbuff.WriteByte(32);
                }
                tbuff.WriteByte((Byte) b);
            }
        }

        private static readonly ASCIIEncoding ASCII = new ASCIIEncoding();

        private static void ProcessBook(Int32 size)
        {
            String str = String.Intern(ASCII.GetString(s_bookBuff, 0, size));
            Int32 i;

            if (s_curBook != Books.None
                && s_books[(Int32) s_curBook] == str)
            {
                if (s_bookPost) throw new Exception("There was verse data after post data.");
                // NO-OP
                return;
            }
            s_bookPost = false;

            for (i = 0; i < s_books.Length; ++i)
            {
                if (s_books[i] == str)
                {
                    var newBook = (Books) i;
                    if (newBook == s_curBook) return;
                    s_curBook = newBook;
                    str = newBook.ToString();
                    var buff = new Byte[32];
                    Int32 len = ASCII.GetBytes(str, 0, str.Length, buff, 0);
                    if (newBook != Books.Genesis)
                        WriteEOL();
                    s_out.WriteByte(66);
                    s_out.WriteByte(58);
                    for (Int32 j = 0; j < len; ++j)
                        s_out.WriteByte(buff[j]);
                    s_bookCount++;
                    return;
                }
            }

            throw new Exception("Book not found: " + str);
        }
    }
}
