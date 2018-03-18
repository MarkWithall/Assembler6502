using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using static Assembler6502.AddressingMode;
using static Assembler6502.InstructionInformation;

namespace Assembler6502
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

        public abstract ushort Length { get; }

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

    public class NoAddressInstruction : Instruction
    {
        public NoAddressInstruction(LabelFinder labelFinder) : base(labelFinder)
        {
        }

        public override ushort Length { get; } = 1;
    }


    public class SingleByteAddressInstruction : Instruction
    {
        public SingleByteAddressInstruction(LabelFinder labelFinder) : base(labelFinder)
        {
        }

        public override ushort Length { get; } = 2;
    }


    public class TwoByteAddressInstruction : Instruction
    {
        public TwoByteAddressInstruction(LabelFinder labelFinder) : base(labelFinder)
        {
        }

        public override ushort Length { get; } = 3;
    }
}