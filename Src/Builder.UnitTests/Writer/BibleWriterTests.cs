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
using BibleLib.Reader;
using Builder.Model;
using Builder.UnitTests.HandMocks;
using Builder.Writer;
using NUnit.Framework;
using Rhino.Mocks;

namespace Builder.UnitTests.Writer
{
    [TestFixture]
    public class BibleWriterTests
    {
        private static IBibleWriter CreateWriterUnderTest()
        {
            return new BibleWriter();
        }

        private static IEnumerable<IBibleTableWriter> CreateTables(int count)
        {
            return
                Enumerable.Range(0, count)
                .Select(i =>
                            {
                                var ti = MockRepository.GenerateStub<IBibleTableInfo>();
                                ti.Stub(t => t.DataStream).Return(new MemoryStream());
                                var tw = MockRepository.GenerateStub<IBibleTableWriter>();
                                tw.Stub(t => t.BuildTable(Arg<IBible>.Is.Anything)).Return(ti);
                                return tw;
                            });
        }

        [Test]
        public void Write_with_null_output_should_throw()
        {
            var writer = CreateWriterUnderTest();
            var bible = new BibleStub();

            Assert.Throws<ArgumentNullException>(() => writer.Write(null, bible, null));
        }

        [Test]
        public void Write_with_null_bible_should_throw()
        {
            var writer = CreateWriterUnderTest();
            var binaryWriter = new BinaryWriter(new MemoryStream());

            Assert.Throws<ArgumentNullException>(() => writer.Write(binaryWriter, null, null));
        }

        [Test]
        public void Write_with_no_tables_should_write_simple_header()
        {
            var writer = CreateWriterUnderTest();
            var binaryWriter = new BinaryWriter(new MemoryStream());
            var binaryReader = new BinaryReader(binaryWriter.BaseStream);
            var bible = new BibleStub();

            writer.Write(binaryWriter, bible, null);

            binaryWriter.Seek(0, SeekOrigin.Begin);
            var header = Header.ReadFrom(binaryReader);

            Assert.That(header.FileVersion, Is.EqualTo(BibleWriter.FileVersion));
            Assert.That(header.HeaderRecords.Count, Is.EqualTo(0));
        }

        [Test]
        public void Write_with_more_than_255_tables_should_throw()
        {
            var writer = CreateWriterUnderTest();
            var tables = CreateTables(300);
            var binaryWriter = new BinaryWriter(new MemoryStream());
            var bible = new BibleStub();

            Assert.Throws<InvalidOperationException>(() => writer.Write(binaryWriter, bible, tables));
        }

        [Test]
        public void Write_with_single_table_should_write_header_and_table_data()
        {
            var bible = new BibleStub();
            var writer = CreateWriterUnderTest();
            var tableData = CreateByteArray('a', 2500);
            var tables = new [] {CreateBibleTableWriter(bible, tableData)};
            var binaryWriter = new BinaryWriter(new MemoryStream());
            var binaryReader = new BinaryReader(binaryWriter.BaseStream);

            writer.Write(binaryWriter, bible, tables);

            binaryWriter.Seek(0, SeekOrigin.Begin);
            var header = Header.ReadFrom(binaryReader);

            Assert.Ignore("TODO");
        }

        private static byte[] CreateByteArray(char ch, int count)
        {
            return Enumerable.Range(0, count).Select(i => (byte) ch).ToArray();
        }

        private static IBibleTableWriter CreateBibleTableWriter(IBible bible, byte[] data)
        {
            var stream = new MemoryStream(data, false);

            var tableInfo = MockRepository.GenerateStub<IBibleTableInfo>();
            tableInfo.Stub(ti => ti.Flags).Return(HeaderFlags.None);
            tableInfo.Stub(ti => ti.DataStream).Return(stream);

            var tableWriter = MockRepository.GenerateStub<IBibleTableWriter>();
            tableWriter.Stub(tw => tw.TableId).Return(BibleTableId.Verses);
            tableWriter.Stub(tw => tw.BuildTable(bible)).Return(tableInfo);

            return tableWriter;
        }
    }
}