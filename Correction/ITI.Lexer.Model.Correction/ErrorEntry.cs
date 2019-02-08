using System;

namespace ITI.Lexer
{
    [Serializable]
    public sealed class ErrorEntry
    {
        public string[] Lines { get; }
        public string Message { get; }

        public Severity Severity { get; }

        public SourceSpan Span { get; }

        public ErrorEntry(string message, string[] lines, Severity severity, SourceSpan span)
        {
            Severity = severity;
            Message = message;
            Lines = lines;
            Span = span;
        }
    }
}
