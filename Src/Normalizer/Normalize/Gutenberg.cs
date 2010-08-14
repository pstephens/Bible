using System;
using System.IO;
using System.Text;

namespace Normalize
{
    public class Gutenberg
    {
        private const Int32 fileBuffSize = 65536;
        private static FileStream s_in;
        private static FileStream s_out;
        private static Int32 s_bookCount;
        private static Int32 s_verseCount;
        private static readonly ASCIIEncoding ASCII = new ASCIIEncoding();
        private static readonly Byte[] buff = new Byte[32];

        // File format
        // Books are separated by two or more blank lines,
        // followed by the long title of the book,
        // followed by one or more blank lines followed by
        // the book's verses.
        // More than one verse can appear on one line.
        // One verse can appear on more than one line.

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
                try
                {
                    s_in = fileIn;
                    s_out = fileOut;

                    Console.WriteLine("Parsing: {0}", path);
                    ProcessFile();

                    Console.WriteLine("Success-->");
                }
                finally
                {
                    s_in = null;
                    s_out = null;

                    // Output statistics
                    Console.WriteLine("Books: {0}", s_bookCount);
                    Console.WriteLine("Verses: {0}", s_verseCount);
                }
            }
        }

        private static void ProcessFile()
        {
            // start out with high line count to init first book
            Int32 emptyLineCount = 4;
            Boolean lineData = false;
            Boolean ws_accum = false;

            while (true)
            {
                Int32 b = s_in.ReadByte();
                if (b == 13)
                {
                    b = s_in.ReadByte();
                    if (b != 10) throw new Exception();
                    if (!lineData) ++emptyLineCount;
                    lineData = false;
                    ws_accum = true;
                    continue;
                }
                if (b == 32 || b == 9)
                {
                    ws_accum = true;
                    continue;
                }
                if (b >= 48 && b <= 57)
                {
                    // Start a new verse
                    ++s_verseCount;
                    emptyLineCount = 0;
                    lineData = true;
                    WriteEOL();
                    s_out.WriteByte((Byte) b);
                    b = s_in.ReadByte();
                    while (b != 32 && b != 13)
                    {
                        s_out.WriteByte((Byte) b);
                        b = s_in.ReadByte();
                    }
                    ws_accum = true;
                    if (b == 13)
                    {
                        b = s_in.ReadByte();
                        if (b != 10) throw new Exception();
                    }
                    continue;
                }
                if (b == -1) return;
                if (emptyLineCount >= 2)
                {
                    // Output book header
                    emptyLineCount = 0;
                    lineData = false;
                    ws_accum = false;
                    var book = (Books) s_bookCount;
                    ++s_bookCount;
                    if (s_bookCount > 1) WriteEOL();
                    s_out.WriteByte(66);
                    s_out.WriteByte(58);
                    WriteString(book.ToString());
                    while (b != 13) b = s_in.ReadByte();
                    b = s_in.ReadByte();
                    if (b != 10) throw new Exception();
                    continue;
                }

                // Regular line
                emptyLineCount = 0;
                lineData = true;
                if (ws_accum)
                {
                    s_out.WriteByte(32);
                    ws_accum = false;
                }
                s_out.WriteByte((Byte) b);
            }
        }

        private static void WriteEOL()
        {
            s_out.WriteByte(13);
            s_out.WriteByte(10);
        }

        private static void WriteString(String str)
        {
            Int32 len = ASCII.GetBytes(str, 0, str.Length, buff, 0);
            for (Int32 i = 0; i < len; ++i)
            {
                s_out.WriteByte(buff[i]);
            }
        }
    }
}