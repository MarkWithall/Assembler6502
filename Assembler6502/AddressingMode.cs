namespace Assembler6502
{
    public enum AddressingMode
    {
        Unknown,
        Implicit,
        Accumulator,
        Immediate,
        Relative,
        Absolute,
        AbsoluteXIndexed,
        AbsoluteYIndexed,
        ZeroPage,
        ZeroPageXIndexed,
        ZeroPageYIndexed,
        Indirect,
        XIndexedIndirect,
        IndirectYIndexed
    }
}