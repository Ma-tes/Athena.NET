using Athena.NET.Athena.NET.Lexer;
using Athena.NET.Athena.NET.Lexer.Structures;
using Athena.NET.Athena.NET.Parser.Interfaces;
using Athena.NET.Athena.NET.Parser.Nodes.DataNodes;
using System.Diagnostics.CodeAnalysis;

namespace Athena.NET.Athena.NET.Parser.Nodes
{
    internal abstract class OperatorNode : IEvaluationNode
    {
        public abstract OperatorPrecedence Precedence { get; }
        public abstract TokenIndentificator NodeToken { get; }

        public ChildrenNodes ChildNodes { get; private set; }
        public int ChildNodesCount { get; } = 0;

        public OperatorNode() { }

        public void CreateNodes(ReadOnlyMemory<Token> tokens, int nodeIndex) 
        {
            ChildNodes = SepareteNodes(tokens, nodeIndex);
        }

        //TODO: Create a better readability of this
        //implementation
        private ChildrenNodes SepareteNodes(ReadOnlyMemory<Token> tokens, int nodeIndex)
        {
            var tokensSpan = tokens.Span;
            int leftIdentfierIndex = OperatorHelper.IndexOfOperatorNextToken(tokensSpan, TokenIndentificator.Int, nodeIndex);

            //TODO: Make sure that everything will be
            //generics for every primitive type
            INode leftNode;
            ReadOnlyMemory<Token> rightTokens = leftIdentfierIndex < nodeIndex ? tokens[(nodeIndex + 1)..] :
                    tokens[0..(nodeIndex - 1)];
            if (leftIdentfierIndex == -1)
            {
                int idetifierIndex = OperatorHelper.IndexOfToken(tokensSpan, TokenIndentificator.Identifier);
                leftNode = new IdentifierNode(tokensSpan[idetifierIndex].Data);
                rightTokens = tokens[0..(nodeIndex - 1)];
            }
            else 
            {
                int leftData = int.Parse(tokensSpan[leftIdentfierIndex].Data.Span);
                leftNode = new DataNode<int>(tokensSpan[leftIdentfierIndex].TokenId, leftData);

            }

            var rightTokensSpan = rightTokens.Span;
            int rightOperatorIndex = OperatorHelper.IndexOfOperator(rightTokensSpan);
            if (rightOperatorIndex == -1) 
            {
                int rightIdentfierIndex = OperatorHelper.IndexOfToken(rightTokensSpan, TokenIndentificator.Int);
                if (rightIdentfierIndex == -1) 
                {
                    int idetifierIndex = OperatorHelper.IndexOfToken(rightTokensSpan, TokenIndentificator.Identifier);
                    var idetifierNode = new IdentifierNode(rightTokensSpan[idetifierIndex].Data);
                    return new(leftNode, idetifierNode);
                }

                int rightData = int.Parse(rightTokensSpan[rightIdentfierIndex].Data.Span);

                var rightNode = new DataNode<int>(rightTokensSpan[rightIdentfierIndex].TokenId, rightData);
                return new(leftNode, rightNode);
            }
            var currentOperator = rightTokensSpan[rightOperatorIndex].TokenId;
            if (!OperatorHelper.TryGetOperator(out OperatorNode rightOperatorNode, rightTokensSpan[rightOperatorIndex].TokenId))
                throw new Exception($"Operator with token: {currentOperator} wasn't implemented");

            rightOperatorNode.CreateNodes(rightTokens, rightOperatorIndex);
            return new(leftNode, rightOperatorNode);
        }

        public void Evaluate() 
        {
            var evaluatedLeftNode = GetEvaluatedNode(ChildNodes.LeftNode);
            var evaluatedRightNode = GetEvaluatedNode(ChildNodes.RightNode);

            ChildNodes = new(evaluatedLeftNode, evaluatedRightNode);
        }

        protected abstract int CalculateData(int firstData, int secondData);
 
        private INode GetEvaluatedNode(INode node) 
        {
            if(TryGetEvaluateNode(out IEvaluationNode currentNode, node))
            {
                currentNode.Evaluate();
                if (currentNode.ChildNodes.LeftNode is DataNode<int> leftData &&
                    currentNode.ChildNodes.RightNode is DataNode<int> rightData &&
                    currentNode is OperatorNode currentOperator) 
                {
                    var returnData = currentOperator.CalculateData(leftData.NodeData, rightData.NodeData);
                    return new DataNode<int>(TokenIndentificator.Int, returnData);
                }
            }
            return node;
        }

        private bool TryGetEvaluateNode([NotNullWhen(true)]out IEvaluationNode evaluationNode, INode node)
        {
            bool isEvaluate = node.GetType().IsAssignableTo(typeof(IEvaluationNode));
            evaluationNode = isEvaluate ? (IEvaluationNode)node : null!;
            return isEvaluate;
        }
    }
}
