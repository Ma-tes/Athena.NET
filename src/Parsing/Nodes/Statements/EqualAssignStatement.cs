using Athena.NET.Lexing;
using Athena.NET.Lexing.Structures;
using Athena.NET.Parsing.Interfaces;
using Athena.NET.Parsing.Nodes.Data;

namespace Athena.NET.Parsing.Nodes.Statements;

internal sealed class EqualAssignStatement : StatementNode
{
    public override TokenIndentificator NodeToken { get; } =
        TokenIndentificator.EqualAssignment;

    protected override bool TryParseLeftNode(out NodeResult<INode> nodeResult, ReadOnlySpan<Token> tokens)
    {
        int identifierIndex = tokens.IndexOfToken(TokenIndentificator.Identifier);
        if (!tokens.TryGetIndexOfToken(out _, TokenIndentificator.Identifier))
        {
            nodeResult = new ErrorNodeResult<INode>("Identifier wasn't defined");
            return false;
        }

        int tokenTypeIndex = tokens.IndexOfTokenType();
        ReadOnlyMemory<char> identifierData = tokens[identifierIndex].Data;
        INode returnNode = tokenTypeIndex != -1 ? new InstanceNode(tokens[tokenTypeIndex].TokenId, identifierData) :
            new IdentifierNode(identifierData);
        nodeResult = new SuccessulNodeResult<INode>(returnNode);
        return true;
    }

    protected override bool TryParseRigthNode(out NodeResult<INode> nodeResult, ReadOnlySpan<Token> tokens) =>
        TryGetNodeData(out nodeResult, tokens);
}
