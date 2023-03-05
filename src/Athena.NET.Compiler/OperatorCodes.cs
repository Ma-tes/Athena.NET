namespace Athena.NET.Compiler
{
    //Every operator is slightly inspired
    //by x86 instructions
    internal enum OperatorCodes : uint
    {
        Store = 0x01,
        Load = 0x02,

        //I have a great idea of having
        //two registers, so it will be just
        //24 bits
        AH = 0xA0, //Register of 8 bits
        AX = 0xB0, //Register of 16 bits

        //Arithmetic and logic instruction
        //Syntax:
        //add [reg]AH 4 [reg]AH 8
        //add [reg]AH 4 [const]255
        Add = 0x03,
        Sub = 0x04,
        Mul = 0x05,
        Div = 0x06,
        //TODO: Implement logic operators
 
        //Syntax: jumpE [reg]AH 4 [reg]AH 8 [jumpFrameId] 0
        Jump = 0xC0,
        JumpE = 0xC1,
        JumpNE = 0xC2,
        JumpG = 0xC3,
        JumpGE = 0xC4,
        JumpL = 0xC5,
        JumpLE = 0xC6,
    }
}
