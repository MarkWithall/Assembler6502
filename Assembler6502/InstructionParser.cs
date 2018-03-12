using System;

namespace Assembler6502
{
    public static class InstructionParser
    {
        public static Instruction Parse(string instruction)
        {
            var opCodeString = instruction.Substring(0, 3).ToUpperInvariant();
            var addressString = instruction.Replace(" ", "").Substring(3).ToUpperInvariant();
            return new Instruction
            {
                Code = Enum.Parse<OpCode>(opCodeString),
                Mode = addressString == string.Empty ? AddressingMode.Implicit : AddressingMode.Accumulator
            };
        }
    }
}
