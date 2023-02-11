using Athena.NET.Athena.NET.Lexer;
using Athena.NET.Athena.NET.Lexer.Structures;
using Athena.NET.Athena.NET.Parser.Interfaces;

namespace Athena.NET.Athena.NET.Parser.Nodes.StatementNodes
{
    internal abstract class StatementNode : INode
    {
        public abstract TokenIndentificator NodeToken { get; }
        public ChildrenNodes ChildNodes { get; }

        public StatementResult CreateStatementResult(ReadOnlyMemory<Token> tokens, int tokenIndex) 
        {
            //Actually this token checking is probably
            //redudant, by still I will leave it here
            //for debugging
            var statementToken = tokens.Span[tokenIndex].TokenId;
            if(statementToken != NodeToken)
                return StatementResult.ErrorResult();

            if (TryParseStatement(tokens, tokenIndex))
                return StatementResult.SuccessfulResult(this);
            return StatementResult.ErrorResult();
        }

        protected abstract bool TryParseStatement(ReadOnlyMemory<Token> tokens, int tokenIndex);
    }
}
