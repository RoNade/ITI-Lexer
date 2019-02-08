using System.Collections;
using System.Collections.Generic;

namespace ITI.Lexer
{
    /**
     * Component of the compilation process that
     * follows the code throughout its transformation.
     * Its purpose is to provide a standard mechanism
     * for reporting errors in the user's code.
     */
    public class ErrorSink : IEnumerable<ErrorEntry>
    {
        private List<ErrorEntry> _errors;

        public IEnumerable<ErrorEntry> Errors => _errors.AsReadOnly();
        public bool HasErrors => _errors.Count > 0;

        public ErrorSink()
        {
            _errors = new List<ErrorEntry>();
        }

        public void AddError(string message, SourceCode sourceCode, Severity severity, SourceSpan span)
        {
            _errors.Add(new ErrorEntry(message, sourceCode.GetLines(span.Start.Line, span.End.Line), severity, span));
        }

        public void Clear()
        {
            _errors.Clear();
        }

        public IEnumerator<ErrorEntry> GetEnumerator()
        {
            return _errors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _errors.GetEnumerator();
        }
    }
}
