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

        public sealed override NodeResult<StatementNode> CreateStatementResult(ReadOnlyMemory<Token> tokens, int tokenIndex)
        {
            int invokerIndex = tokens.Span[tokenIndex..].IndexOfToken(invokerToken);
            int returnTokenIndex = invokerIndex == -1 ? tokenIndex : invokerIndex;
            return base.CreateStatementResult(tokens, returnTokenIndex);
        }

        protected sealed override bool TryParseRigthNode([NotNullWhen(true)] out NodeResult<INode> nodeResult, ReadOnlySpan<Token> tokens)
        {
            //TODO: Create GetFirstNode method in a NodeHelper,
            //where we then need to determine how to splittem
            //up. Easiest solution will probably be save of a length
            //in every node
            nodeResult = null!;
            return false;
        }
    }
}
