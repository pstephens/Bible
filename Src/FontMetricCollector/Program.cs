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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Markup;
using System.Windows.Media;

namespace FontMetricCollector
{
    class Program
    {
        static int Main(string[] args)
        {
            OutputBanner();

            var options = CommandLineOptions.Parse(args);
            if(!options.IsValid)
            {
                OutputHelpMessage(options.GetOutputMessage());
                return 1;
            }

            try
            {
                OutputFontMetricData(options.OutputPath, options.FontFamily);
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed while outputing font metric data:\n" +
                                  ex.Message);
                return 1;
            }
        }

        private static void OutputBanner()
        {
            Console.WriteLine("{0} {1}",
                              GetAssemblyAttributeData<AssemblyTitleAttribute, string>(attr => attr.Title),
                              Assembly.GetExecutingAssembly().GetName().Version);
            Console.WriteLine(
                GetAssemblyAttributeData<AssemblyCopyrightAttribute, string>(attr => attr.Copyright));
            Console.WriteLine();
        }

        private static TRet GetAssemblyAttributeData<TAttr, TRet>(Func<TAttr, TRet> transform)
        {
            return
                Assembly.GetExecutingAssembly()
                    .GetCustomAttributes(typeof (TAttr), false)
                    .Cast<TAttr>()
                    .Select(transform)
                    .SingleOrDefault();
        }

        private static void OutputHelpMessage(IEnumerable<string> msgs)
        {
            foreach(var msg in msgs)
                Console.WriteLine(msg);
        }

        private static void OutputFontMetricData(string path, string font)
        {
            var fontfamily = LookupFontFamily(font);

            using (var str = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var writer = new FontMetricWriter(str, fontfamily);
                writer.Write();
            }
        }

        private static FontFamily LookupFontFamily(string font)
        {
            var matchingFontFamilies = Fonts.SystemFontFamilies
                .Where(f => f.FamilyNames[XmlLanguage.GetLanguage("us-en")].Equals(font))
                .ToArray();

            if (matchingFontFamilies.Length <= 0)
                throw new Exception(string.Format("Font family '{0}' was not found.", font));
            if (matchingFontFamilies.Length > 1)
                throw new Exception(string.Format("More than one font family matches the name '{0}'.", font));
            return matchingFontFamilies[0];
        }
    }
}
