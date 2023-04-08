using Athena.NET.Lexer;
using Athena.NET.Lexer.Structures;
using Athena.NET.Parser.Interfaces;
using Athena.NET.Parser.Nodes.DataNodes;
using Athena.NET.Parser.Nodes.OperatorNodes;

namespace Athena.NET.Parser.Nodes.StatementNodes
{
    internal sealed class EqualAssignStatement : StatementNode 
    {
        public override TokenIndentificator NodeToken { get; } =
            TokenIndentificator.EqualAssignment;

        protected override bool TryParseLeftNode(out NodeResult<INode> nodeResult, ReadOnlySpan<Token> tokens)
        {
            int tokenTypeIndex = tokens.IndexOfTokenType();
            int identifierIndex = tokens.IndexOfToken(TokenIndentificator.Identifier);
            if (identifierIndex == -1) 
            {
                nodeResult = new ErrorNodeResult<INode>("Identifier wasn't defined");
                return false;
            }

            ReadOnlyMemory<char> identifierData = tokens[identifierIndex].Data;
            INode returnNode = tokenTypeIndex != -1 ? new InstanceNode(tokens[tokenTypeIndex].TokenId, identifierData) :
                new IdentifierNode(identifierData);
            nodeResult = new SuccessulNodeResult<INode>(returnNode);
            return true;
        }

        protected override bool TryParseRigthNode(out NodeResult<INode> nodeResult, ReadOnlySpan<Token> tokens)
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
}
