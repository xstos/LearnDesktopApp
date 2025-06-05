using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Primitives;
using Avalonia.Data.Core.Plugins;
using Avalonia.Input;
using Avalonia.Interactivity;
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
        Action<string> onCommand = s => { };
        var dockPanel = new DockPanel();
        var sideList = new ListBox();
        var txt = new TextBlock();
        txt.Focusable = true;
        
        sideList._Dock(Dock.Left);
        sideList.AddRange("hi", "there");
        sideList.SelectionChanged += (s, e) =>
        {
            txt.Focus();
        };
        
        
        /*
         var il = new Run("a"); 
        il.FontSize = 30;
        il.Foreground = new SolidColorBrush(Colors.Yellow);
        txt.Inlines.Add(il);
        */
        txt.ClipToBounds = false;
        txt.TextWrapping = TextWrapping.Wrap;
        dockPanel.Children.Add(sideList);
        dockPanel.Children.Add(txt);
        var xaml = AvaloniaRuntimeXamlLoader.Parse<TextBlock>(
            File.ReadAllText("test.xaml"));
        var seed = 1;
        var cursor = CreateCursor();
        var rootNode = cursor.GetRoot();
        var foo = "";
        void refresh()
        {
            txt.Text = cursor.NodesStr();

        }
        
        const string ctrl = "control";
        onCommand = s =>
        {
            if (!txt.IsFocused) return;
            _ = s switch
            {
                "left" => cursor.MoveBack(),
                "right" => cursor.MoveForward(),
                "enter" => cursor.InsertCell(),
                "back" => cursor.Backspace(),
                "delete" => cursor.DeleteAtom(),
                $"{ctrl}+enter" => cursor.InsertAtom("\n"),
                $"{ctrl}+delete" => cursor.DeleteCell(),
                _ => cursor.InsertAtom(s.Length<2 ? Convert.ToChar(s) : s)
            };

            refresh();
        };
        
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
                case Key.OemTilde: txt += "tilde"; break;
                default: return;
            }

            onCommand(txt);
        }
        refresh();
        Control root = dockPanel;
        
        switch (ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime app:
            {
                new MainWindow().Var(out var o);
                o.FontFamily = new FontFamily("Courier New");
                o.FontWeight = FontWeight.Bold;
                o.FontSize = 12;
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
                o.FontWeight = FontWeight.Bold;
                o.FontSize = 12;
                o.Content = root;
                app.MainView = o;
                o.TextInput += OnTextInput;
                o.KeyDown += OnKeyDown;
                break;
            }
            
        }

        void OnTxtOnLoaded(object? s, RoutedEventArgs e)
        {
            txt.Focus(NavigationMethod.Tab);
        }

        txt.Loaded += OnTxtOnLoaded;

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

        base.OnFrameworkInitializationCompleted();
        ;
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
    public static ListBox AddRange(this ListBox list, params string[] items)
    {
        foreach (var item in items)
        {
            list.Items.Add(item);
        }

        return list;
    }
}

public enum Direction
{
    Left,
    Right,
    Up,
    Down,
}
