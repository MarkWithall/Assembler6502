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

        private static readonly Regex ImmediateRegex = new Regex(@"^#(?<address>\$\w+)$", RegexOptions.Compiled);
        private static readonly Regex AbsoluteAndZeroPageRegex = new Regex(@"^\$\w+$", RegexOptions.Compiled);
        private static readonly Regex IndexedRegex = new Regex(@"^(?<address>.*),(?<index>[XY])$", RegexOptions.Compiled);
        private static readonly Regex IndirectRegex = new Regex(@"^\((?<address>\$\w+)\)$", RegexOptions.Compiled);
        private static readonly Regex XIndexedIndirectRegex = new Regex(@"^\((?<address>\$\w+),X\)$", RegexOptions.Compiled);
        private static readonly Regex IndirectYIndexedRegex = new Regex(@"^\((?<address>\$\w+)\),Y$", RegexOptions.Compiled);

        private static (AddressingMode, ushort) ParseAddress(string addressString)
        {
            if (addressString == string.Empty)
                return (Implicit, 0x0000);

            if (addressString == "A")
                return (Accumulator, 0x0000);

            var immediateMatch = ImmediateRegex.Match(addressString);
            if (immediateMatch.Success)
                return (Immediate, ParseNumber(immediateMatch.Groups["address"].Value));

            switch (addressString[0])
            {
                case '*': return (Relative, ParseNumber(addressString.Substring(1)));
                case '(':
                    var indirectMatch = IndirectRegex.Match(addressString);
                    if (indirectMatch.Success)
                        return (Indirect, ParseNumber(indirectMatch.Groups["address"].Value));
                    var xIndexedIndirectMatch = XIndexedIndirectRegex.Match(addressString);
                    if (xIndexedIndirectMatch.Success)
                        return (XIndexedIndirect, ParseNumber(xIndexedIndirectMatch.Groups["address"].Value));
                    var indirectYIndexedMatch = IndirectYIndexedRegex.Match(addressString);
                    if (indirectYIndexedMatch.Success)
                        return (IndirectYIndexed, ParseNumber(indirectYIndexedMatch.Groups["address"].Value));
                    break;
            }

            if (AbsoluteAndZeroPageRegex.IsMatch(addressString))
            {
				var address = ParseNumber(addressString);
                return (address < 256 ? ZeroPage : Absolute, address);
            }

            var indexMatch = IndexedRegex.Match(addressString);
            if (indexMatch.Success)
            {
                var address = ParseNumber(indexMatch.Groups["address"].Value);
                return indexMatch.Groups["index"].Value == "X"
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
