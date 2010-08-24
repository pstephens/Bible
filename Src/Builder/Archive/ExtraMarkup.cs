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
using Builder.Model;

namespace Builder.Archive
{
    public enum ExtraMarkupType : byte
    {
        Min = 0,

        /// <summary>
        /// Change to book. The string value is the enum identifier of the book.
        /// </summary>
        Book = 0,

        /// <summary>
        /// Title part 1. String value.
        /// </summary>
        Title0 = 1,

        /// <summary>
        /// Title part 1. String value.
        /// </summary>
        Title1 = 2,

        /// <summary>
        /// Title part 1. String value.
        /// </summary>
        Title2 = 3,

        /// <summary>
        /// Title part 1. String value.
        /// </summary>
        Title3 = 4,

        /// <summary>
        /// Title part 1. String value.
        /// </summary>
        Title4 = 5,

        /// <summary>
        /// Title part 1. String value.
        /// </summary>
        Title5 = 6,

        /// <summary>
        /// Paragraph marker. Value follows the form: "Chapter:Verse1,Verse2,VerseN"
        /// where Chapter is the number of the chapter in the current book, and 
        /// Verse1, Verse2, etc. are the number of the verses that start new 
        /// paragraphs.
        /// </summary>
        Paragraph = 7,

        /// <summary>
        /// Alternate book title. The value contains the more natural title for the
        /// book if the books enum tag is not natural. For example: The alternate 
        /// title for Samuel1 is "1 Samuel"
        /// </summary>
        Alt = 8,

        /// <summary>
        /// Abbreviation. "1Sa" for 1st Samuel
        /// </summary>
        Abbr = 9,

        /// <summary>
        /// Psalm 119 heading. The value is the heading that should appear above this
        /// verse. Example for Psalm 119:9: "Beth". The format is: "119:V:L:Title" 
        /// where V is the verse number, L is the index of the Hebrew letter, and 
        /// Title is the title of the Hebrew letter.
        /// </summary>
        Hebrew = 10,

        Eof = 11,

        Max = 11
    }

    public class ParseExtraMarkup
    {
        #region Misc
        private ParseExtraMarkup() { }
        #endregion

        public static Stream Parse(Stream inputData)
        {
            String lineBuff;
            Int32 lineNo = 0;

            var output = new MemoryStream();
            var writer = new BinaryWriter(output, Encoding.ASCII);

            using(TextReader reader = new StreamReader(inputData, Encoding.ASCII))
            {
                // Parse each line of the input stream reader
                while(true)
                {
                    ++lineNo;

                    lineBuff = reader.ReadLine();
                    if(lineBuff == null) break;

                    // Find the first colon:
                    Int32 pos = lineBuff.IndexOf(":");
                    if(pos < 0) throw new Exception("Invalid extra markup line: no colon. (" +
                                    lineNo + ")");
                    String key = lineBuff.Substring(0, pos).ToUpper();
                    lineBuff = lineBuff.Substring(pos + 1);

                    // Parse each line
                    switch(key)
                    {
                        case "B":
                            ParseBook(writer, lineBuff);
                            break;

                        case "T0":
                            OutputString(writer, ExtraMarkupType.Title0, lineBuff);
                            break;

                        case "T1":
                            OutputString(writer, ExtraMarkupType.Title1, lineBuff);
                            break;

                        case "T2":
                            OutputString(writer, ExtraMarkupType.Title2, lineBuff);
                            break;

                        case "T3":
                            OutputString(writer, ExtraMarkupType.Title3, lineBuff);
                            break;

                        case "T4":
                            OutputString(writer, ExtraMarkupType.Title4, lineBuff);
                            break;

                        case "T5":
                            OutputString(writer, ExtraMarkupType.Title5, lineBuff);
                            break;

                        case "P":
                            ParseParagraphMarkers(writer, lineBuff);
                            break;

                        case "A":
                            OutputString(writer, ExtraMarkupType.Alt, lineBuff);
                            break;

                        case "H":
                            ParseHebrew(writer, lineBuff);
                            break;

                        case "Z":
                            OutputString(writer, ExtraMarkupType.Abbr, lineBuff);
                            break;

                        default:
                            throw new Exception("Invalid extra markup type: unknown line type '" +
                                key + "'. (" + lineNo + ")");
                    }
                }

                // Write eof marker
                writer.Write((Byte) ExtraMarkupType.Eof);

                return output;
            }
        }

        private static void ParseBook(BinaryWriter wr, String bookEnum)
        {
            var name = (BookName) Enum.Parse(typeof(BookName), bookEnum, true);
            wr.Write((Byte) ExtraMarkupType.Book);
            wr.Write((Byte) name);
        }

        private static void OutputString(BinaryWriter wr, ExtraMarkupType t, String str)
        {
            wr.Write((Byte) t);
            wr.Write(str.Trim());
        }

        private static void ParseParagraphMarkers(BinaryWriter wr, String str)
        {
            // Output paragraph marker info
            Int32 pos = str.IndexOf(':');
            if(pos < 0) throw new Exception("Paragraph marker string is invalid: no colon.");
            String[] elems = str.Substring(pos + 1).Split(',');
            wr.Write((Byte) ExtraMarkupType.Paragraph);  // first byte: extra markup id
            wr.Write((Byte) elems.Length);               // second byte: number of paragraph markers
            wr.Write(Byte.Parse(str.Substring(0, pos))); // third byte: chapter number
            // fourth byte, etc: paragraph marker verse positions
            foreach (string t in elems)
            {
                wr.Write(Byte.Parse(t));
            }
        }

        private static void ParseHebrew(BinaryWriter wr, String str)
        {
            String[] elems = str.Split(':');
            if(elems.Length != 4)
                throw new Exception("Hebrew markup is invalid: there must be 4 parts.");
            wr.Write((Byte) ExtraMarkupType.Hebrew); // first byte: extra markup id
            wr.Write(Byte.Parse(elems[0]));          // second byte: chapter id
            wr.Write(Byte.Parse(elems[1]));          // thrid byte: verse id
            wr.Write(Byte.Parse(elems[2]));          // fourth byte: Hebrew index id
            wr.Write(elems[3].Trim());               // fifth byte, etc: Hebrew alphabet transliteration
        }
    }
}