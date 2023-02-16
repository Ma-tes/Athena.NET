using Athena.NET.Athena.NET.Lexer;
using Athena.NET.Athena.NET.Lexer.Structures;
using Athena.NET.Athena.NET.Parser.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Athena.NET.Athena.NET.Parser.Nodes.StatementNodes
{
    internal abstract class StatementNode : INode
    {
        public abstract TokenIndentificator NodeToken { get; }

        public ChildrenNodes ChildNodes { get; private set; }

        //TODO: Reduce the amount of nullable types
        public virtual NodeResult<StatementNode> CreateStatementResult(ReadOnlyMemory<Token> tokens, int tokenIndex)
        {
            //Actually this token checking is probably
            //redudant, by still... I will leave it just
            //for debugging
            var statementToken = tokens.Span[tokenIndex].TokenId;
            if (statementToken != NodeToken)
                return new ErrorNodeResult<StatementNode>("Statement node wasn't found in array of tokens");

            var leftData = tokens[0..tokenIndex];
            var rightData = tokens[(tokenIndex + 1)..];

            if (!TryParseLeftNode(out NodeResult<INode> leftResult, leftData.Span) && leftResult is not null)
                return new ErrorNodeResult<StatementNode>(leftResult.Message);
            if (!TryParseRigthNode(out NodeResult<INode> rightResult, rightData.Span) && rightResult is not null)
                return new ErrorNodeResult<StatementNode>(rightResult.Message);

            //TODO: Create a better handling of another errors
            //Probably I should return the error message from
            //the TryParseStatement
            ChildNodes = new(leftResult!.Node!, rightResult!.Node!);
            return new SuccessulNodeResult<StatementNode>(this);
        }

        protected virtual bool TryParseLeftNode([NotNullWhen(true)]out NodeResult<INode> nodeResult, ReadOnlySpan<Token> tokens) 
        {
            nodeResult = null!;
            return false;
        }

        protected virtual bool TryParseRigthNode([NotNullWhen(true)]out NodeResult<INode> nodeResult, ReadOnlySpan<Token> tokens) 
        {
            nodeResult = null!;
            return false;
        }
    }
}
