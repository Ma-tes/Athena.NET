﻿using Athena.NET.Parsing.Interfaces;
using Athena.NET.Parsing.Nodes.Data;
using Athena.NET.Parsing.Nodes.Operators;
using Athena.NET.ParsingView.Interfaces;
using Athena.NET.ParsingView.NodeElements;
using System.Collections.Immutable;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.Versioning;

namespace Athena.NET.ParsingView;

[SupportedOSPlatform("windows")]
public sealed class NodeViewer : IDisposable
{
    private Bitmap nodeBitmap;
    private Size originalSize;

    private readonly ImmutableArray<INodeDrawer> drawElements =
        ImmutableArray.Create<INodeDrawer>
        (
            new NodeGraphElement(new NodeShape[]
            {
                new(typeof(OperatorNode),
                    (INode node, Rectangle rectangle, Graphics graphics)
                        => graphics.FillPolygon(Brushes.DimGray, new PointF[]
                        {
                            new(rectangle.X, rectangle.Y),
                            new(rectangle.X + rectangle.Width, rectangle.Y),
                            new(rectangle.X + (rectangle.Width / 2), rectangle.Y + rectangle.Height),
                        })),
                new(typeof(DataNode<>),
                    (INode node, Rectangle rectangle, Graphics graphics )
                        =>
                        {
                            graphics.FillRectangle(Brushes.LightGray, rectangle);
                            _ = NodeHelper.TryGetDataNode(out DataNode<object> dataNode, node);
                            graphics.DrawString(dataNode.NodeData.ToString(), SystemFonts.DefaultFont, Brushes.Black,
                                rectangle.X + (rectangle.Width / 4), rectangle.Y + (rectangle.Width / 3));
                        })
            })
        );

    public ReadOnlyMemory<INode> RenderNodes { get; }
    public Size ImageSize { get; }
    public Graphics NodeGraphics { get; }

    public NodeViewer(ReadOnlyMemory<INode> nodes, Size imageSize)
    {
        RenderNodes = nodes;
        originalSize = imageSize;

        float width = imageSize.Width * (nodes.Length);
        float height = imageSize.Height * (nodes.Length);
        ImageSize = new((int)width, (int)height);

        //TODO: Make crossplatform
        nodeBitmap = new Bitmap(ImageSize.Width, ImageSize.Height)!;
        nodeBitmap.SetResolution(400, 400);

        NodeGraphics = Graphics.FromImage(nodeBitmap);
        NodeGraphics.SmoothingMode = SmoothingMode.HighQuality;
        NodeGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        NodeGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        NodeGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;
    }

    public Image CreateImage()
    {
        ReadOnlySpan<INode> nodesSpan = RenderNodes.Span;
        int nodesLenght = nodesSpan.Length;
        for (int i = 0; i < nodesLenght; i++)
        {
            var currentPosition = new Point(ImageSize.Width / 2, (originalSize.Height * i));
            DrawNodeElements(nodesSpan[i], currentPosition);
        }
        return nodeBitmap;
    }

    private void DrawNodeElements(INode node, Point position)
    {
        for (int i = 0; i < drawElements.Length; i++)
        {
            INodeDrawer currentElement = drawElements[i];
            currentElement.OnDraw(new(node, position), NodeGraphics);
        }
    }

    public void Dispose()
    {
        nodeBitmap.Dispose();
        NodeGraphics.Dispose();
        GC.SuppressFinalize(this);
    }
}
