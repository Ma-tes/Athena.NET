using Athena.NET.Athena.NET.Parser.Interfaces;
using System.Drawing;

namespace Athena.NET.ParseViewer.Interfaces
{
    internal interface INodeDrawer
    {
        public void OnDraw(INode node, Graphics graphics);
    }
}
