using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Media.TextFormatting;


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
}