using Athena.NET.Attributes;

namespace Athena.NET.Lexing;

//TODO: Add missing identifiers
public enum TokenIndentificator : uint
{
    Whitespace = 0,
    //Arithmetic operators
    Add = 1,
    Sub = 2,
    Mul = 3,
    Div = 4,
    //Relational operators
    EqualLogical = 5,
    NotEqual = 6,
    GreaterThan = 7,
    GreaterEqual = 8,
    LessThan = 9,
    LessEqual = 10,
    EqualAssignment = 11,

    //TODO: Improve abstraction of the following

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
    Invoker = 33,
    Print = 34,
    Definition = 35
}
