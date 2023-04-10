namespace Athena.NET.Parsing.Nodes.Operators;

internal enum OperatorPrecedence : uint
{
    Multiplicative = 1,
    Additive = 2,
    Logical = 3,
    Brace = 100
}
