namespace Athena.NET.Attributes;

internal class PrimitiveTypeAttribute : TokenTypeAttribute
{
    public Type Type { get; }

    public PrimitiveTypeAttribute(Type type)
    {
        Type = type;
    }
}
