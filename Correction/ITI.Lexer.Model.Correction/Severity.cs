using System;

namespace ITI.Lexer
{
    [Serializable]
    public enum Severity
    {
        None,
        Message,
        Warning,
        Error,
        Fatal
    }
}
