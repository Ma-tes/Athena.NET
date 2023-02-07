using Athena.NET.Athena.NET.Lexer;
using Athena.NET.Athena.NET.Parser.Interfaces;
using Athena.NET.Athena.NET.Parser.Nodes.DataNodes;
using System.Diagnostics.CodeAnalysis;

namespace Athena.NET.Athena.NET.ParseViewer
{
    public static class NodeHelper
    {
        private static readonly string nodeData = "NodeData";

        public static bool TryGetDataNode([NotNullWhen(true)]out DataNode<object> returnNode, INode node)
        {
            returnNode = null!;
            var currentData = GetGenericNodeData(node);
            if (currentData is not null) 
            {
                returnNode = new DataNode<object>(node.NodeToken, currentData);
                return true;
            }
            return false;
        }

        private static object? GetGenericNodeData(INode node) 
        {
            Type nodeType = node.GetType();
            if (nodeType.IsGenericTypeEqual(typeof(DataNode<>))) 
            {
                var propertyInformation = nodeType.GetProperty(nodeData);
                return propertyInformation!.GetValue(node);
            }
            return null;
        }

        public static bool IsGenericTypeEqual(this Type sourceNodeType, Type abstractType) =>
            abstractType.IsGenericType && (sourceNodeType.GUID == abstractType.GUID);

        public static string GetEnumTokenName(this TokenIndentificator token) =>
            Enum.GetName(typeof(TokenIndentificator), token)!;
    }
}
