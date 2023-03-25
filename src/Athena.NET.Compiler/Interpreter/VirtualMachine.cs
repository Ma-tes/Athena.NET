using Athena.NET.Compiler.Instructions;

namespace Athena.NET.Compiler.Interpreter
{
    internal sealed class VirtualMachine : IDisposable
    {
        /// <summary>
        /// Implementation of a <see cref="RegisterMemory"/> class as
        /// a 8-bit register with a code <see cref="OperatorCodes.AH"/>.
        /// </summary>
        internal RegisterMemory<byte> RegisterAH { get; }
            = new(OperatorCodes.AH);
        /// <summary>
        /// Implementation of a <see cref="RegisterMemory"/> class as
        /// a 16-bit register with a code <see cref="OperatorCodes.AX"/>.
        /// </summary>
        internal RegisterMemory<short> RegisterAX { get; }
            = new(OperatorCodes.AX);
        /// <summary>
        /// Implementation of a <see cref="RegisterMemory"/> class as
        /// a 16-bit temporary register with a code <see cref="OperatorCodes.TM"/>.
        /// </summary>
        internal RegisterMemory<short> TemporaryRegisterTM { get; }
            = new(OperatorCodes.TM);

        public void CreateInterpretation(ReadOnlySpan<uint> instructions)
        {
            int lastNopInstruction = 0;
            int instructionCount = 0;
            while (instructionCount != instructions.Length) 
            {

            }
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

        private bool TryInterpretInstruction(OperatorCodes instructionCode, ReadOnlySpan<uint> instructionData) => instructionCode switch
        {
            OperatorCodes.Store => new StoreInstruction()
                .InterpretInstruction(instructionData, this),
            OperatorCodes.JumpE | OperatorCodes.JumpNE |
            OperatorCodes.JumpG | OperatorCodes.JumpGE |
            OperatorCodes.JumpL | OperatorCodes.JumpLE 
                => new JumpInstruction()
                    .InterpretInstruction(instructionData, this),

            OperatorCodes.Add | OperatorCodes.Sub |
            OperatorCodes.Mul | OperatorCodes.Div
            => new OperatorInstruction()
                .InterpretInstruction(instructionData, this),
            _ => false
        };

        public void Dispose() 
        {
            RegisterAH.Dispose();
            RegisterAX.Dispose();
            TemporaryRegisterTM.Dispose();
        }
    }
}
