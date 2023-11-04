using Athena.NET.Parsing.Interfaces;
using Athena.NET.ParsingView.FormatRenderer.Interfaces;
using Athena.NET.ParsingView.Structures;

namespace Athena.NET.ParsingView;

public struct NodeElementData<T>
    where T : IFormatGraphics
{
    public Type ElementNodeType { get; }
    public Action<T, INode, VectorPointF> ElementDrawFunction { get; set; }

    public NodeElementData(Type nodeType, Action<T, INode, VectorPointF> elementDrawFunction)
    {
        ElementDrawFunction = nodeType;
        ElementDrawFunction = elementDrawFunction;
    }
} 
