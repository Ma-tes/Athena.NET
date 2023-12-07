using Athena.NET.ExceptionResult.Interfaces;
 
namespace Athena.NET.ExceptionResult.Structures;

public readonly struct IndexResultProvider<T>
    where T : IResultProvider<T>
{
    public int Index { get; }
    public T ResultProvider { get; }

    public IndexResultProvider(int index, T resultProvider)
    {
        Index = index;
        ResultProvider = resultProvider;
    }
}
