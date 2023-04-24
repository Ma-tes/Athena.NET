using Athena.NET.Lexing;
using Athena.NET.Lexing.Structures;

namespace Athena.NET.Tests.Lexing;

public class KeywordsTests : TokenReaderTestsBase
{
    [Fact]
    public void ReadingText_WithInt_RecognizesKeyword()
    {
        Token[] actual = Tokenize("int");
        Token[] expected = Tokens((TokenIndentificator.Int, "int"));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ReadingText_WithFloat_RecognizesKeyword()
    {
        Token[] actual = Tokenize("float");
        Token[] expected = Tokens((TokenIndentificator.Float, "float"));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ReadingText_WithByte_RecognizesKeyword()
    {
        Token[] actual = Tokenize("byte");
        Token[] expected = Tokens((TokenIndentificator.Byte, "byte"));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ReadingText_WithChar_RecognizesKeyword()
    {
        Token[] actual = Tokenize("char");
        Token[] expected = Tokens((TokenIndentificator.Char, "char"));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ReadingText_WithIf_RecognizesKeyword()
    {
        Token[] actual = Tokenize("if");
        Token[] expected = Tokens((TokenIndentificator.If, "if"));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ReadingText_WithElse_RecognizesKeyword()
    {
        Token[] actual = Tokenize("else");
        Token[] expected = Tokens((TokenIndentificator.Else, "else"));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ReadingText_WithEqualLogical_RecognizesKeyword()
    {
        Token[] actual = Tokenize("==");
        Token[] expected = Tokens((TokenIndentificator.EqualLogical, "=="));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ReadingText_WithNotEqual_RecognizesKeyword()
    {
        Token[] actual = Tokenize("!=");
        Token[] expected = Tokens((TokenIndentificator.NotEqual, "!="));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ReadingText_WithGreaterEqual_RecognizesKeyword()
    {
        Token[] actual = Tokenize(">=");
        Token[] expected = Tokens((TokenIndentificator.GreaterEqual, ">="));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ReadingText_WithGreaterThan_RecognizesKeyword()
    {
        Token[] actual = Tokenize(">");
        Token[] expected = Tokens((TokenIndentificator.GreaterThan, ">"));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ReadingText_WithLessEqual_RecognizesKeyword()
    {
        Token[] actual = Tokenize("<=");
        Token[] expected = Tokens((TokenIndentificator.LessEqual, "<="));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ReadingText_WithLessThan_RecognizesKeyword()
    {
        Token[] actual = Tokenize("<");
        Token[] expected = Tokens((TokenIndentificator.LessThan, "<"));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ReadingText_WithInvoker_RecognizesKeyword()
    {
        Token[] actual = Tokenize("->");
        Token[] expected = Tokens((TokenIndentificator.Invoker, "->"));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ReadingText_WithPrint_RecognizesKeyword()
    {
        Token[] actual = Tokenize("print");
        Token[] expected = Tokens((TokenIndentificator.Print, "print"));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ReadingText_WithDefinition_RecognizesKeyword()
    {
        Token[] actual = Tokenize("def");
        Token[] expected = Tokens((TokenIndentificator.Definition, "def"));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ReadingText_WithEndOfLine_RecognizesKeyword()
    {
        Token[] actual = Tokenize("\r\n");
        Token[] expected = Tokens((TokenIndentificator.EndLine, "\0n"));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ReadingText_WithTab_RecognizesKeyword()
    {
        Token[] actual = Tokenize("\t");
        Token[] expected = Tokens((TokenIndentificator.Tabulator, "\t"));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ReadingText_WithWhitespace_RecognizesKeyword()
    {
        Token[] actual = Tokenize(" ");
        Token[] expected = Tokens((TokenIndentificator.Whitespace, " "));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ReadingText_WithSemicolon_RecognizesKeyword()
    {
        Token[] actual = Tokenize(";");
        Token[] expected = Tokens((TokenIndentificator.Semicolon, ";"));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ReadingText_WithPlusSign_RecognizesKeyword()
    {
        Token[] actual = Tokenize("+");
        Token[] expected = Tokens((TokenIndentificator.Add, "+"));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ReadingText_WithMinusSign_RecognizesKeyword()
    {
        Token[] actual = Tokenize("-");
        Token[] expected = Tokens((TokenIndentificator.Sub, "-"));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ReadingText_WithAsterisk_RecognizesKeyword()
    {
        Token[] actual = Tokenize("*");
        Token[] expected = Tokens((TokenIndentificator.Mul, "*"));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ReadingText_WithForwardSlash_RecognizesKeyword()
    {
        Token[] actual = Tokenize("/");
        Token[] expected = Tokens((TokenIndentificator.Div, "/"));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ReadingText_WithLogicalAnd_RecognizesKeyword()
    {
        Token[] actual = Tokenize("&");
        Token[] expected = Tokens((TokenIndentificator.LogicalAnd, "&"));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ReadingText_WithLogicalOr_RecognizesKeyword()
    {
        Token[] actual = Tokenize("|");
        Token[] expected = Tokens((TokenIndentificator.LogicalOr, "|"));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ReadingText_WithLogicalXor_RecognizesKeyword()
    {
        Token[] actual = Tokenize("^");
        Token[] expected = Tokens((TokenIndentificator.LogicalXor, "^"));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ReadingText_WithLeftShift_RecognizesKeyword()
    {
        Token[] actual = Tokenize("<<");
        Token[] expected = Tokens((TokenIndentificator.LogicalLshift, "<<"));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ReadingText_WithRightShift_RecognizesKeyword()
    {
        Token[] actual = Tokenize(">>");
        Token[] expected = Tokens((TokenIndentificator.LogicalRshift, ">>"));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ReadingText_WithEqualSign_RecognizesKeyword()
    {
        Token[] actual = Tokenize("=");
        Token[] expected = Tokens((TokenIndentificator.EqualAssignment, "="));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ReadingText_WithOpenBrace_RecognizesKeyword()
    {
        Token[] actual = Tokenize("(");
        Token[] expected = Tokens((TokenIndentificator.OpenBrace, "("));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ReadingText_WithCloseBrace_RecognizesKeyword()
    {
        Token[] actual = Tokenize(")");
        Token[] expected = Tokens((TokenIndentificator.CloseBrace, ")"));

        Assert.Equal(expected, actual);
    }
}
