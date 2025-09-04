using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Media.TextFormatting;
using Avalonia.Platform;
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
    public static Color Invert(this Color c) => Color.FromArgb(c.A, c.R.Invert(), c.G.Invert(), c.B.Invert());

    public static byte Invert(this byte b) {
        unchecked {
            return (byte)(b + 128);
        }
    }

    public static WriteableBitmap ToWritableBitmap(this RenderTargetBitmap rtb)
    {
        var wb = new WriteableBitmap(rtb.PixelSize,rtb.Dpi,rtb.Format,rtb.AlphaFormat);
        using var fb = wb.Lock();

        rtb.CopyPixels(new PixelRect(0,0, rtb.PixelSize.Width, rtb.PixelSize.Height), 
            fb.Address, fb.RowBytes * rtb.PixelSize.Height, fb.RowBytes);

        return wb;
    }
    public static WriteableBitmap Colorize(this WriteableBitmap bmp, Color c)
    {
        using var l = bmp.Lock();
        unsafe
        {
            byte* pixelPtr = (byte*)l.Address.ToPointer();
            int bytesPerPixel = l.Format.BitsPerPixel / 8;
            var sizeWidth = l.Size.Width;
            var sizeHeight = l.Size.Height;
            var lRowBytes = l.RowBytes;
            
            for (int y = 0; y < sizeHeight; y++)
            {
                byte* rowPtr = pixelPtr + (y * lRowBytes);

                for (int x = 0; x < sizeWidth; x++)
                {
                    byte* pixel = rowPtr + (x * bytesPerPixel);
                
                    // Access individual color channels
                    var b = pixel[0];
                    var g = pixel[1];
                    var r = pixel[2];
                    var a = pixel[3];
                    if (b != 0 || g != 0 || r != 0 || a != 0)
                    {
                        pixel[0] = c.B;
                        pixel[1] = c.G;
                        pixel[2] = c.R;
                        pixel[3] = c.A;
                    }
                    
                    //if (bytesPerPixel == 4) pixel[3] = alpha; // Alpha
                }
            }
        }

        return bmp;
    }
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

    public static Rect WithWidthOffset(this Rect r, double w)
    {
        return r.WithWidth(r.Width+w);
    }
    public static Rect WithHeightOffset(this Rect r, double h)
    {
        return r.WithHeight(r.Height+h);
    }
    public static Rect WithSizeOffset(this Rect r, double w, double h)
    {
        return r.WithWidthOffset(w).WithHeightOffset(h);
    }

    public static (int, int) ToInt(this Point p)
    {
        return (p.X.ToInt(), p.Y.ToInt());
    }
    public static IEnumerable<(int x,int y)> CoordsXY(this Rect r)
    {
        var li = r.Left.ToInt();
        var ri = r.Right.ToInt();
        var ti = r.Top.ToInt();
        var bi = r.Bottom.ToInt();
        
        for (int x = li; x <= ri; x++)
        {
            for (int y = ti; y <= bi; y++)
            {
                yield return (x, y);
            }
        }
    }
    public static (Rect,Rect) Cut(this Rect r, Cut cmd,double a, Action<Rect, Rect>? action=null)
    {
        switch (cmd)
        {
            case Left:
            {
                var pWidth = r.Width;
                if (a > pWidth) a = pWidth;
                var r1 = r.WithWidth(a);
                var r2 = r.WithWidth(pWidth-a).Translate(a,0);
                action?.Invoke(r1, r2);
                return  (r1, r2);
                break;
            }
            case Right:
            {
                var pWidth = r.Width;
                if (a > pWidth) a = pWidth;
                var r1 = r.WithWidth(a).Translate(pWidth-a,0);
                var r2 = r.WithWidth(pWidth-a);
                action?.Invoke(r1,r2);
                return  (r1, r2);
                break;
            }
            case Top:
            {
                var pHeight = r.Height;
                if (a>pHeight) a = pHeight;
                var r1 = r.WithHeight(a);
                var r2 = r.WithHeight(pHeight-a).Translate(0,a);
                action?.Invoke(r1,r2);
                return  (r1, r2);
                break;
            }
            case Bottom:
            {
                var pHeight = r.Height;
                if (a>pHeight) a = pHeight;
                var r1 = r.WithHeight(a).Translate(0,pHeight-a);
                var r2 = r.WithHeight(pHeight-a);
                action?.Invoke(r1,r2);
                return  (r1, r2);
                break;
            }
        }
        throw new Exception();
    }

}

public struct MouseInfo
{
    public (int RowIndex, int ColIndex) Hover;
}

