using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Media.TextFormatting;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;

namespace AvaloniaApplication1;

public class MyCanvas : Control
{
    public Action<MyCanvas, DrawingContext> RenderFun;
    public MyCanvas()
    {
        var cd = new CDO();
        var stream = __Assets("hi.png");
        var wb = WriteableBitmap.Decode(stream);
        
        RenderFun = (canvas, context) =>
        {
            var formattedText = new FormattedText(Bounds.ToString(),CultureInfo.CurrentCulture, FlowDirection.LeftToRight, 
                new Typeface("Courier"), 12, Brushes.White);
        
            context.DrawText(formattedText, new Point(0,50));;
            cd.RenderFun = (cdo, idctx) =>
            {
                idctx.DrawBitmap(wb,new Rect(200,200,64,64));     
            };
            cd.GetBounds = ()=>Bounds;
            cd.EqualsFun = (cdo, other) => false;
            cd.HitTestFun = (cdo, p) => true;
            context.Custom(cd);
        };
    }
    public static Size CalculateTextSize(string myText, FontFamily myFont, int myFontSize)
    {
        var ts = TextShaper.Current;
        var typeface = new Typeface(myFont);
        ShapedBuffer shaped = ts.ShapeText(myText, new TextShaperOptions(typeface.GlyphTypeface, myFontSize));
        var run = new ShapedTextRun(shaped, new GenericTextRunProperties(typeface, myFontSize));
        return run.Size;
    }
    public override void Render(DrawingContext context)
    {
        RenderFun(this,context);
    }
}

public class CDO : ICustomDrawOperation
{
    public Func<Rect> GetBounds;
    public Action<CDO,ImmediateDrawingContext> RenderFun;
    public Func<CDO,ICustomDrawOperation?, bool> EqualsFun;
    public Func<CDO,Point, bool> HitTestFun;
    public bool Equals(ICustomDrawOperation? other)
    {
        return EqualsFun(this,other);
    }

    public void Dispose()
    {
    }

    public bool HitTest(Point p)
    {
        return HitTestFun(this,p);
    }

    public void Render(ImmediateDrawingContext context)
    {
        RenderFun(this,context);
       
    }

    public Rect Bounds => GetBounds();
}
