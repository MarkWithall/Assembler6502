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
            return Enum.TryParse<OpCode>(opCodeString, out var code) ? code : OpCode.Unknown;
        }

        private static (AddressingMode, ushort) ParseAddress(string addressString)
        {
            if (addressString == string.Empty)
                return (Implicit, 0x0000);

            if (addressString == "A")
                return (Accumulator, 0x0000);

            try
            {
                switch (addressString)
                {
                    case var s when Matches(s, "#", "", out var address):
                        return (Immediate, address);

                    case var s when Matches(s, "*", "", out var address):
                        return (Relative, address);

                    case var s when Matches(s, "<", ",X", out var address):
                        return (ZeroPageXIndexed, address);

                    case var s when Matches(s, "", ",X", out var address):
                        return (AbsoluteXIndexed, address);

                    case var s when Matches(s, "(", "),Y", out var address):
                        return (IndirectYIndexed, address);

                    case var s when Matches(s, "(", ",X)", out var address):
                        return (XIndexedIndirect, address);

                    case var s when Matches(s, "(", ")", out var address):
                        return (Indirect, address);

                    case var s when Matches(s, "<", ",Y", out var address):
                        return (ZeroPageYIndexed, address);

                    case var s when Matches(s, "", ",Y", out var address):
                        return (AbsoluteYIndexed, address);

                    case var s when Matches(s, "<", "", out var address):
                        return (ZeroPage, address);

                    default:
                        return (Absolute, ParseNumber(addressString, 0, 0));
                }
            }
            catch
            {
                return (Unknown, 0x0000);
            }
        }

        private static bool Matches(string addressString, string prefix, string suffix, out ushort address)
        {
            address = 0x0000;

            if (addressString.StartsWith(prefix) && addressString.EndsWith(suffix))
            {
                address = ParseNumber(addressString, prefix.Length, suffix.Length);
                return true;
            }

            return false;
        }

        private static ushort ParseNumber(string addressString, int startSkip, int endSkip)
        {
            startSkip = startSkip + 1; // also remove $ prefix
            var numberString = addressString.Substring(startSkip, addressString.Length - (startSkip + endSkip));
            return Convert.ToUInt16(numberString, 16);
        }
    }
}