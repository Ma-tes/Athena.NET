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

    public virtual NodeResult<INode> CreateStatementResult(ReadOnlySpan<Token> tokens, int tokenIndex)
    {
        ReadOnlySpan<Token> leftTokens = tokens[..tokenIndex];
        ReadOnlySpan<Token> rightTokens = tokens[(tokenIndex + 1)..];

        if (!TryParseLeftNode(out NodeResult<INode> leftResult, leftTokens) && leftResult is not null)
            return new ErrorNodeResult<INode>(leftResult.Message);
        if (!TryParseRigthNode(out NodeResult<INode> rightResult, rightTokens) && rightResult is not null)
            return new ErrorNodeResult<INode>(rightResult.Message);

        ChildNodes = new(leftResult!.Node!, rightResult!.Node!);
        return new SuccessulNodeResult<INode>(this);
    }

    /// <summary>
    /// Provides parsing a left portion of statement <paramref name="tokens"/>.
    /// </summary>
    /// <param name="nodeResult">
    /// Result of final parsing, that could be <see cref="SuccessulNodeResult{T}"/> or
    /// <see cref="ErrorNodeResult{T}"/>.
    /// </param>
    /// <param name="tokens">Specifically splitted portion of tokens.</param>
    /// <returns>
    /// If <see cref="TryParseLeftNode(out NodeResult{INode}, ReadOnlySpan{Token})"/>
    /// wasn't overriden it returns <see langword="true"/>, otherwise <see langword="false"/>.
    /// </returns>
    protected virtual bool TryParseLeftNode([NotNullWhen(true)] out NodeResult<INode> nodeResult, ReadOnlySpan<Token> tokens)
    {
        nodeResult = null!;
        return false;
    }

    /// <summary>
    /// Provides parsing a rigth portion of statement <paramref name="tokens"/>.
    /// </summary>
    /// <param name="nodeResult">
    /// Result of final parsing, that could be <see cref="SuccessulNodeResult{T}"/> or
    /// <see cref="ErrorNodeResult{T}"/>.
    /// </param>
    /// <param name="tokens">Specifically splitted portion of tokens.</param>
    /// <returns>
    /// If <see cref="TryParseRigthNode(out NodeResult{INode}, ReadOnlySpan{Token})"/>
    /// wasn't overriden it returns <see langword="true"/>, otherwise <see langword="false"/>.
    /// </returns>
    protected virtual bool TryParseRigthNode([NotNullWhen(true)] out NodeResult<INode> nodeResult, ReadOnlySpan<Token> tokens)
    {
        nodeResult = null!;
        return false;
    }

    /// <summary>
    /// Tries to get data from related <paramref name="tokens"/>,
    /// that must be ended by <see cref="TokenIndentificator.Semicolon"/>.
    /// </summary>
    protected bool TryGetNodeData([NotNullWhen(true)] out NodeResult<INode> nodeResult, ReadOnlySpan<Token> tokens)
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
