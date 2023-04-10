using Athena.NET.Lexing;
using Athena.NET.Lexing.Structures;
using Athena.NET.Parsing.Interfaces;

namespace Athena.NET.Parsing.Nodes.Data;

//I'm still not sure about
//this implementation, maybe I should
//create a different implementation
//of this data holding node
public class DataNode<T> : INode
{
    public TokenIndentificator NodeToken { get; }
    public ChildrenNodes ChildNodes { get; set; } =
        ChildrenNodes.BlankNodes;

    public T NodeData { get; }

    public DataNode(TokenIndentificator token, T data)
    {
        NodeToken = token;
        NodeData = data;
    }

    public NodeResult<INode> CreateStatementResult(ReadOnlySpan<Token> tokens, int tokenIndex) =>
        new SuccessulNodeResult<INode>(this);
}
