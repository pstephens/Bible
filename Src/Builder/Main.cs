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
using System.Linq;
using Builder.Model;
using Builder.Parser;
using Builder.Services;

namespace Builder
{
    internal class MainClass
    {
        public static int Main()
        {
            SetupServiceLocator();

            IBible bible;
            if (!ParseBible(out bible))
                return 1;

            DisplayStatistics(bible);

            //string tempPath = Path.Combine(Path.GetTempPath(), "BibleTemp");
            //Directory.CreateDirectory(tempPath);

            //const string outputFile = @"..\Artifacts\Binary\Av.bible";
            //string outputPath = Path.GetDirectoryName(outputFile);
            //Directory.CreateDirectory(outputPath);

            //var bible = new BibleAccum(@"..\Artifacts\Normalized\Kjv3.txt",
            //                           tempPath, outputFile);

            //bible.Parse();
            //bible.Process();
            //bible.Write();

            //DisplayStats(bible.Bible, bible.WordIndex);

            //Console.WriteLine("Press the enter key to continue...");
            //Console.ReadLine();

            return 0;
        }

        private static void SetupServiceLocator()
        {
            /*var unityContainer = new UnityContainer();
            unityContainer
                .RegisterType(typeof (ITokenToVerseMap), typeof (TokenToVerseMap))
                .RegisterType(typeof (IVerseTokens), typeof (VerseTokens))
                .RegisterType(typeof (IWordsCaseInsensitive), typeof (WordsCaseInsensitive))
                .RegisterType(typeof (IWordsCaseSensitive), typeof (WordsCaseSensitive));

            var serviceLocator = new UnityServiceLocator(unityContainer);

            Services.Service.ServiceLocator = serviceLocator;*/
        }

        private static bool ParseBible(out IBible bible)
        {
            bible = null;

            var args = Environment.GetCommandLineArgs();
            if (args.Length < 2)
            {
                ShowHelp();
                return false;
            }
            var fileName = args[1];

            if (!File.Exists(fileName))
            {
                Console.WriteLine("Input file '{0}' not found.", fileName);
                return false;
            }

            try
            {
                var parser = new BibleParser();
                using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read,
                    FileShare.Read))
                {
                    bible = parser.Parse(stream);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to parse input file:");
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Builder.exe <InputFileName>");
        }

        private static void DisplayStatistics(IBible bible)
        {
            Console.WriteLine("Bible Statistics: ");
            Console.WriteLine("Total case sensitive words: {0}", 
                              bible.GetService<IWordsCaseSensitive>().Words().Count());
            Console.WriteLine("Total case insensitive words: {0}",
                              bible.GetService<IWordsCaseInsensitive>().Words().Count());


            const int limit = 10;
            var topWordsByFrequency =
                bible.GetService<ITokenToVerseMap>().TokenFrequency()
                    .Where(tokFreq => tokFreq.Token.IsWord)
                    .Select(tokFreq => new {tokFreq.Token, Frequency = tokFreq.RelatedVerses().Sum(rv => rv.Frequency)})
                    .OrderByDescending(tokFreq => tokFreq.Frequency)
                    .Take(limit)
                    .Select(tokFreq => tokFreq.Token + ": " + tokFreq.Frequency);
            Console.WriteLine();
            Console.WriteLine("Top {0} words by frequency: {1}", 
                limit,
                string.Join(", ", topWordsByFrequency));

            var nonWordsByFrequency =
                bible.GetService<ITokenToVerseMap>().TokenFrequency()
                    .Where(tokFreq => !tokFreq.Token.IsWord)
                    .Select(tokFreq => new {tokFreq.Token, Frequency = tokFreq.RelatedVerses().Sum(rv => rv.Frequency)})
                    .OrderByDescending(tokFreq => tokFreq.Frequency)
                    .Select(tokFreq => "'" + tokFreq.Token + "': " + tokFreq.Frequency);
            Console.WriteLine();
            Console.WriteLine("Non-word tokens: {0}", string.Join(", ", nonWordsByFrequency));
        }

        /*public static void DisplayStats(Builder.Archive.Bible bible, WordIndex idx)
        {
            // Calc some stats
            Int32 minVerseRef = -1,
                  maxVerseRef = -1,
                  minVerseCount = -1,
                  maxVerseCount = -1,
                  minChapCount = -1,
                  maxChapCount = -1,
                  minVerseSize = -1,
                  maxVerseSize = -1;
            for (Int32 i = 0; i < bible.Books.Count; ++i)
            {
                Book b = bible.Books[(Builder.Model.BookName) i];
                if (minChapCount == -1 || minChapCount > b.Chapters.Count)
                    minChapCount = b.Chapters.Count;
                if (maxChapCount == -1 || maxChapCount < b.Chapters.Count)
                    maxChapCount = b.Chapters.Count;

                for (Int32 j = 0; j < b.Chapters.Count; ++j)
                {
                    Chapter ch = b.Chapters[j];
                    if (minVerseCount == -1 || minVerseCount > ch.Verses.Count)
                        minVerseCount = ch.Verses.Count;
                    if (maxVerseCount == -1 || maxVerseCount < ch.Verses.Count)
                        maxVerseCount = ch.Verses.Count;

                    for (Int32 k = 0; k < ch.Verses.Count; ++k)
                    {
                        Verse v = ch.Verses[k];
                        if (minVerseSize == -1 || minVerseSize > v.VerseData.Length)
                            minVerseSize = v.VerseData.Length;
                        if (maxVerseSize == -1 || maxVerseSize < v.VerseData.Length)
                            maxVerseSize = v.VerseData.Length;
                    }
                }
            }

            // Calc some word stats
            Int32 verseRefTot = 0, wordData = 0;
            var words = idx.CaseSensitiveWords.GetAllWords();
            foreach (var t in words)
            {
                if (minVerseRef == -1 || minVerseRef > t.VerseRefCount)
                    minVerseRef = t.VerseRefCount;
                if (maxVerseRef == -1 || maxVerseRef < t.VerseRefCount)
                    maxVerseRef = t.VerseRefCount;
                verseRefTot += t.VerseRefCount;
                wordData += t.Word.Length;
            }
            Console.WriteLine("CaseSensitiveIndex:");
            Console.WriteLine(" Words: {0}", words.Length);
            Console.WriteLine(" Verse References: {0}", verseRefTot);
            Console.WriteLine(" Total word bytes: {0}", wordData);
            Console.WriteLine(" Max verse references per word: {0}", maxVerseRef);
            Console.WriteLine(" Min verse references per word: {0}", minVerseRef);

            verseRefTot = 0;
            wordData = 0;
            minVerseRef = -1;
            maxVerseRef = -1;
            words = idx.CaseInsensitiveWords.GetAllWords();
            foreach (WordObject t in words)
            {
                if (minVerseRef == -1 || minVerseRef > t.VerseRefCount)
                    minVerseRef = t.VerseRefCount;
                if (maxVerseRef == -1 || maxVerseRef < t.VerseRefCount)
                    maxVerseRef = t.VerseRefCount;
                verseRefTot += t.VerseRefCount;
                wordData += t.Word.Length;
            }
            Console.WriteLine("CaseInsensitiveIndex:");
            Console.WriteLine(" Words: {0}", words.Length);
            Console.WriteLine(" Verse References: {0}", verseRefTot);
            Console.WriteLine(" Total word bytes: {0}", wordData);
            Console.WriteLine(" Max verse references per word: {0}", maxVerseRef);
            Console.WriteLine(" Min verse references per word: {0}", minVerseRef);

            // Print stats
            Console.WriteLine("Max chapters per book: {0}", maxChapCount);
            Console.WriteLine("Min chapters per book: {0}", minChapCount);
            Console.WriteLine("Max verses per chapter: {0}", maxVerseCount);
            Console.WriteLine("Min verses per chapter: {0}", minVerseCount);
            Console.WriteLine("Max verse length: {0}", maxVerseSize);
            Console.WriteLine("Min verse length: {0}", minVerseSize);
        }*/
    }
}
