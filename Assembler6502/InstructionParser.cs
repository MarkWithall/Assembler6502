using System;
using System.Text.RegularExpressions;
using Assembler6502.InstructionTypes;
using static Assembler6502.AddressingMode;

namespace Assembler6502
{
    internal sealed class InstructionParser
    {
        private readonly InstructionFactory _factory;

        public InstructionParser(ILabelFinder labelFinder)
        {
            _factory = new InstructionFactory(labelFinder);
        }

        public Instruction Parse(string instruction, int lineNumber)
        {
            var normalizedInstruction = Regex.Replace(instruction, @"\s+", "").ToUpperInvariant();

            var (label, instructionPart) = ParseLabel(normalizedInstruction);
            var opCode = ParseOpCode(instructionPart.Substring(0, 3));
            var (mode, addressString) = ParseAddress(instructionPart.Substring(3));

            return _factory.Create(opCode, mode, addressString, lineNumber, label);
        }

        private static (string?, string) ParseLabel(string normalizedInstruction)
        {
            var parts = normalizedInstruction.Split(':');
            return parts.Length == 2 ? (parts[0], parts[1]) : (null, normalizedInstruction);
        }

        private static OpCode ParseOpCode(string opCodeString)
        {
            return Enum.TryParse<OpCode>(opCodeString, out var code) ? code : OpCode.Unknown;
        }

        private static (AddressingMode, string?) ParseAddress(string addressString) =>
            addressString switch
            {
                var s when s == string.Empty => (Implicit, null),
                var s when s == "A" => (Accumulator, null),
                var s when TryMatch(s, "#", "", out var address) => (Immediate, address),
                var s when TryMatch(s, "*", "", out var address) => (Relative, address),
                var s when TryMatch(s, "<", ",X", out var address) => (ZeroPageXIndexed, address),
                var s when TryMatch(s, "", ",X", out var address) => (AbsoluteXIndexed, address),
                var s when TryMatch(s, "(", "),Y", out var address) => (IndirectYIndexed, address),
                var s when TryMatch(s, "(", ",X)", out var address) => (XIndexedIndirect, address),
                var s when TryMatch(s, "(", ")", out var address) => (Indirect, address),
                var s when TryMatch(s, "<", ",Y", out var address) => (ZeroPageYIndexed, address),
                var s when TryMatch(s, "", ",Y", out var address) => (AbsoluteYIndexed, address),
                var s when TryMatch(s, "<", "", out var address) => (ZeroPage, address),
                var s when Regex.IsMatch(s, @"^(\$[0-9A-Z]+|\w+)$") => (Absolute, ExtractNumber(addressString, 0, 0)),
                _ => (Unknown, null)
            };

        private static bool TryMatch(string addressString, string prefix, string suffix, out string? address)
        {
            address = null;

            if (addressString.StartsWith(prefix, StringComparison.InvariantCulture) &&
                addressString.EndsWith(suffix, StringComparison.InvariantCulture))
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