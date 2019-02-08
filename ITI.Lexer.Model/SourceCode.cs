using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace ITI.Lexer
{
    /**
     * Provides a singular representation for a source file
     * as well as a bunch of helpful methods that work as
     * a better system to retrieve a subset of code.
     */
    public sealed class SourceCode
    {
        private Lazy<string[]> _lines;
        private string _sourceCode;

        public string Content => _sourceCode;

        public string[] Lines => _lines.Value;

        public char this[int index] => _sourceCode.CharAt(index);

        private class Subset<T> : IEnumerable<T>
        {
            private int _end;
            private int _start;
            private IEnumerable<T> _set;

            private struct SubsetEnumerator : IEnumerator<T>
            {
                private bool _disposed;
                private int _index;
                private Subset<T> _subset;

                public T Current => _subset._set.ElementAt(_index);

                object IEnumerator.Current => _subset._set.ElementAt(_index);

                public SubsetEnumerator(Subset<T> subset)
                {
                    _disposed = false;
                    _index = subset._start - 1;
                    _subset = subset;
                }

                public void Dispose()
                {
                    throw new NotImplementedException();
                }

                public bool MoveNext()
                {
                    throw new NotImplementedException();
                }

                public void Reset()
                {
                    throw new NotImplementedException();
                }
            }

            public Subset(IEnumerable<T> collection, int start, int end)
            {
                _set = collection;
                _start = start;
                _end = end;
            }

            public IEnumerator<T> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        public SourceCode(string sourceCode)
        {
            throw new NotImplementedException();
        }

        public string GetLine(int line)
        {
            throw new NotImplementedException();
        }

        public string[] GetLines(int start, int end)
        {
            throw new NotImplementedException();
        }

        public string GetSpan(SourceSpan span)
        {
            var length = span.Length;
            var start = span.Start.Index;
            return _sourceCode.Substring(start, length);
        }

        public string GetSpan(SyntaxNode node)
        {
            return GetSpan(node.Span);
        }
    }
}
