using Athena.NET.Athena.NET.Lexer;

namespace Athena.NET.Athena.NET.Parser.Nodes.DataNodes
{
    //Maybe I should separete them into parent instance node
    //that contains children node identifier
    internal sealed class InstanceNode : DataNode<ReadOnlyMemory<char>>
    {
        public InstanceNode(TokenIndentificator typeToken, ReadOnlyMemory<char> identifierName) : base(typeToken, identifierName) 
        {
        }
    } 
}
