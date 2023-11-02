using Athena.NET.ParsingView.FormatRenderer.Structures;
using Athena.NET.ParsingView.Structures;

namespace Athena.NET.ParsingView.FormatRenderer.Interfaces;

public interface IFormatGraphics : IDisposable
{
    public void DrawPixel(VectorPointF position, ColorData colorData);
    public Task DrawPixelAsync(VectorPointF position, ColorData colorData);
}
