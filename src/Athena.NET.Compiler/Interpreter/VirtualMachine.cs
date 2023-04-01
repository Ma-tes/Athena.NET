using Athena.NET.Compiler.DataHolders;
using Athena.NET.Compiler.Instructions;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Athena.NET.Compiler.Interpreter
{
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

        public int LastInstructionNopIndex { get; internal set; }

        public void CreateInterpretation(ReadOnlySpan<uint> instructions)
        {
            LastInstructionNopIndex = IndexOfNopInstruction(instructions);
            int instructionIndex = 0;
            while (LastInstructionNopIndex != instructions.Length)
            {
                OperatorCodes currentInstructionCode = (OperatorCodes)instructions[LastInstructionNopIndex + 1];
                int nextNopInstruction = IndexOfNopInstruction(instructions[(LastInstructionNopIndex + 1)..]);
                nextNopInstruction = nextNopInstruction == -1 ? instructions.Length :
                    nextNopInstruction + (LastInstructionNopIndex + 1);

                ReadOnlySpan<uint> currentInstructions = instructions[(LastInstructionNopIndex + 1)..(nextNopInstruction)];
                LastInstructionNopIndex = nextNopInstruction;

                instructionIndex += currentInstructions.Length;
                if(!TryInterpretInstruction(currentInstructionCode, currentInstructions))
                    throw new Exception("Instruction wasn't completed or found");
            }
        }

        //TODO: This implementation needs to
        //be reimplemented as soon as possible
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
                instructionCount = isRegisterMemory ?
                    instructionCount + 3 : instructionCount + 1;
            }
            return returnData.Span;
        }

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

        //TODO: Create a propriete factory pattern calling
        //for every instruction
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

        //TODO: Dispose memory registers with more elegant way,
        //then using for loop
        public void Dispose() 
        {
        }
    }
}
