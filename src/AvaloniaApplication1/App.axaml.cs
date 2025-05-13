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
using CSScriptLib;
using Jint;
using Jint.Native;
using Jint.Native.Json;
using Jint.Runtime;
using Microsoft.Extensions.DependencyInjection;
using static UI;

namespace AvaloniaApplication1;

public interface ICalc
{
    Action<Panel> Sum();
}
public partial class App : Application
{
    public static MainWindow Win { get; set; }
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
        
        //todo: tiny todo app
        
        new WrapPanel().Var(out var pnl);
        pnl.Height=Double.NaN;
        pnl.Width=Double.NaN;
        new ScrollViewer().Var(out var sv);
        
        sv.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
        sv.Content = pnl;
        var root = sv;
        var cur = new TextBlock() { Tag = "cursor", Text = "█" };
        pnl.Children.Add(cur);
        
        

        void onCommand(string cmd)
        {
            Console.WriteLine(cmd);
            var parent = cur.GetLogicalParent() as Panel;
            var ix =pnl.Children.IndexOf(cur);

            void newLine()
            {
                var ctrl = new TextBlock()
                {
                    Text = new string(' ',10000),
                    Margin = new Thickness(0), Padding = new Thickness(0),
                    Background = new SolidColorBrush(Colors.Red),
                };
                parent.Children.Insert(ix,ctrl);
            }

            void backSpace()
            {
                if (ix > 0) parent.Children.RemoveAt(ix-1);
            }

            void delete()
            {
                if (ix<parent.Children.Count-1) parent.Children.RemoveAt(ix+1);
            }

            void cursorLeft()
            {
                if (ix > 0) Swap(pnl,ix);
            }

            void cursorRight()
            {
                if (ix < parent.Children.Count - 1) Swap(pnl, ix+1);
            }

            void writeChar()
            {
                var tb = new TextBlock() { Text = cmd, Margin = new Thickness(0), Padding = new Thickness(0)};
                parent.Children.Insert(ix,tb);
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
        void OnTextInput(object? sender, TextInputEventArgs args)
        {
            var key = args.Text;
            onCommand(key);
        }
        void OnWinOnKeyDown(object? sender, KeyEventArgs args)
        {
            var txt = "";
            if (args.KeyModifiers.HasFlag(KeyModifiers.Control)) txt += "control+";
            if (args.KeyModifiers.HasFlag(KeyModifiers.Shift)) txt += "shift+";
            switch (args.Key)
            {
                case Key.Enter: onCommand(txt+"enter"); break;
                case Key.Back: onCommand(txt+"back"); break;
                case Key.Left: onCommand(txt+"left"); break;
                case Key.Right: onCommand(txt+"right"); break;
                case Key.Delete: onCommand(txt+"delete"); break;
            }
        }
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            new MainWindow().Var(out MainWindow win);
            win.FontFamily = new FontFamily("Courier New");

            win.Content = root;
            Win = win;
            desktop.MainWindow = win;
            win.TextInput += OnTextInput;
            win.KeyDown += OnWinOnKeyDown;
            
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView().Var(out var mv);
            mv.FontFamily = new FontFamily("Courier New");

            mv.Content = root;
            mv.TextInput += OnTextInput;
            mv.KeyDown += OnWinOnKeyDown;

        }
        
        base.OnFrameworkInitializationCompleted();
        
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

    private void DisableAvaloniaDataAnnotationValidation()
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

