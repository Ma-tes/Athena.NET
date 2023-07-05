using Athena.NET.ExceptionResult.Interfaces;
using System.Collections;

namespace Athena.NET.ExceptionResult;

//TODO: Add ICollection with a propriete methods
//and try to reduce redudant allocations.
internal sealed class ResultMemory<T> : IEnumerable<IResultProvider<T>>
{
    private readonly List<IResultProvider<T>> resultValues = new();
    private Action<ErrorResult<T>>? onErrorResult;

    public int ErrorResultIndex { get; private set; }
    public int Count => resultValues.Count;

    public ResultMemory() { }
    public ResultMemory(Action<IResultProvider<T>> onErrorResult) 
    {
        this.onErrorResult = onErrorResult;
    }

    public bool AddResult(IResultProvider<T> result) 
    {
        resultValues.Add(result);
        bool isErrorResult = result is ErrorResult<T>;

        if (isErrorResult && onErrorResult is not null)
            onErrorResult.Invoke((ErrorResult<T>)result);
        return isErrorResult;
    }

    public IEnumerator<IResultProvider<T>> GetEnumerator() =>
        resultValues.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() =>
        ((IEnumerable)resultValues).GetEnumerator();
}
