using Athena.NET.Athena.NET.Parser.Interfaces;
using System.Drawing;
using System.Runtime.Versioning;

namespace Athena.NET.Athena.NET.ParseViewer
{
    [SupportedOSPlatform("windows")]
    public readonly struct NodeShape
    {
        public Type NodeType { get; }
        public Action<INode, Rectangle, Graphics> DrawShape { get; }

        public static NodeShape DefaultShape { get; } =
            new(null!,
                (INode node, Rectangle rectangle, Graphics graphics) => 
                {
                    graphics.FillRectangle(Brushes.LightGray, rectangle);
                });

        public NodeShape(Type nodeType, Action<INode, Rectangle, Graphics> drawShape)
        {
            NodeType = nodeType;
            DrawShape = drawShape;
        }
    }
}
