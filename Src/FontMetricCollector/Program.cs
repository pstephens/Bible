#region Copyright Notice

/* Copyright 2009-2011 Peter Stephens

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
using System.Collections.Generic;

namespace FontMetricCollector
{
    class Program
    {
        static int Main(string[] args)
        {
            var options = new CommandLineOptions(args);
            if(!options.IsValid)
            {
                OutputHelpMessage(options.Messages);
                return 1;
            }

            try
            {
                OutputFontMetricData(options);
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed while outputing font metric data:\n" +
                                  ex.Message);
                return 1;
            }
        }

        private static void OutputHelpMessage(IEnumerable<string> msgs)
        {
            foreach(var msg in msgs)
                Console.WriteLine(msg);
            Console.WriteLine("Syntax: FontMetricCollector <outputPath> FontFamily1 [FontFamily2] [FontFamilyN] [-help]");
        }

        private static void OutputFontMetricData(CommandLineOptions options)
        {
            
        }
    }
}
