using Athena.NET.ExceptionResult;
using Athena.NET.ExceptionResult.Interfaces;
using Athena.NET.Lexing;
using Athena.NET.Lexing.Structures;
using Athena.NET.Parsing.Interfaces;
using Athena.NET.Parsing.Nodes.Data;
using System.Diagnostics.CodeAnalysis;

namespace Athena.NET.Parsing.Nodes.Operators;

internal abstract class OperatorNode : IEvaluationNode
{
    private static readonly Type evaluationType = typeof(IEvaluationNode);

    public abstract OperatorPrecedence Precedence { get; }
    public abstract TokenIndentificator NodeToken { get; }

    public ChildrenNodes ChildNodes { get; private set; }

    public OperatorNode() { }

    public IResultProvider<INode> CreateStatementResult(ReadOnlySpan<Token> tokens, int tokenIndex)
    {
        ChildNodes = SepareteNodes(tokens, tokenIndex);
        if (ChildNodes.LeftNode is null || ChildNodes.LeftNode is null)
            return ErrorResult<INode>.Create($"Parsing the nodes from {nameof(OperatorNode)} went wrong.");
        return SuccessfulResult<INode>.Create<ParsingResult>(this, tokenIndex);
    }

    private ChildrenNodes SepareteNodes(ReadOnlySpan<Token> tokens, int nodeIndex)
    {
        ReadOnlySpan<Token> leftTokens = tokens[..nodeIndex];
        int semicolonIndex = tokens.IndexOfToken(TokenIndentificator.Semicolon);
        int rightLength = semicolonIndex > nodeIndex ? semicolonIndex : tokens.Length;

        ReadOnlySpan<Token> rightTokens = tokens[(nodeIndex + 1)..rightLength];
        INode leftNode = GetChildrenNode(leftTokens);
        INode rightNode = GetChildrenNode(rightTokens);
        return new(leftNode, rightNode);
    }

    public INode GetChildrenNode(ReadOnlySpan<Token> tokens)
    {
        int operatorIndex = OperatorHelper.IndexOfOperator(tokens);
        if (operatorIndex == -1)
        {
            if (!tokens.TryGetIndexOfToken(out int valueIndex, TokenIndentificator.Int))
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
        bool isEvaluate = node.GetType().IsAssignableTo(evaluationType);
        evaluationNode = isEvaluate ? (IEvaluationNode)node : null!;
        return isEvaluate;
    }
}
