using Athena.NET.Lexer;
using Athena.NET.Lexer.Structures;
using Athena.NET.Parser.Interfaces;
using Athena.NET.Parser.Nodes.DataNodes;
using System.Diagnostics.CodeAnalysis;

namespace Athena.NET.Parser.Nodes.OperatorNodes
{
    internal abstract class OperatorNode : IEvaluationNode
    {
        public abstract OperatorPrecedence Precedence { get; }
        public abstract TokenIndentificator NodeToken { get; }

        public ChildrenNodes ChildNodes { get; private set; }

        public OperatorNode() { }

        public NodeResult<INode> CreateStatementResult(ReadOnlySpan<Token> tokens, int tokenIndex)
        {
            ChildNodes = SepareteNodes(tokens, tokenIndex);
            if (ChildNodes.LeftNode is null || ChildNodes.LeftNode is null)
                return new ErrorNodeResult<INode>($"Parsing nodes from token {tokens[tokenIndex]} wen't wrong");
            return new SuccessulNodeResult<INode>(this);
        }

        private ChildrenNodes SepareteNodes(ReadOnlySpan<Token> tokens, int nodeIndex)
        {
            ReadOnlySpan<Token> leftTokens = tokens[..nodeIndex];
            int semicolonIndex = tokens.IndexOfToken(TokenIndentificator.Semicolon);
            int rightLength = semicolonIndex > nodeIndex ? semicolonIndex : tokens.Length;

            ReadOnlySpan<Token> rightTokens = tokens[(nodeIndex + 1)..(rightLength)];
            INode leftNode = GetChildrenNode(leftTokens);
            INode rightNode = GetChildrenNode(rightTokens);
            return new(leftNode, rightNode);
        }

        public INode GetChildrenNode(ReadOnlySpan<Token> tokens)
        {
            int operatorIndex = OperatorHelper.IndexOfOperator(tokens);
            if (operatorIndex == -1)
            {
                int valueIndex = tokens.IndexOfToken(TokenIndentificator.Int);
                if (valueIndex == -1)
                {
                    int idetifierIndex = tokens.IndexOfToken(TokenIndentificator.Identifier);
                    var idetifierNode = new IdentifierNode(tokens[idetifierIndex].Data);
                    return idetifierNode;
                }

                int currentData = int.Parse(tokens[valueIndex].Data.Span);
                var returnNode = new DataNode<int>(tokens[valueIndex].TokenId, currentData);
                return returnNode;
            }

            TokenIndentificator currentOperator = tokens[operatorIndex].TokenId;
            if (!OperatorHelper.TryGetOperator(out OperatorNode returnOperatorNode, tokens[operatorIndex].TokenId))
                throw new Exception($"Operator with token: {currentOperator} wasn't implemented");

            _ = returnOperatorNode.CreateStatementResult(tokens, operatorIndex);
            return returnOperatorNode;
        }

        public void Evaluate()
        {
            INode evaluatedLeftNode = GetEvaluatedNode(ChildNodes.LeftNode);
            INode evaluatedRightNode = GetEvaluatedNode(ChildNodes.RightNode);

            ChildNodes = new(evaluatedLeftNode, evaluatedRightNode);
        }

        public abstract int CalculateData(int firstData, int secondData);

        private INode GetEvaluatedNode(INode node)
        {
            if (TryGetEvaluateNode(out IEvaluationNode currentNode, node))
            {
                currentNode.Evaluate();
                if (currentNode.ChildNodes.LeftNode is DataNode<int> leftData &&
                    currentNode.ChildNodes.RightNode is DataNode<int> rightData &&
                    currentNode is OperatorNode currentOperator)
                {
                    int returnData = currentOperator.CalculateData(leftData.NodeData, rightData.NodeData);
                    return new DataNode<int>(TokenIndentificator.Int, returnData);
                }
            }
            return node;
        }

        private bool TryGetEvaluateNode([NotNullWhen(true)] out IEvaluationNode evaluationNode, INode node)
        {
            bool isEvaluate = node.GetType().IsAssignableTo(typeof(IEvaluationNode));
            evaluationNode = isEvaluate ? (IEvaluationNode)node : null!;
            return isEvaluate;
        }
    }
}
