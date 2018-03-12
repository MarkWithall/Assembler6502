using System;
using System.Text.RegularExpressions;
using static Assembler6502.AddressingMode;

namespace Assembler6502
{
    public static class InstructionParser
    {
        public static Instruction Parse(string instruction)
		{
            var normalizedInstruction = Regex.Replace(instruction, @"\s+", "").ToUpperInvariant();
            var opCodeString = normalizedInstruction.Substring(0, 3);
            var addressString = normalizedInstruction.Substring(3);
            var (mode, address) = ParseAddress(addressString);
			return new Instruction
			{
                Code = ParseOpCode(opCodeString),
				Mode = mode,
                Address = address
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

        private static (AddressingMode, ushort) ParseAddress(string addressString)
        {
            if (addressString == string.Empty)
                return (Implicit, 0x0000);

            switch (addressString[0])
            {
                case 'A': return (Accumulator, 0x0000);
                case '#': return (Immediate, ParseNumber(addressString.Substring(1)));
                case '*': return (Relative, ParseNumber(addressString.Substring(1)));
                case '(':
                    if (addressString.EndsWith(",X)", StringComparison.InvariantCulture))
                        return (XIndexedIndirect, 0x0000);
                    if (addressString.EndsWith(",Y", StringComparison.InvariantCulture))
                        return (IndirectYIndexed, 0x0000);
                    return (Indirect, 0x0000);
            }

            if (Regex.IsMatch(addressString, @"^\$\d+$"))
            {
				var address = ParseNumber(addressString);
                return (address < 256 ? ZeroPage : Absolute, address);
            }

            var xIndexMatch = Regex.Match(addressString, @"^(.*),X$");
            if (xIndexMatch.Success)
            {
                var address = ParseNumber(xIndexMatch.Groups[1].Value);
                return (address < 256 ? ZeroPageXIndexed : AbsoluteXIndexed, address);
            }
            var yIndexMatch = Regex.Match(addressString, @"^(.*),Y$");
            if (yIndexMatch.Success)
            {
                var address = ParseNumber(yIndexMatch.Groups[1].Value);
                return (address < 256 ? ZeroPageYIndexed : AbsoluteYIndexed, address);
            }

            return (Unknown, 0x0000);
        }

        private static ushort ParseNumber(string numberString)
        {
            return Convert.ToUInt16(numberString.Substring(1), 16);
        }
    }
}
