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

namespace Builder.Services
{
    public struct Token
    {
        private readonly string token;

        public Token(string token) : this()
        {
            this.token = token;
        }

        public static implicit operator Token(string token)
        {
            return new Token(token);
        }

        public static implicit operator string(Token token)
        {
            return token.TokenString;
        }

        public bool Equals(Token other)
        {
            return Equals(other.token, token);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof (Token)) return false;
            return Equals((Token) obj);
        }

        public override int GetHashCode()
        {
            return token.GetHashCode();
        }

        public string TokenString
        {
            get { return token; }
        }

        public bool IsWord
        {
            get { return VerseTokens.IsWordChar(TokenString[0]); }
        }
    }
}