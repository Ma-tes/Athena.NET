using Athena.NET.Athena.NET.Lexer;
using Athena.NET.Athena.NET.Lexer.Structures;
using Athena.NET.Athena.NET.Parser.Interfaces;
using Athena.NET.Athena.NET.Parser.Nodes.DataNodes;
using System.Diagnostics.CodeAnalysis;

namespace Athena.NET.Athena.NET.Parser.Nodes.StatementNodes.BodyStatements
{
    internal abstract class BodyStatement : StatementNode
    {
        private static readonly TokenIndentificator invokerToken =
            TokenIndentificator.Invoker;
        //TODO: Move this to BodyNode.cs
        public int BodyLength { get; private set; }

        public sealed override NodeResult<INode> CreateStatementResult(ReadOnlySpan<Token> tokens, int tokenIndex)
        {
            int invokerIndex = tokens[tokenIndex..].IndexOfToken(invokerToken);
            int returnTokenIndex = invokerIndex == -1 ? tokenIndex : invokerIndex;
            BodyLength = invokerIndex;

            return base.CreateStatementResult(tokens, returnTokenIndex);
        }

        protected sealed override bool TryParseRigthNode([NotNullWhen(true)] out NodeResult<INode> nodeResult, ReadOnlySpan<Token> tokens)
        {
            ReadOnlySpan<Token> bodyTokens = GetBodyTokens(tokens);
            BodyLength += bodyTokens.Length;

            var bodyNodes = bodyTokens.CreateNodes();
            var bodyNode = new BodyNode(bodyNodes.ToArray());
            nodeResult = new SuccessulNodeResult<INode>(bodyNode);
            return true;
        }

        private ReadOnlySpan<Token> GetBodyTokens(ReadOnlySpan<Token> tokens) 
        {
            Span<Token> returnBodyNodes = new Token[tokens.Length];

            int currentBodyLength = 0;
            int currentTabulatorIndex = tokens.IndexOfToken(TokenIndentificator.Tabulator);
            while (currentTabulatorIndex != -1)
            {
                var shiftedTokens = tokens[(currentTabulatorIndex + 1)..];
                int nextTabulatorIndex = IndexOfLineTabulator(shiftedTokens);
                ReadOnlySpan<Token> bodyNodes = nextTabulatorIndex != -1 ?
                    shiftedTokens[0..(nextTabulatorIndex - 1)] :
                    shiftedTokens[0..(shiftedTokens.IndexOfToken(TokenIndentificator.EndLine))];

                bodyNodes.CopyTo(returnBodyNodes[currentBodyLength..]);
                currentBodyLength += bodyNodes.Length;
                currentTabulatorIndex = nextTabulatorIndex == -1 ? nextTabulatorIndex :
                    currentTabulatorIndex + nextTabulatorIndex;
            }
            return returnBodyNodes[0..(currentBodyLength)];
        }

        private int IndexOfLineTabulator(ReadOnlySpan<Token> tokens) 
        {
            int currentOperatorIndex = tokens.IndexOfToken(TokenIndentificator.Tabulator);
            while (currentOperatorIndex != -1) 
            {
                if (currentOperatorIndex != 0 &&
                    tokens[currentOperatorIndex - 1].TokenId == TokenIndentificator.EndLine)
                    return currentOperatorIndex;
                currentOperatorIndex = tokens[(currentOperatorIndex + 1)..].IndexOfToken(TokenIndentificator.Tabulator);
            }
            return -1;
        }
    }
}
