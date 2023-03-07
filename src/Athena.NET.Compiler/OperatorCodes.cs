namespace Athena.NET.Compiler
{
    //Every operator is slightly inspired
    //by x86 instructions
    internal enum OperatorCodes : uint
    {
        Store = 0xffee01,
        Load = 0xffee02,
        //For now, it's being used for determine
        //whetever current uint is a instruction
        Nop = 0xffee07,

        //I have a great idea of having
        //two registers, so it will be just
        //24 bits
        AH = 0xffeeA0, //Register of 8 bits
        AX = 0xffeeB0, //Register of 16 bits
        TM = 0xffeeB1, //Temporary access memory

        //Arithmetic and logic instruction
        //Syntax:
        //add [reg]AH 4 [reg]AH 8
        //add [reg]AH 4 [const]255
        Add = 0xffee03,
        Sub = 0xffee04,
        Mul = 0xffee05,
        Div = 0xffee06,
        //TODO: Implement logic operators
 
        //Syntax: jumpE [reg]AH 4 [reg]AH 8 [jumpFrameId] 0
        Jump = 0xffeeC0,
        JumpE = 0xffeeC1,
        JumpNE = 0xffeeC2,
        JumpG = 0xffeeC3,
        JumpGE = 0xffeeC4,
        JumpL = 0xffeeC5,
        JumpLE = 0xffeeC6,
    }
}
