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

        public void CreateInterpretation(ReadOnlySpan<uint> instructions)
        {
            int lastNopInstruction = IndexOfNopInstruction(instructions);
            int instructionCount = 0;
            while (instructionCount != instructions.Length) 
            {
                OperatorCodes currentInstructionCode = (OperatorCodes)instructions[lastNopInstruction + 1];
                int nextNopInstruction = IndexOfNopInstruction(instructions[(lastNopInstruction + 1)..]);
                nextNopInstruction = nextNopInstruction == -1 ? instructions.Length :
                    nextNopInstruction + (lastNopInstruction + 1);

                ReadOnlySpan<uint> currentInstructions = instructions[(lastNopInstruction + 2)..(nextNopInstruction)];
                if(!TryInterpretInstruction(currentInstructionCode, currentInstructions))
                    throw new Exception("Instruction wasn't completed or found");

                lastNopInstruction = nextNopInstruction;
                instructionCount += currentInstructions.Length + 2;
            }
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
