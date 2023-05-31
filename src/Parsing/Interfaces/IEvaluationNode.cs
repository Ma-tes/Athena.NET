namespace Athena.NET.Parsing.Interfaces;

/// <summary>
/// Standart implemntation of <see cref="INode"/>,
/// with addition of being able to evaluate.
/// </summary>
internal interface IEvaluationNode : INode
{
    /// <summary>
    /// Executes implemented evaluation.
    /// </summary>
    public void Evaluate();
}
