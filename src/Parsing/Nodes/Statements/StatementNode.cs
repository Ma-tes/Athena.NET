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
    public abstract TokenIndentificator NodeToken { get; }
    public ChildrenNodes ChildNodes { get; internal set; }

    public virtual IResultProvider<INode> CreateStatementResult(ReadOnlySpan<Token> tokens, int tokenIndex)
    {
        ReadOnlySpan<Token> leftTokens = tokens[..tokenIndex];
        ReadOnlySpan<Token> rightTokens = tokens[(tokenIndex + 1)..];

        if (!TryParseLeftNode(out IResultProvider<INode> leftResult, leftTokens) && leftResult is not null)
            return new ErrorResult<INode>(leftResult.Message);
        if (!TryParseRigthNode(out IResultProvider<INode> rightResult, rightTokens) && rightResult is not null)
            return new ErrorResult<INode>(rightResult.Message);

        ChildNodes = new(leftResult!.Node!, rightResult!.Node!);
        return new SuccessulResult<INode>(this);
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
    protected virtual IResultProvider<INode> CreateParseLeftNode(ReadOnlySpan<Token> tokens) => null!;

    /// <summary>
    /// Provides parsing a rigth portion of statement <paramref name="tokens"/>.
    /// </summary>
    /// <param name="tokens">Specifically splitted portion of tokens.</param>
    /// <returns>
    /// Related result of <see cref="IResultProvider{T}"/>, which is only going to
    /// be equal to<see cref="SuccessfulResult{T}"/> or <see cref="ErrorResult{T}"/>,
    /// with related type of <see cref="INode"/>.
    /// </returns>
    protected virtual IResultProvider<INode> CreateParseRigthNode(ReadOnlySpan<Token> tokens) => null!;

    /// <summary>
    /// Tries to get data from related <paramref name="tokens"/>,
    /// that must be ended by <see cref="TokenIndentificator.Semicolon"/>.
    /// </summary>
    protected bool TryGetNodeData([NotNullWhen(true)] out IResultProvider<INode> nodeResult, ReadOnlySpan<Token> tokens)
    {
        int semicolonIndex = tokens.IndexOfToken(TokenIndentificator.Semicolon);
        if (OperatorHelper.TryGetOperatorResult(out nodeResult, tokens[..semicolonIndex]))
            return true;

        INode? resultNode = tokens[..semicolonIndex].GetDataNode();
        nodeResult = resultNode is not null ?
            new SuccessulNodeResult<INode>(resultNode) :
            new ErrorNodeResult<INode>("Any related node could be created from current tokens");
        return resultNode is not null;
    }
}
