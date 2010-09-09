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
using BibleLib.Raw;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace BibleLib.UnitTests.Raw
{
    [TestFixture]
    public class HeaderTests
    {
        [Test]
        public void Correctly_formatted_header_with_zero_records_should_read()
        {
            var str = new MemoryStream();
            WriteAscii(str, Header.ExpectedHeaderString);
            WriteVersion(str, new Version(2, 1, 1, 1));
            str.WriteByte(0);
            str.Seek(0, SeekOrigin.Begin);

            var header = Header.ReadFrom(str);

            Assert.That(header.FileVersion, Is.EqualTo(new Version(2, 1, 1, 1)));
            Assert.That(header.HeaderRecords.Count, Is.EqualTo(0));
        }

        [Test]
        public void File_that_is_not_at_least_8_bytes_should_throw()
        {
            var bytes = Encoding.ASCII.GetBytes("Almost8");
            var str = new MemoryStream(bytes);
            Assert.That(str.Position, Is.EqualTo(0));

            Assert.Throws<BibleFormatException>(() => Header.ReadFrom(str));
        }

        [Test] 
        public void File_that_doesnt_start_with_right_file_tag_should_throw()
        {
            var str = new MemoryStream();
            WriteAscii(str, "NotRight");
            WriteVersion(str, new Version(2, 0, 0, 0));
            str.WriteByte(0);
            str.Seek(0, SeekOrigin.Begin);

            Assert.Throws<BibleFormatException>(() => Header.ReadFrom(str));
        }

        [Test]
        public void File_that_doesnt_have_proper_version_bytes_should_throw()
        {
            Assert.Ignore();
        }

        [TestCase(0, true)]
        [TestCase(1, true)]
        [TestCase(2, false)]
        [TestCase(3, true)]
        public void File_that_doesnt_have_major_file_version_of_2_should_throw(
            int majorVersion, bool shouldThrow)
        {
            var str = new MemoryStream();
            WriteAscii(str, Header.ExpectedHeaderString);
            WriteVersion(str, new Version(majorVersion, 0, 0, 0));
            str.WriteByte(0);
            str.Seek(0, SeekOrigin.Begin);

            if (shouldThrow)
                Assert.Throws<BibleFormatException>(() => Header.ReadFrom(str));
            else
                Assert.That(Header.ReadFrom(str), Is.Not.Null);
        }

        private static void WriteAscii(Stream str, string stringToWrite)
        {
            var bytes = Encoding.ASCII.GetBytes(stringToWrite);
            str.Write(bytes, 0, bytes.Length);
        }

        private static void WriteVersion(Stream str, Version ver)
        {
            str.WriteByte((byte) ver.Major);
            str.WriteByte((byte) ver.Minor);
            str.WriteByte((byte) ver.Build);
            str.WriteByte((byte) ver.Revision);
        }
    }
}