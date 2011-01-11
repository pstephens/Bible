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
using System.Text;
using BibleLib.Reader;
using NUnit.Framework;

namespace BibleLib.UnitTests.Reader
{
    [TestFixture]
    public class HeaderTests
    {
        private readonly Version ExpectedVersion = new Version(2, 0, 0, 0);

        [Test]
        public void Correctly_formatted_header_with_zero_records_should_read()
        {
            var wr = new BinaryWriter(new MemoryStream());
            var rd = new BinaryReader(wr.BaseStream);
            WriteAscii(wr, Header.ExpectedHeaderString);
            WriteVersion(wr, new Version(2, 1, 1, 1));
            wr.Write((byte)0);
            wr.Seek(0, SeekOrigin.Begin);

            var header = Header.ReadFrom(rd);

            Assert.That(header.FileVersion, Is.EqualTo(new Version(2, 1, 1, 1)));
            Assert.That(header.HeaderRecords.Count, Is.EqualTo(0));
        }

        [Test]
        public void File_that_is_not_at_least_8_bytes_should_throw()
        {
            var bytes = Encoding.UTF8.GetBytes("Almost8");
            var str = new MemoryStream(bytes);
            Assert.That(str.Position, Is.EqualTo(0));
            var rd = new BinaryReader(str);

            Assert.Throws<BibleFormatException>(() => Header.ReadFrom(rd));
        }

        [Test] 
        public void File_that_doesnt_start_with_right_file_tag_should_throw()
        {
            var wr = new BinaryWriter(new MemoryStream());
            var rd = new BinaryReader(wr.BaseStream);
            WriteAscii(wr, "NotRight");
            WriteVersion(wr, ExpectedVersion);
            wr.Write((byte) 0);
            wr.Seek(0, SeekOrigin.Begin);

            Assert.Throws<BibleFormatException>(() => Header.ReadFrom(rd));
        }

        [TestCase(0, true)]
        [TestCase(1, true)]
        [TestCase(2, false)]
        [TestCase(3, true)]
        public void File_that_doesnt_have_major_file_version_of_2_should_throw(
            int majorVersion, bool shouldThrow)
        {
            var wr = new BinaryWriter(new MemoryStream());
            var rd = new BinaryReader(wr.BaseStream);
            WriteAscii(wr, Header.ExpectedHeaderString);
            WriteVersion(wr, new Version(majorVersion, 0, 0, 0));
            wr.Write((byte) 0);
            wr.Seek(0, SeekOrigin.Begin);

            if (shouldThrow)
                Assert.Throws<BibleFormatException>(() => Header.ReadFrom(rd));
            else
                Assert.That(Header.ReadFrom(rd), Is.Not.Null);
        }

        [Test]
        public void File_that_doesnt_have_enought_bytes_for_version_should_throw()
        {
            var wr = new BinaryWriter(new MemoryStream());
            var rd = new BinaryReader(wr.BaseStream);
            WriteAscii(wr, Header.ExpectedHeaderString);
            wr.Write((byte) 2);
            wr.Write((byte) 0); // The real version data needs two more bytes.
            wr.Seek(0, SeekOrigin.Begin);

            Assert.Throws<BibleFormatException>(() => Header.ReadFrom(rd));
        }

        [Test]
        public void File_missing_header_table_count_byte_should_throw()
        {
            var wr = new BinaryWriter(new MemoryStream());
            var rd = new BinaryReader(wr.BaseStream);
            WriteAscii(wr, Header.ExpectedHeaderString);
            WriteVersion(wr, ExpectedVersion);
            // <-- Missing table count byte.
            wr.Seek(0, SeekOrigin.Begin);

            Assert.Throws<BibleFormatException>(() => Header.ReadFrom(rd));
        }

        [Test]
        public void File_with_1_header_records_should_parse()
        {
            var rec = new HeaderRec { 
                TableId = BibleTableId.Books, 
                Flags = HeaderFlags.None,
                Size = 25,
                CompressedSize = 25,
                StartByteIndex = 23
            };

            var wr = new BinaryWriter(new MemoryStream());
            var rd = new BinaryReader(wr.BaseStream);
            WriteAscii(wr, Header.ExpectedHeaderString);
            WriteVersion(wr, ExpectedVersion);
            wr.Write((byte) 1);
            WriteRecord(wr, ref rec);
            wr.Seek(0, SeekOrigin.Begin);

            var header = Header.ReadFrom(rd);

            Assert.That(header.HeaderRecords[0], Is.EqualTo(rec));
        }

        [Test]
        public void File_with_2_header_records_should_parse()
        {
            var rec1 = new HeaderRec
                           {
                               TableId = BibleTableId.Books,
                               Flags = HeaderFlags.None,
                               Size = 25,
                               CompressedSize = 20,
                               StartByteIndex = 33
                           };
            var rec2 = new HeaderRec
                           {
                               TableId = BibleTableId.Verses,
                               Flags = HeaderFlags.None,
                               Size = 50,
                               CompressedSize = 50,
                               StartByteIndex = 53
                           };
            var wr = new BinaryWriter(new MemoryStream());
            var rd = new BinaryReader(wr.BaseStream);
            WriteAscii(wr, Header.ExpectedHeaderString);
            WriteVersion(wr, ExpectedVersion);
            wr.Write((byte) 2);
            WriteRecord(wr, ref rec1);
            WriteRecord(wr, ref rec2);
            wr.Seek(0, SeekOrigin.Begin);

            var header = Header.ReadFrom(rd);

            Assert.That(header.HeaderRecords[0], Is.EqualTo(rec1));
            Assert.That(header.HeaderRecords[1], Is.EqualTo(rec2));
            Assert.That(header.HeaderRecords.Count, Is.EqualTo(2));
        }

        [Test]
        public void File_with_truncated_header_record_should_throw()
        {
            var wr = new BinaryWriter(new MemoryStream());
            var rd = new BinaryReader(wr.BaseStream);
            WriteAscii(wr, Header.ExpectedHeaderString);
            WriteVersion(wr, ExpectedVersion);
            wr.Write((byte)1);
            wr.Write((byte) 0);
            wr.Write((byte) 0); // Only a partial header record.
            wr.Seek(0, SeekOrigin.Begin);

            Assert.Throws<BibleFormatException>(() => Header.ReadFrom(rd));
        }

        [Test]
        public void File_with_invalid_header_record_should_throw()
        {
            var rec = new HeaderRec
                          {
                              TableId = BibleTableId.Books,
                              Flags = HeaderFlags.None,
                              Size = -5,
                              CompressedSize = 20,
                              StartByteIndex = 23
                          };

            var wr = new BinaryWriter(new MemoryStream());
            var rd = new BinaryReader(wr.BaseStream);
            WriteAscii(wr, Header.ExpectedHeaderString);
            WriteVersion(wr, ExpectedVersion);
            wr.Write((byte)1);
            WriteRecord(wr, ref rec);
            wr.Seek(0, SeekOrigin.Begin);

            Assert.Throws<BibleFormatException>(() => Header.ReadFrom(rd));
        }

        private static void WriteRecord(BinaryWriter wr, ref HeaderRec rec)
        {
            wr.Write((byte) rec.TableId);
            wr.Write((byte) rec.Flags);
            wr.Write(rec.Size);
            wr.Write(rec.CompressedSize);
        }

        private static void WriteAscii(BinaryWriter wr, string stringToWrite)
        {
            var bytes = Encoding.UTF8.GetBytes(stringToWrite);
            wr.Write(bytes, 0, bytes.Length);
        }

        private static void WriteVersion(BinaryWriter wr, Version ver)
        {
            wr.Write((byte) ver.Major);
            wr.Write((byte) ver.Minor);
            wr.Write((byte) ver.Build);
            wr.Write((byte) ver.Revision);
        }
    }
}