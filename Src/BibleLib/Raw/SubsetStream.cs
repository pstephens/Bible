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

namespace BibleLib.Raw
{
    public class SubsetStream : Stream
    {
        private readonly Stream baseStream;
        private readonly long start;
        private readonly long length;

        public SubsetStream(Stream baseStream, long start, long length, 
            bool leaveOpen = false)
        {
            if(baseStream == null)
                throw new ArgumentNullException("baseStream");
            if(start < 0)
                throw new ArgumentException("'start' must be zero or greater.", "start");
            if(length < 0)
                throw new ArgumentException("'length' must be zero or greater.", "length");
            if (!baseStream.CanRead || !baseStream.CanSeek)
                throw new InvalidOperationException("baseStream must be readable.");

            this.baseStream = baseStream;
            this.start = start;
            this.length = length;

            baseStream.Seek(start, SeekOrigin.Begin);
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override long Length
        {
            get { return Math.Min(length, Math.Max(0, baseStream.Length - start)); }
        }

        public override long Position
        {
            get { return 0; }
            set { throw new NotImplementedException(); }
        }
    }
}