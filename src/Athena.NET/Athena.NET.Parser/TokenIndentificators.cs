namespace Athena.NET.Athena.NET.Parser
{
    //TODO: There are a lot of missing,
    //identificators, but it's just for
    //testing
    internal enum TokenIndentificators : uint
    {
        WHITESPACE = 0,
        //Arithmetic operators
        ADD = 1,
        SUB = 2,
        MUL = 3,
        DIV = 4,
        //Relation operators
        LT = 5,
        LE = 6,
        GT = 7,
        GE = 8,
        EQ_logical = 9,
        EQ_asigment = 10,
        NE = 11,

        //TODO: This part is really temporary
        //I would really want something more
        //abstract, but now I need to focus
        //results

        //Possible types
        INT = 12,
        CHAR = 13,
        //Possible syntax keywords
        SEMICOLON = 14,
        IF = 15,
        OPEN_round = 16,
        CLOSE_round = 17,
        OPEN_curly = 18,
        CLOSE_curly = 19,
    }
}
