using Athena.NET.Compiler.DataHolders;
using Athena.NET.Compiler.Instructions;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Athena.NET.Compiler.Interpreter
{
    /// <summary>
    /// Provides a complete interpretation
    /// of generated instructions, that are
    /// specifically created from <see cref="OperatorCodes"/>
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
        /// Index of a last found <see cref="OperatorCodes.Nop"/> instruction
        /// </summary>
        /// <remarks>
        /// It could be internally changed in a runtime
        /// </remarks>
        public int LastInstructionNopIndex { get; internal set; }

        /// <summary>
        /// Creates virtualized interpretation of
        /// already set instructions
        /// </summary>
        /// <param name="instructions">
        /// <see cref="OperatorCodes"/> instructions
        /// for creating an intepretation
        /// </param>
        /// <exception cref="Exception">
        /// Temporary <see cref="Exception"/> for
        /// failed interpretion of a instruction
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
                if(!TryInterpretInstruction(currentInstructionCode, currentInstructions))
                    throw new Exception("Instruction wasn't completed or found");
            }
        }

        /// <summary>
        /// Provides parsing of a possible <see cref="Structures.RegisterData"/> instruction
        /// or already determined values
        /// </summary>
        /// <param name="instructions">
        /// <see cref="OperatorCodes"/> instructions for parsing values
        /// </param>
        /// <returns>
        /// Values from instructions in a <see cref="ReadOnlySpan{T}"/> <see langword="int"/>
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
        /// Tries to get a coresponding <see langword="out"/>
        /// <see cref="RegisterMemory"/> <paramref name="registerMemory"/>,
        /// from <see cref="OperatorCodes"/> <paramref name="operatorCode"/>
        /// </summary>
        /// <param name="registerMemory">
        /// Coresponding <see langword="out"/> <see cref="RegisterMemory"/>,
        /// that was choosed by <see cref="OperatorCodes"/> <paramref name="operatorCode"/>
        /// </param>
        /// <param name="instructions">
        /// <see cref="OperatorCodes"/> instruction for specifing type of a register
        /// </param>
        /// <returns>
        /// A <see langword="bool"/> value, if <see cref="Structures.Register"/> with
        /// <see cref="OperatorCodes"/> <paramref name="operatorCode"/> was found
        /// </returns>
        public bool TryGetRegisterMemory([NotNullWhen(true)]out RegisterMemory? registerMemory, OperatorCodes operatorCode) 
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
        /// Finds index of a <see cref="OperatorCodes.Nop"/>
        /// instruction, from <paramref name="instructions"/>
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
        /// Executes interpretation of <paramref name="instructionCode"/>
        /// and it will return a <see langword="bool"/> value, if was
        /// succesful
        /// </summary>
        private bool TryInterpretInstruction(OperatorCodes instructionCode, ReadOnlySpan<uint> instructionData)
        {
            if (instructionCode == OperatorCodes.Store)
                return new StoreInstruction()
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

        /// <summary>
        /// Manages a dispose for every single
        /// inicialized <see cref="RegisterMemory"/>
        /// of a <see cref="virtualRegisters"/>
        /// </summary>
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
}
