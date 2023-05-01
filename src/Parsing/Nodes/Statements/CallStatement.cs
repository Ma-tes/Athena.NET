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

    public override NodeResult<INode> CreateStatementResult(ReadOnlySpan<Token> tokens, int tokenIndex)
    {
        return base.CreateStatementResult(tokens, tokenIndex + 2);
    }
    protected override bool TryParseLeftNode([NotNullWhen(true)] out NodeResult<INode> nodeResult, ReadOnlySpan<Token> tokens)
    {
        int identiferIndex = tokens.IndexOfToken(TokenIndentificator.Identifier);
        //TODO: Create better handling of errors
        if (identiferIndex == -1) 
        {
            nodeResult = new ErrorNodeResult<INode>("Identifier wasn't found");
            return false;
        }
        nodeResult = new SuccessulNodeResult<INode>(new IdentifierNode(tokens[identiferIndex].Data));
        return true;
    }

    protected override bool TryParseRigthNode([NotNullWhen(true)] out NodeResult<INode> nodeResult, ReadOnlySpan<Token> tokens)
    {
        int semicolonIndex = tokens.IndexOfToken(TokenIndentificator.Semicolon);
        ReadOnlyMemory<INode> argumentNodes = GetArgumentsNodes(tokens[..semicolonIndex]).ToArray();
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
            argumentNodesList.Add(GetArgumentNode(argumentTokens));
            if(currentSeparatorIndex == -1)
                argumentNodesList.Add(GetArgumentNode(argumentsTokens));
        }
        return argumentNodesList.ToArray();
    }

    private static INode GetArgumentNode(ReadOnlySpan<Token> argumentTokens)
    {
        int operatorIndex = OperatorHelper.IndexOfOperator(argumentTokens);
        if (operatorIndex != -1 && OperatorHelper.TryGetOperator(out OperatorNode operatorNode, argumentTokens[operatorIndex].TokenId))
            return operatorNode;
        return argumentTokens.GetDataNode();
    }
}
