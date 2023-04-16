using Athena.NET.Lexing;
using Athena.NET.Lexing.Structures;
using Athena.NET.Parsing.Interfaces;
using Athena.NET.Parsing.Nodes.Data;
using System.Diagnostics.CodeAnalysis;

namespace Athena.NET.Parsing.Nodes.Statements.Body;

internal sealed class DefinitionStatement : BodyStatement
{
    public override TokenIndentificator NodeToken { get; } =
        TokenIndentificator.Definition;

    protected override bool TryParseLeftNode([NotNullWhen(true)] out NodeResult<INode> nodeResult, ReadOnlySpan<Token> tokens)
    {
        int definitionIndex = tokens.IndexOfTokenType();
        TokenIndentificator definitionType = definitionIndex != -1 ?
            tokens[definitionIndex].TokenId : TokenIndentificator.Unknown;

        int definitionTokenIndex = tokens.IndexOfToken(TokenIndentificator.Definition);
        if (definitionTokenIndex == -1)
        {
            nodeResult = new ErrorNodeResult<INode>("Definition token wasn't found");
            return false;
        }

        int identifierIndex = tokens.IndexOfToken(TokenIndentificator.Identifier);
        if (identifierIndex == -1) 
        {
            nodeResult = new ErrorNodeResult<INode>("Definition identifier token wasn't found");
            return false;
        }

        Token identifierToken = tokens[identifierIndex];
        ReadOnlyMemory<InstanceNode> definitionArguments = GetArgumentInstances(tokens[(identifierIndex + 1)..]);
        var returnDefinitionNode = new DefinitionNode(definitionType, identifierToken, definitionArguments);
        nodeResult = new SuccessulNodeResult<INode>(returnDefinitionNode);
        return true;
    }

    //TODO: Improve this implementation
    private ReadOnlyMemory<InstanceNode> GetArgumentInstances(ReadOnlySpan<Token> tokens)
    {
        var returnInstances = new List<InstanceNode>();
        int currentTokenTypeIndex = tokens.IndexOfTokenType();
        while (currentTokenTypeIndex != tokens.Length)
        {
            int nextTokenIndex = currentTokenTypeIndex + 2;
            if(nextTokenIndex >= tokens.Length)
                return returnInstances.ToArray();

            Token currentIdentiferToken = tokens[nextTokenIndex];
            if (currentIdentiferToken.TokenId == TokenIndentificator.Identifier) 
                returnInstances.Add(new(tokens[currentTokenTypeIndex].TokenId, currentIdentiferToken.Data));
            currentTokenTypeIndex = (tokens[nextTokenIndex..].IndexOfTokenType() + nextTokenIndex);
        }
        return returnInstances.ToArray();
    }
}
