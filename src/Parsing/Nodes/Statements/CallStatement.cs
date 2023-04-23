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

    protected override bool TryParseLeftNode([NotNullWhen(true)] out NodeResult<INode> nodeResult, ReadOnlySpan<Token> tokens)
    {
        nodeResult = new SuccessulNodeResult<INode>(null!);
        return true;
    }

    protected override bool TryParseRigthNode([NotNullWhen(true)] out NodeResult<INode> nodeResult, ReadOnlySpan<Token> tokens)
    {
        int semicolonIndex = tokens.IndexOfToken(TokenIndentificator.Semicolon);
        int identiferIndex = tokens[..semicolonIndex].IndexOfToken(TokenIndentificator.Identifier);

        //TODO: Create better handling of errors
        if (identiferIndex == -1) 
        {
            nodeResult = new ErrorNodeResult<INode>("Identifier wasn't found");
            return false;
        }
        ReadOnlyMemory<INode> argumentNodes = GetArgumentsNodes(tokens[identiferIndex..]).ToArray();
        var definitionCallNode = new DefinitionCallNode(
                new IdentifierNode(tokens[identiferIndex].Data),
                argumentNodes
            );
        nodeResult = new SuccessulNodeResult<INode>(definitionCallNode);
        return true;
    }

    private static IEnumerable<INode> GetArgumentsNodes(ReadOnlySpan<Token> argumentsTokens)
    {
        int currentSeparatorIndex = argumentsTokens.IndexOfToken(TokenIndentificator.Separator);
        while (currentSeparatorIndex != -1)
        {
            ReadOnlySpan<Token> argumentTokens = argumentsTokens[..currentSeparatorIndex]; 
            argumentsTokens = argumentsTokens[(currentSeparatorIndex + 1)..];

            currentSeparatorIndex = argumentsTokens.IndexOfToken(TokenIndentificator.Separator);
            yield return GetArgumentNode(argumentTokens);
        }
    }

    private static INode GetArgumentNode(ReadOnlySpan<Token> argumentTokens)
    {
        int operatorIndex = OperatorHelper.IndexOfOperator(argumentTokens);
        if (OperatorHelper.TryGetOperator(out OperatorNode operatorNode, argumentTokens[operatorIndex].TokenId))
            return operatorNode;
        return argumentTokens.GetDataNode();
    }
}
