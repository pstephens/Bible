#region Copyright Notice

/* Copyright 2009-2011 Peter Stephens

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
using System.Linq;
using NUnit.Framework;

namespace FontMetricCollector.UnitTests
{
    [TestFixture]
    public class CommandLineOptionsTests
    {
        [TestCase(0, false)]
        [TestCase(1, false)]
        [TestCase(2, true)]
        [TestCase(3, false)]
        public void Parse_should_require_exactly_2_arguments(int argumentCount, bool shouldBeValid)
        {
            var arguments = Enumerable.Repeat("validArg", argumentCount).ToArray();

            var options = CommandLineOptions.Parse(arguments);

            Assert.That(options.IsValid, Is.EqualTo(shouldBeValid));
        }
        
        [Test]
        public void Parse_with_null_args_should_throw()
        {
            Assert.Throws<ArgumentNullException>(() => CommandLineOptions.Parse(null));
        }

        [Test]
        public void Options_should_have_message_when_invalid()
        {
            var options = CommandLineOptions.Parse(new string[0]);

            Assert.That(options.GetOutputMessage().Count(), Is.GreaterThan(0));
        }

        [Test]
        public void Options_should_have_no_message_when_valid()
        {
            var args = Enumerable.Repeat("asdf", 2).ToArray();

            var options = CommandLineOptions.Parse(args);

            Assert.That(options.GetOutputMessage().Count(), Is.EqualTo(0));
        }
    }
}