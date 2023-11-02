using Athena.NET.ParsingView.FormatRenderer.Interfaces;

namespace Athena.NET.ParsingView.Interfaces;

public interface INodeRenderer<T> : IDisposable
    where T : INodeRenderer<T>
{
    public IFormatGraphics RendererGraphics { get; }

    public void DrawRelativeNode(NodeElementData<T> elementNode);
}
