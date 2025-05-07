using System.Dynamic;
using Avalonia.Controls;
using ImpromptuInterface;


public static class Ext
{
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

    public static void r(string s)
    {
        
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