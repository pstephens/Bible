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
using Builder.Services;
using NUnit.Framework;

namespace Builder.UnitTests.Services
{
    [TestFixture]
    public class TokenTests
    {
        [Test] 
        public void Implicit_cast_from_string_to_token_should_return_token()
        {
            const string val = "Hello";

            Token tok = val;

            Assert.That(tok.TokenString, Is.EqualTo(val));
        }

        [Test]
        public void Implicit_cast_from_token_to_string_should_return_string()
        {
            var token = new Token("text");

            string stringOfToken = token;

            Assert.That(stringOfToken, Is.EqualTo("text"));
        }

        [Test]
        public void GetHashCode_of_similar_token_should_return_same()
        {
            Token token1 = "some sort of token";
            Token token2 = "some sort of token";

            var hash1 = token1.GetHashCode();
            var hash2 = token2.GetHashCode();

            Assert.That(hash1, Is.EqualTo(hash2));
        }

        [Test]
        public void GetHashCode_of_different_tokens_should_return_different()
        {
            Token token1 = "some sort of token";
            Token token2 = "a different kind of token";

            var hash1 = token1.GetHashCode();
            var hash2 = token2.GetHashCode();

            Assert.That(hash1, Is.Not.EqualTo(hash2));
        }

        [Test]
        public void Equals_with_null_should_return_false()
        {
            Token token = "a token";

            Assert.That(token.Equals((IDisposable)null), Is.False);
        }

        [Test]
        public void Equals_with_a_non_token_should_return_false()
        {
            Token token = "a token";

            Assert.That(token.Equals(new Random()), Is.False);
        }

        [Test]
        public void Equals_with_different_token_should_return_false()
        {
            Token token = "a token";
            Token token2 = "a different token";

            Assert.That(token.Equals(token2), Is.False);
        }

        [Test]
        public void Equals_with_different_token_as_object_should_return_false()
        {
            Token token = "a token";
            object token2 = new Token("a different token");

            Assert.That(token.Equals(token2), Is.False);
        }

        [Test]
        public void Equals_with_similar_token_should_return_true()
        {
            Token token = "a token";
            Token token2 = "a token";

            Assert.That(token.Equals(token2), Is.True);
        }

        [Test]
        public void Equals_with_similar_token_as_object_should_return_true()
        {
            Token token = "a token";
            object token2 = (Token) "a token";

            Assert.That(token.Equals(token2), Is.True);
        }
    }
}