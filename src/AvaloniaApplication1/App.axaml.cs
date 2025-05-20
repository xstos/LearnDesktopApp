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

[DebuggerDisplay("{Data}")]
public record Node
{
    public object Data;
    public Node Next { get; set; }
    public Node Prev { get; set; }
    public static implicit operator Node(char c)
    {
        return new Node() { Data = c };
    }

    public static implicit operator Node(string c)
    {
        return new Node() { Data = c };
    }

    public static Node[] N(params Node[] nodes)
    {
        return nodes;
    }

    public static void E(params Node[] nodes)
    {
        for (int i = 1; i < nodes.Length; i++)
        {
            Edge(nodes[i-1], nodes[i]);
        }
    }

    public void InsBef(params Node[] nodes)
    {
        E([Prev,.. nodes, this ]); // 
    }

    public void InsAft(params Node[] nodes)
    {
        E([this,.. nodes, Next ]);
    }
    public static void Edge(Node a, Node b)
    {
        a.Next = b;
        b.Prev = a;
    }

    public IEnumerable<Node> All()
    {
        IEnumerable<Node> Prevs()
        {
            var cur = Prev;
            while (cur != null)
            {
                yield return cur;
                cur = cur.Prev;
            }
        }

        IEnumerable<Node> Nexts()
        {
            var cur = Next;
            while (cur != null)
            {
                yield return cur;
                cur = cur.Next;
            }
        }
        return Prevs().Reverse().Concat([this]).Concat(Nexts());
    }

    public string AllStr()
    {
        return All().Select(n => n.Data.ToString())._Join(" ");
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
        Control root = new TextBlock();
        var (r1, cur, r2) = N("<0", "@█", ">0");
        E(r1, cur, r2);
        cur.InsBef([.."hi"]);
        cur.InsAft([.."there"]);
        Console.WriteLine(cur.AllStr());
        void DocumentModel()
        {
            var root = new LinkedList<Node>();
            LinkedListNode<Node> cursor;

            var cursym = "@█";
            
            AddLast("<0", cursym, ">0");
            var seed = 1;
            
            cursor = root.Find(cursym);


            void AddLast(params string[] nodes)
            {
                foreach (var node in nodes)
                {
                    root.AddLast(node);
                }
            }

            void Insert(OneOf<IEnumerable<char>, IEnumerable<string>> args)
            {
                args.Match(txt =>
                {
                    foreach (var c in txt)
                    {
                        root.AddBefore(cursor, c.ToString());
                    }

                    return true;
                }, words =>
                {
                    var loc = cursor;
                    foreach (var s in words)
                    {
                        loc = root.AddAfter(loc, s);
                    }

                    return true;
                });
                Print();
            }

            void Move(Direction direction)
            {
                LinkedListNode<Node> n;
                switch (direction)
                {
                    case Direction.Left:
                        n = cursor.Previous;
                        if (n.Value.Data == "<0") return;
                        cursor.Previous.SwapWith(cursor);
                        break;
                    case Direction.Right:
                        n = cursor.Next;
                        if (n.Value.Data == ">0") return;
                        cursor.Next.SwapWith(cursor);
                        break;
                    case Direction.Up:
                        break;
                    case Direction.Down:
                        break;
                }

                Print();
            }

            void Delete()
            {
            }

            void InsertCell()
            {
                var strs = enu("<" + seed, ">" + seed);
                seed++;
                Insert(strs);
                Print();
            }

            void Print()
            {
                foreach (var n in root)
                {
                    Console.Write(n.Data + " ");
                }

                Console.WriteLine();
            }

            Insert("hi");
            InsertCell();
            Move(Direction.Right);
        }


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
                case Key.Enter: onCommand(txt + "enter"); break;
                case Key.Back: onCommand(txt + "back"); break;
                case Key.Left: onCommand(txt + "left"); break;
                case Key.Right: onCommand(txt + "right"); break;
                case Key.Delete: onCommand(txt + "delete"); break;
            }
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
