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
        private static readonly string nodeData = "NodeData";

        public Pen OutlinePen { get; set; } = Pens.Black;
        public Brush TextBrush { get; set; } = Brushes.Black;

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

            string tokenName = GetEnumTokenName(node.NodeToken);
            var textRectangle = new Rectangle(new(position.X + (nodeSize.Width / 10), position.Y + 10), new(nodeSize.Width - (tokenName.Length * 2), nodeSize.Height - 40));
            graphics.DrawString(tokenName, SystemFonts.DefaultFont, TextBrush, textRectangle);
            if (TryGetDataNode(out DataNode<object> dataNode, node)) 
            {
                int currentYPosition = nodeRectangle.Y + (nodeSize.Height / 2);
                graphics.DrawString(dataNode.NodeData.ToString(), SystemFonts.DefaultFont, TextBrush, nodeRectangle.X, currentYPosition);
            }

            int childrenYPosition = position.Y + NodeDistance;
            var leftNodePosition = new Point(position.X - (NodeDistance), childrenYPosition);
            var rightNodePosition = new Point(position.X + (NodeDistance), childrenYPosition);

            var childrenNodes = node.ChildNodes;
            OnDraw(childrenNodes.LeftNode, graphics, leftNodePosition);
            OnDraw(childrenNodes.RightNode, graphics, rightNodePosition);
        }

        private Size CalculateNodeSize(INode node)
        {
            int returnSize = GetEnumTokenName(node.NodeToken).Length;
            if (TryGetDataNode(out DataNode<object> dataNode, node)) 
            {
                int dataLength = dataNode.NodeData.ToString()!.Length;
                if (dataLength > returnSize)
                    returnSize = dataLength;
            }
            int scaleSize = returnSize * 50;

            return returnSize > 7 ? new(scaleSize / (((returnSize) / 5)), returnSize * (returnSize * 3)) :
                new(scaleSize, scaleSize);
        }

        private bool TryGetDataNode([NotNullWhen(true)]out DataNode<object> returnNode, INode node)
        {
            returnNode = null!;
            var currentData = GetGenericNodeData(node);

            if (currentData is not null) 
            {
                returnNode = new DataNode<object>(node.NodeToken, currentData);
                return true;
            }
            return false;
        }

        private object? GetGenericNodeData(INode node) 
        {
            Type nodeType = node.GetType();
            Type dataNodeType = typeof(DataNode<>);

            if (nodeType.GUID == dataNodeType.GUID) 
            {
                var propertyInformation = nodeType.GetProperty(nodeData);
                return propertyInformation!.GetValue(node);
            }
            return null;
        }

        private string GetEnumTokenName(TokenIndentificator token) =>
            Enum.GetName(typeof(TokenIndentificator), token)!;
    }
}
