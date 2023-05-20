using Athena.NET.Compilation.Structures;
using Athena.NET.Parsing.Nodes.Data;

namespace Athena.NET.Compilation.Instructions.Structures;

/// <summary>
/// Provides storing data of <see cref="Definition"/>,
/// that's mostly precompiled.
/// </summary>
public struct DefinitionData
{
    /// <summary>
    /// Identificator, that's calculated from
    /// <see cref="DefinitionNode.DefinitionIdentifier"/>.
    /// </summary>
    public uint Identificator { get; }

    /// <summary>
    /// Relative position index of current definition,
    /// in a <see cref="InstructionWriter.InstructionList"/>.
    /// </summary>
    public int DefinitionIndex { get; set; }

    /// <summary>
    /// Relative length of all instructions in
    /// definition.
    /// </summary>
    public int DefinitionLength { get; set; }

    /// <summary>
    /// Provides storing all arguments as a <see cref="MemoryData"/>,
    /// in selected <see cref="OperatorCodes.TM"/> <see cref="Register"/>.
    /// </summary>
    public ReadOnlyMemory<MemoryData> DefinitionArguments { get; }

    internal BodyNode DefinitionBodyNode { get; }
    internal MemoryData DefinitionMemoryData { get; }

    public DefinitionData(uint identificator, int definitionIndex, int definitionLength,
        ReadOnlyMemory<MemoryData> definitionArguments, BodyNode definitionBodyNode, MemoryData definitionMemoryData)
    {
        Identificator = identificator;
        DefinitionIndex = definitionIndex;
        DefinitionLength = definitionLength;
        DefinitionArguments = definitionArguments;
        DefinitionBodyNode = definitionBodyNode;
        DefinitionMemoryData = definitionMemoryData;
    }
} 
