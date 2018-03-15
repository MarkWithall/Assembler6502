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
                    case var s when Matches(s, "#", ""):
                        return (Immediate, ParseNumber(addressString, 1, 0));

                    case var s when Matches(s, "*", ""):
                        return (Relative, ParseNumber(addressString, 1, 0));

                    case var s when Matches(s, "", ",X"):
                    {
                        var address = ParseNumber(addressString, 0, 2);
                        return (address < 256 ? ZeroPageXIndexed : AbsoluteXIndexed, address);
                    }

                    case var s when Matches(s, "(", "),Y"):
                        return (IndirectYIndexed, ParseNumber(addressString, 1, 3));

                    case var s when Matches(s, "(", ",X)"):
                        return (XIndexedIndirect, ParseNumber(addressString, 1, 3));

                    case var s when Matches(s, "(", ")"):
                        return (Indirect, ParseNumber(addressString, 1, 1));

                    case var s when Matches(s, "", ",Y"):
                    {
                        var address = ParseNumber(addressString, 0, 2);
                        return (address < 256 ? ZeroPageYIndexed : AbsoluteYIndexed, address);
                    }

                    default:
                    {
                        var address = ParseNumber(addressString, 0, 0);
                        return (address < 256 ? ZeroPage : Absolute, address);
                    }
                }
            }
            catch
            {
                return (Unknown, 0x0000);
            }
        }

        private static bool Matches(string addressString, string prefix, string suffix)
        {
            return addressString.StartsWith(prefix) && addressString.EndsWith(suffix);
        }

        private static ushort ParseNumber(string addressString, int startSkip, int endSkip)
        {
            startSkip = startSkip + 1; // also remove $ prefix
            var numberString = addressString.Substring(startSkip, addressString.Length - (startSkip + endSkip));
            return Convert.ToUInt16(numberString, 16);
        }
    }
}