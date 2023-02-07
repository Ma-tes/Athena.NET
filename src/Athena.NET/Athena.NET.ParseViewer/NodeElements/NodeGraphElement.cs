using Athena.NET.Athena.NET.Parser;
using Athena.NET.Athena.NET.Parser.Interfaces;
using Athena.NET.Athena.NET.Parser.Nodes.DataNodes;
using Athena.NET.Athena.NET.ParseViewer.Interfaces;
using System.Drawing;
using System.Runtime.Versioning;

namespace Athena.NET.Athena.NET.ParseViewer.NodeElements
{
    //TODO: Make sure that drawing a node
    //tree scheme will be multiplaform
    [SupportedOSPlatform("windows")]
    public sealed class NodeGraphElement : INodeDrawer
    {
        private ReadOnlyMemory<NodeShape> nodeShapes;

        public Brush TextBrush { get; set; } = Brushes.Black;
        public int NodeDistance { get; set; }

        public NodeGraphElement(int distance, ReadOnlyMemory<NodeShape> nodeShapes)
        {
            NodeDistance = distance;
            this.nodeShapes = nodeShapes;
        }

        public void OnDraw(INode node, Graphics graphics, Point position)
        {
            if (node is null)
                return;

            var nodeSize = CalculateNodeSize(node);
            var nodeRectangle = new Rectangle(position, nodeSize);

            NodeShape currentShape = GetNodeShape(node);
            currentShape.DrawShape.Invoke(node, nodeRectangle, graphics);

            string tokenName = node.NodeToken.GetEnumTokenName();
            var textRectangle = new Rectangle(new(position.X + (nodeSize.Width / 10), position.Y + 10), new(nodeSize.Width - (tokenName.Length * 2), nodeSize.Height - 40));
            graphics.DrawString(tokenName, SystemFonts.DefaultFont, TextBrush, textRectangle);

            DrawNodeChildrens(node.ChildNodes, graphics, position);
        }

        private NodeShape GetNodeShape(INode node) 
        {
            Type nodeType = node.GetType();
            var shapesSpan = nodeShapes.Span;
            for (int i = 0; i < shapesSpan.Length; i++)
            {
                var currentShape = shapesSpan[i];
                if(nodeType.IsGenericTypeEqual(currentShape.NodeType) ||
                    nodeType.IsAssignableTo(currentShape.NodeType)) 
                {
                    return currentShape;
                }
            }
            return NodeShape.DefaultShape;
        }

        private void DrawNodeChildrens(ChildrenNodes childrenNodes, Graphics graphics, Point position)
        {
            int childrenYPosition = position.Y + NodeDistance;
            var leftNodePosition = new Point(position.X - NodeDistance, childrenYPosition);
            var rightNodePosition = new Point(position.X + NodeDistance, childrenYPosition);

            OnDraw(childrenNodes.LeftNode, graphics, leftNodePosition);
            OnDraw(childrenNodes.RightNode, graphics, rightNodePosition);
        }

        private Size CalculateNodeSize(INode node)
        {
            int returnSize = node.NodeToken.GetEnumTokenName().Length;
            if (NodeHelper.TryGetDataNode(out DataNode<object> dataNode, node)) 
            {
                int dataLength = dataNode.NodeData.ToString()!.Length;
                if (dataLength > returnSize)
                    returnSize = dataLength;
            }
            int scaleSize = returnSize * 50;

            return returnSize > 7 ? new(scaleSize / (((returnSize) / 5)), returnSize * (returnSize * 3)) :
                new(scaleSize, scaleSize);
        }
    }
}
