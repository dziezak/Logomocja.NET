using Logomocja.Language.Lexing;
using Xunit;

namespace Logomocja.Tests.Language.Lexer;

public sealed class LexerTokensTests
{
    private static List<Token> TokenizeAll(string source)
    {
        using var lexer = Logomocja.Language.Lexing.Lexer.FromString(source);
        var tokens = new List<Token>();

        while (true)
        {
            Token token = lexer.NextToken();
            tokens.Add(token);
            if (token.Type == TokenType.Eof)
            {
                break;
            }
        }

        return tokens;
    }

    [Theory]
    [InlineData("start", TokenType.Start)]
    [InlineData("oto", TokenType.Oto)]
    [InlineData("jeżeli", TokenType.Jeżeli)]
    [InlineData("inaczej", TokenType.Inaczej)]
    [InlineData("powtórz", TokenType.Powtórz)]
    [InlineData("wynik", TokenType.Wynik)]
    [InlineData("lub", TokenType.Lub)]
    [InlineData("oraz", TokenType.Oraz)]
    [InlineData("nie", TokenType.Nie)]
    [InlineData("prawda", TokenType.Prawda)]
    [InlineData("fałsz", TokenType.Fałsz)]
    [InlineData("Run", TokenType.Run)]
    [InlineData("brak", TokenType.Brak)]
    [InlineData("spróbuj", TokenType.Spróbuj)]
    [InlineData("złap", TokenType.Złap)]
    [InlineData("rzuć", TokenType.Rzuć)]
    public void RecognizesAllReservedKeywords(string keyword, TokenType expectedType)
    {
        List<Token> tokens = TokenizeAll(keyword);
        Assert.Equal(2, tokens.Count);
        Assert.Equal(new Token(expectedType, null, new SourcePosition(1,1)), tokens[0]);
        Assert.Equal(TokenType.Eof, tokens[1].Type);
    }

    [Theory]
    [InlineData("kwadrat")]
    [InlineData("żółw")]
    [InlineData("GetŻółw")]
    [InlineData("zmienna_123")]
    [InlineData("ąęćłńóśźżĄĘĆŁŃÓŚŹŻ")]
    [InlineData("run")]
    [InlineData("Start")]
    public void RecognizesIdentifiersWithUt8AndUnderscores(string identifier)
    {
        List<Token> tokens = TokenizeAll(identifier);
        Assert.Equal(2, tokens.Count);
        Assert.Equal(new Token(TokenType.Identifier, identifier, new SourcePosition(1,1)), tokens[0]);
    }
    
    [Theory]
    [InlineData("0", 0.0)]
    [InlineData("42", 42.0)]
    [InlineData("3.14", 3.14)]
    [InlineData("0.005", 0.005)]
    public void ParsesNumbersImmediatelyToDouble(string literal, double expectedValue)
    {
        List<Token> tokens = TokenizeAll(literal);

        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.IsType<double>(tokens[0].Value);
        Assert.Equal(expectedValue, (double)tokens[0].Value!, precision: 10);
    }
    
    [Fact]
    public void UnaryMinusIsSeparateTokenFromNumber()
    {
        List<Token> tokens = TokenizeAll("-15.5");

        Assert.Equal(3, tokens.Count);
        Assert.Equal(new Token(TokenType.Minus, new SourcePosition(1, 1)), tokens[0]);
        Assert.Equal(new Token(TokenType.Number, 15.5, new SourcePosition(1, 2)), tokens[1]);
    }
    
    [Theory]
    [InlineData("x<-10")]
    [InlineData("x <- 10")]
    [InlineData("x<- 10")]
    [InlineData("x <-10")]
    public void AssignmentWithoutSpacesProducesIdenticalTokens(string code)
    {
        List<Token> tokens = TokenizeAll(code);

        Assert.Equal(4, tokens.Count);
        Assert.Equal(TokenType.Identifier, tokens[0].Type);
        Assert.Equal("x", tokens[0].Value);
        Assert.Equal(TokenType.Assign, tokens[1].Type);
        Assert.Equal(TokenType.Number, tokens[2].Type);
        Assert.Equal(10.0, tokens[2].Value);
    }
    
    [Fact]
    public void RecognizesAllOperatorsAndDelimiters()
    {
        string input = "<- <> <= >= ?. ?? + - * / % = < > . ( ) { } ,";
        List<Token> tokens = TokenizeAll(input);

        TokenType[] expectedTypes =
        [
            TokenType.Assign,
            TokenType.NotEqual,
            TokenType.LessOrEqual,
            TokenType.GreaterOrEqual,
            TokenType.NullConditionalDot,
            TokenType.NullCoalescing,
            TokenType.Plus,
            TokenType.Minus,
            TokenType.Star,
            TokenType.Slash,
            TokenType.Percent,
            TokenType.Equal,
            TokenType.Less,
            TokenType.Greater,
            TokenType.Dot,
            TokenType.LParen,
            TokenType.RParen,
            TokenType.LBrace,
            TokenType.RBrace,
            TokenType.Comma,
            TokenType.Eof
        ];

        Assert.Equal(expectedTypes, tokens.Select(t => t.Type));
    }
}