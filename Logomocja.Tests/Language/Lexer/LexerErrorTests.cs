using Logomocja.Language.Lexing;
using Xunit;

namespace Logomocja.Tests.Language.Lexer;

public sealed class LexerErrorsTests
{
    private static void TokenizeAll(string source, LexerOptions? options = null)
    {
        using var lexer = Logomocja.Language.Lexing.Lexer.FromString(source, options);
        while (lexer.NextToken().Type != TokenType.Eof)
        {
        }
    }

    [Fact]
    public void L1_UnterminatedString_ThrowsWithStartPosition()
    {
        var ex = Assert.Throws<LexerException>(() => TokenizeAll("start {\n  x <- \"niezamknięty"));
        Assert.Equal(new SourcePosition(2, 8), ex.Position);
    }

    [Fact]
    public void L2_UnterminatedComment_ThrowsWithStartPosition()
    {
        var ex = Assert.Throws<LexerException>(() => TokenizeAll("start\n# komentarz bez końca\n{}"));
        Assert.Equal(new SourcePosition(2, 1), ex.Position);
    }

    [Theory]
    [InlineData("@")]
    [InlineData("$x")]
    [InlineData("?x")]
    [InlineData("12.")]
    public void L3_UnrecognizedCharacterOrMalformedNumber_ThrowsLexerException(string invalidCode)
    {
        Assert.Throws<LexerException>(() => TokenizeAll(invalidCode));
    }

    [Fact]
    public void L4_InvalidEscapeSequence_ThrowsWithEscapePosition()
    {
        var ex = Assert.Throws<LexerException>(() => TokenizeAll("\"zła ucieczka \\q\""));
        Assert.Equal(new SourcePosition(1, 16), ex.Position);
    }

    [Fact]
    public void L5_UnterminatedInterpolatedString_ThrowsLexerException()
    {
        Assert.Throws<LexerException>(() => TokenizeAll("$\"wynik: {1 + 2"));
    }

    [Fact]
    public void L6_ExceedingLengthLimits_ThrowsImmediately()
    {
        var strictOptions = new LexerOptions(MaxNumberLength: 5, MaxIdentifierLength: 5, MaxStringLength: 5);

        Assert.Throws<LexerException>(() => TokenizeAll("123456", strictOptions));
        Assert.Throws<LexerException>(() => TokenizeAll("abcdef", strictOptions));
        Assert.Throws<LexerException>(() => TokenizeAll("\"123456\"", strictOptions));
    }
}