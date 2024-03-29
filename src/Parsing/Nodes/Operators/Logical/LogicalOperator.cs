﻿namespace Athena.NET.Parsing.Nodes.Operators.Logical;

internal abstract class LogicalOperator : OperatorNode
{
    public sealed override OperatorPrecedence Precedence { get; } =
        OperatorPrecedence.Logical;

    protected abstract bool CalculateLogicalBool(int firstValue, int secondValue);

    public sealed override int CalculateData(int firstData, int secondData)
    {
        //TODO: Improve this
        return CalculateLogicalBool(firstData, secondData) ? 1 : 0;
    }
}
