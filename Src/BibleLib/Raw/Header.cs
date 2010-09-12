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
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BibleLib.Raw
{
    public class Header
    {
        public const int HeaderSize = 8 + 4 + 1;
        public const string ExpectedHeaderString = "AvBible-";

        public Version FileVersion { get; private set; }

        public IList<HeaderRec> HeaderRecords { get; private set; }

        private Header(BinaryReader input)
        {
            var buff = new byte[16];
            if (input.Read(buff, 0, ExpectedHeaderString.Length) != ExpectedHeaderString.Length)
                throw BibleFormatException("Not a Bible file.");

            var headerString = Encoding.ASCII.GetString(buff, 0, ExpectedHeaderString.Length);
            if (!headerString.Equals(ExpectedHeaderString, StringComparison.Ordinal))
                throw BibleFormatException("Not a Bible file.");

            if (input.Read(buff, 0, 4) != 4)
                throw BibleFormatException("Invalid Bible file version.");

            FileVersion = new Version(buff[0], buff[1], buff[2], buff[3]);
            if (FileVersion.Major != 2)
                throw BibleFormatException("Major Bible file version must be equal to 2. Instead it is {0}.",
                                           FileVersion);
            
            if(input.Read(buff, 0, 1) != 1)
                throw BibleFormatException("Invalid file header.");
            var recCount = buff[0];

            LoadHeaderRecords(input, recCount);
        }

        private void LoadHeaderRecords(BinaryReader input, int recCount)
        {
            var headerRecords = new List<HeaderRec>(recCount);
            HeaderRecords = headerRecords.AsReadOnly();
            try
            {
                var startIndex = HeaderSize + recCount*HeaderRec.RecSize;
                for (var i = 0; i < recCount; ++i)
                {
                    var rec = HeaderRec.ReadFrom(input, startIndex);
                    startIndex += rec.CompressedSize;
                    headerRecords.Add(rec);
                }
            }
            catch(Exception ex)
            {
                if (ex is BibleFormatException)
                    throw;
                throw BibleFormatException("Failed to read file header records.", ex);
            }
        }

        private static Exception BibleFormatException(string message, params object[] args)
        {
            return BibleFormatException(message, null, args);
        }

        private static Exception BibleFormatException(string message, Exception innerException, 
            params object[] args)
        {
            return new BibleFormatException(string.Format(message, args), innerException);
        }

        public static Header ReadFrom(BinaryReader input)
        {
            return new Header(input);
        }
    }
}