using Logomocja.Language.Lexing;
using Logomocja.Tests.TestMocks;
using Xunit;

namespace Logomocja.Tests.Language.Lexer;

public sealed class LexerSourcesAndLazinessTests
{
    [Fact]
    public void ReadsLazilyWithoutConsumingRestOfStream()
    {
        var explodingReader = new ExplodingTextReader("start { np(100) }", maxAllowedReads: 12);
        using var lexer = new Logomocja.Language.Lexing.Lexer(explodingReader);

        Token first = lexer.NextToken();
        Token second = lexer.NextToken();

        Assert.Equal(TokenType.Start, first.Type);
        Assert.Equal(TokenType.LBrace, second.Type);
        Assert.True(explodingReader.ReadsCount <= 12);
    }

    [Fact]
    public void ReadsFromFileWithUtf8Characters()
    {
        string tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile, "start { żółw(\"Ala\").np(50) }");

            using var lexer = Logomocja.Language.Lexing.Lexer.FromFile(tempFile);
            Assert.Equal(TokenType.Start, lexer.NextToken().Type);
            Assert.Equal(TokenType.LBrace, lexer.NextToken().Type);
            Assert.Equal("żółw", lexer.NextToken().Value);
            Assert.Equal(TokenType.LParen, lexer.NextToken().Type);
            Assert.Equal("Ala", lexer.NextToken().Value);
            Assert.Equal(TokenType.RParen, lexer.NextToken().Type);
            Assert.Equal(TokenType.Dot, lexer.NextToken().Type);
            Assert.Equal("np", lexer.NextToken().Value);
            Assert.Equal(TokenType.LParen, lexer.NextToken().Type);
            Assert.Equal(50.0, lexer.NextToken().Value);
            Assert.Equal(TokenType.RParen, lexer.NextToken().Type);
            Assert.Equal(TokenType.RBrace, lexer.NextToken().Type);
            Assert.Equal(TokenType.Eof, lexer.NextToken().Type);
            Assert.Equal(TokenType.Eof, lexer.NextToken().Type);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [Fact]
    public void TokenizesCompleteProgramWithExceptionsAndNullOperators()
    {
        string program = """
                         oto pobierz(t, i) {
                             jeżeli (i < 0) {
                                 rzuć $"Błędny indeks: {i}"
                             }
                             wynik t?.na(i) ?? -1
                         }

                         start {
                             spróbuj {
                                 w <- pobierz(brak, 0)
                             } złap (e) {
                                 pisz(e)
                             }
                         }
                         """;

        using var lexer = Logomocja.Language.Lexing.Lexer.FromString(program);
        var tokens = new List<Token>();

        Token current;
        do
        {
            current = lexer.NextToken();
            tokens.Add(current);
        } while (current.Type != TokenType.Eof);

        Assert.Contains(tokens, t => t.Type == TokenType.NullConditionalDot);
        Assert.Contains(tokens, t => t.Type == TokenType.NullCoalescing);
        Assert.Contains(tokens, t => t.Type == TokenType.Spróbuj);
        Assert.Contains(tokens, t => t.Type == TokenType.Złap);
        Assert.Contains(tokens, t => t.Type == TokenType.Rzuć);
        Assert.Equal(TokenType.Eof, tokens[^1].Type);
    }
}