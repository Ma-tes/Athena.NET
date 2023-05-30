using Athena.NET.Lexing;
using Athena.NET.Lexing.Structures;
using Athena.NET.Parsing.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Athena.NET.Parsing.Nodes.Statements;

internal sealed class ReturnStatement : StatementNode
{
    public override TokenIndentificator NodeToken { get; } =
        TokenIndentificator.Print;

    protected override bool TryParseLeftNode([NotNullWhen(true)] out NodeResult<INode> nodeResult, ReadOnlySpan<Token> tokens)
    {
        nodeResult = new SuccessulNodeResult<INode>(null!);
        return true;
    }

    protected override bool TryParseRigthNode(out NodeResult<INode> nodeResult, ReadOnlySpan<Token> tokens) =>
        TryGetNodeData(out nodeResult, tokens);
}
