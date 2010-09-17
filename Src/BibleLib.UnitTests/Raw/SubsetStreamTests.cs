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
using BibleLib.Raw;
using NUnit.Framework;
using Rhino.Mocks;

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
            str.Seek(0, SeekOrigin.Begin);
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

        [Test]
        public void Constructor_input_stream_must_not_be_null()
        {
            Assert.Throws<ArgumentNullException>(() => new SubsetStream(null, 0, 10));
        }

        [TestCase(true)]
        [TestCase(false, ExpectedException=typeof(InvalidOperationException))]
        public void Exercise_Constructor_with_input_stream_with_various_CanRead(bool canRead)
        {
            var baseStream = MockRepository.GenerateStub<Stream>();
            baseStream.Stub(bs => bs.CanSeek).Return(true);
            baseStream.Stub(bs => bs.CanRead).Return(canRead);

            new SubsetStream(baseStream, 10, 100);
        }

        [TestCase(true)]
        [TestCase(false, ExpectedException = typeof(InvalidOperationException))]
        public void Exercise_Constructor_with_input_stream_with_various_CanSeek(bool canSeek)
        {
            var baseStream = MockRepository.GenerateStub<Stream>();
            baseStream.Stub(bs => bs.CanRead).Return(true);
            baseStream.Stub(bs => bs.CanSeek).Return(canSeek);

            new SubsetStream(baseStream, 10, 100);
        }

        [Test]
        public void Length_should_return_between_zero_and_length()
        {
            var baseStream = CreateStreamWithPattern();
            var stream = new SubsetStream(baseStream, 5, 20);

            Assert.That(stream.Length, Is.EqualTo(20));
        }

        [TestCase(10, 20, Result = 20)]
        [TestCase(990, 20, Result = 10)]
        [TestCase(1000, 20, Result = 0)]
        [TestCase(1200, 20, Result = 0)]
        [TestCase(0, 1200, Result = 1000)]
        [TestCase(-5, 20, ExpectedException = typeof(ArgumentException))]
        [TestCase(0, -5, ExpectedException = typeof(ArgumentException))]
        [TestCase(0, 0, Result = 0)]
        public long Exercise_constructor_with_various_start_and_length(int start, int length)
        {
            var baseStream = CreateStreamWithPattern();
            var stream = new SubsetStream(baseStream, start, length);
            return stream.Length;
        }

        [TestCase(20, Result = 20)]
        [TestCase(0, Result = 0)]
        [TestCase(1000, Result = 1000)]
        [TestCase(1100, Result = 1100)]
        public long BaseStream_Position_should_be_equal_to_physical_start_of_stream(int start)
        {
            var baseStream = CreateStreamWithPattern();
            new SubsetStream(baseStream, start, 100);
            return baseStream.Position;
        }

        [TestCase(10, 20)]
        [TestCase(0, 20)]
        [TestCase(1100, 50)]
        public void Position_should_be_zero_on_newly_constructed_SubsetStream(int start, int length)
        {
            var baseStream = CreateStreamWithPattern();
            var stream = new SubsetStream(baseStream, start, length);
            Assert.That(stream.Position, Is.EqualTo(0));
        }

        [TestCase(-5, ExpectedException = typeof(ArgumentException))]
        [TestCase(0, Result = 0)]
        [TestCase(500, Result = 500)]
        [TestCase(1000, Result = 1000)]
        [TestCase(1100, Result = 1100)]
        public long Exercise_setting_Position(long setPositionTo)
        {
            var baseStream = CreateStreamWithPattern();
            var stream = new SubsetStream(baseStream, 10, 500);

            stream.Position = setPositionTo;

            return stream.Position;
        }

        [Test]
        public void Flush_should_not_fail()
        {
            var baseStream = new MemoryStream();
            var stream = new SubsetStream(baseStream, 10, 20);

            stream.Flush();
        }

        [Test]
        public void BaseStream_should_be_disposed_when_leaveOpen_is_false()
        {
            var baseStream = MockRepository.GenerateStub<Stream>();
            baseStream.Stub(bs => bs.CanRead).Return(true);
            baseStream.Stub(bs => bs.CanSeek).Return(true);
            using (new SubsetStream(baseStream, 10, 20))
            {
            }
            
            baseStream.AssertWasCalled(bs => bs.Dispose());
        }

        [Test]
        public void BaseStream_should_NOT_be_disposed_when_leaveOpen_is_true()
        {
            var baseStream = MockRepository.GenerateStub<Stream>();
            baseStream.Stub(bs => bs.CanRead).Return(true);
            baseStream.Stub(bs => bs.CanSeek).Return(true);
            using (new SubsetStream(baseStream, 10, 20, true))
            {
            }

            baseStream.AssertWasNotCalled(bs => bs.Dispose());
        }

        
    }
}