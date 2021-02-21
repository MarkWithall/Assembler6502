﻿using Assembler6502.InstructionTypes;
using static Assembler6502.InstructionInformation;

namespace Assembler6502
{
    internal sealed class InstructionFactory
    {
        private readonly ILabelFinder _labelFinder;

        public InstructionFactory(ILabelFinder labelFinder)
        {
            _labelFinder = labelFinder;
        }

        public Instruction Create(OpCode code, AddressingMode mode, string? addressString, int lineNumber, string? label)
        {
            Instruction instruction = (Code: code, Mode: mode) switch
            {
                var i when i.Code == OpCode.Unknown || i.Mode == AddressingMode.Unknown => new UnknownInstruction(_labelFinder),
                var i when SingleByteAddressModes.Contains(i.Mode) => new SingleByteAddressInstruction(_labelFinder),
                var i when TwoByteAddressModes.Contains(i.Mode) => new TwoByteAddressInstruction(_labelFinder),
                _ => new NoAddressInstruction(_labelFinder)
            };

            instruction.Code = code;
            instruction.Mode = mode;
            instruction.AddressString = addressString;
            instruction.LineNumber = lineNumber;
            instruction.Label = label;

            return instruction;
        }
    }
}