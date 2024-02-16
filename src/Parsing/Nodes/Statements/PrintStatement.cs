using Athena.NET.ExceptionResult;
using Athena.NET.ExceptionResult.Interfaces;
using Athena.NET.Lexing;
using Athena.NET.Lexing.Structures;
using Athena.NET.Parsing.Interfaces;

namespace Athena.NET.Parsing.Nodes.Statements;

internal sealed class PrintStatement : StatementNode
{
    public override TokenIndentificator NodeToken { get; } =
        TokenIndentificator.Print;

    protected override IResultProvider<INode> ExecuteParseLeftNode(ReadOnlySpan<Token> tokens) =>
        SuccessfulResult<INode>.Create(null!);

    protected override IResultProvider<INode> ExecuteParseRigthNode(ReadOnlySpan<Token> tokens) =>
        GetRelativeDataNodeResult(tokens);
}
