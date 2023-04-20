using Athena.NET.Lexing;
using Athena.NET.Lexing.Structures;
using Athena.NET.Parsing.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Athena.NET.Parsing.Nodes.Statements;

internal sealed class CallStatement : StatementNode
{
    public override TokenIndentificator NodeToken { get; } =
        TokenIndentificator.DefinitionCall;

    protected override bool TryParseLeftNode([NotNullWhen(true)] out NodeResult<INode> nodeResult, ReadOnlySpan<Token> tokens)
    {
        nodeResult = new SuccessulNodeResult<INode>(null!);
        return true;
    }

    protected override bool TryParseRigthNode([NotNullWhen(true)] out NodeResult<INode> nodeResult, ReadOnlySpan<Token> tokens)
    {
        return base.TryParseRigthNode(out nodeResult, tokens);
    }
}
