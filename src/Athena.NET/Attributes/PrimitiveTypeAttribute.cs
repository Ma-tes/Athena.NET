namespace Athena.NET.Athena.NET;

[AttributeUsage(AttributeTargets.Field)]
internal class PrimitiveTypeAttribute : Attribute
{
    public Type Type { get; }

    public PrimitiveTypeAttribute(Type type) 
    {
        Type = type;
    }
}
