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
            var opCode = ParseOpCode(normalizedInstruction.Substring(0, 3));
            var (mode, address) = ParseAddress(normalizedInstruction.Substring(3));
            return new Instruction
            {
                Code = opCode,
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

        private static readonly Regex AddressRegex = new Regex(@"^(?<pre>.*)(?<address>\$[0-9A-F]{2,4})(?<post>.*)$", RegexOptions.Compiled);

        private static (AddressingMode, ushort) ParseAddress(string addressString)
        {
            if (addressString == string.Empty)
                return (Implicit, 0x0000);

            if (addressString == "A")
                return (Accumulator, 0x0000);

            var match = AddressRegex.Match(addressString);
            if (match.Success)
            {
                var address = ParseNumber(match.Groups["address"].Value);
                var modeString = match.Groups["pre"].Value + match.Groups["post"].Value;

                switch (modeString)
                {
                    case "#": return (Immediate, address);
                    case "*": return (Relative, address);
                    case "": return (address < 256 ? ZeroPage : Absolute, address);
                    case ",X": return (address < 256 ? ZeroPageXIndexed : AbsoluteXIndexed, address);
                    case ",Y": return (address < 256 ? ZeroPageYIndexed : AbsoluteYIndexed, address);
                    case "()": return (Indirect, address);
                    case "(,X)": return (XIndexedIndirect, address);
                    case "(),Y": return (IndirectYIndexed, address);
                }
            }

            return (Unknown, 0x0000);
        }

        private static ushort ParseNumber(string numberString)
        {
            return Convert.ToUInt16(numberString.Substring(1), 16);
        }
    }
}
