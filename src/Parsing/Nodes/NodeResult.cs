using Athena.NET.Parsing.Interfaces;

namespace Athena.NET.Parsing.Nodes;

public enum StatementResultMessage : uint
{
    Successful = 0,
    Error = 1
}

public abstract class NodeResult<T> where T : INode
{
    public T? Node { get; }
    public StatementResultMessage ResultMessage { get; }
    public string Message { get; protected set; } =
        string.Empty;

    public NodeResult(T node, StatementResultMessage resultMessage)
    {
        Node = node;
        ResultMessage = resultMessage;
    }
}

public sealed class SuccessulNodeResult<T> : NodeResult<T> where T : INode
{
    public SuccessulNodeResult(T node) :
        base(node, StatementResultMessage.Successful)
    {
    }
}

public sealed class ErrorNodeResult<T> : NodeResult<T> where T : INode
{
    public ErrorNodeResult(string message) :
        base(default!, StatementResultMessage.Error)
    {
        Message = message;
    }
}
