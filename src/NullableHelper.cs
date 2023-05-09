namespace Athena.NET;

internal static class NullableHelper
{
    public static bool NullableOutValue<T>(out T value) where T : struct 
    {
        value = default(T);
        return false;
    }
}
