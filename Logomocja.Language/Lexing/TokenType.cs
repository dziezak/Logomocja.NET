namespace Logomocja.Language.Lexing;

public enum TokenType
{
    // Słowa zastrzeżone
    Start,          // start
    Oto,            // oto
    Jeżeli,         // jeżeli
    Inaczej,        // inaczej
    Powtórz,        // powtórz
    Wynik,          // wynik
    Lub,            // lub
    Oraz,           // oraz
    Nie,            // nie
    Prawda,         // prawda
    Fałsz,          // fałsz
    Run,            // Run
    Brak,           // brak
    Spróbuj,        // spróbuj
    Złap,           // złap
    Rzuć,           // rzuć

    // Identyfikatory i literały
    Identifier,     
    Number,         
    String,         

    // Interpolacja tekstu
    InterpolatedStringStart, // $"
    InterpolatedText,        // fragment tekstu wewnątrz $"..." (Value = string)
    InterpolatedStringEnd,   // zamykające "

    // Operatory wieloznakowe
    Assign,             // <-
    NotEqual,           // <>
    LessOrEqual,        // <=
    GreaterOrEqual,     // >=
    NullConditionalDot, // ?.
    NullCoalescing,     // ??

    // Operatory jednoznakowe
    Plus,           // +
    Minus,          // -
    Star,           // *
    Slash,          // /
    Percent,        // %
    Equal,          // =
    Less,           // <
    Greater,        // >
    Dot,            // .

    // Nawiasy i separatory
    LParen,         // (
    RParen,         // )
    LBrace,         // {
    RBrace,         // }
    Comma,          // ,

    // Koniec pliku
    Eof
}