﻿using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using static Assembler6502.AddressingMode;

namespace Assembler6502.InstructionTypes
{
    public abstract class Instruction
    {
        private readonly ILabelFinder _labelFinder;

        protected Instruction(ILabelFinder labelFinder)
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
                if (Regex.IsMatch(AddressString, @"^\$([0-9A-Z]{1,4})$"))
                    return ushort.Parse(AddressString.Substring(1), NumberStyles.HexNumber);
                if (Mode == Relative)
                    return _labelFinder.RelativeAddressFor(AddressString, this);
                return _labelFinder.AbsoluteAddressFor(AddressString);
            }
        }

        public bool IsValid => ErrorMessage == null;

        public virtual string ErrorMessage
        {
            get
            {
                if (!InstructionInformation.Instructions.ContainsKey((Code, Mode)))
                   return Error("invalid op code/addressing mode combination");
                if (AddressString != null && Regex.IsMatch(AddressString, @"^\w+$") && !_labelFinder.HasLabel(AddressString))
                    return Error($"unknown address label '{AddressString}'");
                if (Mode == Relative && (short)Address > 127)
                    return Error("address label 'LABEL' is greater than 127 bytes away");
                if (Mode == Relative && (short)Address < -127)
                    return Error("address label 'LABEL' is greater than -128 bytes away");
                if (this is SingleByteAddressInstruction && Address > 0xFF)
                    return Error("single byte address must be less than 256");
                if (this is TwoByteAddressInstruction && Regex.IsMatch(AddressString, @"^\$[0-9A-Z]{5,}$"))
                    return Error("two byte address must be less than 65536");
                return null;
            }
        }

        public abstract ushort Length { get; }

        public abstract IEnumerable<byte> Bytes { get; }

        protected string Error(string message) => $"Error (line {LineNumber}) - {message}.";
    }
}