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
    public class BfOrg
    {
        private static FileStream s_in;
        private static FileStream s_out;
        private static Int32 s_bookCount;
        private static Int32 s_verseCount;
        private static String s_path;

        private const Int32 fileBuffSize = 65536;

        private static readonly String[] s_files =
            {
                "ge.av", "ex.av", "le.av", "nu.av", "de.av",
                "jos.av", "jud.av", "ru.av", "1sa.av", "2sa.av",
                "1ki.av", "2ki.av", "1ch.av", "2ch.av", "ezr.av",
                "ne.av", "es.av", "job.av", "ps.av", "pr.av",
                "ec.av", "so.av", "isa.av", "jer.av", "la.av",
                "eze.av", "da.av", "ho.av", "joe.av", "am.av",
                "ob.av", "jon.av", "mic.av", "na.av", "hab.av",
                "zep.av", "hag.av", "zec.av", "mal.av",
                "mt.av", "mr.av", "lu.av", "joh.av", "ac.av",
                "ro.av", "1co.av", "2co.av", "ga.av", "eph.av",
                "php.av", "col.av", "1th.av", "2th.av", "1ti.av",
                "2ti.av", "tit.av", "phm.av", "heb.av", "jas.av",
                "1pe.av", "2pe.av", "1jo.av", "2jo.av", "3jo.av",
                "jude.av", "re.av"
            };

        // File format
        // Each book is in its own file.
        // Lines are delimited with a CRLF.
        // Verses can be broken onto multiple lines.
        // If the line begins with a space, it is a verse continuation.
        // If a line begins with a number it is the start of a verse.

        public static void Parse(String path, String output)
        {
            s_path = path;
            using (var outFile = new FileStream(output,
                                                FileMode.Create,
                                                FileAccess.Write,
                                                FileShare.None,
                                                fileBuffSize,
                                                false))
            {
                Console.WriteLine("Parsing: {0}", path);
                try
                {
                    s_out = outFile;

                    ProcessFiles();

                    Console.WriteLine("Success-->");
                }
                finally
                {
                    s_out = null;

                    // Print out statistics
                    Console.WriteLine("Books: {0}", s_bookCount);
                    Console.WriteLine("Verses: {0}", s_verseCount);
                }
            }
        }

        private static void ProcessFiles()
        {
            for (Int32 i = 0; i < s_files.Length; ++i)
            {
                if (i > 0) WriteEOL();
                s_out.WriteByte(66);
                s_out.WriteByte(58);
                WriteBookName(i);
                ProcessFile(i);
            }
        }

        private static void ProcessFile(Int32 i)
        {
            ++s_bookCount;
            using (var fileIn = new FileStream(s_path + s_files[i],
                                               FileMode.Open,
                                               FileAccess.Read,
                                               FileShare.None,
                                               fileBuffSize,
                                               false))
            {
                try
                {
                    s_in = fileIn;

                    ProcessVerses();
                }
                finally
                {
                    s_in = null;
                }
            }
        }

        private static readonly TempBuff s_verserefAccum = new TempBuff();
        private static readonly TempBuff s_verseAccum = new TempBuff();
        private static readonly TempBuff s_preAccum = new TempBuff();
        private static readonly TempBuff s_postAccum = new TempBuff();
        private static Int32 s_chapId;

        private static void ProcessVerses()
        {
            Boolean ws_accum = false;
            Boolean outputItalics = MainClass.OutputItalics;

            while (true)
            {
                Int32 b = s_in.ReadByte();
                if (b == -1) break;
                if (b > 48 && b <= 57)
                {
                    OutputVerse();
                    ws_accum = false;
                    b = ProcessVerseRef(b);
                }
                if (b == 9 || b == 32 || b == 10 || b == 13)
                {
                    ws_accum = true;
                    continue;
                }
                if (b == 60)
                {
                    ProcessPrePost();
                    continue;
                }
                if ((b == 91 || b == 93) &&
                    !outputItalics) continue;
                if (ws_accum)
                {
                    ws_accum = false;
                    if (s_verseAccum.Length > 0)
                        s_verseAccum.WriteByte(32);
                }
                s_verseAccum.WriteByte((Byte) b);
            }

            OutputVerse();
        }

        private static void ProcessPrePost()
        {
            TempBuff tb = s_verseAccum.Length > 0 ? s_postAccum : s_preAccum;
            Boolean ws_accum = false;
            while (true)
            {
                Int32 b = s_in.ReadByte();
                if (b == -1 || b == 60)
                    throw new Exception("Bad pre/post syntax.");
                if ((b == 91 || b == 93) &&
                    (!MainClass.OutputItalics)) continue;
                if (b == 62)
                {
                    return;
                }
                if (b == 32 || b == 9 || b == 10 || b == 13)
                {
                    ws_accum = true;
                    continue;
                }
                if (ws_accum)
                {
                    ws_accum = false;
                    if (tb.Length > 0)
                        tb.WriteByte(32);
                }
                tb.WriteByte((Byte) b);
            }
        }

        private static Int32 ProcessVerseRef(Int32 b)
        {
            s_chapId = 0;
            do
            {
                s_verserefAccum.WriteByte((Byte) b);
                s_chapId = checked(s_chapId*10 + b - 48);
                b = s_in.ReadByte();
            } while (b >= 48 && b <= 57);

            if (b != 58) throw new Exception("Verse reference syntax error.");
            s_verserefAccum.WriteByte(58);
            b = s_in.ReadByte();
            if (b < 48 || b > 57) throw new Exception("Verse reference syntax error.");
            while (b >= 48 && b <= 57)
            {
                s_verserefAccum.WriteByte((Byte) b);
                b = s_in.ReadByte();
            }

            return b;
        }

        private static void OutputVerse()
        {
            if (s_verseAccum.Length > 0)
            {
                ++s_verseCount;

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
                    }
                }
            }

            s_verserefAccum.Reset();
            s_verseAccum.Reset();
            s_preAccum.Reset();
            s_postAccum.Reset();
        }

        private static readonly ASCIIEncoding ASCII = new ASCIIEncoding();

        private static void WriteBookName(Int32 i)
        {
            var b = (Books) i;
            Byte[] data = ASCII.GetBytes(b.ToString());
            s_out.Write(data, 0, data.Length);
        }

        private static void WriteEOL()
        {
            s_out.WriteByte(13);
            s_out.WriteByte(10);
        }
    }
}
