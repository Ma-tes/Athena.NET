using Athena.NET.Parsing.Interfaces;
using Athena.NET.ParsingView.Interfaces;
using Athena.NET.ParsingView.Structures;

namespace Athena.NET.ParsingView;

public class NodeElementData<T> where T : INodeRenderer<T>
{
    public INode ElementNode { get; }
    public VectorPointF Position { get; set; }
    public Action 
} 
