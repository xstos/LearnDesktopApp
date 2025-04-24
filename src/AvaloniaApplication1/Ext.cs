namespace AvaloniaApplication1;

public static class Ext
{
    public static T Var<T>(this T v, out T v2)
    {
        return v2 = v;
    }
}