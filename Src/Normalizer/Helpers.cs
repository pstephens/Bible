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

namespace Normalize
{
    public class TempBuff
    {
        private readonly Byte[] _buff;
        private Int32 _pos;
        
        public Int32 Length
        {
            get { return _pos; }
        }
        
        public TempBuff()
        {
            _buff = new Byte[1024];
            _pos = 0;
        }
        
        public void Reset()
        {
            _pos = 0;
        }
        
        public void WriteToStream(Stream s)
        {
            s.Write(_buff, 0, _pos);
        }
        
        public void WriteByte(Byte b)
        {
            _buff[_pos++] = b;
        }
    }
}
