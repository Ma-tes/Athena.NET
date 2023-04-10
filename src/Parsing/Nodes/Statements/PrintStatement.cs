using Athena.NET.Lexing;
using Athena.NET.Lexing.Structures;
using Athena.NET.Parsing.Interfaces;
using Athena.NET.Parsing.Nodes.Operators;
using System.Diagnostics.CodeAnalysis;

namespace Athena.NET.Parsing.Nodes.Statements;

internal sealed class PrintStatement : StatementNode
{
    public override TokenIndentificator NodeToken { get; } =
        TokenIndentificator.Print;

    protected override bool TryParseLeftNode([NotNullWhen(true)] out NodeResult<INode> nodeResult, ReadOnlySpan<Token> tokens)
    {
        nodeResult = new SuccessulNodeResult<INode>(null!);
        return true;
    }

    protected override bool TryParseRigthNode([NotNullWhen(true)] out NodeResult<INode> nodeResult, ReadOnlySpan<Token> tokens)
    {
        int semicolonIndex = tokens.IndexOfToken(TokenIndentificator.Semicolon);
        if (OperatorHelper.TryGetOperatorResult(out nodeResult, tokens[..semicolonIndex]))
            return true;

        INode resultNode = tokens[..semicolonIndex].GetDataNode();
        nodeResult = resultNode is not null ? new SuccessulNodeResult<INode>(resultNode) :
            new ErrorNodeResult<INode>("Any valid node wasn't found");
        return resultNode is not null;
    }
}
