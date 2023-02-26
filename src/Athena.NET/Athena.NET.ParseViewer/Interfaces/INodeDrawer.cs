using Athena.NET.Athena.NET.ParseViewer.NodeElements;
using System.Drawing;

namespace Athena.NET.Athena.NET.ParseViewer.Interfaces
{
    public interface INodeDrawer
    {
        public NodePosition OnDraw(NodePosition nodePosition, Graphics graphics);
    }
}
