namespace Athena.NET.Compiler
{
    //Every operator is slightly inspired
    //by x86 instructions
    internal enum OperatorCodes : uint
    {
        //TODO: Add an instruction, that will dispose specific
        //part of a memory in a coresponding register

        Store = 0xffee01,
        Load = 0xffee02,
        Print = 0xffee08,
        //For now, it's being used for determine
        //whetever current uint is a instruction
        Nop = 0xffee07,

        //I have a great idea of having
        //two registers, so it will be just
        //24 bits
        AH = 0xffeeA0, //Register of 8 bits
        AX = 0xffeeB0, //Register of 16 bits
        EAX = 0xffeeC0, // Register of 32 bits
        TM = 0xffeeB1, //Temporary access memory

        //Arithmetic and logic instruction
        //Syntax:
        //add TM [reg] AH [reg] 4 [size] 0 [offset] AH [reg] 8 [size] 0 [offset]
        //add TM [reg] AH [red] 4 [size] 4 [offset] 255 [const]
        Add = 0xffee03,
        Sub = 0xffee04,
        Mul = 0xffee05,
        Div = 0xffee06,
        //TODO: Implement logic operators
 
        Goto = 0xffeeD0, // Goto [number(-x to x)] 5, the number determines line count from the current Goto instruction
        Jump = 0xffeeD1, // Jump [number(0 to instructions-length)] 5, the number determines line number of specific instruction

        //Syntax: jumpE [reg]AH 4 [reg]AH 8 [Jump] 0
        JumpE = 0xffeeC0,
        JumpNE = 0xffeeC1,
        JumpG = 0xffeeC2,
        JumpGE = 0xffeeC3,
        JumpL = 0xffeeC4,
        JumpLE = 0xffeeC5,
    }
}
