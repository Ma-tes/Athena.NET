using Athena.NET.Lexer;
using Athena.NET.Lexer.Structures;
using Athena.NET.Parser.Interfaces;
using Athena.NET.Parser.Nodes.DataNodes;
using System.Diagnostics.CodeAnalysis;

namespace Athena.NET.Parser.Nodes.StatementNodes.BodyStatements
{
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
}
