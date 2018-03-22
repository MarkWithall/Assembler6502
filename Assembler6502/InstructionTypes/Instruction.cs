using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using static Assembler6502.AddressingMode;

namespace Assembler6502.InstructionTypes
{
    public abstract class Instruction
    {
        private readonly LabelFinder _labelFinder;

        protected Instruction(LabelFinder labelFinder)
        {
            _labelFinder = labelFinder;
        }

        public OpCode Code { get; set; }
        public AddressingMode Mode { get; set; }
        public string AddressString { get; set; }
        public int LineNumber { get; set; }
        public string Label { get; set; }

        public ushort Address
        {
            get
            {
                if (AddressString == null)
                    return 0x0000;
                if (Regex.IsMatch(AddressString, @"^\$([0-9A-Z]{2}|[0-9A-Z]{4})$"))
                    return ushort.Parse(AddressString.Substring(1), NumberStyles.HexNumber);
                if (Mode == Relative)
                    return _labelFinder.RelativeAddressFor(AddressString, this);
                return _labelFinder.AbsoluteAddressFor(AddressString);
            }
        }

        public bool IsValid => ErrorMessage == null;

        public string ErrorMessage
        {
            get
            {
                if (!InstructionInformation.Instructions.ContainsKey((Code, Mode)))
                   return $"Error (line {LineNumber}) - invalid op code/addressing mode combination.";
                if (this is SingleByteAddressInstruction && Address > 0xFF)
                    return $"Error (line {LineNumber}) - single byte address must be less than 256.";
                return null;
            }
        }

        public abstract ushort Length { get; }

        public abstract IEnumerable<byte> Bytes { get; }
    }
}