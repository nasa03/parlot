﻿using System;
using System.Collections.Generic;

namespace Parlot.Fluent
{
    public class OneOrMany<T> : Parser<IList<T>>
    {
        private readonly IParser<T> parser;
        private readonly bool _skipWhitespace;

        public OneOrMany(IParser<T> parser, bool skipWhitespace = true)
        {
            this.parser = parser;
            _skipWhitespace = skipWhitespace;
        }

        public override bool Parse(Scanner scanner, IParseResult<IList<T>> result)
        {
            if (_skipWhitespace)
            {
                scanner.SkipWhiteSpace();
            }

            var parsed = result != null ? new ParseResult<T>() : null;
            
            if (!parser.Parse(scanner, parsed))
            {
                result?.Fail();
                return false;
            }

            var start = parsed.Start;
            var results = new List<T>();

            TextPosition end;

            do
            {
                end = parsed.End;

                results.Add(parsed.GetValue());

                parsed = result != null ? new ParseResult<T>() : null;
            } while (parser.Parse(scanner, parsed));

            result?.Succeed(scanner.Buffer, start, end, results);
            return true;
        }
    }
}
