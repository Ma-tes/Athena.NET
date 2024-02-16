using Athena.NET.ExceptionResult;
using Athena.NET.ExceptionResult.Interfaces;
using Athena.NET.Lexing;
using Athena.NET.Lexing.Structures;
using Athena.NET.Parsing.Interfaces;
using Athena.NET.Parsing.Nodes.Operators;
using System.Diagnostics.CodeAnalysis;

namespace Athena.NET.Parsing.Nodes.Statements;

/// <summary>
/// Definines a structure of node, that's
/// being separated by <see cref="Token"/>,
/// to a left and right portion.
/// </summary>
internal abstract class StatementNode : INode
{
    protected int OriginalTokenIndex { get; private set; }

    public abstract TokenIndentificator NodeToken { get; }
    public ChildrenNodes ChildNodes { get; internal set; }

    public virtual IResultProvider<INode> CreateStatementResult(ReadOnlySpan<Token> tokens, int tokenIndex)
    {
        ReadOnlySpan<Token> leftTokens = tokens[..tokenIndex];
        ReadOnlySpan<Token> rightTokens = tokens[(tokenIndex + 1)..];

        IResultProvider<INode> leftNodeResult = ExecuteParseLeftNode(leftTokens);
        IResultProvider<INode> rightNodeResult = ExecuteParseRigthNode(rightTokens);

        if (!TryGetStatementResultValue(out INode leftNode, leftNodeResult)) return leftNodeResult;
        if (!TryGetStatementResultValue(out INode rightNode, rightNodeResult)) return rightNodeResult;

        ChildNodes = new ChildrenNodes(leftNode, rightNode);
        OriginalTokenIndex = tokenIndex;
        return SuccessfulResult<INode>.Create<ParsingResult>(this, tokenIndex);
    }

    /// <summary>
    /// Provides parsing a left portion of statement <paramref name="tokens"/>.
    /// </summary>
    /// <param name="tokens">Specifically splitted portion of tokens.</param>
    /// <returns>
    /// Related result of <see cref="IResultProvider{T}"/>, which is only going to
    /// be equal to<see cref="SuccessfulResult{T}"/> or <see cref="ErrorResult{T}"/>,
    /// with related type of <see cref="INode"/>.
    /// </returns>
    protected virtual IResultProvider<INode> ExecuteParseLeftNode(ReadOnlySpan<Token> tokens) => null!;

    /// <summary>
    /// Provides parsing a rigth portion of statement <paramref name="tokens"/>.
    /// </summary>
    /// <param name="tokens">Specifically splitted portion of tokens.</param>
    /// <returns>
    /// Related result of <see cref="IResultProvider{T}"/>, which is only going to
    /// be equal to<see cref="SuccessfulResult{T}"/> or <see cref="ErrorResult{T}"/>,
    /// with related type of <see cref="INode"/>.
    /// </returns>
    protected virtual IResultProvider<INode> ExecuteParseRigthNode(ReadOnlySpan<Token> tokens) => null!;

    /// <summary>
    /// Tries to get data from related <paramref name="tokens"/>,
    /// that must be ended by <see cref="TokenIndentificator.Semicolon"/>.
    /// </summary>
    protected IResultProvider<INode> GetRelativeDataNodeResult(ReadOnlySpan<Token> tokens)
    {
        int semicolonIndex = tokens.IndexOfToken(TokenIndentificator.Semicolon);
        if (OperatorHelper.TryGetOperatorResult(out IResultProvider<INode> nodeResult, tokens[..semicolonIndex]))
            return nodeResult;

        INode? resultNode = tokens[..semicolonIndex].GetDataNode();
        return resultNode is not null ?
            SuccessfulResult<INode>.Create<ParsingResult>(resultNode, semicolonIndex) :
            ErrorResult<INode>.Create("Any related node could be created from current tokens");
    }

    private static bool TryGetStatementResultValue([NotNullWhen(false)]out INode returnResult,
        IResultProvider<INode> resultProvider)
    {
        if (resultProvider is ErrorResult<INode>)
            return NullableHelper.NullableOutValue(out returnResult);
        returnResult = resultProvider.ValueResult.Result!;
        return true;
    }
}
