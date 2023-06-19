using Athena.NET.Lexing;
using Athena.NET.Lexing.Structures;
using Athena.NET.Parsing.Interfaces;
using Athena.NET.Parsing.Nodes.Data;
using System.Diagnostics.CodeAnalysis;

namespace Athena.NET.Parsing.Nodes.Statements.Body;

/// <summary>
/// Definines and parse right node, from
/// every statement, with related tokens.
/// </summary>
internal abstract class BodyStatement : StatementNode
{
    private static readonly TokenIndentificator invokerToken =
        TokenIndentificator.Invoker;
    private int tabulatorCount;

    /// <summary>
    /// Provides count of all tokens, that
    /// are after <see cref="TokenIndentificator.Invoker"/>.
    /// </summary>
    public int BodyLength { get; private set; }

    public sealed override NodeResult<INode> CreateStatementResult(ReadOnlySpan<Token> tokens, int tokenIndex)
    {
        int invokerIndex = tokens[tokenIndex..].IndexOfToken(invokerToken);
        int returnTokenIndex = invokerIndex == -1 ? tokenIndex : tokenIndex + invokerIndex;
        tabulatorCount = GetTabulatorCount(tokens[..returnTokenIndex]) + 1;

        return base.CreateStatementResult(tokens, returnTokenIndex);
    }

    protected sealed override bool TryParseRigthNode([NotNullWhen(true)] out NodeResult<INode> nodeResult, ReadOnlySpan<Token> tokens)
    {
        ReadOnlySpan<Token> bodyTokens = GetBodyTokens(tokens);
        BodyLength = bodyTokens.Length;
        ReadOnlyMemory<INode> bodyNodes = bodyTokens.CreateNodes();

        var bodyNode = new BodyNode(bodyNodes);
        nodeResult = new SuccessulNodeResult<INode>(bodyNode);
        return true;
    }

    //TODO: Make sure that sepration is relative to
    //a tabulator count on the individual body statement
    /// <summary>
    /// Gets all related tokens, that after <see cref="TokenIndentificator.Invoker"/>
    /// and with specific count of <see cref="TokenIndentificator.Tabulator"/>.
    /// </summary>
    private ReadOnlySpan<Token> GetBodyTokens(ReadOnlySpan<Token> tokens)
    {
        Span<Token> returnBodyNodes = new Token[tokens.Length];

        int currentBodyLength = 0;
        int currentTabulatorIndex = tokens.IndexOfToken(TokenIndentificator.Tabulator);
        while (currentTabulatorIndex != -1)
        {
            ReadOnlySpan<Token> shiftedTokens = tokens[currentTabulatorIndex..];
            int nextTabulatorIndex = IndexOfLineTabulator(shiftedTokens);
            ReadOnlySpan<Token> bodyNodes = nextTabulatorIndex != -1 ?
                shiftedTokens[..nextTabulatorIndex] :
                shiftedTokens[..(shiftedTokens.IndexOfToken(TokenIndentificator.EndLine) + 1)];

            bodyNodes.CopyTo(returnBodyNodes[currentBodyLength..]);
            currentBodyLength += bodyNodes.Length;
            currentTabulatorIndex = nextTabulatorIndex == -1 ? nextTabulatorIndex :
                currentTabulatorIndex + nextTabulatorIndex;
        }
        return returnBodyNodes[..(currentBodyLength)];
    }

    /// <summary>
    /// Calculates count of <see cref="TokenIndentificator.Tabulator"/>,
    /// in related <paramref name="tokens"/>.
    /// </summary>
    private int GetTabulatorCount(ReadOnlySpan<Token> tokens) 
    {
        int currentIndex = 0;
        int firstTabulatorIndex = tokens.IndexOfToken(TokenIndentificator.Tabulator);
        while (firstTabulatorIndex != -1 && currentIndex < tokens.Length)
        {
            Token currentToken = tokens[(firstTabulatorIndex + currentIndex)];
            if (currentToken.TokenId != TokenIndentificator.Tabulator)
                return currentIndex;
            currentIndex++;
        }
        return 0;
    }

    /// <summary>
    /// Gets index of first <see cref="TokenIndentificator.Tabulator"/>,
    /// that is after <see cref="TokenIndentificator.EndLine"/> in <paramref name="tokens"/>.
    /// </summary>
    /// <returns>
    /// First index of related <see cref="TokenIndentificator.Tabulator"/>, if it was found,
    /// otherwise it will return -1.
    /// </returns>
    private int IndexOfLineTabulator(ReadOnlySpan<Token> tokens)
    { 
        int currentOperatorIndex = tokens.IndexOfToken(TokenIndentificator.EndLine);
        int lastTabulatorIndex = currentOperatorIndex + tabulatorCount;
        if (lastTabulatorIndex < tokens.Length && (currentOperatorIndex != -1) &&
            (tokens[lastTabulatorIndex].TokenId == TokenIndentificator.Tabulator))
            return lastTabulatorIndex;
        return -1;
    }
}
