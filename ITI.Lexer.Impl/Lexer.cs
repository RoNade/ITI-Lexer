using System;
using System.Text;
using System.Collections.Generic;

namespace ITI.Lexer
{
    /**
     * The lexer or "Tokenizer" is the first step of the compilation
     * process. Its job is to convert the input into a stream of tokens 
     * representing the smallest individual unit of code in the program.
     *
     * A lexer is a deterministic finite automaton, it takes a finite
     * string of characters and outputs a deterministic (invariant) result 
     * in the same way that "2 + 3" always outputs "5". For this reason, you 
     * must ensure that for a given input, the output token is always the same.
     */
    public sealed class Lexer
    {
        private char _current => _sourceCode[_index];
        private char _last => Peek(-1);
        private char _next => Peek(1);

        private readonly ErrorSink _errorSink;

        private static readonly string[] _keywords =
        {
            "class", "func", "prop", "ctor", "new", "if", "else", "switch", "case", "default",
            "break", "return", "do", "while", "for", "var", "null", "true", "false"
        };

        private SourceLocation _tokenStart;
        private StringBuilder _builder;
        private SourceCode _sourceCode;
        private int _column;
        private int _index;
        private int _line;

        public Lexer() : this(new ErrorSink())
        { }

        public Lexer(ErrorSink errorSink)
        {
            _builder = new StringBuilder();
            _errorSink = errorSink;
            _sourceCode = null;
        }

        public ErrorSink ErrorSink => _errorSink;

        public IEnumerable<Token> LexFile(string sourceCode)
        {
            return LexFile(new SourceCode(sourceCode));
        }

        public IEnumerable<Token> LexFile(SourceCode sourceCode)
        {
            _sourceCode = sourceCode;
            _builder.Clear();
            _column = 0;
            _index = 0;
            _line = 1;

            CreateToken(TokenKind.EndOfFile);

            return LexContent();
        }

        private void AddError(string message, Severity severity)
        {
            var span = new SourceSpan(_tokenStart, new SourceLocation(_index, _line, _column));
            _errorSink.AddError(message, _sourceCode, severity, span);
        }

        private void Advance()
        {
            throw new NotImplementedException();
        }

        private void Consume()
        {
            throw new NotImplementedException();
        }

        private Token CreateToken(TokenKind kind)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<Token> LexContent()
        {
            while (!IsEOF())
            {
                yield return LexToken();
            }

            yield return CreateToken(TokenKind.EndOfFile);
        }

        private Token LexToken()
        {
            if (IsEOF())
            {
                return CreateToken(TokenKind.EndOfFile);
            }
            else if (IsNewLine())
            {
                return ScanNewLine();
            }
            else if (IsWhiteSpace())
            {
                return ScanWhiteSpace();
            }
            else if (IsDigit())
            {
                return ScanInteger();
            }
            else if (_current == '/' && (_next == '/' || _next == '*'))
            {
                return ScanComment();
            }
            else if (IsLetter() || _current == '_')
            {
                return ScanIdentifier();
            }
            else if (_current == '"')
            {
                return ScanStringLiteral();
            }
            else if (_current == '.' && char.IsDigit(_next))
            {
                return ScanFloat();
            }
            else if (IsPunctuation())
            {
                return ScanPunctuation();
            }
            else
            {
                return ScanWord();
            }
        }

        private void DoNewLine()
        {
            throw new NotImplementedException();
        }

        private bool IsDigit()
        {
            throw new NotImplementedException();
        }

        private bool IsEOF()
        {
            throw new NotImplementedException();
        }

        private bool IsIdentifier()
        {
            throw new NotImplementedException();
        }

        private bool IsKeyword()
        {
            throw new NotImplementedException();
        }

        private bool IsLetter()
        {
            throw new NotImplementedException();
        }

        private bool IsLetterOrDigit()
        {
            throw new NotImplementedException();
        }

        private bool IsNewLine()
        {
            throw new NotImplementedException();
        }

        private bool IsPunctuation()
        {
            throw new NotImplementedException();
        }

        private bool IsWhiteSpace()
        {
            throw new NotImplementedException();
        }

        private Token ScanBlockComment()
        {
            throw new NotImplementedException();
        }

        private Token ScanComment()
        {
            throw new NotImplementedException();
        }

        private Token ScanFloat()
        {
            throw new NotImplementedException();
        }

        private Token ScanIdentifier()
        {
            throw new NotImplementedException();
        }

        private Token ScanInteger()
        {
            throw new NotImplementedException();
        }

        private Token ScanNewLine()
        {
            throw new NotImplementedException();
        }

        private Token ScanPunctuation()
        {
            throw new NotImplementedException();
        }

        private Token ScanStringLiteral()
        {
            throw new NotImplementedException();
        }

        private Token ScanWhiteSpace()
        {
            throw new NotImplementedException();
        }

        private Token ScanWord(Severity severity = Severity.Error, string message = "Unexpected Token '{0}'")
        {
            while (!IsWhiteSpace() && !IsEOF() && !IsPunctuation())
            {
                Consume();
            }

            AddError(string.Format(message, _builder), severity);
            return CreateToken(TokenKind.Error);
        }

        public char Peek(int ahead)
        {
            throw new NotImplementedException();
        }
    }
}
