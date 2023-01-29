using Athena.NET.Athena.NET.Lexer.Structures;
using Athena.NET.Athena.NET.Parser.Interfaces;
using Athena.NET.Athena.NET.Parser.Nodes.DataNodes;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Athena.NET.Athena.NET.Parser.Nodes
{
    internal abstract class OperatorNode<T, TReturn> : IEvaluationNode where T : OperatorNode<T, TReturn>, new()
    {
        private static ReadOnlySpan<OperatorNode<T, TReturn>> operators =>
            new(GetDefaultOperators(typeof(OperatorNode<T, TReturn>)).ToArray());
        protected abstract int operatorWeight { get; }

        public abstract TokenIndentificator NodeToken { get; }
        public abstract T FactoryCreate(ReadOnlyMemory<Token> tokens);

        public ChildrenNodes ChildNodes { get; private set; }
        public int ChildNodesCount { get; } = 0;

        public OperatorNode(ReadOnlyMemory<Token> tokens) 
        {
        }

        public ChildrenNodes SepareteNodes(ReadOnlyMemory<Token> tokens)
        {

        }

        public void Evaluate() 
        {
            var evaluatedLeftNode = GetEvaluatedNode(ChildNodes.LeftNode);
            var evaluatedRightNode = GetEvaluatedNode(ChildNodes.RightNode);

            ChildNodes = new(evaluatedLeftNode, evaluatedRightNode);
        }

        internal abstract TReturn CalculateData(TReturn firstData, TReturn secondData);

        private INode GetEvaluatedNode(INode node) 
        {
            if(IsEvaluate(out IEvaluationNode currentNode, node))
            {
                currentNode.Evaluate();
                if (node.ChildNodes.LeftNode is DataNode<TReturn> leftData &&
                    node.ChildNodes.RightNode is DataNode<TReturn> rightData) 
                {
                    var returnData = CalculateData(leftData.NodeData, rightData.NodeData);
                    return new DataNode<TReturn>(TokenIndentificator.Int, returnData);
                }
            }
            return node;
        }

        private bool IsEvaluate([NotNullWhen(true)]out IEvaluationNode evaluationNode, INode node)
        {
            bool isEvaluate = node.GetType().IsDefined(typeof(IEvaluationNode));
            evaluationNode = isEvaluate ? (IEvaluationNode)node : null!;
            return isEvaluate;
        }

        private static IEnumerable<OperatorNode<T, TReturn>> GetDefaultOperators(Type assemblyType)
        {
            var currentAssembly = Assembly.GetAssembly(assemblyType);

            Type[] assemblytypes = currentAssembly!.GetTypes();
            int typesLength = assemblytypes.Length;
            for (int i = 0; i < typesLength; i++)
            {
                Type currentType = assemblytypes[i];
                if (currentType.IsSubclassOf(typeof(OperatorNode<T, TReturn>)) && !currentType.IsAbstract)
                    yield return (OperatorNode<T, TReturn>)Activator.CreateInstance(currentType)!;
            }
        }
    }
}
