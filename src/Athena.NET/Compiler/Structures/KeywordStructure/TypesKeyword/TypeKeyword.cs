using System.Collections.Immutable;

namespace Athena.NET.Compiler.Structures.KeywordStructure.TypesKeyword
{
    public sealed class TypeKeyword : Keyword
    {
        //This is not how the final implementation
        //would looks like
        public static ImmutableArray<TypeKeyword> Keywords { get; } =
            ImmutableArray.Create
            (
                new TypeKeyword(1, "int"),
                new TypeKeyword(2, "float"),
                new TypeKeyword(3, "double")
            );

        protected override byte identificatorShift { get; } = 2;

        public TypeKeyword(int id, string name) : base(id, name)
        {

        }
    }
}
