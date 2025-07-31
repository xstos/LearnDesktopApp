using Avalonia;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;

namespace AvaloniaApplication1;

public static partial class Ext
{
    public static Size CalculateTextSize(string myText, FontFamily myFont, int myFontSize)
    {
        var ts = TextShaper.Current;
        var typeface = new Typeface(myFont);
        ShapedBuffer shaped = ts.ShapeText(myText, new TextShaperOptions(typeface.GlyphTypeface, myFontSize));
        var run = new ShapedTextRun(shaped, new GenericTextRunProperties(typeface, myFontSize));
        return run.Size;
    }
}