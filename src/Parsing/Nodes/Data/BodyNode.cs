using Athena.NET.ExceptionResult.Interfaces;
using Athena.NET.Parsing.Interfaces;

namespace Athena.NET.Parsing.Nodes.Data;

public sealed class BodyNode : DataNode<ReadOnlyMemory<INode>>
{
    public BodyNode(ReadOnlyMemory<INode> bodyNodes) : base(Lexing.TokenIndentificator.Unknown, bodyNodes)
    {
    }

    public static BodyNode CreateResultBodyNode(Span<IResultProvider<INode>> resultValues)
    {
        Memory<INode> returnNodes = new INode[resultValues.Length];
        Span<INode> returnNodesSpan = returnNodes.Span;

        int valuesLength = resultValues.Length;
        for (int i = 0; i < valuesLength; i++) { returnNodesSpan[i] = resultValues[i].ValueResult.Result; }
        return new BodyNode(returnNodes);
    }
}
