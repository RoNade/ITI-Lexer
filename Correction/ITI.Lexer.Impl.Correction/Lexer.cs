using System;
using System.Linq;
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
    [Serializable]
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
            _index++;
            _column++;
        }

        private void Consume()
        {
            _builder.Append(_current);
            Advance();
        }

        private Token CreateToken(TokenKind kind)
        {
            string content = _builder.ToString();
            SourceLocation end = new SourceLocation(_index, _line, _column);
            SourceLocation start = _tokenStart;

            _tokenStart = end;
            _builder.Clear();

            return new Token(kind, content, start, end);
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
            _line++;
            _column = 0;
        }

        private bool IsDigit()
        {
            return char.IsDigit(_current);
        }

        private bool IsEOF()
        {
            return _current == '\0';
        }

        private bool IsIdentifier()
        {
            return IsLetterOrDigit() || _current == '_';
        }

        private bool IsKeyword()
        {
            return _keywords.Contains(_builder.ToString());
        }

        private bool IsLetter()
        {
            return char.IsLetter(_current);
        }

        private bool IsLetterOrDigit()
        {
            return char.IsLetterOrDigit(_current);
        }

        private bool IsNewLine()
        {
            return _current == '\n';
        }

        private bool IsPunctuation()
        {
            return "<>{}()[]!%^&*+-=/.,?;:|".Contains(_current);
        }

        private bool IsWhiteSpace()
        {
            return (char.IsWhiteSpace(_current) || IsEOF()) && !IsNewLine();
        }

        private Token ScanBlockComment()
        {
            Func<bool> isEndOfComment = () => _current == '*' && _next == '/';

            while (!isEndOfComment())
            {
                if (IsEOF())
                {
                    return CreateToken(TokenKind.Error);
                }

                if (IsNewLine())
                {
                    DoNewLine();
                }

                Consume();
            }

            Consume();
            Consume();

            return CreateToken(TokenKind.BlockComment);
        }

        private Token ScanComment()
        {
            Consume();

            if (_current == '*')
            {
                return ScanBlockComment();
            }

            Consume();

            while (!IsNewLine() && !IsEOF())
            {
                Consume();
            }

            return CreateToken(TokenKind.LineComment);
        }

        private Token ScanFloat()
        {
            if (_current == 'f')
            {
                Advance();

                if ((!IsWhiteSpace() && !IsPunctuation() && !IsEOF()) || _current == '.')
                {
                    return ScanWord(message: "Remove 'f' in floating point number.");
                }

                return CreateToken(TokenKind.FloatLiteral);
            }

            int preDotLength = _index - _tokenStart.Index;

            if (_current == '.')
            {
                Consume();
            }

            while (IsDigit())
            {
                Consume();
            }

            if (_last == '.')
            {
                return ScanWord(message: "Must contain digits after '.'.");
            }

            if (_current == 'e')
            {
                Consume();

                if (preDotLength > 1)
                {
                    return ScanWord(message: "Coefficient must be less than 10.");
                }

                if (_current == '+' || _current == '-')
                {
                    Consume();
                }

                while (IsDigit())
                {
                    Consume();
                }
            }

            if (_current == 'f')
            {
                Consume();
            }

            if (!IsWhiteSpace() && !IsPunctuation() && !IsEOF())
            {
                if (IsLetter())
                {
                    return ScanWord(message: "'{0}' is an invalid float value.");
                }

                return ScanWord();
            }

            return CreateToken(TokenKind.FloatLiteral);
        }

        private Token ScanIdentifier()
        {
            while (IsIdentifier())
            {
                Consume();
            }

            if (!IsWhiteSpace() && !IsPunctuation() && !IsEOF())
            {
                return ScanWord();
            }

            if (IsKeyword())
            {
                return CreateToken(TokenKind.Keyword);
            }

            return CreateToken(TokenKind.Identifier);
        }

        private Token ScanInteger()
        {
            while (IsDigit())
            {
                Consume();
            }

            if (_current == 'f' || _current == '.' || _current == 'e')
            {
                return ScanFloat();
            }

            if (!IsWhiteSpace() && !IsPunctuation() && !IsEOF())
            {
                return ScanFloat();
            }

            return CreateToken(TokenKind.IntegerLiteral);
        }

        private Token ScanNewLine()
        {
            Consume();
            DoNewLine();

            return CreateToken(TokenKind.NewLine);
        }

        private Token ScanPunctuation()
        {
            switch (_current)
            {
                case ';':
                    Consume();
                    return CreateToken(TokenKind.Semicolon);

                case ':':
                    Consume();
                    return CreateToken(TokenKind.Colon);

                case '{':
                    Consume();
                    return CreateToken(TokenKind.LeftBracket);

                case '}':
                    Consume();
                    return CreateToken(TokenKind.RightBracket);

                case '[':
                    Consume();
                    return CreateToken(TokenKind.LeftBrace);

                case ']':
                    Consume();
                    return CreateToken(TokenKind.RightBrace);

                case '(':
                    Consume();
                    return CreateToken(TokenKind.LeftParenthesis);

                case ')':
                    Consume();
                    return CreateToken(TokenKind.RightParenthesis);

                case '>':
                    Consume();
                    if (_current == '=')
                    {
                        Consume();
                        return CreateToken(TokenKind.GreaterThanOrEqual);
                    }
                    else if (_current == '>')
                    {
                        Consume();
                        return CreateToken(TokenKind.BitShiftRight);
                    }
                    return CreateToken(TokenKind.GreaterThan);

                case '<':
                    Consume();
                    if (_current == '=')
                    {
                        Consume();
                        return CreateToken(TokenKind.LessThanOrEqual);
                    }
                    else if (_current == '<')
                    {
                        Consume();
                        return CreateToken(TokenKind.BitShiftLeft);
                    }
                    return CreateToken(TokenKind.LessThan);

                case '+':
                    Consume();
                    if (_current == '=')
                    {
                        Consume();
                        return CreateToken(TokenKind.PlusEqual);
                    }
                    else if (_current == '+')
                    {
                        Consume();
                        return CreateToken(TokenKind.PlusPlus);
                    }
                    return CreateToken(TokenKind.Plus);

                case '-':
                    Consume();
                    if (_current == '=')
                    {
                        Consume();
                        return CreateToken(TokenKind.MinusEqual);
                    }
                    else if (_current == '>')
                    {
                        Consume();
                        return CreateToken(TokenKind.Arrow);
                    }
                    else if (_current == '-')
                    {
                        Consume();
                        return CreateToken(TokenKind.MinusMinus);
                    }
                    return CreateToken(TokenKind.Minus);

                case '=':
                    Consume();
                    if (_current == '=')
                    {
                        Consume();
                        return CreateToken(TokenKind.Equal);
                    }
                    else if (_current == '>')
                    {
                        Consume();
                        return CreateToken(TokenKind.FatArrow);
                    }
                    return CreateToken(TokenKind.Assignment);

                case '!':
                    Consume();
                    if (_current == '=')
                    {
                        Consume();
                        return CreateToken(TokenKind.NotEqual);
                    }
                    return CreateToken(TokenKind.Not);

                case '*':
                    Consume();
                    if (_current == '=')
                    {
                        Consume();
                        return CreateToken(TokenKind.MulEqual);
                    }
                    return CreateToken(TokenKind.Mul);

                case '/':
                    Consume();
                    if (_current == '=')
                    {
                        Consume();
                        return CreateToken(TokenKind.DivEqual);
                    }
                    return CreateToken(TokenKind.Div);

                case '.':
                    Consume();
                    return CreateToken(TokenKind.Dot);

                case ',':
                    Consume();
                    return CreateToken(TokenKind.Comma);

                case '&':
                    Consume();
                    if (_current == '&')
                    {
                        Consume();
                        return CreateToken(TokenKind.BooleanAnd);
                    }
                    else if (_current == '=')
                    {
                        Consume();
                        return CreateToken(TokenKind.BitwiseAndEqual);
                    }
                    return CreateToken(TokenKind.BitwiseAnd);

                case '|':
                    Consume();
                    if (_current == '|')
                    {
                        Consume();
                        return CreateToken(TokenKind.BooleanOr);
                    }
                    else if (_current == '=')
                    {
                        Consume();
                        return CreateToken(TokenKind.BitwiseOrEqual);
                    }
                    return CreateToken(TokenKind.BitwiseOr);

                case '%':
                    Consume();
                    if (_current == '=')
                    {
                        Consume();
                        return CreateToken(TokenKind.ModEqual);
                    }
                    return CreateToken(TokenKind.Mod);

                case '^':
                    Consume();
                    if (_current == '=')
                    {
                        Consume();
                        return CreateToken(TokenKind.BitwiseXorEqual);
                    }
                    return CreateToken(TokenKind.BitwiseXor);

                case '?':
                    Consume();
                    if (_current == '?')
                    {
                        Consume();
                        return CreateToken(TokenKind.DoubleQuestion);
                    }

                    return CreateToken(TokenKind.Question);

                default: return ScanWord();
            }
        }

        private Token ScanStringLiteral()
        {
            Advance();

            while (_current != '"')
            {
                if (IsEOF())
                {
                    AddError("Unexpected End Of File", Severity.Fatal);
                    return CreateToken(TokenKind.Error);
                }

                Consume();
            }

            Advance();

            return CreateToken(TokenKind.StringLiteral);
        }

        private Token ScanWhiteSpace()
        {
            while (IsWhiteSpace())
            {
                Consume();
            }

            return CreateToken(TokenKind.WhiteSpace);
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
            return _sourceCode[_index + ahead];
        }
    }
}
