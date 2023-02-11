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
        public Pen LinePen { get; set; } =
            new Pen(Brushes.Black, 3);

        public int NodeDistance { get; private set; }

        public NodeGraphElement(int distance, ReadOnlyMemory<NodeShape> nodeShapes)
        {
            NodeDistance = distance;
            this.nodeShapes = nodeShapes;
        }

        public void OnDraw(NodePosition nodePosition, Graphics graphics)
        {
            if (nodePosition.Node is null)
                return;

            var nodeSize = CalculateNodeSize(nodePosition.Node);
            var nodeRectangle = new Rectangle(nodePosition.Position, nodeSize);

            NodeShape currentShape = GetNodeShape(nodePosition.Node);
            currentShape.DrawShape.Invoke(nodePosition.Node, nodeRectangle, graphics);

            string tokenName = nodePosition.Node.NodeToken.GetEnumTokenName();
            var textRectangle = new Rectangle(new(nodePosition.Position.X + (nodeSize.Width / 10), nodePosition.Position.Y + 10), new(nodeSize.Width - (tokenName.Length * 3), nodeSize.Height - 40));
            graphics.DrawString(tokenName, SystemFonts.DefaultFont, TextBrush, textRectangle);

            DrawNodeChildrens(nodePosition, graphics);
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

        private void DrawNodeChildrens(NodePosition parentPositionNode, Graphics graphics)
        {
            ChildrenNodes parentChildrenNode = parentPositionNode.Node.ChildNodes;
            Point parentPosition = parentPositionNode.Position;

            int childrenYPosition = parentPosition.Y + NodeDistance;

            var leftNodePosition = new NodePosition(parentChildrenNode.LeftNode, parentPosition.X - NodeDistance, childrenYPosition);
            var rightNodePosition = new NodePosition(parentChildrenNode.RightNode, parentPosition.X + NodeDistance, childrenYPosition);

            DrawNodeConnection(leftNodePosition, parentPositionNode, graphics);
            DrawNodeConnection(rightNodePosition, parentPositionNode, graphics);

            OnDraw(leftNodePosition, graphics);
            OnDraw(rightNodePosition, graphics);
        }

        //I know that looks wierd and I will
        //totally rewrite this in a next days
        private void DrawNodeConnection(NodePosition firstNode, NodePosition secondNode, Graphics graphics)
        {
            if (firstNode.Node is null || secondNode.Node is null)
                return;

            var firstNodeCenter = CalculateCenterPosition(firstNode);
            var secondNodeCenter = CalculateCenterPosition(secondNode);
            graphics.DrawLine(LinePen, firstNodeCenter, secondNodeCenter);
        }

        private Point CalculateCenterPosition(NodePosition nodePosition) 
        {
            var nodeSize = CalculateNodeSize(nodePosition.Node);
            return new(nodePosition.Position.X + (nodeSize.Width / 2),
                nodePosition.Position.Y + (nodeSize.Height / 2));
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
