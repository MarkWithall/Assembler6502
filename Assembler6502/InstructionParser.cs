using System;

namespace Assembler6502
{
    public static class InstructionParser
    {
        public static Instruction Parse(string instruction)
        {
            return new Instruction
            {
                Code = Enum.Parse<OpCode>(instruction.ToUpperInvariant()),
                Mode = AddressingMode.Implicit
            };
        }
    }
}
