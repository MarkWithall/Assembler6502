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
                Mode = ParseAddress(addressString)
            };
        }

        private static AddressingMode ParseAddress(string addressString)
        {
            if (addressString == string.Empty)
                return AddressingMode.Implicit;
            return addressString == "A" ? AddressingMode.Accumulator : AddressingMode.Immediate;
        }
    }
}
