using Athena.NET.Athena.NET.Lexer;
using Athena.NET.Athena.NET.Parser.Interfaces;
using Athena.NET.Athena.NET.Parser.Nodes.DataNodes;
using Athena.NET.Athena.NET.ParseViewer.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace Athena.NET.Athena.NET.ParseViewer.NodeElements
{
    public sealed class NodeGraphElement : INodeDrawer
    {
        public Pen OutlinePen { get; set; } = Pens.White;
        public Brush TextBrush { get; set; } = Brushes.White;

        public int NodeDistance { get; set; }

        public NodeGraphElement(int distance) 
        {
            NodeDistance = distance;
        }

        public void OnDraw(INode node, Graphics graphics, Point position)
        {
            if (node is null)
                return;

            var nodeSize = CalculateNodeSize(node);
            var nodeRectangle = new Rectangle(position, nodeSize);
            graphics.DrawEllipse(OutlinePen, nodeRectangle);

            graphics.DrawString(GetEnumTokenName(node.NodeToken), SystemFonts.DefaultFont, TextBrush, nodeRectangle.X, nodeRectangle.Y);
            if (TryGetDataNode(out DataNode<object> dataNode, node)) 
            {
                int currentYPosition = nodeRectangle.Y + (nodeSize.Height / 2);
                graphics.DrawString(dataNode.NodeData.ToString(), SystemFonts.DefaultFont, TextBrush, nodeRectangle.X, currentYPosition);
            }

            int childrenYPosition = position.Y + NodeDistance;
            var leftNodePosition = new Point(position.X - (NodeDistance / 5), childrenYPosition);
            var rightNodePosition = new Point(position.X + (NodeDistance / 2), childrenYPosition);

            var childrenNodes = node.ChildNodes;
            OnDraw(childrenNodes.LeftNode, graphics, leftNodePosition);
            OnDraw(childrenNodes.RightNode, graphics, rightNodePosition);
        }

        private Size CalculateNodeSize(INode node)
        {
            int returnSize = GetEnumTokenName(node.NodeToken).Length;
            if (TryGetDataNode(out DataNode<object> dataNode, node))
                returnSize += dataNode.NodeData.ToString()!.Length;
            return new(returnSize * 10, (returnSize * 10) / (returnSize / 3));
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
