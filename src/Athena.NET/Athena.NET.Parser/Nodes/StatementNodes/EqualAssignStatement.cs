using Athena.NET.Athena.NET.Lexer;
using Athena.NET.Athena.NET.Lexer.Structures;
using Athena.NET.Athena.NET.Parser.Interfaces;
using Athena.NET.Athena.NET.Parser.Nodes.DataNodes;
using Athena.NET.Athena.NET.Parser.Nodes.OperatorNodes;

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
            //This is going to be changed to more
            //elegant solution, without unnecessary
            //index checking
            int operatorIndex = OperatorHelper.IndexOfOperator(tokens);
            if (operatorIndex != -1 && OperatorHelper.TryGetOperator(out OperatorNode operatorNode, tokens[operatorIndex].TokenId))
            {
                operatorNode.CreateNodes(tokens, operatorIndex);
                nodeResult = new SuccessulNodeResult<INode>(operatorNode);
                return true;
            }

            INode resultNode = null!;
            int identifierIndex = tokens.IndexOfToken(TokenIndentificator.Identifier);
            int tokenTypeIndex = tokens.IndexOfTokenType();
            if (identifierIndex != -1)
                resultNode = new IdentifierNode(tokens[identifierIndex].Data);
            if (tokenTypeIndex != -1) 
            {
                int currentData = int.Parse(tokens[tokenTypeIndex].Data.Span);
                resultNode = new DataNode<int>(tokens[tokenTypeIndex].TokenId, currentData);
            }

            nodeResult = resultNode is not null ? new SuccessulNodeResult<INode>(resultNode) :
                new ErrorNodeResult<INode>("Any valid node wasn't found");
            return resultNode is not null;
        }
    }
}
