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
using System.Collections.Generic;
using System.Linq;
using Builder.Model;

namespace Builder.Services
{
    public class VerseTokens : IService<IVerse>
    {
        private IVerse verse;
        private Token[] tokens;

        public IVerse Related
        {
            get { return verse; }
            set { 
                verse = value;
                tokens = null;
            }
        }

        public IEnumerable<Token> Tokens()
        {
            if (tokens == null)
                BuildTokenList();

            return tokens;
        }

        private void BuildTokenList()
        {
            if(verse == null)
                throw new InvalidOperationException(
                    "The related verse must be set before retrieving tokens.");

            tokens = TokenizeString(verse.Text).ToArray();
        }

        private enum State
        {
            Word,
            NonWord
        }

        public static IEnumerable<Token> TokenizeString(string str)
        {
            var state = State.NonWord;
            var tokenStartPos = 0;
            for (var i = 0; i < str.Length; ++i)
            {
                switch(state)
                {
                    case State.NonWord:
                        if (IsWordChar(str[i]))
                        {
                            tokenStartPos = i;
                            state = State.Word;
                        }
                        else
                        {
                            yield return new Token { TokenString = str.Substring(i, 1)};
                            state = State.NonWord;
                        }
                        break;
                    case State.Word:
                        if (!IsWordChar(str[i]))
                        {
                            yield return new Token { IsWord = true, 
                                TokenString = str.Substring(tokenStartPos, i - tokenStartPos)};
                            yield return new Token { TokenString = str.Substring(i, 1)};
                            state = State.NonWord;
                        }
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }

            if(state == State.Word)
                yield return new Token { IsWord = true, TokenString = str.Substring(tokenStartPos)};
        }

        private static bool IsWordChar(char ch)
        {
            return (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || ch == '\'';
        }
    }
}