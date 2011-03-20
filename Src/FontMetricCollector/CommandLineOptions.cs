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
    internal class CommandLineOptions
    {
        public string OutputPath { get; private set; }
        public string FontFamily { get; private set; }
        public bool IsValid { get; private set; }
        public string ErrorMessage { get; private set; }

        public static CommandLineOptions Parse(string[] args)
        {
            if(args == null)
                throw new ArgumentNullException("args");

            var options = new CommandLineOptions();

            if (args.Length < 1)
            {
                options.SetErrorMessage("OutputFile is a required argument.");
            }
            else if (args.Length < 2)
            {
                options.SetErrorMessage("FontFamily is a required argument.");
            }
            else if (args.Length > 2)
            {
                options.SetErrorMessage("Only two arguments are supported.");
            }
            else
            {
                options.OutputPath = args[0];
                options.FontFamily = args[1];
                options.IsValid = true;
            }

            return options;
        }

        public IEnumerable<string> GetOutputMessage()
        {
            if (IsValid)
                yield break;
            if(!string.IsNullOrEmpty(ErrorMessage))
                yield return "Error: " + ErrorMessage;
            yield return "Syntax: FontMetricCollector path fontfamily";
        }

        private void SetErrorMessage(string msg)
        {
            
        }
    }
}