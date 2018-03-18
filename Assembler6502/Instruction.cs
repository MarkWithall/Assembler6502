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

        public abstract IEnumerable<byte> Bytes { get; }
    }

    public class UnknownInstruction : Instruction
    {
        public UnknownInstruction() : base(null)
        {
        }

        public override ushort Length => throw new NotImplementedException();

        public override IEnumerable<byte> Bytes
        {
            get
            {
                if (Code == OpCode.Unknown)
                    throw new InvalidOperationException("Cannot get bytes for unknown op code");
                throw new InvalidOperationException("Cannot get bytes for unknown addressing mode");
            }
        }
    }

    public class NoAddressInstruction : Instruction
    {
        public NoAddressInstruction() : base(null)
        {
        }

        public override ushort Length { get; } = 1;

        public override IEnumerable<byte> Bytes
        {
            get { yield return Instructions[(Code, Mode)]; }
        }
    }


    public class SingleByteAddressInstruction : Instruction
    {
        public SingleByteAddressInstruction(LabelFinder labelFinder) : base(labelFinder)
        {
        }

        public override ushort Length { get; } = 2;

        public override IEnumerable<byte> Bytes
        {
            get
            {
                yield return Instructions[(Code, Mode)];
                yield return (byte)Address;
            }
        }
    }


    public class TwoByteAddressInstruction : Instruction
    {
        public TwoByteAddressInstruction(LabelFinder labelFinder) : base(labelFinder)
        {
        }

        public override ushort Length { get; } = 3;

        public override IEnumerable<byte> Bytes
        {
            get
            {
                yield return Instructions[(Code, Mode)];
                yield return (byte)Address;
                yield return (byte)(Address >> 8);
            }
        }
    }
}