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
        Int = 12,
        Char = 13,
        //Possible syntax keywords
        Semicolon = 14,
        If = 15,
        OpenParenthsis = 16,
        CloseParenthsis = 17,
        OpenBrace = 18,
        CloseBrace = 19,

        EndLine = 20,
        Unknown = 21,
        Identifier = 22,
        Tabulator = 23
    }
}
