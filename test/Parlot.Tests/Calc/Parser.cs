namespace Parlot.Tests.Calc
{

    // Recursive descent parser
    // https://craftinginterpreters.com/parsing-expressions.html#recursive-descent-parsing

    /*
     * Grammar:
     * expression     => term ;
     * term           => factor ( ( "-" | "+" ) factor )* ;
     * factor         => unary ( ( "/" | "*" ) unary )* ;
     * unary          => ( "-" ) unary
     *                 | primary ;
     * primary        => NUMBER
     *                  | "(" expression ")" ;
    */

    /// <summary>
    /// This verion of the Parser creates and intermediate AST.
    /// </summary>
    public class Parser : Parser<Expression>
    {
        private Scanner _scanner;

        public override Expression Parse(string text)
        {
            _scanner = new Scanner(text);

            return ParseExpression();
        }

        private Expression ParseExpression()
        {
            return ParseTerm();
        }

        private Expression ParseTerm()
        {
            var expression = ParseFactor();

            _scanner.SkipWhiteSpace();

            while (true)
            {
                if (_scanner.ReadText("+", "Plus"))
                {
                    _scanner.SkipWhiteSpace();

                    expression = new Addition(expression, ParseFactor());
                }
                else if (_scanner.ReadText("-", "Minus"))
                {
                    _scanner.SkipWhiteSpace();

                    expression = new Substraction(expression, ParseFactor());
                }
                else
                {
                    break;
                }
            }

            return expression;
        }

        private Expression ParseFactor()
        {
            var expression = ParseUnaryExpression();

            _scanner.SkipWhiteSpace();

            while (true)
            {
                if (_scanner.ReadText("*", "Times"))
                {
                    _scanner.SkipWhiteSpace();

                    expression = new Multiplication(expression, ParseUnaryExpression());
                }
                else if (_scanner.ReadText("/", "Divided"))
                {
                    _scanner.SkipWhiteSpace();

                    expression = new Division(expression, ParseUnaryExpression());
                }
                else
                {
                    break;
                }
            }

            return expression;
        }

        /*
         unary =    ( "!" | "-" ) unary
                    | primary ;
        */

        private Expression ParseUnaryExpression()
        {
            return ParsePrimaryExpression();
        }

        /*
          primary = NUMBER
                    | "(" expression ")" ;
        */

        private Expression ParsePrimaryExpression()
        {
            _scanner.SkipWhiteSpace();

            var number = _scanner.ReadDecimal();

            if (number)
            {
                return new Number(decimal.Parse(number.Token.Span));
            }

            if (_scanner.ReadText("("))
            {
                var expression = ParseExpression();

                if (!_scanner.ReadText(")"))
                {
                    throw new ParseException("Expected ')'", _scanner.Cursor.Position);
                }

                return expression;
            }

            throw new ParseException("Expected primary expression", _scanner.Cursor.Position);
        }
    }
}