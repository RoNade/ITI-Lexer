﻿using System;

namespace ITI.Lexer
{
    public sealed class Token : IEquatable<Token>
    {
        private Lazy<TokenCategory> _category;
        public TokenCategory Category => _category.Value;
        public TokenKind Kind { get; }
        public SourceSpan Span { get; }
        public string Value { get; }

        public Token(TokenKind kind, string content, SourceLocation start, SourceLocation end)
        {
            Kind = kind;
            Value = content;
            Span = new SourceSpan(start, end);

            _category = new Lazy<TokenCategory>(GetTokenCategory);
        }

        public static bool operator !=(Token left, string right)
        {
            throw new NotImplementedException();
        }

        public static bool operator !=(string left, Token right)
        {
            throw new NotImplementedException();
        }

        public static bool operator !=(Token left, TokenKind right)
        {
            throw new NotImplementedException();
        }

        public static bool operator !=(TokenKind left, Token right)
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(Token left, string right)
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(string left, Token right)
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(Token left, TokenKind right)
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(TokenKind left, Token right)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public bool Equals(Token other)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode() ^ Span.GetHashCode() ^ Kind.GetHashCode();
        }

        public bool IsTrivia()
        {
            return Category == TokenCategory.WhiteSpace || Category == TokenCategory.Comment;
        }

        private TokenCategory GetTokenCategory()
        {
            switch (Kind)
            {
                case TokenKind.Arrow:
                case TokenKind.FatArrow:
                case TokenKind.Colon:
                case TokenKind.Semicolon:
                case TokenKind.Comma:
                case TokenKind.Dot:
                    return TokenCategory.Punctuation;

                case TokenKind.Equal:
                case TokenKind.NotEqual:
                case TokenKind.Not:
                case TokenKind.LessThan:
                case TokenKind.LessThanOrEqual:
                case TokenKind.GreaterThan:
                case TokenKind.GreaterThanOrEqual:
                case TokenKind.Minus:
                case TokenKind.MinusEqual:
                case TokenKind.MinusMinus:
                case TokenKind.Mod:
                case TokenKind.ModEqual:
                case TokenKind.Mul:
                case TokenKind.MulEqual:
                case TokenKind.Plus:
                case TokenKind.PlusEqual:
                case TokenKind.PlusPlus:
                case TokenKind.Question:
                case TokenKind.DoubleQuestion:
                case TokenKind.DivEqual:
                case TokenKind.Div:
                case TokenKind.BooleanOr:
                case TokenKind.BooleanAnd:
                case TokenKind.BitwiseXorEqual:
                case TokenKind.BitwiseXor:
                case TokenKind.BitwiseOrEqual:
                case TokenKind.BitwiseOr:
                case TokenKind.BitwiseAndEqual:
                case TokenKind.BitwiseAnd:
                case TokenKind.BitShiftLeft:
                case TokenKind.BitShiftRight:
                case TokenKind.Assignment:
                    return TokenCategory.Operator;

                case TokenKind.BlockComment:
                case TokenKind.LineComment:
                    return TokenCategory.Comment;

                case TokenKind.NewLine:
                case TokenKind.WhiteSpace:
                    return TokenCategory.WhiteSpace;

                case TokenKind.LeftBrace:
                case TokenKind.LeftBracket:
                case TokenKind.LeftParenthesis:
                case TokenKind.RightBrace:
                case TokenKind.RightBracket:
                case TokenKind.RightParenthesis:
                    return TokenCategory.Grouping;

                case TokenKind.Identifier:
                case TokenKind.Keyword:
                    return TokenCategory.Identifier;

                case TokenKind.StringLiteral:
                case TokenKind.IntegerLiteral:
                case TokenKind.FloatLiteral:
                    return TokenCategory.Constant;

                case TokenKind.Error:
                    return TokenCategory.Constant;

                default: return TokenCategory.Unknown;
            }
        }
    }
}
