namespace Athena.NET.Athena.NET;

[AttributeUsage(AttributeTargets.All)]
internal class PrimitiveTypeAttribute : Attribute
{
    public Type Type { get; }

    public PrimitiveTypeAttribute(Type type) 
    {
        Type = type;
    }
}
