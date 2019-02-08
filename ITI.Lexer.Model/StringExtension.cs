namespace ITI.Lexer
{
    public static class StringExtension
    {
        /**
         * Returns an ASCII NULL character
         * if the location is out of range
         * or the character at given index
         */
        public static char CharAt(this string str, int index)
        {
            if (index > str.Length - 1 || index < 0)
            {
                return '\0';
            }

            return str[index];
        }
    }
}
