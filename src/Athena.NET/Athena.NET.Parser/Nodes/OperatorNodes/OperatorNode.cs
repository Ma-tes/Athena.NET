using Athena.NET.Athena.NET.Lexer;
using Athena.NET.Athena.NET.Lexer.Structures;
using Athena.NET.Athena.NET.Parser.Interfaces;
using Athena.NET.Athena.NET.Parser.Nodes.DataNodes;
using System.Diagnostics.CodeAnalysis;

namespace Athena.NET.Athena.NET.Parser.Nodes.OperatorNodes
{
    internal abstract class OperatorNode : IEvaluationNode
    {
        public abstract OperatorPrecedence Precedence { get; }
        public abstract TokenIndentificator NodeToken { get; }

        public ChildrenNodes ChildNodes { get; private set; }

        public OperatorNode() { }

        public void CreateNodes(ReadOnlyMemory<Token> tokens, int nodeIndex)
        {
            ChildNodes = SepareteNodes(tokens, nodeIndex);
        }

        private ChildrenNodes SepareteNodes(ReadOnlyMemory<Token> tokens, int nodeIndex)
        {
            var leftData = tokens[0..nodeIndex];
            var rightData = tokens[(nodeIndex + 1)..];

            INode leftNode = GetChildrenNode(leftData);
            INode rightNode = GetChildrenNode(rightData);
            return new(leftNode, rightNode);
        }

        public INode GetChildrenNode(ReadOnlyMemory<Token> tokens)
        {
            var tokensSpan = tokens.Span;
            int operatorIndex = OperatorHelper.IndexOfOperator(tokensSpan);
            if (operatorIndex == -1)
            {
                int identfierIndex = tokensSpan.IndexOfToken(TokenIndentificator.Int);
                if (identfierIndex == -1)
                {
                    int idetifierIndex = tokensSpan.IndexOfToken(TokenIndentificator.Identifier);
                    var idetifierNode = new IdentifierNode(tokensSpan[idetifierIndex].Data);
                    return idetifierNode;
                }

                int currentData = int.Parse(tokensSpan[identfierIndex].Data.Span);
                var returnNode = new DataNode<int>(tokensSpan[identfierIndex].TokenId, currentData);
                return returnNode;
            }

            var currentOperator = tokensSpan[operatorIndex].TokenId;
            if (!OperatorHelper.TryGetOperator(out OperatorNode returnOperatorNode, tokensSpan[operatorIndex].TokenId))
                throw new Exception($"Operator with token: {currentOperator} wasn't implemented");

            returnOperatorNode.CreateNodes(tokens, operatorIndex);
            return returnOperatorNode;
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
            if (TryGetEvaluateNode(out IEvaluationNode currentNode, node))
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

        private bool TryGetEvaluateNode([NotNullWhen(true)] out IEvaluationNode evaluationNode, INode node)
        {
            bool isEvaluate = node.GetType().IsAssignableTo(typeof(IEvaluationNode));
            evaluationNode = isEvaluate ? (IEvaluationNode)node : null!;
            return isEvaluate;
        }
    }
}
