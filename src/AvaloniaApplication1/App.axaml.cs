using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
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
        var parsingOptions = new ScriptParsingOptions
        {
            Tolerant = true,
        };
        var engine = new Engine(cfg => cfg
            .AllowClr()
        );
        var serializer = new JsonSerializer(engine);
        //todo: tiny todo app
        //how to type text in wpf?
        
        
        object GetContent()
        {
            
            new WrapPanel().Var(out var pnl);
            return pnl;
            new ListBox().Var(out var lb);
            lb.Items.Add(new ListBoxItem().Var(out var li));
            li.Content = "hi";
            lb._Dock(Dock.Left);
            new TextBox().Var(out var tb2);
            tb2._Dock(Dock.Top);
            new TextBox() { Text = "hello world" }.Var(out var tb);
            tb._Dock(Dock.Top);
            pnl.Children.Add(lb);

            pnl.Children.Add(tb);
            pnl.Children.Add(tb2);
            tb2.TextWrapping = TextWrapping.Wrap;
            tb.TextWrapping = TextWrapping.Wrap;
            tb.AcceptsReturn = true;
            tb.TextChanged += (sender, args) =>
            {
                tb2.Text = Eval(tb.Text);
            };
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
            return pnl;
        }

        var pnl = GetContent() as WrapPanel;
        var cur = new TextBlock() { Tag = "cursor", Text = "█" };
        pnl.Children.Add(cur);

        void OnTextInput(object? sender, TextInputEventArgs args)
        {
            var parent = cur.GetLogicalParent() as Panel;
            var ix =pnl.Children.IndexOf(cur);
            var key = args.Text;
            switch (key)
            {
                case "enter": 
                    var ctrl = new TextBlock()
                    {
                        Text = new string(' ',10000),
                        Margin = new Thickness(0), Padding = new Thickness(0),
                        Background = new SolidColorBrush(Colors.Red),
                        //HorizontalAlignment = HorizontalAlignment.Stretch
                    };
                    parent.Children.Insert(ix,ctrl);
                    break;
                case "back":
                    if (ix > 0) parent.Children.RemoveAt(ix-1);
                    break;
                case "left":
                    if (ix > 0)
                    {
                        Swap(pnl,ix);
                    }
                    break;
                case "right":
                    if (ix < parent.Children.Count - 1)
                    {
                        Swap(pnl, ix+1);
                    }
                    break;
                default:
                    var tb = new TextBlock() { Text = args.Text, Margin = new Thickness(0), Padding = new Thickness(0)};
                    parent.Children.Insert(ix,tb);
                    break;
            }
        }
        
        void OnWinOnKeyDown(object? sender, KeyEventArgs args)
        {
            switch (args.Key)
            {
                case Key.Enter: send(sender, "enter"); break;
                case Key.Back: send(sender, "back"); break;
                case Key.Left: send(sender, "left"); break;
                case Key.Right: send(sender, "right"); break;
            }
        }
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            new MainWindow().Var(out MainWindow win);
            win.Content = pnl;
            Win = win;
            //ScriptExample(win);
            desktop.MainWindow = win;
            win.TextInput += OnTextInput;
            win.KeyDown += OnWinOnKeyDown;
            win.FontFamily = new FontFamily("Courier New");
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView().Var(out var mv);
            mv.Content = pnl;
            mv.TextInput += OnTextInput;
            mv.KeyDown += OnWinOnKeyDown;
            mv.FontFamily = new FontFamily("Courier New");

        }
        
        base.OnFrameworkInitializationCompleted();

        void send(object? sender, string? text)
        {
            OnTextInput(sender,new TextInputEventArgs() {Text = text});
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