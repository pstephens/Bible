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
            var processedTables = ProcessTables(bible, tables);

            OutputFileType(output);
            OutputFileVersion(output);
            OutputRecordCount(output, processedTables.Count());

            OutputHeaderRecs(output, processedTables);
            OutputTableData(output, processedTables);
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
            if(tableCount > 255)
                throw new InvalidOperationException("Only 255 tables allowed per file.");
            output.Write((byte) tableCount);
        }

        private static IEnumerable<TableData> ProcessTables(IBible bible, IEnumerable<IBibleTableWriter> tables)
        {
            return
                (from table in tables
                 let tableInfo = table.BuildTable(bible)
                 let data = ReadStreamData(tableInfo.DataStream)
                 select new TableData
                            {
                                Id = table.TableId,
                                Flags = tableInfo.Flags,
                                Data = data,
                                StoredData = data
                            })
                    .ToArray();
        }

        private static byte[] ReadStreamData(Stream dataStream)
        {
            var len = (int) dataStream.Length;
            dataStream.Seek(0, SeekOrigin.Begin);
            var bytes = new byte[len];
            dataStream.Read(bytes, 0, len);
            return bytes;
        }

        private static void OutputHeaderRecs(BinaryWriter writer, IEnumerable<TableData> processedTables)
        {
            foreach (var table in processedTables)
            {
                writer.Write((byte) table.Id);
                writer.Write((byte) table.Flags);
                writer.Write(table.Data.Length);
                writer.Write(table.StoredData.Length);
            }
        }

        private static void OutputTableData(BinaryWriter writer, IEnumerable<TableData> processedTables)
        {
            foreach (var table in processedTables)
            {
                writer.Write(table.StoredData);
            }
        }

        private class TableData
        {
            public BibleTableId Id;
            public HeaderFlags Flags;
            public Byte[] Data;
            public Byte[] StoredData;
        }
    }
}