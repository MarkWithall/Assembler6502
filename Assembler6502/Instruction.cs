using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using static Assembler6502.AddressingMode;
using static Assembler6502.InstructionInformation;

namespace Assembler6502
{
    public class Instruction
    {
        private readonly LabelFinder _labelFinder;

        public Instruction(LabelFinder labelFinder)
        {
            _labelFinder = labelFinder;
        }

        public OpCode Code { get; set; }
        public AddressingMode Mode { get; set; }
        public string AddressString { get; set; }
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

        public ushort Length
        {
            get
            {
                if (SingleByteAddressModes.Contains(Mode))
                    return 2;
                if (TwoByteAddressModes.Contains(Mode))
                    return 3;
                return 1;
            }
        }

        public IEnumerable<byte> Bytes
        {
            get
            {
                if (Code == OpCode.Unknown)
                    throw new InvalidOperationException("Cannot get bytes for unknown op code");
                if (Mode == AddressingMode.Unknown)
                    throw new InvalidOperationException("Cannot get bytes for unknown addressing mode");

                yield return Instructions[(Code, Mode)];
                if (SingleByteAddressModes.Contains(Mode))
                    yield return (byte)Address;
                else if (TwoByteAddressModes.Contains(Mode))
                {
                    yield return (byte)Address;
                    yield return (byte)(Address >> 8);
                }
            }
        }
    }
}