using Athena.NET.Compiler.DataHolders;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Athena.NET.Compiler.Structures
{
    /// <summary>
    /// Is a specific memory manager, that can be created by single 
    /// <see cref="RegisterCode"/> and <see cref="TypeSize"/>.
    /// </summary>
    /// <remarks>
    /// For memory managment is used <see langword="unmanaged"/>
    /// <see cref="NativeMemoryList{T}"/> of <see cref="MemoryData"/>
    /// that contains<br/> <see cref="MemoryData.IdentifierId"/>,
    /// <see cref="MemoryData.Size"/> and <see cref="MemoryData.Offset"/>.
    /// </remarks>
    internal sealed class Register : IDisposable
    {
        private NativeMemoryList<MemoryData> memoryData;
        private int lastOffset = 0;

        /// <summary>
        /// Specified maximum size of current register in bits,
        /// from <see langword="unmanaged"/> <see cref="Type"/>
        /// <code><see cref="Marshal.SizeOf{T}"/> * 8;</code>
        /// </summary>
        public int TypeSize { get; }
        /// <summary>
        /// Register code from <see cref="OperatorCodes"/>
        /// <see langword="enum"/>
        /// </summary>
        public OperatorCodes RegisterCode { get; }

        public Register(OperatorCodes registerCode, Type type)
        {
            RegisterCode = registerCode;
            TypeSize = Marshal.SizeOf(type) * 8;
            memoryData = new NativeMemoryList<MemoryData>();
        }

        /// <summary>
        /// Attach inicialized <see cref="MemoryData"/> into register
        /// <see cref="NativeMemoryList{T}"/>
        /// </summary>
        /// <param name="identificatorName">Name of instance or identifier</param>
        /// <param name="dataSize">Size of data in a bits</param>
        /// <returns>
        /// Coresponding <see cref="MemoryData"/>, that were already attached
        /// </returns>
        public MemoryData AddRegisterData(ReadOnlyMemory<char> identificatorName, int dataSize)
        {
            var returnData = new MemoryData(identificatorName, lastOffset, dataSize);
            memoryData.Add(returnData);
            lastOffset += dataSize;

            return returnData;
        }

        /// <summary>
        /// If identificator exists, then it removes a coresponding
        /// <see cref="MemoryData"/> by identifier id.
        /// </summary>
        /// <param name="identifierId">Identificator of a specific identifier</param>
        public void RemoveRegisterData(uint identifierId) 
        {
            if (TryGetIndexOfIdentifier(out int identifierIndex, identifierId))
                memoryData.RemoveOn(identifierIndex);
        }

        /// <summary>
        /// Tries to get a corespoding <see cref="MemoryData"/> by identifier id.
        /// </summary>
        /// <returns>
        /// Coresponding <see cref="bool"/>, if <see cref="MemoryData"/> was found, with
        /// particular <see langword="out"/> result
        /// </returns>
        public bool TryGetMemoryData([NotNullWhen(true)]out MemoryData resultData, uint identiferId)
        {
            if (TryGetIndexOfIdentifier(out int identifierIndex, identiferId)) 
            {
                resultData = memoryData.Span[identifierIndex];
                return true;
            }

            resultData = default;
            return false;
        }

        /// <summary>
        /// Tries to a get a value index from <see cref="NativeMemoryList{T}"/>
        /// <see cref="memoryData"/> by identifier id.
        /// </summary>
        /// <param name="returnIndex"><see langword="out"/> return value</param>
        /// <param name="identifierId">Identificator of a specific identifier</param>
        /// <returns>Coresponding <see cref="bool"/>, if indetificator in
        /// <see cref="MemoryData"/> was found</returns>
        private bool TryGetIndexOfIdentifier(out int returnIndex, uint identifierId)
        {
            Span<MemoryData> memoryDataSpan = memoryData.Span;
            for (int i = 0; i < memoryDataSpan.Length; i++)
            {
                if (identifierId == memoryDataSpan[i].IdentifierId) 
                {
                    returnIndex = i;
                    return true;
                }
            }
            returnIndex = -1;
            return false;
        }

        /// <summary>
        /// Calculates the maximum offset for
        /// current data
        /// </summary>
        /// <returns>
        /// If dataOffset isn't greater then <see cref="TypeSize"/> it will return offset as a bit size.<br/>
        /// Else it will normally return <see cref="TypeSize"/> as a maximum size
        /// </returns>
        public int CalculateByteSize(int data)
        {
            int currentOffset = 0;
            while (currentOffset != TypeSize)
            {
                int offsetValue = (data >> (currentOffset)) & 255;
                if (offsetValue == 0)
                    return currentOffset;
                currentOffset += 4;
            }
            return TypeSize;
        }

        /// <summary>
        /// After it calls the <see cref="Dispose"/>,
        /// it will create a new instance of <see cref="NativeMemoryList{T}"/>
        /// </summary>
        public void ReDispose()
        {
            Dispose();
            memoryData = new NativeMemoryList<MemoryData>();
        }

        /// <summary>
        /// Create dispose for <see cref="memoryData"/>, which is <see cref="NativeMemoryList{T}"/>
        /// </summary>
        public void Dispose()
        {
            memoryData.Dispose();
        }
    }
}
