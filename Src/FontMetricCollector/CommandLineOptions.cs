﻿#region Copyright Notice

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

using System.Collections.Generic;

namespace FontMetricCollector
{
    internal class CommandLineOptions
    {
        public string OutputPath { get; private set; }
        public string FontFamily { get; private set; }
        public bool IsValid { get; private set; }
        private string ErrorMessage { get; set; }

        public static CommandLineOptions Parse(string[] args)
        {
            return new CommandLineOptions();
        }

        public IEnumerable<string> GetOutputMessage()
        {
            yield break;
            // Console.WriteLine("Syntax: FontMetricCollector <outputPath> FontFamily1 [FontFamily2] [FontFamilyN] [-help]");
        }
    }
}