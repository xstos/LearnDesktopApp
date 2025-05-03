using System.Dynamic;
using Avalonia.Controls;


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

public class UI
{
    public int Id { get; set; }
    public static LinkedList<UI> Stack = new();
    public static Dictionary<string,UI> Values = new();
    public string Tag { get; set; }
    public static Action _ = () => { };
    public static int IdSeed = 0;
    public string Name => Tag + "_" + Id;
    public static string Path => Stack.Select(u => u.Name)._Join("/");

    public UI()
    {
        Id = IdSeed++;
    }
    public static void Render(Action a)
    {
        el("root",a);
    }

    public static void el(string tag, Action a)
    {
        var o = new UI() {Tag = tag};
        Stack.AddLast(o);
        Values[Path] = o;
        Console.WriteLine(Path);
        a();
        Stack.RemoveLast();
    }
    public static void stackPanel(Action a) => el("stackpanel",a);
    public static void dockPanel(Action a) => el("dockpanel",a);
    public static void button(Action a) => el("button",a);
    public static void textBox(Action a) => el("textbox",a);
}

