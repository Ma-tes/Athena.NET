using Athena.NET.Parsing;
using Athena.NET.Parsing.Interfaces;
using Athena.NET.Parsing.Nodes.Data;
using Athena.NET.ParsingView.Interfaces;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

namespace Athena.NET.ParsingView.NodeElements;

//TODO: Make sure that drawing a node
//tree scheme will be multiplaform
[SupportedOSPlatform("windows")]
public sealed class NodeGraphElement : INodeDrawer
{
    private NodePosition lastNodePosition;
    private ReadOnlyMemory<NodeShape> nodeShapes;

    public Brush TextBrush { get; set; } = Brushes.Black;

    public int DistanceMultiplier { get; set; } = 25;
    public int NodeDistance { get; private set; }

    public NodeGraphElement(ReadOnlyMemory<NodeShape> nodeShapes)
    {
        this.nodeShapes = nodeShapes;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NodePosition OnDraw(NodePosition nodePosition, Graphics graphics)
    {
        if (nodePosition.Node is null)
            return default;
        NodeDistance = CalculateGraphDistance(nodePosition.Node) * DistanceMultiplier;
        lastNodePosition = nodePosition;

        var currentNodePosition = new NodePosition(nodePosition.Node, nodePosition.Position);
        if (!nodePosition.Node.ChildNodes.Equals(ChildrenNodes.BlankNodes))
            DrawNodeChildrens(currentNodePosition, graphics);

        Size nodeSize = CalculateNodeSize(nodePosition.Node);
        var nodeRectangle = new Rectangle(nodePosition.Position, nodeSize);
        NodeShape currentShape = GetNodeShape(nodePosition.Node);
        currentShape.DrawShape.Invoke(nodePosition.Node, nodeRectangle, graphics);

        string tokenName = nodePosition.Node.NodeToken.GetEnumTokenName();
        var textRectangle = new Rectangle(
            new Point(nodePosition.Position.X + (nodeSize.Width / 10), nodePosition.Position.Y + 10),
            new Size(nodeSize.Width - (tokenName.Length * 3), nodeSize.Height - 40));
        graphics.DrawString(tokenName, SystemFonts.DefaultFont, TextBrush, textRectangle);
        return currentNodePosition;
    }

    private NodeShape GetNodeShape(INode node)
    {
        Type nodeType = node.GetType();
        ReadOnlySpan<NodeShape> shapesSpan = nodeShapes.Span;
        for (int i = 0; i < shapesSpan.Length; i++)
        {
            NodeShape currentShape = shapesSpan[i];
            if (nodeType.IsGenericTypeEqual(currentShape.NodeType) ||
                nodeType.IsAssignableTo(currentShape.NodeType))
            {
                return currentShape;
            }
        }
        return NodeShape.DefaultShape;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DrawNodeChildrens(NodePosition parentPositionNode, Graphics graphics)
    {
        ChildrenNodes parentChildrenNode = parentPositionNode.Node.ChildNodes;
        Point parentPosition = parentPositionNode.Position;

        int childrenYPosition = parentPosition.Y + (60 + NodeDistance);
        var leftNodePosition = CalculateRelativeNodePosition(new NodePosition(parentChildrenNode.LeftNode, parentPosition.X - NodeDistance, childrenYPosition));
        var rightNodePosition = CalculateRelativeNodePosition(new NodePosition(parentChildrenNode.RightNode, parentPosition.X + NodeDistance, childrenYPosition));

        DrawNodeConnection(leftNodePosition, parentPositionNode, graphics);
        DrawNodeConnection(rightNodePosition, parentPositionNode, graphics);

        OnDraw(leftNodePosition, graphics);
        OnDraw(rightNodePosition, graphics);
    }

    private void DrawNodeConnection(NodePosition firstNode, NodePosition secondNode, Graphics graphics)
    {
        if (firstNode.Node is null || secondNode.Node is null)
            return;

        Point firstNodePosition = CalculateCenterPosition(firstNode);
        Point secondNodePosition = CalculateCenterPosition(secondNode);

        graphics.DrawCurve(Pens.Black, new Point[]
        {
            secondNodePosition,
            new(firstNodePosition.X, secondNodePosition.Y + 5),
            firstNodePosition
        });
    }

    private NodePosition CalculateRelativeNodePosition(NodePosition nodePosition)
    {
        Size nodeSize = CalculateNodeSize(nodePosition.Node);
        int positionDifferenceIndex = Math.Abs(lastNodePosition.Position.X - nodePosition.Position.X) /
            (lastNodePosition.Position.X - nodePosition.Position.X);

        int currentPositionX = nodeSize.Width < 10 ?
            nodePosition.Position.X : nodePosition.Position.X - ((nodeSize.Width) * positionDifferenceIndex);
        var newPosition = new Point(currentPositionX, nodePosition.Position.Y);
        return new(nodePosition.Node, newPosition);
    }

    private Point CalculateCenterPosition(NodePosition nodePosition)
    {
        Size nodeSize = CalculateNodeSize(nodePosition.Node);
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
        return returnSize > 7 ? new(scaleSize / (((returnSize) / 5)), returnSize * (returnSize + (returnSize / 4))) :
            new(scaleSize, scaleSize);
    }

    private int CalculateGraphDistance(INode parentNode, int offset = 0)
    {
        ChildrenNodes childrenNodes = parentNode.ChildNodes;
        if (childrenNodes.Equals(ChildrenNodes.BlankNodes))
            return offset;
        offset++;
        return (CalculateGraphDistance(childrenNodes.LeftNode!, offset) +
            CalculateGraphDistance(childrenNodes.RightNode!, offset));
    }
}
