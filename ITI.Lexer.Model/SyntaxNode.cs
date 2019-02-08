namespace ITI.Lexer
{
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
