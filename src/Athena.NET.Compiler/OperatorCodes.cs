namespace Athena.NET.Athena.NET.Compiler
{
    internal enum OperatorCodes : uint
    {
        BPush = 0x01,
        SPush = 0x02,
        IPush = 0x03,
        LPush = 0x04,

        //I have a great idea of having
        //two registers, so it will be just
        //24 bits
        R0Store = 0xA1, //Store data in register of 8 bits
        R1Store = 0xA2, //Store data in register of 16 bits
        R0Load = 0xB1,  //Load data from register of 8 bits
        R1Load = 0xB2,  //Load data from register of 16 bits
    }
}
