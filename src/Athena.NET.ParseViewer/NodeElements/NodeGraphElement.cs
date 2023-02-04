using Athena.NET.Athena.NET.Lexer;
using Athena.NET.Athena.NET.Parser.Interfaces;
using Athena.NET.Athena.NET.Parser.Nodes.DataNodes;
using Athena.NET.ParseViewer.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace Athena.NET.ParseViewer.NodeElements
{
    internal sealed class NodeGraphElement : INodeDrawer
    {
        private Point nodePosition;

        public Pen OutlinePen { get; set; } = Pens.Black;
        public Brush TextBrush { get; set; } = Brushes.Black;

        public int NodeDistance { get; set; }

        public NodeGraphElement(Point startPosition, int distance) 
        {
            nodePosition = startPosition;
            NodeDistance = distance;
        }

        public void OnDraw(INode node, Graphics graphics)
        {
            var nodeSize = CalculateNodeSize(node);
            var nodeRectangle = new Rectangle(nodePosition, nodeSize);

            graphics.DrawEllipse(OutlinePen, nodeRectangle);
            graphics.DrawString(GetEnumTokenName(node.NodeToken), SystemFonts.DefaultFont)
        }

        private Size CalculateNodeSize(INode node)
        {
            int returnSize = GetEnumTokenName(node.NodeToken).Length;;
            if (TryGetDataNode(out DataNode<object> dataNode, node))
                returnSize += dataNode.NodeData.ToString()!.Length;
            return new(returnSize, returnSize);
        }

        private bool TryGetDataNode([NotNullWhen(true)]out DataNode<object> returnNode, INode node) 
        {
            returnNode = null!;
            if (node.GetType().IsAssignableFrom(typeof(DataNode<>))) 
            {
                returnNode = (DataNode<object>)node;
                return true;
            }
            return false;
        }

        private string GetEnumTokenName(TokenIndentificator token) =>
            Enum.GetName(typeof(TokenIndentificator), token)!;
    }
}
