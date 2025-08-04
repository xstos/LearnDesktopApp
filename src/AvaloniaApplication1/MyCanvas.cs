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
    public Action<MyCanvas, DrawingContext> render;
    public CDO CDO=new();
    public MyCanvas(Action<MyCanvas> ctor)
    {
        CDO.Parent = this;
        ctor(this);
    }
   
    public override void Render(DrawingContext context)
    {
        render.Invoke(this,context);
    }
}

public class CDO : ICustomDrawOperation
{
    public MyCanvas Parent;
    public Func<CDO,Rect> getBounds;
    public Action<CDO,ImmediateDrawingContext> render;
    public Func<CDO,ICustomDrawOperation?, bool> equals;
    public Func<CDO,Point, bool> hitTest;
    public bool Equals(ICustomDrawOperation? other)
    {
        return equals(this,other);
    }

    public void Dispose()
    {
    }

    public bool HitTest(Point p)
    {
        return hitTest(this,p);
    }

    public void Render(ImmediateDrawingContext context)
    {
        render(this,context);
       
    }

    public Rect Bounds => getBounds(this);
}
