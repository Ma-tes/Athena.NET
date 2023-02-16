
using Athena.NET.Attributes;

namespace Athena.NET.Athena.NET.Lexer
{
    //TODO: There are a lot of missing,
    //identificators, but it's just for
    //testing
    public enum TokenIndentificator : uint
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
        EqualAssignment = 10,
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

        //Binary operators
        LogicalAnd = 15,
        LogicalOr = 16,
        LogicalXor = 17,
        LogicalLshift = 18,
        LogicalRshift = 19,

        [TokenType]
        Char = 20,
        [TokenType]
        String = 21,
        //Possible syntax keywords
        Semicolon = 22,
        If = 23,
        Else = 24,
        OpenParenthsis = 25,
        CloseParenthsis = 26,
        OpenBrace = 27,
        CloseBrace = 28,

        EndLine = 29,
        Unknown = 30,
        Identifier = 31,
        Tabulator = 32,
        Invoker = 33
    }
}
