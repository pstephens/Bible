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
