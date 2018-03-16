﻿using System;
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
            var (mode, addressString) = ParseAddress(normalizedInstruction.Substring(3));
            return new Instruction
            {
                Code = opCode,
                Mode = mode,
                AddressString = addressString
            };
        }

        private static OpCode ParseOpCode(string opCodeString)
        {
            return Enum.TryParse<OpCode>(opCodeString, out var code) ? code : OpCode.Unknown;
        }

        private static (AddressingMode, string) ParseAddress(string addressString)
        {
            switch (addressString)
            {
                case var s when s == string.Empty:
                    return (Implicit, null);

                case var s when s == "A":
                    return (Accumulator, null);

                case var s when TryMatch(s, "#", "", out var address):
                    return (Immediate, address);

                case var s when TryMatch(s, "*", "", out var address):
                    return (Relative, address);

                case var s when TryMatch(s, "<", ",X", out var address):
                    return (ZeroPageXIndexed, address);

                case var s when TryMatch(s, "", ",X", out var address):
                    return (AbsoluteXIndexed, address);

                case var s when TryMatch(s, "(", "),Y", out var address):
                    return (IndirectYIndexed, address);

                case var s when TryMatch(s, "(", ",X)", out var address):
                    return (XIndexedIndirect, address);

                case var s when TryMatch(s, "(", ")", out var address):
                    return (Indirect, address);

                case var s when TryMatch(s, "<", ",Y", out var address):
                    return (ZeroPageYIndexed, address);

                case var s when TryMatch(s, "", ",Y", out var address):
                    return (AbsoluteYIndexed, address);

                case var s when TryMatch(s, "<", "", out var address):
                    return (ZeroPage, address);

                case var s when Regex.IsMatch(s, @"^\$[0-9A-Z]{4}$"):
                    return (Absolute, ExtractNumber(addressString, 0, 0));

                default:
                    return (Unknown, null);
            }
        }

        private static bool TryMatch(string addressString, string prefix, string suffix, out string address)
        {
            address = null;

            if (addressString.StartsWith(prefix) && addressString.EndsWith(suffix))
            {
                address = ExtractNumber(addressString, prefix.Length, suffix.Length);
                return true;
            }

            return false;
        }

        private static string ExtractNumber(string addressString, int startSkip, int endSkip)
        {
            return addressString.Substring(startSkip, addressString.Length - (startSkip + endSkip));
        }
    }
}