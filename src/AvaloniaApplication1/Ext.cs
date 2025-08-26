global using static Globals;
using System.Dynamic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using ImpromptuInterface;
public static class Globals
{
    public static Stream __Assets(string name) => AssetLoader.Open(new Uri($"avares://AvaloniaApplication1/Assets/{name}"));
}

public static partial class Ext
{
    public static Func<T> Enumerator<T>(this IEnumerable<T> items)
    {
        var enu = items.GetEnumerator();
        return () =>
        {
            enu.MoveNext();
            return enu.Current;
        };
    }
    public static int Floor(this double d) => Math.Floor(d).ToInt();
    public static int Ceil(this double d) => Math.Ceiling(d).ToInt();

    public static int ToInt(this double d) => (int)d;

    public static AppBuilder WithMyConfig(this AppBuilder builder)
    {
        return builder;
        return builder.With(new SkiaOptions()
        {
            MaxGpuResourceSizeBytes = 1073741824*2L
        });
    }
    public static T Var<T>(this T v, out T v2)
    {
        return v2 = v;
    }
    public static T _Dock<T>(this T _, Dock dock) where T : Control
    {
        DockPanel.SetDock(_, dock);
        return _;
    }

    public static string _Join(this IEnumerable<string> strings, string separator)
    {
        return String.Join(separator, strings.ToArray());
    }
    public static T[] enu<T>(params T[] items)
    {
        return items;
    }
    public static void Do<T>(this T item, Action<T> action)
    {
        action(item);
    }

    public static IEnumerable<T> Run<T>(this IEnumerable<T> e)
    {
        foreach (var _ in e);
        return e;
    }
    public static LinkedListNode<T> SwapWith<T>(this LinkedListNode<T> node2, LinkedListNode<T> node1)
    {
        node1.List.Remove(node1);
        node2.List.AddAfter(node2, node1);
        return node2;
    }
}


public static class ArrayDeconstruct
{
    public static void Deconstruct<T>(this T[] array, out T t1)
    {
        t1 = array[0];
    }
    public static void Deconstruct<T>(this T[] array, out T t1, out T t2)
    {
        t1 = array[0];
        t2 = array[1];
    }
    public static void Deconstruct<T>(this T[] array, out T t1, out T t2, out T t3)
    {
        t1 = array[0];
        t2 = array[1];
        t3 = array[2];
    }
    public static void Deconstruct<T>(this T[] array, out T t1, out T t2, out T t3, out T t4)
    {
        t1 = array[0];
        t2 = array[1];
        t3 = array[2];
        t4 = array[3];
    }
    public static void Deconstruct<T>(this T[] array, out T t1, out T t2, out T t3, out T t4, out T t5)
    {
        t1 = array[0];
        t2 = array[1];
        t3 = array[2];
        t4 = array[3];
        t5 = array[4];
    }
    public static void Deconstruct<T>(this T[] array, out T t1, out T t2, out T t3, out T t4, out T t5, out T t6)
    {
        t1 = array[0];
        t2 = array[1];
        t3 = array[2];
        t4 = array[3];
        t5 = array[4];
        t6 = array[5];
    }
    public static void Deconstruct<T>(this T[] array, out T t1, out T t2, out T t3, out T t4, out T t5, out T t6, out T t7)
    {
        t1 = array[0];
        t2 = array[1];
        t3 = array[2];
        t4 = array[3];
        t5 = array[4];
        t6 = array[5];
        t7 = array[6];
    }
    public static void Deconstruct<T>(this T[] array, out T t1, out T t2, out T t3, out T t4, out T t5, out T t6, out T t7, out T t8)
    {
        t1 = array[0];
        t2 = array[1];
        t3 = array[2];
        t4 = array[3];
        t5 = array[4];
        t6 = array[5];
        t7 = array[6];
        t8 = array[7];
    }
}