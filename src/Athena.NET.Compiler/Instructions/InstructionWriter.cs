using Athena.NET.Compiler.DataHolders;
using Athena.NET.Compiler.Structures;
using Athena.NET.Parser.Interfaces;
using Athena.NET.Parser.Nodes.OperatorNodes;
using Athena.NET.Parser.Nodes.StatementNodes;
using Athena.NET.Parser.Nodes.StatementNodes.BodyStatements;

namespace Athena.NET.Compiler.Instructions
{
    /// <summary>
    /// Provides a generation of raw byte code, 
    /// by <see cref="OperatorCodes"/> [custom instructions]
    /// </summary>
    /// <remarks>
    /// This implementation isn't that fully autonomous, 
    /// because it requires adding every single
    /// <see cref="Register"/> to individual methods.<br/>
    /// Due to the compilation speed of reflection, 
    /// this class must be hand coded as much as possible.
    /// </remarks>
    public sealed class InstructionWriter : IDisposable
    {
        /// <summary>
        /// Implementation of a <see cref="Register"/> class as
        /// a 8-bit register with a code <see cref="OperatorCodes.AH"/>.
        /// </summary>
        internal Register RegisterAH { get; }
            = new(OperatorCodes.AH, typeof(byte));
        /// <summary>
        /// Implementation of a <see cref="Register"/> class as
        /// a 16-bit register with a code <see cref="OperatorCodes.AX"/>.
        /// </summary>
        internal Register RegisterAX { get; }
            = new(OperatorCodes.AX, typeof(short));
        /// <summary>
        /// Implementation of a <see cref="Register"/> class as
        /// a 16-bit temporary register with a code <see cref="OperatorCodes.TM"/>.
        /// </summary>
        internal Register TemporaryRegisterTM { get; }
            = new(OperatorCodes.TM, typeof(short));

        /// <summary>
        /// It's being used for storing individual
        /// instructions as a <see cref="uint"/> in
        /// a <see cref="NativeMemoryList{T}"/>.
        /// </summary>
        public NativeMemoryList<uint> InstructionList { get; }
            = new();

        /// <summary>
        /// Creates individual instructions 
        /// from nodes, which are then stored
        /// in a <see cref="InstructionWriter.InstructionList"/>.
        /// </summary>
        public void CreateInstructions(ReadOnlySpan<INode> nodes)
        {
            int nodesLength = nodes.Length;
            for (int i = 0; i < nodesLength; i++)
            {
                InstructionList.Add((uint)OperatorCodes.Nop);
                if (!TryGetEmitInstruction(nodes[i]))
                    throw new Exception("Instruction wasn't completed or found");
            }
        }

        /// <summary>
        /// Executes a related instruction to a specific 
        /// node that was derived from <see cref="INode"/>
        /// </summary>
        /// <returns>
        /// Specific <see cref="bool"/> state of a
        /// <see cref="IInstruction{T}.EmitInstruction(T, InstructionWriter)"/>
        /// <see langword="where"/> T : <see cref="INode"/>
        /// </returns>
        private bool TryGetEmitInstruction(INode node) => node switch
        {
            EqualAssignStatement equalNode => new StoreInstruction()
                .EmitInstruction(equalNode, this),
            BodyStatement bodyNode => new JumpInstruction()
                .EmitInstruction(bodyNode, this),
            OperatorNode operatorNode => new OperatorInstruction()
                .EmitInstruction(operatorNode, this),
            _ => false
        };

        /// <summary>
        /// Choose maching <see cref="Register"/> from current
        /// <see cref="InstructionWriter"/>, by size of data
        /// </summary>
        /// <returns>Specific register for current data size</returns>
        internal Register? GetEmitIntRegister(int data)
        {
            if (RegisterAH.CalculateByteSize(data) != RegisterAH.TypeSize) { return RegisterAH; }
            if (RegisterAX.CalculateByteSize(data) != RegisterAX.TypeSize) { return RegisterAX; }
            return null;
        }

        /// <summary>
        /// Choose maching <see cref="Register"/> and get <see cref="MemoryData"/>
        /// from current <see cref="InstructionWriter"/>, by indetifier id
        /// </summary>
        /// <returns>Specific register and coresponding
        /// <see langword="out"/> <see cref="MemoryData"/>
        /// </returns>
        internal Register? GetIdentifierData(out MemoryData returnData, uint identifierId)
        {
            if (RegisterAH.TryGetMemoryData(out MemoryData AHData, identifierId)) { returnData = AHData; return RegisterAH; }
            if (RegisterAX.TryGetMemoryData(out MemoryData AXData, identifierId)) { returnData = AXData; return RegisterAX; }
            returnData = default!;
            return null;
        }

        /// <summary>
        /// Choose maching <see cref="Register"/> and get <see cref="MemoryData"/>
        /// from current <see cref="InstructionWriter"/>, by indetifier name
        /// </summary>
        /// <returns>Specific register and coresponding
        /// <see langword="out"/> <see cref="MemoryData"/>
        /// </returns>
        internal Register? GetIdentifierData(out MemoryData returnData, ReadOnlyMemory<char> identifierName)
        {
            uint identiferId = MemoryData.CalculateIdentifierId(identifierName);
            return GetIdentifierData(out returnData, identiferId);
        }

        /// <summary>
        /// Manage disposes for every <see cref="Register"/>
        /// and the <see cref="InstructionWriter.InstructionList"/>
        /// </summary>
        public void Dispose()
        {
            RegisterAH.Dispose();
            RegisterAX.Dispose();
            InstructionList.Dispose();
        }
    }
}
