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
    /// Bool statement if last result is <see cref="ErrorResult{T}"/>.
    /// </summary>
    public bool IsErrorResult { get; private set; }

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
    /// <returns>
    /// Returns <see langword="true"/>, if <paramref name="result"/>
    /// is not <see cref="ErrorResult{T}"/>, otherwise <see langword="false"/>.
    /// </returns>
    public bool AddResult(IResultProvider<T> result)
    {
        resultValues.Add(result);
        IsErrorResult = result is ErrorResult<T>;

        if (IsErrorResult && onErrorResult is not null)
            onErrorResult.Invoke((ErrorResult<T>)result);
        return IsErrorResult;
    }

    public IEnumerator<IResultProvider<T>> GetEnumerator() =>
        resultValues.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() =>
        ((IEnumerable)resultValues).GetEnumerator();
}
