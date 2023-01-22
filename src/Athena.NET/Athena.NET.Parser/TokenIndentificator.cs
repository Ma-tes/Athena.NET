using Athena.NET.Athena.NET.Parser.LexicalAnalyzer.Attributes;

namespace Athena.NET.Athena.NET.Parser
{
    //TODO: There are a lot of missing,
    //identificators, but it's just for
    //testing
    internal enum TokenIndentificator : uint
    {
        Whitespace = 0,
        //Arithmetic operators
        Add = 1,
        Sub = 2,
        Mul = 3,
        Div = 4,
        //Relation operators
        LessThan = 5,
        LessEqual = 6,
        GreaterThan = 7,
        GreaterEqual = 8,
        EqualLogical = 9,
        EqualAsigment = 10,
        NotEqual = 11,

        //TODO: This part is really temporary
        //I would really want something more
        //abstract, but now I need to focus
        //results

        //Possible types
        [PrimitiveType(typeof(int))]
        Int = 12,
        [PrimitiveType(typeof(float))]
        Float = 13,
        [PrimitiveType(typeof(byte))]
        Byte = 14,
        [PrimitiveType(typeof(char))]
        Char = 15,
        String = 16,
        //Possible syntax keywords
        Semicolon = 17,
        If = 18,
        OpenParenthsis = 19,
        CloseParenthsis = 20,
        OpenBrace = 21,
        CloseBrace = 22,

        EndLine = 23,
        Unknown = 24,
        Identifier = 25,
        Tabulator = 26
    }
}
