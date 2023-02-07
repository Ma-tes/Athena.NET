using Athena.NET.Athena.NET.Parser.Interfaces;
using Athena.NET.Athena.NET.Parser.Nodes;
using Athena.NET.Athena.NET.Parser.Nodes.DataNodes;
using Athena.NET.Athena.NET.ParseViewer.Interfaces;
using Athena.NET.Athena.NET.ParseViewer.NodeElements;
using System.Drawing;
using System.Runtime.Versioning;

namespace Athena.NET.Athena.NET.ParseViewer
{
    [SupportedOSPlatform("windows")]
    public sealed class NodeViewer : IDisposable
    {
        private Bitmap nodeBitmap;
        public Graphics NodeGraphics { get; }

        private readonly ReadOnlyMemory<INodeDrawer> drawElements =
            new INodeDrawer[]
            {
                new NodeGraphElement(150, new NodeShape[]
                {
                    new(typeof(OperatorNode),
                        (INode node, Rectangle rectangle, Graphics graphics)
                            => graphics.DrawPolygon(Pens.Black, new PointF[]
                            {
                                new(rectangle.X, rectangle.Y),
                                new(rectangle.X + rectangle.Width, rectangle.Y),
                                new(rectangle.X + (rectangle.Width / 2), rectangle.Y + rectangle.Height),
                            })),
                    new(typeof(DataNode<>),
                        (INode node, Rectangle rectangle, Graphics graphics)
                            => 
                            {
                                graphics.FillEllipse(Brushes.Green, rectangle);
                                _ = NodeHelper.TryGetDataNode(out DataNode<object> dataNode, node);
                                graphics.DrawString(dataNode.NodeData.ToString(), SystemFonts.DefaultFont, Brushes.White,
                                    rectangle.X + (rectangle.Width / 3), rectangle.Y + (rectangle.Width / 3));
                            })
                })
            };
        private Size originalSize;

        public ReadOnlyMemory<INode> RenderNodes { get; }
        public Size ImageSize { get; }

        public NodeViewer(ReadOnlyMemory<INode> nodes, Size imageSize)
        {
            RenderNodes = nodes;
            originalSize = imageSize;

            float width = imageSize.Width * (nodes.Length);
            float height = imageSize.Height * (nodes.Length);
            ImageSize = new((int)width, (int)height);

            //TODO: Create better solutions that
            //will be multiplatform and not just
            //for windows
            nodeBitmap = new Bitmap(ImageSize.Width, ImageSize.Height)!;
            nodeBitmap.SetResolution(400, 400);

            NodeGraphics = Graphics.FromImage(nodeBitmap);
            NodeGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            NodeGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            NodeGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            NodeGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
        }

        public Image CreateImage() 
        {
            var nodesSpan = RenderNodes.Span;
            int nodesLenght = nodesSpan.Length;
            for (int i = 0; i < nodesLenght; i++)
            {
                var currentPosition = new Point(originalSize.Width / 2, (originalSize.Height * i));
                DrawNodeElements(nodesSpan[i], currentPosition);
            }
            return nodeBitmap;
        }

        private void DrawNodeElements(INode node, Point position)
        {
            var nodeElementsSpan = drawElements.Span;
            for (int i = 0; i < nodeElementsSpan.Length; i++)
            {
                var currentElement = nodeElementsSpan[i];
                currentElement.OnDraw(node, NodeGraphics, position);
            }
        }

        public void Dispose() 
        {
            nodeBitmap.Dispose();
            NodeGraphics.Dispose();
        }
    }
}
