using Athena.NET.ExceptionResult.Interfaces;
using System.Collections;

namespace Athena.NET.ExceptionResult;

//TODO: Add ICollection with a propriete methods
//and try to reduce redudant allocations.

/// <summary>
/// Provides manageable storing for <see cref="IResultProvider{T}"/>,
/// that could execute actions on specific <see cref="ErrorResult{T}"/>.
/// </summary>
public sealed class ResultMemory<T> : IEnumerable<IResultProvider<T>>
{
    private readonly List<IResultProvider<T>> resultValues = new();
    private Action<ErrorResult<T>>? onErrorResult;

    /// <summary>
    /// Index of the last found <see cref="ErrorResult{T}"/>.
    /// </summary>
    public int ErrorResultIndex { get; private set; }

    /// <summary>
    /// Gets the number of results, that are contained in <see cref="resultValues"/>
    /// </summary>
    public int Count => resultValues.Count;

    public IResultProvider<T> this[int index] 
    {
        get => resultValues[index];

        //It's possible, that there is not going to
        //be any usable usecase, however it's gonna
        //stay here for future improves.
        internal set => resultValues[index] = value;
    }

    public ResultMemory() { }
    public ResultMemory(Action<IResultProvider<T>> onErrorResult) 
    {
        this.onErrorResult = onErrorResult;
    }

    /// <summary>
    /// Adds results to <see cref="resultValues"/> and it will
    /// execute <see cref="onErrorResult"/>, if <paramref name="result"/>
    /// is <see cref="ErrorResult{T}"/>.
    /// </summary>
    /// <param name="result"></param>
    /// <returns>
    /// Returns <see langword="true"/>, if <paramref name="result"/>
    /// is not <see cref="ErrorResult{T}"/>, otherwise <see langword="false"/>.
    /// </returns>
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
