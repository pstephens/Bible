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
using System.Linq;

namespace FontMetricCollector
{
    internal class CommandLineOptions
    {
        public string OutputPath { get; private set; }
        public bool IsValid { get; private set; }
        public bool ShowHelp { get; private set; }
        public ICollection<string> FontFamilies { get; private set; }
        public ICollection<string> Messages { get; private set; }

        public CommandLineOptions(IEnumerable<string> args)
        {
            FontFamilies = new List<string>();
            Messages = new List<string>();
            IsValid = Parse(args ?? Enumerable.Empty<string>()) && Validate();
        }

        private bool Parse(IEnumerable<string> args)
        {
            if (args == null) 
                return true;
            foreach(var arg in args.Where(arg => arg != null && !string.IsNullOrWhiteSpace(arg)))
            {
                if (arg.StartsWith("-") || arg.StartsWith("/"))
                {
                    if (!ParseSwitch(arg)) 
                        return false;
                }
                else
                {
                    if (OutputPath == null)
                        OutputPath = arg;
                    else
                        FontFamilies.Add(arg);
                }
            }
            return true;
        }

        private bool Validate()
        {
            if (OutputPath == null)
            {
                AddMessage("The outputPath parameter must be specified.");
                return false;
            }

            return true;
        }

        private void AddMessage(string msg)
        {
            Messages.Add(msg);
        }

        private void AddMessage(string msg, params object[] args)
        {
            AddMessage(string.Format(msg, args));
        }

        private bool ParseSwitch(string arg)
        {
            var p = arg.Substring(1).ToLower();
            switch(p)
            {
                case "?":
                case "h":
                case "help":
                    ShowHelp = true;
                    return true;

                default:
                    AddMessage("Invalid argument: '{0}'", arg);
                    return false;
            }
        }
    }
}