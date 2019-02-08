using System;

namespace ITI.Lexer
{
    [Serializable]
    public enum TokenCategory
    {
        Unknown,
        WhiteSpace,
        Comment,

        Constant,
        Identifier,
        Grouping,
        Punctuation,
        Operator,

        Invalid
    }
}
