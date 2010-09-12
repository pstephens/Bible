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
using BibleLib.Raw;
using NUnit.Framework;

namespace BibleLib.UnitTests.Raw
{
    [TestFixture]
    public class BibleFormatExceptionTests
    {
        [Test]
        public void Constructor_should_create_new_exception_object()
        {
            var ex = new BibleFormatException("Message");

            Assert.That(ex.Message, Is.EqualTo("Message"));
            Assert.That(ex.InnerException, Is.Null);
        }

        [Test]
        public void BibleFormatException_should_serialize_and_deserialize()
        {
            var ex = new BibleFormatException("Something");
            var formatter = new BinaryFormatter();
            var stream = new MemoryStream();

            formatter.Serialize(stream, ex);
            stream.Seek(0, SeekOrigin.Begin);
            var anotherException = (BibleFormatException) formatter.Deserialize(stream);

            Assert.That(anotherException.Message, Is.EqualTo("Something"));
        }
    }
}