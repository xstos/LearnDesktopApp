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
    public static LinkedListNode<T> SwapWith<T>(this LinkedListNode<T> node2, LinkedListNode<T> node1)
    {
        node1.List.Remove(node1);
        node2.List.AddAfter(node2, node1);
        return node2;
    }
}

public interface IFoo
{
    void Text(params string[] txt);
}
public class UI
{
    public static LinkedList<UI> Path { get; set; }=new();
    public string Tag { get; set; }
    public UI Parent { get; set; }
    public List<UI> Children { get; set; }=new();
    public dynamic Props { get; set; }
    public static Dictionary<string,UI> Vars { get; set; }
    public static List<Action> useRefs { get; set; }=new();

    public static dynamic Ref(out IFoo o)
    {
        var parent = Path.Last.Value;
        void Text(params string[] txt)
        {
            if (txt.Length == 1)
            {
                
            }
        }

        var derp = Text;
        var ret = new
        {
            Text=derp
        };
        o=ret.ActLike<IFoo>();
        return ret;
    }
    UI()
    {
        Vars = new();
    }

    
    public static UI Render(Action a)
    {
        var root = new UI();
        Path.AddLast(root);
        a();
        Path.Clear();
        return root;
    }

    public static UI el(string tag, Action a)
    {
        var parent = Path.Last.Value;
        var o = new UI() {Tag = tag, Parent = parent };
        dynamic props = new Props(o);
        o.Props = props;
        parent.Children.Add(o);
        Path.AddLast(o);
        a();
        Path.RemoveLast();
        return o;
    }
    
    public static UI stackPanel(Action a) => el("stackpanel",a);
    public static UI dockPanel(Action a) => el("dockpanel",a);
    public static UI button(Action a) => el("button",a);
    public static UI textBox(Action a) => el("textbox",a);
}

public delegate bool TryGetMember(GetMemberBinder binder, out object result);
public class Interceptor : DynamicObject
{
    TryGetMember tryget;
    public Interceptor(TryGetMember mem)
    {
        tryget = mem;
    }
    public static dynamic New(TryGetMember mem)
    {
        dynamic cept = new Interceptor(mem);
        return cept;
    }
    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
        return tryget(binder, out result);
    }

}
public class Props : DynamicObject
{
    readonly dynamic e = new ExpandoObject();
    public readonly IDictionary<string, object> map;
        
    public Props(UI parent)
    {
        map = e;
    }

    public override IEnumerable<string> GetDynamicMemberNames()
    {
        return map.Keys;
    }

    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
        var key = binder.Name;
        return get(key, out result);
    }

    bool get(string key, out object result)
    {
        if (map.TryGetValue(key, out result)) return true;
        map[key] = result;
        return true;
    }

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
        map[binder.Name] = value;
        return true;
    }

    public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
    {
        return get(indexes[0]?.ToString(), out result);
    }

    public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
    {
        map[indexes[0]?.ToString()] = value;
        return true;
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