using Athena.NET.Athena.NET.Lexer;
using Athena.NET.Athena.NET.Lexer.Structures;
using Athena.NET.Athena.NET.Parser.Interfaces;
using Athena.NET.Athena.NET.Parser.Nodes.DataNodes;

namespace Athena.NET.Athena.NET.Parser.Nodes.StatementNodes
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
            return base.TryParseRigthNode(out nodeResult, tokens);
        }
    }
}
