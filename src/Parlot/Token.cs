﻿namespace Parlot
{
    public struct Token<T>
    {
        public static readonly Token<T> Empty = new(default, null, TextPosition.Start, TextPosition.Start);

        public Token(T type, string buffer, TextPosition start, TextPosition end)
        {
            Buffer = buffer;
            Type = type;
            Start = start;
            End = end;
        }

        public string Buffer { get; }
        public TextPosition Start { get; }
        public TextPosition End { get; }
        public T Type { get; }
        public int Length => End - Start;
        public string Text => Buffer.Substring(Start.Offset, Length);
    }
}