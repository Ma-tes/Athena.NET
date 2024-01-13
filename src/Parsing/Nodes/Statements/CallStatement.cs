using Athena.NET.Lexing;
using Athena.NET.Lexing.Structures;
using Athena.NET.Parsing.Interfaces;
using Athena.NET.Parsing.Nodes.Data;
using Athena.NET.Parsing.Nodes.Operators;
using System.Diagnostics.CodeAnalysis;

namespace Athena.NET.Parsing.Nodes.Statements;

internal sealed class CallStatement : StatementNode
{
    public override TokenIndentificator NodeToken { get; } =
        TokenIndentificator.DefinitionCall;

    public override NodeResult<INode> CreateStatementResult(ReadOnlySpan<Token> tokens, int tokenIndex) =>
        base.CreateStatementResult(tokens, tokenIndex + 2);

    protected override bool TryParseLeftNode([NotNullWhen(true)] out NodeResult<INode> nodeResult, ReadOnlySpan<Token> tokens)
    {
        nodeResult = tokens.TryGetIndexOfToken(out int identifierIndex, TokenIndentificator.Identifier) ?
            new SuccessulNodeResult<INode>(new IdentifierNode(tokens[identifierIndex].Data)) :
            new ErrorNodeResult<INode>("Identifier wasn't found");
        return nodeResult is SuccessulNodeResult<INode>;
    }

    protected override bool TryParseRigthNode([NotNullWhen(true)] out NodeResult<INode> nodeResult, ReadOnlySpan<Token> tokens)
    {
        int semicolonIndex = tokens.IndexOfToken(TokenIndentificator.Semicolon);
        ReadOnlyMemory<INode> argumentNodes = GetArgumentsNodes(tokens[..semicolonIndex]);

        var definitionCallNode = new DefinitionCallNode(argumentNodes);
        nodeResult = new SuccessulNodeResult<INode>(definitionCallNode);
        return true;
    }

    private static ReadOnlyMemory<INode> GetArgumentsNodes(ReadOnlySpan<Token> argumentsTokens)
    {
        var argumentNodesList = new List<INode>();
        int currentSeparatorIndex = argumentsTokens.IndexOfToken(TokenIndentificator.Separator);
        while (currentSeparatorIndex != -1)
        {
            ReadOnlySpan<Token> argumentTokens = argumentsTokens[..currentSeparatorIndex];
            argumentsTokens = argumentsTokens[(currentSeparatorIndex + 1)..];

            currentSeparatorIndex = argumentsTokens.IndexOfToken(TokenIndentificator.Separator);
            if(TryGetArgumentNode(out INode currentNode, argumentTokens))
                argumentNodesList.Add(currentNode);
        }
        if(TryGetArgumentNode(out INode argumentNode, argumentsTokens))
            argumentNodesList.Add(argumentNode);
        return argumentNodesList.ToArray();
    }

    private static bool TryGetArgumentNode([NotNullWhen(true)]out INode argumentNode, ReadOnlySpan<Token> argumentTokens)
    {
        int operatorIndex = OperatorHelper.IndexOfOperator(argumentTokens);
        if (operatorIndex != -1 && OperatorHelper.TryGetOperator(out OperatorNode operatorNode, argumentTokens[operatorIndex].TokenId)) 
        {
            argumentNode = operatorNode;
            return true;
        }
        argumentNode = argumentTokens.GetDataNode()!;
        return argumentNode is not null;
    }
}
