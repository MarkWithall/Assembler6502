using System;
using static Assembler6502.AddressingMode;

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
                return Implicit;

            switch (addressString[0])
            {
                case 'A': return Accumulator;
                case '#': return Immediate;
                case '*': return Relative;
            }

            if (addressString.EndsWith(",X", StringComparison.InvariantCulture))
                return AbsoluteXIndexed;
            if (addressString.EndsWith(",Y", StringComparison.InvariantCulture))
                return AbsoluteYIndexed;

            return Absolute;
        }
    }
}
