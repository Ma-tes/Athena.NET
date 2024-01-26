using Athena.NET.ExceptionResult;
using Athena.NET.ExceptionResult.Interfaces;
using Athena.NET.Lexing;
using Athena.NET.Lexing.Structures;
using Athena.NET.Parsing.Interfaces;
using Athena.NET.Parsing.Nodes.Data;

namespace Athena.NET.Parsing.Nodes.Statements;

internal sealed class EqualAssignStatement : StatementNode
{
    public override TokenIndentificator NodeToken { get; } =
        TokenIndentificator.EqualAssignment;

    protected override IResultProvider<INode> ExecuteParseLeftNode(ReadOnlySpan<Token> tokens)
    {
        int identifierIndex = tokens.IndexOfToken(TokenIndentificator.Identifier);
        if (!tokens.TryGetIndexOfToken(out _, TokenIndentificator.Identifier))
            return ErrorResult<INode>.Create("Identifier wasn't defined");

        int tokenTypeIndex = tokens.IndexOfTokenType();
        ReadOnlyMemory<char> identifierData = tokens[identifierIndex].Data;
        INode returnNode = tokenTypeIndex != -1 ? new InstanceNode(tokens[tokenTypeIndex].TokenId, identifierData) :
            new IdentifierNode(identifierData);
        return SuccessfulResult<INode>.Create<ParsingResult>(returnNode, identifierIndex);
    }

    protected override IResultProvider<INode> ExecuteParseRigthNode(ReadOnlySpan<Token> tokens) =>
        TryGetNodeData(out nodeResult, tokens);
}
