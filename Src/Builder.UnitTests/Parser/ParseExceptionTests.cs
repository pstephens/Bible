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
using System.Runtime.Serialization.Formatters.Binary;
using Builder.Parser;
using NUnit.Framework;

namespace Builder.UnitTests.Parser
{
    [TestFixture]
    public class ParseExceptionTests
    {
        [Test]
        public void Test_constructor()
        {
            var except = new ParseException(25, "The message");

            Assert.That(except.LineNumber, Is.EqualTo(25));
            Assert.That(except.Message, Is.EqualTo("The message"));
        }

        [Test]
        public void Test_serialization()
        {
            var except = new ParseException(25, "The message");
            var formatter = new BinaryFormatter();
            var stream = new MemoryStream();

            formatter.Serialize(stream, except);
            stream.Seek(0, SeekOrigin.Begin);
            var deserialized = (ParseException)formatter.Deserialize(stream);

            Assert.That(deserialized, Is.Not.SameAs(except));
            Assert.That(deserialized.LineNumber, Is.EqualTo(25));
            Assert.That(deserialized.Message, Is.EqualTo("The message"));
        }
    }
}