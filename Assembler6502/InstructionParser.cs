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
                Code = ParseOpCode(opCodeString),
				Mode = ParseAddress(addressString)
			};
        }

        private static OpCode ParseOpCode(string opCodeString)
        {
            try
            {
                return Enum.Parse<OpCode>(opCodeString);
            }
            catch (ArgumentException)
            {
                return OpCode.Unknown;
            }
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
                case '(':
                    if (addressString.EndsWith(",X)", StringComparison.InvariantCulture))
                        return XIndexedIndirect;
                    if (addressString.EndsWith(",Y", StringComparison.InvariantCulture))
                        return IndirectYIndexed;
                    return Indirect;
            }

            if (!addressString.Contains(","))
                return addressString.Length == 3 ? ZeroPage : Absolute;

            if (addressString.EndsWith(",X", StringComparison.InvariantCulture))
                return addressString.Length == 5 ? ZeroPageXIndexed : AbsoluteXIndexed;
            if (addressString.EndsWith(",Y", StringComparison.InvariantCulture))
                return addressString.Length == 5 ? ZeroPageYIndexed : AbsoluteYIndexed;

            return Unknown;
        }
    }
}
