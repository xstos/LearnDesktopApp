using Avalonia.Media;
using static Avalonia.Media.Brushes;

namespace AvaloniaApplication1;

public class MyColors
{
    public static Func<IImmutableSolidColorBrush> Wheel()
    {
        var enu = Brushes().GetEnumerator();
        return () =>
        {
            enu.MoveNext();
            return enu.Current;
        };
    }
    public static IEnumerable<IImmutableSolidColorBrush> Brushes()
    {
        while (true)
        {
            yield return AliceBlue;
            yield return AntiqueWhite;
            yield return Aqua;
            yield return Aquamarine;
            yield return Azure;
            yield return Beige;
            yield return Bisque;
            yield return Black;
            yield return BlanchedAlmond;
            yield return Blue;
            yield return BlueViolet;
            yield return Brown;
            yield return BurlyWood;
            yield return CadetBlue;
            yield return Chartreuse;
            yield return Chocolate;
            yield return Coral;
            yield return CornflowerBlue;
            yield return Cornsilk;
            yield return Crimson;
            yield return Cyan;
            yield return DarkBlue;
            yield return DarkCyan;
            yield return DarkGoldenrod;
            yield return DarkGray;
            yield return DarkGreen;
            yield return DarkKhaki;
            yield return DarkMagenta;
            yield return DarkOliveGreen;
            yield return DarkOrange;
            yield return DarkOrchid;
            yield return DarkRed;
            yield return DarkSalmon;
            yield return DarkSeaGreen;
            yield return DarkSlateBlue;
            yield return DarkSlateGray;
            yield return DarkTurquoise;
            yield return DarkViolet;
            yield return DeepPink;
            yield return DeepSkyBlue;
            yield return DimGray;
            yield return DodgerBlue;
            yield return Firebrick;
            yield return FloralWhite;
            yield return ForestGreen;
            yield return Fuchsia;
            yield return Gainsboro;
            yield return GhostWhite;
            yield return Gold;
            yield return Goldenrod;
            yield return Gray;
            yield return Green;
            yield return GreenYellow;
            yield return Honeydew;
            yield return HotPink;
            yield return IndianRed;
            yield return Indigo;
            yield return Ivory;
            yield return Khaki;
            yield return Lavender;
            yield return LavenderBlush;
            yield return LawnGreen;
            yield return LemonChiffon;
            yield return LightBlue;
            yield return LightCoral;
            yield return LightCyan;
            yield return LightGoldenrodYellow;
            yield return LightGray;
            yield return LightGreen;
            yield return LightPink;
            yield return LightSalmon;
            yield return LightSeaGreen;
            yield return LightSkyBlue;
            yield return LightSlateGray;
            yield return LightSteelBlue;
            yield return LightYellow;
            yield return Lime;
            yield return LimeGreen;
            yield return Linen;
            yield return Magenta;
            yield return Maroon;
            yield return MediumAquamarine;
            yield return MediumBlue;
            yield return MediumOrchid;
            yield return MediumPurple;
            yield return MediumSeaGreen;
            yield return MediumSlateBlue;
            yield return MediumSpringGreen;
            yield return MediumTurquoise;
            yield return MediumVioletRed;
            yield return MidnightBlue;
            yield return MintCream;
            yield return MistyRose;
            yield return Moccasin;
            yield return NavajoWhite;
            yield return Navy;
            yield return OldLace;
            yield return Olive;
            yield return OliveDrab;
            yield return Orange;
            yield return OrangeRed;
            yield return Orchid;
            yield return PaleGoldenrod;
            yield return PaleGreen;
            yield return PaleTurquoise;
            yield return PaleVioletRed;
            yield return PapayaWhip;
            yield return PeachPuff;
            yield return Peru;
            yield return Pink;
            yield return Plum;
            yield return PowderBlue;
            yield return Purple;
            yield return Red;
            yield return RosyBrown;
            yield return RoyalBlue;
            yield return SaddleBrown;
            yield return Salmon;
            yield return SandyBrown;
            yield return SeaGreen;
            yield return SeaShell;
            yield return Sienna;
            yield return Silver;
            yield return SkyBlue;
            yield return SlateBlue;
            yield return SlateGray;
            yield return Snow;
            yield return SpringGreen;
            yield return SteelBlue;
            yield return Tan;
            yield return Teal;
            yield return Thistle;
            yield return Tomato;
            yield return Transparent;
            yield return Turquoise;
            yield return Violet;
            yield return Wheat;
            yield return White;
            yield return WhiteSmoke;
            yield return Yellow;
            yield return YellowGreen;
        }
    }
}