namespace Athena.NET.Athena.NET.Parser.Nodes.StatementNodes
{
    internal enum StatementResultMessage : uint
    {
        Successful = 0,
        Error = 1
    }

    internal sealed class StatementResult
    {
        public StatementNode? StatementNode { get; }
        public StatementResultMessage ResultMessage { get; }

        public StatementResult(StatementNode? node, StatementResultMessage message) 
        {
            StatementNode = node;
            ResultMessage = message;
        }

        public static StatementResult SuccessfulResult(StatementNode node) =>
            new(node, StatementResultMessage.Successful);
        public static StatementResult ErrorResult() =>
            new(null, StatementResultMessage.Error);
    }
}
