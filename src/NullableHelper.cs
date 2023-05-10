namespace Athena.NET;

internal static class NullableHelper
{
    public static bool NullableOutValue<T>(out T value)
    {
        value = default(T)!;
        return false;
    }

}
