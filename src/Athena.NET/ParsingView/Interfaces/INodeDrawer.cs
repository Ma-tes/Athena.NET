using Athena.NET.ParsingView.NodeElements;
using System.Drawing;

namespace Athena.NET.ParsingView.Interfaces;

public interface INodeDrawer
{
    public NodePosition OnDraw(NodePosition nodePosition, Graphics graphics);
}
