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
using System.Linq;
using System.Text;
using BibleLib.Raw;
using Builder.Model;

namespace Builder.Writer
{
    public class BibleWriter : IBibleWriter
    {
        public static readonly Version FileVersion = new Version(2, 0, 0, 0);

        public void Write(BinaryWriter output, IBible bible, IEnumerable<IBibleTableWriter> tables)
        {
            if(output == null)
                throw new ArgumentNullException("output");
            if(bible == null)
                throw new ArgumentNullException("bible");
            tables = tables ?? Enumerable.Empty<IBibleTableWriter>();

            OutputFileType(output);
            OutputFileVersion(output);
            OutputRecordCount(output, tables.Count());
        }

        private static void OutputFileType(BinaryWriter output)
        {
            var bytes = Encoding.ASCII.GetBytes(Header.ExpectedHeaderString);
            output.Write(bytes, 0, bytes.Length);
        }

        private static void OutputFileVersion(BinaryWriter output)
        {
            output.Write((byte) FileVersion.Major);
            output.Write((byte) FileVersion.Minor);
            output.Write((byte) FileVersion.Revision);
            output.Write((byte) FileVersion.Build);
        }

        private static void OutputRecordCount(BinaryWriter output, int tableCount)
        {
            output.Write((byte) tableCount);
        }
    }
}