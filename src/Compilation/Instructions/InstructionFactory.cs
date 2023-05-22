using Athena.NET.Compilation.Instructions.Definition;

namespace Athena.NET.Compilation.Instructions;

/// <summary>
/// Provides a static inicializations, of all
/// instructions
/// </summary>
internal static class InstructionFactory
{
    /// <summary>
    /// Provides inicialization of store instruction
    /// </summary>
    public static StoreInstruction StoreInstruction { get; } = new StoreInstruction();

    /// <summary>
    /// Provides inicialization of print instruction
    /// </summary>
    public static PrintInstruction PrintInstruction { get; } = new PrintInstruction();

    /// <summary>
    /// Provides inicialization of jump instruction
    /// </summary>
    public static JumpInstruction JumpInstruction { get; } = new JumpInstruction();

    /// <summary>
    /// Provides inicialization of definition instruction
    /// </summary>
    public static DefinitionInstruction DefinitionInstruction { get; } = new DefinitionInstruction();

    /// <summary>
    /// Provides inicialization of definition call instruction
    /// </summary>
    public static DefinitionCallInstruction DefinitionCallInstruction { get; } = new DefinitionCallInstruction();
    /// <summary>
    /// Provides inicialization of operator instruction
    /// </summary> 
    public static OperatorInstruction OperatorInstruction { get; } = new OperatorInstruction();
}
