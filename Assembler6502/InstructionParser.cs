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
                    var indirectMatch = Regex.Match(addressString, @"^\((\$\w+)\)$");
                    if (indirectMatch.Success)
                        return (Indirect, ParseNumber(indirectMatch.Groups[1].Value));
                    var xIndexedIndirectMatch = Regex.Match(addressString, @"^\((\$\w+),X\)$");
                    if (xIndexedIndirectMatch.Success)
                        return (XIndexedIndirect, ParseNumber(xIndexedIndirectMatch.Groups[1].Value));
                    var indirectYIndexedMatch = Regex.Match(addressString, @"^\((\$\w+)\),Y$");
                    if (indirectYIndexedMatch.Success)
                        return (IndirectYIndexed, ParseNumber(indirectYIndexedMatch.Groups[1].Value));
                    break;
            }

            if (Regex.IsMatch(addressString, @"^\$\w+$"))
            {
				var address = ParseNumber(addressString);
                return (address < 256 ? ZeroPage : Absolute, address);
            }

            var indexMatch = Regex.Match(addressString, @"^(.*),([XY])$");
            if (indexMatch.Success)
            {
                var address = ParseNumber(indexMatch.Groups[1].Value);
                return indexMatch.Groups[2].Value == "X"
                    ? (address < 256 ? ZeroPageXIndexed : AbsoluteXIndexed, address)
                    : (address < 256 ? ZeroPageYIndexed : AbsoluteYIndexed, address);
            }

            return (Unknown, 0x0000);
        }

        private static ushort ParseNumber(string numberString)
        {
            return Convert.ToUInt16(numberString.Substring(1), 16);
        }
    }
}
