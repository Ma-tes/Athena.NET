using Athena.NET.Lexing;
using Athena.NET.Lexing.Structures;
using Athena.NET.Parsing.Nodes;

namespace Athena.NET.Parsing.Interfaces;

/// <summary>
/// Provides managing every type of <see cref="TokenIndentificator"/>
/// as a related node.
/// </summary>
public interface INode
{
    /// <summary>
    /// Specific token for current node implementation.
    /// </summary>
    public TokenIndentificator NodeToken { get; }

    /// <summary>
    /// Provides storing <see cref="ChildNodes"/>, that are
    /// always managed by <see cref="CreateStatementResult(ReadOnlySpan{Token}, int)"/>.
    /// </summary>
    public ChildrenNodes ChildNodes { get; }

    /// <summary>
    /// Creates <see cref="INode"/> result, by specific <paramref name="tokens"/> and
    /// relative <paramref name="tokenIndex"/> of <see cref="NodeToken"/>.
    /// </summary>
    /// <param name="tokens">Tokens related for current <see cref="INode"/>.</param>
    /// <param name="tokenIndex">Relative index of <see cref="NodeToken"/>.</param>
    /// <returns>
    /// If creating result was succesful, it will return <see cref="SuccessulNodeResult{T}"/>,
    /// otherwise <see cref="ErrorNodeResult{T}"/> with related error message.
    /// </returns>
    public NodeResult<INode> CreateStatementResult(ReadOnlySpan<Token> tokens, int tokenIndex);
}
