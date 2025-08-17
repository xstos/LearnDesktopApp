using System.Globalization;
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
using Avalonia.Media.Imaging;
using Avalonia.Rendering.SceneGraph;
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
using static Avalonia.Media.Brushes;

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
        var sv = new ScrollViewer()._Dock(Dock.Right);

        const int myFontSize = 24;

        var courierNew = "Courier New";
        
        void Ctor(MyCanvas c)
        {
            var stream = __Assets("hi.png");
            var wb = WriteableBitmap.Decode(stream);
            var textSize = MeasureText(cursorSymbol, new FontFamily(courierNew), myFontSize);
            void Draw(string[] lines)
            {
            }
            FormattedText MakeFormattedText(string txt, IBrush? foreground, double emSize=myFontSize)
            {
                return new FormattedText(txt, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Courier New"), emSize, foreground);
            }
            Draw(Lorem.Text.Split(["\r\n", "\r", "\n"], StringSplitOptions.None));

            bool Equals(CDO cdo, ICustomDrawOperation? operation) => false;
            bool HitTest(CDO cdo, Point point) => false;
            Rect GetBounds(CDO cdo) => c.Bounds;
            var renderOpts = new RenderOptions()
            {
                TextRenderingMode = TextRenderingMode.Alias,
                BitmapInterpolationMode = BitmapInterpolationMode.LowQuality
            };
            var CDO = c.CDO;
            CDO.getBounds = GetBounds;
            CDO.equals = Equals;
            CDO.hitTest = HitTest;
            CDO.render = (cdo, idctx) =>
            {
                var myc = cdo.Parent;
                var bounds = myc.Bounds;
                idctx.DrawBitmap(wb, new Rect(200, 200, 64, 64));
            };
            var ft = MakeFormattedText(cursorSymbol,White);
            var ft2 = MakeFormattedText(historyCursor, White);
            var cursorTile = CreateTextTile(textSize.Width.Ceil(), textSize.Height.Ceil(),(bitmap, context) =>
            {
                context.PushRenderOptions(renderOpts);
                context.DrawText(ft,new Point(0,0));
            });
            var cursorTile2 = CreateTextTile(textSize.Width.Ceil(), textSize.Height.Ceil(),(bitmap, context) =>
            {
                context.PushRenderOptions(renderOpts);
                context.DrawText(ft2,new Point(0,0));
            });
            
            c.render = (canvas, context) =>
            {
                var cBounds = canvas.Bounds;
                cBounds = cBounds
                    .WithWidth(cBounds.Width - 20)
                    .WithHeight(cBounds.Height - 20)
                    .Translate(new Vector(10, 10));
                context.DrawRectangle(Magenta, null, cBounds);
                cBounds.Do(screen =>
                {
                    screen.Cut(Cut.Bottom,50, (left, rest) =>
                    {
                        context.DrawRectangle(Magenta, null, left);
                        context.DrawRectangle(Cyan, null, rest);
                    });
                });
                context.PushRenderOptions(renderOpts);
                var txtW = textSize.Width.Ceil();
                var txtH = textSize.Height.Ceil();
                var bndW = cBounds.Width;
                var bndH = cBounds.Height;
                var (numRows,numCols) = (bndH.ToInt() / txtH, bndW.ToInt() / txtW);
                
                var txt = $" w={bndW} h={bndH} tw={txtW} th={txtH} nr={numRows} nc={numCols}";
                var formattedText = MakeFormattedText(txt, Magenta,12);
                var drawText = context.DrawText;
                
                drawText(formattedText, new Point(10, 10));

                var n = 0;
                for (int x = 0; x < numCols; x++)
                {
                    var xoffs = x * txtW;
                    for (int y = 0; y < numRows; y++)
                    {
                        continue;
                        Point p = new(xoffs, y * txtH);
                        //if (n%2==0) drawText(ft, p);
                        //else drawText(ft2, p);
                        if (n%2==0) context.DrawImage(cursorTile,new Rect(p,cursorTile.Size));
                        else context.DrawImage(cursorTile2,new Rect(p,cursorTile.Size));
                        n += 1;
                    }

                    
                }
                context.Custom(CDO);
            };

        }

        var bottom = new MyCanvas(Ctor);
        
        var sideList = new ListBox()._Dock(Dock.Left);
        var txt = new TextBlock();

        Control getCanvas()
        {
            var border = new Border()
            {
                BorderThickness = new Thickness(0),
                BorderBrush = Red,
                Child = bottom
            };
            return border;
        }
        txt.Focusable = true;
        txt.Classes.Add("TextInputParent");
        sideList._AddRange("hi", "there");
        sideList.SelectionChanged += (s, e) =>
        {
            txt.Focus();
        };
        
        txt.ClipToBounds = true;
        txt.TextWrapping = TextWrapping.Wrap;
        
        //dockPanel.Children.Add(sideList);
        dockPanel.Children.Add(getCanvas());
        //dockPanel.Children.Add(new BlockEditor());
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
        void ButtonPointerMovedHandler(Button o, PointerEventArgs args)
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
        }
        void ButtonFocusHandler(Button o, GotFocusEventArgs args)
        {
            if (!o.Classes.Contains(letterClass)) return;
            txt.Focus();
        }
        void ButtonClickHandler(Button o, RoutedEventArgs args)
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
                cursor.MoveBetween(n.Prev, n);
            }

            refresh();
        }
        void EventHookup()
        {
            Button.PointerMovedEvent.AddClassHandler<Button>(ButtonPointerMovedHandler);
            Button.GotFocusEvent.AddClassHandler<Button>(ButtonFocusHandler);
            Button.ClickEvent.AddClassHandler<Button>(ButtonClickHandler);
        }

        LineBreak BR() => new();
        EventHookup();
        void refresh()
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

        void Init(dynamic w)
        {
            w.FontFamily = new FontFamily(courierNew);
            w.FontWeight = FontWeight.Bold;
            w.FontSize = myFontSize;
            w.Content = content;
        }
        switch (ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime app:
            {
                new MainWindow().Var(out var w);
                w.AddHandler(InputElement.TextInputEvent,OnTextInput, RoutingStrategies.Tunnel);
                w.KeyDown += OnKeyDown;
                Init(w);
                app.MainWindow = w;
                break;
            }
            case ISingleViewApplicationLifetime app:
            {
                new MainView().Var(out var w);
                w.AddHandler(InputElement.TextInputEvent,OnTextInput, RoutingStrategies.Tunnel);
                w.KeyDown += OnKeyDown;
                Init(w);
                app.MainView = w;
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