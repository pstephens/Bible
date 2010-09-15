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

using System.IO;
using BibleLib.Raw;
using NUnit.Framework;

namespace BibleLib.UnitTests.Raw
{
    [TestFixture]
    public class SubsetStreamTests
    {
        private static Stream CreateStreamWithPattern()
        {
            var str = new MemoryStream();
            var bytes = new byte[10];
            for (var i = (byte)0; i < bytes.Length; ++i) 
                bytes[i] = i;
            for(var j = 0; j < 100; ++j)
                str.Write(bytes, 0, bytes.Length);
            return str;
        }

        private static SubsetStream CreateGenericSubsetStream()
        {
            var baseStream = CreateStreamWithPattern();
            return new SubsetStream(baseStream, 0, 2000);
        }

        [Test]
        public void CanWrite_should_return_false()
        {
            var stream = CreateGenericSubsetStream();

            Assert.That(stream.CanWrite, Is.False);
        }

        [Test]
        public void CanRead_should_return_true()
        {
            var stream = CreateGenericSubsetStream();

            Assert.That(stream.CanRead, Is.True);
        }

        [Test]
        public void CanSeek_should_return_false()
        {
            var stream = CreateGenericSubsetStream();

            Assert.That(stream.CanSeek, Is.True);
        }
    }
}