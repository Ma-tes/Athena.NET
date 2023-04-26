using Athena.NET.Lexing;
using Athena.NET.Lexing.Structures;
using Athena.NET.Parsing.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Athena.NET.Parsing.Nodes.Statements;

internal abstract class StatementNode : INode
{
    public abstract TokenIndentificator NodeToken { get; }
    public ChildrenNodes ChildNodes { get; private set; }

    //TODO: Reduce the amount of nullable types
    public virtual NodeResult<INode> CreateStatementResult(ReadOnlySpan<Token> tokens, int tokenIndex)
    {
        ReadOnlySpan<Token> leftTokens = tokens[..tokenIndex];
        ReadOnlySpan<Token> rightTokens = tokens[(tokenIndex + 2)..];

        if (!TryParseLeftNode(out NodeResult<INode> leftResult, leftTokens) && leftResult is not null)
            return new ErrorNodeResult<INode>(leftResult.Message);
        if (!TryParseRigthNode(out NodeResult<INode> rightResult, rightTokens) && rightResult is not null)
            return new ErrorNodeResult<INode>(rightResult.Message);

        ChildNodes = new(leftResult!.Node!, rightResult!.Node!);
        return new SuccessulNodeResult<INode>(this);
    }

    protected virtual bool TryParseLeftNode([NotNullWhen(true)] out NodeResult<INode> nodeResult, ReadOnlySpan<Token> tokens)
    {
        nodeResult = null!;
        return false;
    }

    protected virtual bool TryParseRigthNode([NotNullWhen(true)] out NodeResult<INode> nodeResult, ReadOnlySpan<Token> tokens)
    {
        nodeResult = null!;
        return false;
    }
}
