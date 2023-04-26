using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Athena.NET.Lexing.Structures;

[DebuggerDisplay($"{{{nameof(ToString)}(),nq}}")]
public readonly struct Token : IEquatable<Token>
{
    public TokenIndentificator TokenId { get; }
    public ReadOnlyMemory<char> Data { get; }

    public Token(TokenIndentificator tokenId, ReadOnlyMemory<char> data)
    {
        TokenId = tokenId;
        Data = data;
    }

    public bool Equals(Token other)
    {
        return TokenId == other.TokenId
            && Data.Span.SequenceEqual(other.Data.Span);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is Token other && Equals(other);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(TokenId);
        hash.AddBytes(MemoryMarshal.AsBytes(Data.Span));
        return hash.ToHashCode();
    }

    public static bool operator ==(Token left, Token right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Token left, Token right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        return $"Token:{TokenId} \"{Data.Span}\"";
    }
}
