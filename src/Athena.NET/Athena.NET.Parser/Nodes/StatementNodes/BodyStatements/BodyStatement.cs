using Athena.NET.Athena.NET.Lexer;
using Athena.NET.Athena.NET.Lexer.Structures;
using Athena.NET.Athena.NET.Parser.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Athena.NET.Athena.NET.Parser.Nodes.StatementNodes.BodyStatements
{
    internal abstract class BodyStatement : StatementNode
    {
        private static readonly TokenIndentificator invokerToken =
            TokenIndentificator.Invoker;

        public ReadOnlyMemory<INode> BodyNodes { get; private set; } 
        public int BodyLength { get; private set; }

        public sealed override NodeResult<StatementNode> CreateStatementResult(ReadOnlyMemory<Token> tokens, int tokenIndex)
        {
            int invokerIndex = tokens.Span[tokenIndex..].IndexOfToken(invokerToken);
            int returnTokenIndex = invokerIndex == -1 ? tokenIndex : invokerIndex;
            return base.CreateStatementResult(tokens, returnTokenIndex);
        }

        protected sealed override bool TryParseRigthNode([NotNullWhen(true)] out NodeResult<INode> nodeResult, ReadOnlySpan<Token> tokens)
        {
            ReadOnlySpan<Token> bodyTokens = GetBodyTokens(tokens);

            nodeResult = null!;
            return false;
        }

        private ReadOnlySpan<Token> GetBodyTokens(ReadOnlySpan<Token> tokens) 
        {
            Span<Token> returnBodyNodes = new Token[tokens.Length];

            int currentBodyLength = 0;
            int currentTabulatorIndex = tokens.IndexOfToken(TokenIndentificator.Tabulator);
            while (currentTabulatorIndex != -1)
            {
                var shiftedTokens = tokens[(currentTabulatorIndex + 1)..];
                int nextTabulatorIndex = IndexOfLineTabulator(tokens);
                ReadOnlySpan<Token> bodyNodes = nextTabulatorIndex != -1 ?
                    shiftedTokens[0..(nextTabulatorIndex)] :
                    shiftedTokens[0..(shiftedTokens.IndexOfToken(TokenIndentificator.EndLine))];

                bodyNodes.CopyTo(returnBodyNodes[currentBodyLength..]);
                currentBodyLength += bodyNodes.Length;
                currentTabulatorIndex = nextTabulatorIndex;
            }
            return returnBodyNodes;
        }

        private int IndexOfLineTabulator(ReadOnlySpan<Token> tokens) 
        {
            int currentOperatorIndex = tokens.IndexOfToken(TokenIndentificator.Tabulator);
            while (currentOperatorIndex != -1) 
            {
                if (currentOperatorIndex != 0 &&
                    tokens[currentOperatorIndex - 1].TokenId == TokenIndentificator.EndLine)
                    return currentOperatorIndex;

                int nextEndLine = tokens[currentOperatorIndex..].IndexOfToken(TokenIndentificator.EndLine);
                currentOperatorIndex = tokens[nextEndLine..].IndexOfToken(TokenIndentificator.Tabulator) + nextEndLine;
            }
            return -1;
        }
    }
}
