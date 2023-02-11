using Athena.NET.Athena.NET.Parser.Interfaces;
using System.Drawing;

namespace Athena.NET.Athena.NET.ParseViewer.NodeElements
{
    public readonly struct NodePosition
    {
        public INode Node { get; }
        public Point Position { get; }

        public NodePosition(INode node, int x, int y) 
        {
            Node = node;
            Position = new(x, y);
        }

        public NodePosition(INode node, Point position) 
        {
            Node = node;
            Position = position;
        }
    }
}
