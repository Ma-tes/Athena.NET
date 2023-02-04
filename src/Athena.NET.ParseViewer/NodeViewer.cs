using Athena.NET.Athena.NET.Parser.Interfaces;
using Athena.NET.ParseViewer.Interfaces;
using System.Collections.Immutable;
using System.Drawing;

namespace Athena.NET.ParseViewer
{
    public sealed class NodeViewer : IDisposable
    {
        private Bitmap nodeBitmap;
        private Graphics nodeGraphics;

        private ImmutableArray<INodeDrawer> drawElements =
            ImmutableArray.Create<INodeDrawer>
            (
            );

        public ReadOnlyMemory<INode> RenderNodes { get; }
        public Size ImageSize { get; }

        public NodeViewer(ReadOnlyMemory<INode> nodes, Size imageSize)
        {
            RenderNodes = nodes;

            int width = imageSize.Width * (nodes.Length / 2);
            int height = imageSize.Height * (nodes.Length);
            ImageSize = new(width, height);

            //TODO: Create better solutions that
            //will be multiplatform and not just
            //for windows
            nodeBitmap = new Bitmap(ImageSize.Width, ImageSize.Height)!;
        }

        public Image CreateImage(string path) 
        {
            var nodesSpan = RenderNodes.Span;
            int nodesLenght = nodesSpan.Length;
            for (int i = 0; i < nodesLenght; i++)
            {

            }
        }


        public void Dispose() 
        {
            nodeBitmap.Dispose();
            nodeGraphics.Dispose();
        }
    }
}
