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
using BibleLib.Reader;
using NUnit.Framework;

namespace BibleLib.UnitTests.Reader
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
    }
}