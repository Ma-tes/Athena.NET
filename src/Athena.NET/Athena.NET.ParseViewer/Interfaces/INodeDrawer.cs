using Athena.NET.Athena.NET.Parser.Interfaces;
using System.Drawing;

namespace Athena.NET.Athena.NET.ParseViewer.Interfaces
{
    public interface INodeDrawer
    {
        public void OnDraw(INode node, Graphics graphics, Point position);
    }
}
