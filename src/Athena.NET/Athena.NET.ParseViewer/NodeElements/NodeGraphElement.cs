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
        public Pen LinePen { get; set; } =
            new Pen(Brushes.Black, 3);
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
            var textRectangle = new Rectangle(new(position.X + (nodeSize.Width / 10), position.Y + 10), new(nodeSize.Width - (tokenName.Length * 3), nodeSize.Height - 40));
            graphics.DrawString(tokenName, SystemFonts.DefaultFont, TextBrush, textRectangle);

            DrawNodeChildrens(node, graphics, position);
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

        private void DrawNodeChildrens(INode parentNode, Graphics graphics, Point position)
        {
            int childrenYPosition = position.Y + NodeDistance;
            var leftNodePosition = new Point(position.X - NodeDistance, childrenYPosition);
            var rightNodePosition = new Point(position.X + NodeDistance, childrenYPosition);

            DrawNodeConnection(parentNode.ChildNodes.LeftNode, leftNodePosition, parentNode, position, graphics);
            DrawNodeConnection(parentNode.ChildNodes.RightNode, rightNodePosition, parentNode, position, graphics);
            OnDraw(parentNode.ChildNodes.LeftNode, graphics, leftNodePosition);
            OnDraw(parentNode.ChildNodes.RightNode, graphics, rightNodePosition);
        }

        //I know that looks wierd and I will
        //totally rewrite this in a next days
        private void DrawNodeConnection(INode firstNode, Point firstPosition, INode secondNode, Point secondPosition, Graphics graphics)
        {
            if (firstNode is null || secondNode is null)
                return;

            var firstNodeCenter = CalculateCenterPosition(firstNode, firstPosition);
            var secondNodeCenter = CalculateCenterPosition(firstNode, secondPosition);

            graphics.DrawLine(LinePen, firstNodeCenter, secondNodeCenter);
        }

        private Point CalculateCenterPosition(INode node, Point currentPosition) 
        {
            var nodeSize = CalculateNodeSize(node);
            return new(currentPosition.X + (nodeSize.Width / 2),
                currentPosition.Y + (nodeSize.Height / 2));
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
