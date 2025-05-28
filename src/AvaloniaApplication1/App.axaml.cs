using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Data.Core.Plugins;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using AvaloniaApplication1.Views;
using CSScripting;
using CSScriptLib;
using Jint;
using Jint.Native;
using Jint.Native.Json;
using Jint.Runtime;
using Microsoft.Extensions.DependencyInjection;
using static UI;
using static Ext;
using static Node;
using ValueOf;
using OneOf;
using OneOf.Types;

[DebuggerDisplay("{Data}")]
public class Node
{
    public object Data;
    public Node? Prev { get; set; }
    public Node? Next { get; set; }
    public Node? Parent { get; set; }
    public Node? Partner { get; set; }
    public static int Seed = 1;
    public static implicit operator Node(char c)
    {
        return new Node() { Data = c };
    }
    public static implicit operator Node(string c)
    {
        return new Node() { Data = c };
    }

    public bool IsRoot => Parent == null;
    public bool IsOpen => Equals(Data,"<");
    public bool IsClose => Equals(Data,">");
    public static Node[] N(params Node[] nodes)
    {
        return nodes;
    }

    public static Node CreateCursor()
    {
        Node cur = "@█";
        var (ro, rc) = N("<", ">");
        ro.PartnerWith(rc);
        E(ro, cur, rc);
        cur.Parent = ro;
        return cur;
    }

    public static Node[] Cell()
    {
        var (o, c) = N("<", ">");
        o.PartnerWith(c);
        return [o,c];
    }
    public Node PartnerWith(Node other)
    {
        Partner = other;
        other.Partner = this;
        return this;
    }

    public Node InsertAtom(Node node)
    {
        //prev=>cursor
        //prev=>new=>cursor
        E(Prev, node, this);
        node.Parent = Parent;
        return node;
    }

    public Node InsertCell()
    {
        var (co, cc) = Cell();
        co.Parent = cc.Parent = Parent;
        E(this,co, cc,Next);
        return this;
    }
    public static IEnumerable<Node> E(params Node[] nodes)
    {
        for (int i = 1; i < nodes.Length; i++)
        {
            Edge(nodes[i-1], nodes[i]);
        }
        return nodes;
    }

    public void MoveForward()
    {
        if (Next.IsRoot) return;
        if (Next.IsOpen) Parent = Next;
        if (Next.IsClose) Parent = Next.Parent;
        E(Prev, Next, this, Next.Next);
    }

    public void MoveBack()
    {
        if (Prev.IsRoot) return;
        if (Prev.IsOpen) Parent = Prev.Parent;
        if (Prev.IsClose) Parent = Prev.Partner;
        E(Prev.Prev, this, Prev, Next);
    }
    public static void Edge(Node a, Node b)
    {
        if (a!=null) a.Next = b;
        if (b!=null) b.Prev = a;
    }

    public IEnumerable<Node> Nodes()
    {
        IEnumerable<Node> Before()
        {
            var cur = Prev;
            while (cur != null)
            {
                yield return cur;
                cur = cur.Prev;
            }
        }
        IEnumerable<Node> After()
        {
            var cur = Next;
            while (cur != null)
            {
                yield return cur;
                cur = cur.Next;
            }
        }
        return Before().Reverse().Concat([this,..After()]);
    }

    public string NodesStr()
    {
        return Nodes().Select(n => n.Data.ToString())._Join(" ");
    }
}

public partial class App : Application
{
    public static readonly IServiceProvider? ServiceProvider = BuildDependencyGraph()
        .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });

    public static ServiceCollection BuildDependencyGraph()
    {
        ServiceCollection services = new();
        services.AddLogging();
        return services;
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        void jsInit()
        {
            var parsingOptions = new ScriptParsingOptions
            {
                Tolerant = true,
            };
            var engine = new Engine(cfg => cfg
                .AllowClr()
            );
            var serializer = new JsonSerializer(engine);

            string Eval(string? txt)
            {
                try
                {
                    var result = engine.Evaluate(txt, parsingOptions);
                    JsValue str = result;
                    if (!result.IsPrimitive() && result is not IJsPrimitive)
                    {
                        str = serializer.Serialize(result, JsValue.Undefined, "  ");
                        if (str == JsValue.Undefined)
                        {
                            str = result;
                        }
                    }
                    else if (result.IsString())
                    {
                        str = serializer.Serialize(result, JsValue.Undefined, JsValue.Undefined);
                    }

                    return str + "";
                }
                catch (JavaScriptException je)
                {
                    return je.Message;
                }
                catch (Exception e)
                {
                    return e.ToString();
                }
            }
        }

        Action<string> onCommand = s => { };
        var txt = new TextBlock();
        Control root = txt;
        var seed = 1;
        var cursor = CreateCursor();

        onCommand = s =>
        {
            switch (s)
            {
                case "left": cursor.MoveBack();
                    break;
                case "right": cursor.MoveForward();
                    break;
                case "enter": cursor.InsertCell();
                    break;
                default: 
                    cursor.InsertAtom(s);
                    break;
            }
            
            txt.Text = cursor.NodesStr();
        };
        Console.WriteLine(cursor.NodesStr());
        

        void OnTextInput(object? sender, TextInputEventArgs args)
        {
            var key = args.Text;
            onCommand(key);
        }

        void OnKeyDown(object? sender, KeyEventArgs args)
        {
            var txt = "";
            if (args.KeyModifiers.HasFlag(KeyModifiers.Control)) txt += "control+";
            if (args.KeyModifiers.HasFlag(KeyModifiers.Shift)) txt += "shift+";
            switch (args.Key)
            {
                case Key.Enter: txt += "enter"; break;
                case Key.Back: txt += "back"; break;
                case Key.Left: txt += "left"; break;
                case Key.Right: txt += "right"; break;
                case Key.Delete: txt += "delete"; break;
                default: return;
            }

            onCommand(txt);
        }

        switch (ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime app:
            {
                new MainWindow().Var(out var o);
                o.FontFamily = new FontFamily("Courier New");
                o.Content = root;
                app.MainWindow = o;
                o.TextInput += OnTextInput;
                o.KeyDown += OnKeyDown;
                break;
            }
            case ISingleViewApplicationLifetime app:
            {
                new MainView().Var(out var o);
                o.FontFamily = new FontFamily("Courier New");
                o.Content = root;
                app.MainView = o;
                o.TextInput += OnTextInput;
                o.KeyDown += OnKeyDown;
                break;
            }
        }

        base.OnFrameworkInitializationCompleted();

        void oldCode()
        {
            new WrapPanel().Var(out var pnl);
            pnl.Height = Double.NaN;
            pnl.Width = Double.NaN;
            new ScrollViewer().Var(out var sv);

            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            sv.Content = pnl;
            var scrollViewer = sv;
            var cur = new TextBlock() { Tag = "cursor", Text = "█" };
            pnl.Children.Add(cur);

            void onCommand1(string cmd)
            {
                Console.WriteLine(cmd);
                var parent = cur.GetLogicalParent() as Panel;
                var ix = pnl.Children.IndexOf(cur);

                void newLine()
                {
                    var ctrl = new TextBlock()
                    {
                        Text = new string(' ', 10000),
                        Margin = new Thickness(0), Padding = new Thickness(0),
                        Background = new SolidColorBrush(Colors.Red),
                    };
                    parent.Children.Insert(ix, ctrl);
                }

                void backSpace()
                {
                    if (ix > 0) parent.Children.RemoveAt(ix - 1);
                }

                void delete()
                {
                    if (ix < parent.Children.Count - 1) parent.Children.RemoveAt(ix + 1);
                }

                void cursorLeft()
                {
                    if (ix > 0) Swap(pnl, ix);
                }

                void cursorRight()
                {
                    if (ix < parent.Children.Count - 1) Swap(pnl, ix + 1);
                }

                void writeChar()
                {
                    var tb = new TextBlock() { Text = cmd, Margin = new Thickness(0), Padding = new Thickness(0) };
                    parent.Children.Insert(ix, tb);
                }

                switch (cmd)
                {
                    case "enter":
                        newLine();
                        break;
                    case "back":
                        backSpace();
                        break;
                    case "delete":
                        delete();
                        break;
                    case "left":
                        cursorLeft();
                        break;
                    case "right":
                        cursorRight();
                        break;
                    default:
                        writeChar();
                        break;
                }
            }

            onCommand = onCommand1;
            root = scrollViewer;
        }
    }

    static void Swap(Panel pnl, int ix)
    {
        var a = pnl.Children[ix - 1];
        var b = pnl.Children[ix];
        pnl.Children[ix] = new TextBlock();
        pnl.Children[ix - 1] = b;
        pnl.Children[ix] = a;
    }

    static void ScriptExample(MainWindow win)
    {
        dynamic calc = CSScript.Evaluator.ReferenceDomainAssemblies()
            .LoadCode(
                @"
using Avalonia;
using System;
using Avalonia.Controls;
using AvaloniaApplication1;
public class Script : ICalc
{
    public Action<Panel> Sum()
    {
        void Foo(Panel p) {
            var lb = new ListBox();
            var lbi = new ListBoxItem();
            lb.Items.Add(lbi);
            lbi.Content = ""hi"";
            p.Children.Add(new ListBox());
            p.Children[0]=lb;
        }
        return Foo;                  
    }
}
");
        Action<Panel> result = calc.Sum();
        result(win.Content as Panel);
    }

    void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}

public static partial class Ext
{
    
}

public enum Direction
{
    Left,
    Right,
    Up,
    Down,
}
