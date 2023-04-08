using Athena.NET.Lexer;
using Athena.NET.Lexer.Structures;
using Athena.NET.Parser.Interfaces;
using Athena.NET.Parser.Nodes.OperatorNodes;
using System.Diagnostics.CodeAnalysis;

namespace Athena.NET.Parser.Nodes.StatementNodes
{
    internal sealed class PrintStatement : StatementNode
    {
        public override TokenIndentificator NodeToken { get; } =
            TokenIndentificator.Print;

        protected override bool TryParseLeftNode([NotNullWhen(true)] out NodeResult<INode> nodeResult, ReadOnlySpan<Token> tokens) 
        {
            nodeResult = new SuccessulNodeResult<INode>(null!);
            return true;
        }

        protected override bool TryParseRigthNode([NotNullWhen(true)] out NodeResult<INode> nodeResult, ReadOnlySpan<Token> tokens)
        {
            int semicolonIndex = tokens.IndexOfToken(TokenIndentificator.Semicolon);
            if (OperatorHelper.TryGetOperatorResult(out nodeResult, tokens[..semicolonIndex]))
                return true;

            INode resultNode = tokens[..semicolonIndex].GetDataNode();
            nodeResult = resultNode is not null ? new SuccessulNodeResult<INode>(resultNode) :
                new ErrorNodeResult<INode>("Any valid node wasn't found");
            return resultNode is not null;
        }
    }
}
