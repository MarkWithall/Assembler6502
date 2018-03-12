using System;
using static Assembler6502.AddressingMode;

namespace Assembler6502
{
    public static class InstructionParser
    {
        public static Instruction Parse(string instruction)
        {
            var opCodeString = instruction.Substring(0, 3).ToUpperInvariant();
            OpCode code;
            try
            {
                code = Enum.Parse<OpCode>(opCodeString);
            }
            catch (ArgumentException)
            {
                code = OpCode.Unknown;
            }

            var addressString = instruction.Replace(" ", "").Substring(3).ToUpperInvariant();
            return new Instruction
            {
                Code = code,
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

            var parts = addressString.Split(',');

            if (parts.Length == 1)
            {
                if (addressString[0] == '(')
                    return Indirect;
                return addressString.Length == 3 ? ZeroPage : Absolute;
            }

            if (addressString.EndsWith(",X", StringComparison.InvariantCulture))
                return parts[0].Length == 3 ? ZeroPageXIndexed : AbsoluteXIndexed;
            if (addressString.EndsWith(",Y", StringComparison.InvariantCulture))
            {
                if (addressString[0] == '(')
                    return IndirectYIndexed;
                return parts[0].Length == 3 ? ZeroPageYIndexed : AbsoluteYIndexed;
            }

            return XIndexedIndirect;
        }
    }
}
