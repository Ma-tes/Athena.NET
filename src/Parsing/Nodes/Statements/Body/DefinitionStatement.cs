using Athena.NET.ExceptionResult;
using Athena.NET.ExceptionResult.Interfaces;
using Athena.NET.Lexing;
using Athena.NET.Lexing.Structures;
using Athena.NET.Parsing.Interfaces;
using Athena.NET.Parsing.Nodes.Data;

namespace Athena.NET.Parsing.Nodes.Statements.Body;

internal sealed class DefinitionStatement : BodyStatement
{
    public override TokenIndentificator NodeToken { get; } =
        TokenIndentificator.Definition;

    protected override IResultProvider<INode> ExecuteParseLeftNode(ReadOnlySpan<Token> tokens)
    {
        if (!tokens.TryGetIndexOfToken(out int identifierIndex, TokenIndentificator.Identifier))
            return ErrorResult<INode>.CreateNullResult("Definition identifier token wasn't found", identifierIndex);

        int definitionIndex = tokens[..identifierIndex].IndexOfTokenType();
        TokenIndentificator definitionType = definitionIndex != -1 ?
            tokens[definitionIndex].TokenId : TokenIndentificator.Unknown;

        //TODO: Consider whenever, is this statement related and valid.
        if (!tokens.TryGetIndexOfToken(out _, TokenIndentificator.Definition))
            return ErrorResult<INode>.CreateNullResult("Definition token wasn't found", OriginalTokenIndex);

        Token identifierToken = tokens[identifierIndex];
        ReadOnlyMemory<InstanceNode> definitionArguments = GetArgumentInstances(tokens[(identifierIndex + 1)..]);

        var returnDefinitionNode = new DefinitionNode(definitionType, identifierToken, definitionArguments);
        return SuccessfulResult<INode>.Create<ParsingResult>(returnDefinitionNode, OriginalTokenIndex);
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
                returnInstances.Add(new InstanceNode(tokens[currentTokenTypeIndex].TokenId, currentIdentiferToken.Data));
            currentTokenTypeIndex = tokens[nextTokenIndex..].IndexOfTokenType() + nextTokenIndex;
        }
        return returnInstances.ToArray();
    }
}
