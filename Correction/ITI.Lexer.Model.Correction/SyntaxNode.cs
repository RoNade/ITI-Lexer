using System;

namespace ITI.Lexer
{
    [Serializable]
    public abstract class SyntaxNode
    {
        public abstract SyntaxCategory Category { get; }
        public abstract SyntaxKind Kind { get; }
        public SourceSpan Span { get; }

        protected SyntaxNode(SourceSpan span)
        {
            Span = span;
        }
    }
}
