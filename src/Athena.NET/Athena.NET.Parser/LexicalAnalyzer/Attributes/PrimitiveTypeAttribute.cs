namespace Athena.NET.Athena.NET.Parser.LexicalAnalyzer.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    internal class PrimitiveTypeAttribute : Attribute
    {
        public Type Type { get; }

        public PrimitiveTypeAttribute(Type type) 
        {
            Type = type;
        }
    }
}
