using System.Reflection;
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
using Avalonia.Styling;
using Avalonia.Utilities;
using Avalonia.VisualTree;
using AvaloniaApplication1;
using AvaloniaApplication1.Views;
using CSScripting;
using CSScriptLib;
using Jint;
using Jint.Native;
using Jint.Native.Json;
using Jint.Runtime;
//using Microsoft.Extensions.DependencyInjection;
using static UI;
using static Ext;
using static Node;
using ValueOf;
using OneOf;
using OneOf.Types;

public partial class App : Application
{
    // public static readonly IServiceProvider? ServiceProvider = BuildDependencyGraph()
    //     .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });

    // public static ServiceCollection BuildDependencyGraph()
    // {
    //     ServiceCollection services = new();
    //     services.AddLogging();
    //     return services;
    // }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Action<string> onCommand = s => { };
        var dockPanel = new DockPanel();
        var sv = new ScrollViewer();
        var log = new TextBox();
        log._Dock(Dock.Bottom);
        var sideList = new ListBox();
        var txt = new TextBlock();
        txt.Focusable = true;
        txt.Classes.Add("TextInputParent");
        sideList._Dock(Dock.Left);
        sideList._AddRange("hi", "there");
        sideList.SelectionChanged += (s, e) =>
        {
            txt.Focus();
        };
        
        txt.ClipToBounds = true;
        txt.TextWrapping = TextWrapping.Wrap;
        dockPanel.Children.Add(sideList);
        dockPanel.Children.Add(new MyCanvas());
        sv.Content = txt;
        sv.IsTabStop = true;
        sv.AllowAutoHide = false;
        //dockPanel.Children.Add(sv);
        //dockPanel.Children.Add(log);
        
        var seed = 1;
        var (rootOpen, cursor, rootClosed) = CreateCursor();
        txt.FontSize = 30;
        var letterClass = "letter";
        var rightClass = "right";

        void EventHookup()
        {
            Button.PointerMovedEvent.AddClassHandler<Button>((o, args) =>
            {
                if (!o.Classes.Contains(letterClass)) return;
                var pos = args.GetPosition(o);
                if (pos.X > o.Bounds.Width / 2)
                {
                    o.Classes.Add(rightClass);
                    Console.WriteLine(rightClass);
                
                }
                else
                {
                    o.Classes.Remove(rightClass);
                    Console.WriteLine("left");
                }
            
            });
            Button.GotFocusEvent.AddClassHandler<Button>((o, args) =>
            {
                if (!o.Classes.Contains(letterClass)) return;
                txt.Focus();
            });
            Button.ClickEvent.AddClassHandler<Button>((o, args) =>
            {
                if (!o.Classes.Contains(letterClass)) return;
                var after = o.Classes.Contains(rightClass);
                var n = o.Tag as Node;
                if (after)
                {
                    cursor.MoveBetween(n, n.Next);
                }
                else
                {
                    cursor.MoveBetween(n.Prev,n);
                }
                refresh();
            });
        }

        LineBreak BR()
        {
            return new LineBreak();
        }
        EventHookup();
        void refresh2()
        {
            //txt.Text = cursor.NodesStr();
            txt.Inlines.Clear();
            
            foreach (var node in cursor.Nodes())
            {
                var nodeData = node.Data;
                var content = nodeData + "";
                var isNewLine = nodeData == "\n";
                if (isNewLine)
                {
                    content = "⏎";
                }
                
                var el = new Button();
                el.Tag = node;
                el.Classes.Add(letterClass);
                //el.Padding = new Thickness(0);
                //el.Margin = new Thickness(0);
                el.Content = content;
                
                if (node.IsOpen && cursor.Parent == node
                    || node.IsClose && cursor.Parent == node.Partner)
                {
                    el.Foreground = Brushes.Magenta;
                }
                var iuc = new InlineUIContainer(el);
                
                txt.Inlines.Add(iuc);
                if (isNewLine)
                {
                    txt.Inlines.Add(BR());
                    
                }
            }
            
        }

        void refresh()
        {
            var tb2 = txtblock(get());
            dockPanel.Children[1] = tb2;
        }

        string get()
        {
            var l = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque ipsum magna, rutrum quis nunc at, faucibus rutrum nunc. Nam vitae blandit massa, at venenatis nibh. Etiam ultricies enim diam, a placerat felis consequat sit amet. Maecenas nec diam id mauris varius scelerisque. Cras eget velit nec odio lobortis aliquam vitae quis sem. Fusce feugiat ante lorem, nec varius nibh imperdiet et. Nulla ultricies varius fringilla. Quisque sed massa non nibh tincidunt pulvinar. Mauris fermentum purus sed tristique dignissim. Aliquam in eleifend mi, a interdum sapien.\n\nQuisque eget risus in dolor rutrum tincidunt sed at nulla. Donec sodales libero ac nunc luctus, eget sollicitudin leo imperdiet. Sed pulvinar iaculis imperdiet. Aliquam pharetra, ligula nec posuere fermentum, turpis nunc mattis sem, malesuada cursus nunc orci id mauris. Integer eu posuere ante. Nullam ac ornare erat, ac malesuada sem. Duis eu nisl augue.\n\nFusce dictum condimentum libero eget pulvinar. Donec sed metus pharetra, aliquam felis vitae, posuere elit. Nam sed nulla malesuada, lobortis risus et, bibendum augue. Nulla vel sollicitudin libero, vitae dapibus ante. Vestibulum et gravida lorem. Sed varius lobortis mi, sed consectetur est cursus in. Donec feugiat interdum fermentum. Proin sit amet urna ac est lacinia pretium. Donec interdum felis a nibh aliquam, vel sollicitudin ex pellentesque. Aliquam erat volutpat. Sed cursus urna sed ipsum maximus, eget pharetra dui rutrum. Nulla vitae tellus ut velit euismod tempus quis id nisi. Maecenas et dolor odio. Vestibulum eget lectus mauris.\n\nInteger venenatis eu nisl lacinia tincidunt. Vestibulum blandit vestibulum dolor non malesuada. Sed erat sem, facilisis quis dolor quis, sodales pulvinar lacus. Sed vestibulum metus at tellus mollis volutpat. Donec bibendum felis quis gravida cursus. Vivamus auctor accumsan felis ut efficitur. Nam scelerisque nulla a velit interdum vestibulum. Proin mollis tempus nunc nec lobortis.\n\nCurabitur venenatis elit ut leo pellentesque efficitur. Nullam sagittis est sit amet urna finibus iaculis. Vivamus magna diam, tempor eget lacinia non, dictum nec mauris. Integer congue ut tellus vitae tempor. Aliquam rutrum augue tristique elit elementum tincidunt. Nunc mi risus, fermentum sed volutpat vitae, fermentum sit amet nibh. Nunc consectetur molestie libero at lacinia.";
            return l.ToCharArray().Select(c => $@"<Button>{c}</Button>")._Join("");
        }

        WrapPanel txtblock(string child)
        {
            var b = $@"
<WrapPanel xmlns='https://github.com/avaloniaui'>
    {child}
</WrapPanel>
";
            return AvaloniaRuntimeXamlLoader.Parse<WrapPanel>(b);
        }
        const string shift = "shift";
        onCommand = s =>
        {
            //if (!txt.IsFocused) return;
            cursor = s switch
            {
                "left" => cursor.MoveBack(),
                "right" => cursor.MoveForward(),
                "enter" => cursor.InsertCell(),
                "back" => cursor.Backspace(),
                "delete" => cursor.DeleteAtom(),
                $"{shift}+enter" => cursor.InsertAtom("\n"),
                $"{shift}+delete" => cursor.DeleteCell(),
                $"{shift}+back" => cursor.BackspaceCell(),
                _ => cursor.InsertAtom(s.Length < 2 ? Convert.ToChar(s) : s)
            };
            //Console.WriteLine(s);
            //Console.WriteLine(cursor.NodeStr);
            refresh();
        };

        void addStyleExample()
        {
            var style = new Style(x => x.OfType<Run>().Class("foo"))
            {
                Setters =
                {
                    new Setter(Button.BackgroundProperty, Brushes.Green)
                }
            };
            Current.Styles.Add(style);
        }

        void OnTextInput(object? sender, TextInputEventArgs args)
        {
            var topLevel = TopLevel.GetTopLevel(sender as Visual);
            var el = topLevel.FocusManager.GetFocusedElement();
            if (el is not Control c) return;
            var classes = c.GetSelfAndVisualAncestors().SelectMany(c => c.Classes);
            
            if (!classes.Contains("TextInputParent")) return;
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

        Control content = dockPanel;

        switch (ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime app:
            {
                new MainWindow().Var(out var o);
                o.FontFamily = new FontFamily("Courier New");
                o.FontWeight = FontWeight.Bold;
                o.FontSize = 12;
                o.Content = content;
                app.MainWindow = o;
                o.AddHandler(InputElement.TextInputEvent,OnTextInput, RoutingStrategies.Tunnel);
                //o.TextInput += OnTextInput;
                o.KeyDown += OnKeyDown;
                break;
            }
            case ISingleViewApplicationLifetime app:
            {
                new MainView().Var(out var o);
                o.FontFamily = new FontFamily("Courier New");
                o.FontWeight = FontWeight.Bold;
                o.FontSize = 12;
                o.Content = content;
                app.MainView = o;
                o.AddHandler(InputElement.TextInputEvent,OnTextInput, RoutingStrategies.Tunnel);
                //o.TextInput += OnTextInput;
                o.KeyDown += OnKeyDown;
                break;
            }
        }

        void OnTxtOnLoaded(object? s, RoutedEventArgs e)
        {
            txt.Focus(NavigationMethod.Tab);
            refresh();
        }

        txt.Loaded += OnTxtOnLoaded;

        void xamlLoadExample(string txt)
        {
            var xaml = AvaloniaRuntimeXamlLoader.Parse<TextBlock>(txt);
        }
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
    public static ListBox _AddRange(this ListBox list, params string[] items)
    {
        foreach (var item in items)
        {
            list.Items.Add(item);
        }

        return list;
    }
    
}