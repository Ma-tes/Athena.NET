using Athena.NET.Compilation.DataHolders;
using Athena.NET.Compilation.Instructions;
using Athena.NET.Compilation.Interpretation;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Athena.NET.Compilation.Interpreter;

/// <summary>
/// Interprets instructions corresponding to <see cref="OperatorCodes"/>.
/// </summary>
internal sealed class VirtualMachine : IDisposable
{
    private ImmutableArray<RegisterMemory> virtualRegisters =
        ImmutableArray.Create
        (
            ///8-bit register with a code <see cref="OperatorCodes.AH"/>.
            new RegisterMemory(OperatorCodes.AH, typeof(byte)),
            ///16-bit register with a code <see cref="OperatorCodes.AX"/>.
            new RegisterMemory(OperatorCodes.AX, typeof(short)),
            ///16-bit temporary register with a code <see cref="OperatorCodes.TM"/>.
            new RegisterMemory(OperatorCodes.TM, typeof(short))
        );

    /// <summary>
    /// Index of the last <see cref="OperatorCodes.Nop"/> instruction.
    /// </summary>
    /// <remarks>
    /// Value changes during interpretation.
    /// </remarks>
    public int LastInstructionNopIndex { get; internal set; }

    /// <summary>
    /// Performs virtualized interpretation of
    /// instruction byte-code.
    /// </summary>
    /// <param name="instructions">
    /// <see cref="OperatorCodes"/> instructions.
    /// </param>
    /// <exception cref="Exception">
    /// Interpreting an instruction failed.
    /// </exception>
    public void CreateInterpretation(ReadOnlySpan<uint> instructions)
    {
        LastInstructionNopIndex = IndexOfNopInstruction(instructions);
        int instructionIndex = 0;
        while (LastInstructionNopIndex != instructions.Length)
        {
            int nextInstructionIndex = LastInstructionNopIndex + 1;
            int nextNopInstruction = IndexOfNopInstruction(instructions[nextInstructionIndex..]);
            nextNopInstruction = nextNopInstruction == -1 ? instructions.Length :
                nextNopInstruction + nextInstructionIndex;

            OperatorCodes currentInstructionCode = (OperatorCodes)instructions[nextInstructionIndex];
            ReadOnlySpan<uint> currentInstructions = instructions[(nextInstructionIndex)..(nextNopInstruction)];

            LastInstructionNopIndex = nextNopInstruction;
            instructionIndex += currentInstructions.Length;
            if (!TryInterpretInstruction(currentInstructionCode, currentInstructions))
                throw new Exception("Instruction wasn't completed or found");
        }
    }

    /// <summary>
    /// Provides parsing of a possible <see cref="Structures.RegisterData"/> instruction
    /// or already determined values.
    /// </summary>
    /// <param name="instructions">
    /// <see cref="OperatorCodes"/> instructions for parsing values.
    /// </param>
    /// <returns>
    /// Values from instructions in a <see cref="ReadOnlySpan{T}"/> of <see langword="int"/>.
    /// </returns>
    internal ReadOnlySpan<int> GetInstructionData(ReadOnlySpan<uint> instructions)
    {
        var returnData = new NativeMemoryList<int>(6);
        int instructionCount = 0;
        while (instructionCount != instructions.Length)
        {
            uint firstInstruction = instructions[instructionCount];
            bool isRegisterMemory = TryGetRegisterMemory(out RegisterMemory? dataMemory, (OperatorCodes)firstInstruction);

            int currentData = isRegisterMemory && instructions.Length > 2 ?
                (int)dataMemory!.GetData(new(instructions[instructionCount + 2],
                instructions[instructionCount + 1])) : (int)firstInstruction;
            returnData.Add(currentData);

            instructionCount += isRegisterMemory ? 3 : 1;
        }
        return returnData.Span;
    }

    /// <summary>
    /// Tries to get a corresponding <see langword="out"/>
    /// <see cref="RegisterMemory"/> <paramref name="registerMemory"/>
    /// from <see cref="OperatorCodes"/> <paramref name="operatorCode"/>.
    /// </summary>
    /// <param name="registerMemory">
    /// Corresponding <see langword="out"/> <see cref="RegisterMemory"/>,
    /// that was chosen by <see cref="OperatorCodes"/> <paramref name="operatorCode"/>.
    /// </param>
    /// <param name="instructions">
    /// <see cref="OperatorCodes"/> instruction for specifing type of a register.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <see cref="Structures.Register"/> with
    /// <see cref="OperatorCodes"/> <paramref name="operatorCode"/> was found, otherwise <see langword="false"/>.
    /// </returns>
    public bool TryGetRegisterMemory([NotNullWhen(true)] out RegisterMemory? registerMemory, OperatorCodes operatorCode)
    {
        int registerCount = virtualRegisters.Length;
        for (int i = 0; i < registerCount; i++)
        {
            RegisterMemory currentRegister = virtualRegisters[i];
            if (currentRegister.RegisterCode == operatorCode)
            {
                registerMemory = currentRegister;
                return true;
            }
        }
        registerMemory = null;
        return false;
    }

    /// <summary>
    /// Finds index of the first <see cref="OperatorCodes.Nop"/>
    /// instruction in <paramref name="instructions"/>.
    /// </summary>
    private int IndexOfNopInstruction(ReadOnlySpan<uint> instructions)
    {
        int instructionCount = instructions.Length;
        for (int i = 0; i < instructionCount; i++)
        {
            OperatorCodes currentCode = (OperatorCodes)instructions[i];
            if (currentCode == OperatorCodes.Nop)
                return i;
        }
        return -1;
    }

    /// <summary>
    /// Executes interpretation of an <paramref name="instructionCode"/>
    /// and returns <see langword="true"/>, if successful, otherwise <see langword="false"/>.
    /// </summary>
    private bool TryInterpretInstruction(OperatorCodes instructionCode, ReadOnlySpan<uint> instructionData)
    {
        if (instructionCode == OperatorCodes.Store)
            return new StoreInstruction()
                .InterpretInstruction(instructionData, this);
        if (instructionCode == OperatorCodes.Print)
            return new PrintInstruction()
                .InterpretInstruction(instructionData, this);

        if (instructionCode >= OperatorCodes.JumpE &&
           instructionCode <= OperatorCodes.JumpLE)
            return new JumpInstruction()
                .InterpretInstruction(instructionData, this);

        if (instructionCode >= OperatorCodes.Add &&
           instructionCode <= OperatorCodes.Div)
            return new OperatorInstruction()
            .InterpretInstruction(instructionData, this);
        return false;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        int registerMemoryCount = virtualRegisters.Length;
        for (int i = 0; i < registerMemoryCount; i++)
        {
            RegisterMemory currentMemory = virtualRegisters[i];
            currentMemory.Dispose();
        }
    }
}
