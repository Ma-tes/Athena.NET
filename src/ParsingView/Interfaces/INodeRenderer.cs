using Athena.NET.ParsingView.FormatRenderer.Interfaces;

namespace Athena.NET.ParsingView.Interfaces;

public interface INodeRenderer<T> : IDisposable
    where T : IFormatGraphics
{
    public T RendererGraphics { get; }

    public void DrawElements(ReadOnlyMemory<NodeElementData<T>> elements);
    public Task DrawElementsAsync(ReadOnlyMemory<NodeElementData<T>> elements);

    //TODO: Move it into, abstract class, which would provide
    //certain protected operation.
    //public void DrawRelativeNode(NodeElementData<T> elementNode);
}
