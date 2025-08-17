using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Media.TextFormatting;
using static Cut;
public enum Cut
{
    Top,
    Left,
    Right,
    Bottom,
}

public static partial class Ext
{
    public static Size MeasureText(string myText, FontFamily myFont, int myFontSize)
    {
        var ts = TextShaper.Current;
        var typeface = new Typeface(myFont);
        ShapedBuffer shaped = ts.ShapeText(myText, new TextShaperOptions(typeface.GlyphTypeface, myFontSize));
        var run = new ShapedTextRun(shaped, new GenericTextRunProperties(typeface, myFontSize));
        return run.Size;
    }
    public static Bitmap CreateTextTile(int width,int height, Action<Bitmap,DrawingContext> render)
    {
        // Create bitmap
        
        var bitmap = new RenderTargetBitmap(
            new PixelSize(width, height),
            new Vector(96, 96));

        using var ctx = bitmap.CreateDrawingContext();
        render(bitmap,ctx);
    
        return bitmap;
    }
    public static Rect Translate(this Rect r, double x, double y)
    {
        return r.Translate(new Vector(x, y));
    }
    public static void Cut(this Rect r, Cut cmd,double a, Action<Rect, Rect> action)
    {
        switch (cmd)
        {
            case Left:
            {
                var pWidth = r.Width;
                if (a > pWidth) a = pWidth;
                action(r.WithWidth(a), r.WithWidth(pWidth-a).Translate(a,0));
                break;
            }
            case Right:
            {
                var pWidth = r.Width;
                if (a > pWidth) a = pWidth;
                action(r.WithWidth(a).Translate(pWidth-a,0),r.WithWidth(pWidth-a));
                break;
            }
            case Top:
            {
                var pHeight = r.Height;
                if (a>pHeight) a = pHeight;
                action(r.WithHeight(a),r.WithHeight(pHeight-a).Translate(0,a));;
                break;
            }
            case Bottom:
            {
                var pHeight = r.Height;
                if (a>pHeight) a = pHeight;
                action(r.WithHeight(a).Translate(0,pHeight-a),r.WithHeight(pHeight-a));
                break;
            }
        }
    }

}