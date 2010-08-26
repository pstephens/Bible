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
using System.Runtime.Serialization;

namespace Builder.Parser
{
    [Serializable]
    public class ParseException : Exception
    {
        public int LineNumber { get; private set; }

        public ParseException(int lineNumber, string message)
            : base(message)
        {
            LineNumber = lineNumber;
        }

        protected ParseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            LineNumber = info.GetInt32("LineNumber");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("LineNumber", LineNumber);
            base.GetObjectData(info, context);
        }
        
    }
}