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

        ReadOnlyMemory<InstanceNode> definitionArguments = GetArgumentInstances(tokens[(definitionTokenIndex + 1)..]);
        var returnDefinitionNode = new DefinitionNode(definitionType, definitionArguments);
        nodeResult = new SuccessulNodeResult<INode>(returnDefinitionNode);
        return true;
    }

    private ReadOnlyMemory<InstanceNode> GetArgumentInstances(ReadOnlySpan<Token> tokens)
    {
        var returnInstances = new List<InstanceNode>();
        int currentTokenTypeIndex = tokens.IndexOfTokenType();
        while (currentTokenTypeIndex != -1)
        {
            int nextTokenIndex = currentTokenTypeIndex + 1;
            Token currentIdentiferToken = tokens[nextTokenIndex];
            if (currentIdentiferToken.TokenId != TokenIndentificator.Identifier)
                return null;
            returnInstances.Add(new(tokens[currentTokenTypeIndex].TokenId, currentIdentiferToken.Data));
            currentTokenTypeIndex = tokens[nextTokenIndex..].IndexOfTokenType();
        }
        return returnInstances.ToArray();
    }
}
