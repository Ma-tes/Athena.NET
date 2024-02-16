using System.Diagnostics.CodeAnalysis;

namespace Athena.NET.ExceptionResult.Interfaces;

public interface IResultProvider<T>
{
    public Type ProviderHolderType { get; }

    public IResult<T> ValueResult { get; }
    public string? Message { get; }

    public void LogMessage();
}

internal static class ResultProviderHelper 
{
    public static bool TryGetRelativeResultProvider<T, TSelf>(this IResultProvider<T> relativeProvider,
        [NotNullWhen(true)]out TSelf returnProvider) where TSelf : class, IResultProvider<T> 
    {
        Type selfType = typeof(TSelf);
        if(selfType != relativeProvider.ProviderHolderType)
            return NullableHelper.NullableOutValue(out returnProvider);

        returnProvider = (TSelf)relativeProvider;
        return true;
    }
}
