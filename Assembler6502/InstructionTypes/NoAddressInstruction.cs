﻿using System.Collections.Generic;
using static Assembler6502.InstructionInformation;

namespace Assembler6502.InstructionTypes
{
    internal sealed class NoAddressInstruction : Instruction
    {
        public NoAddressInstruction(OpCode code, AddressingMode mode, ILabelFinder labelFinder) : base(code, mode, labelFinder)
        {
        }

        public override ushort Length { get; } = 1;

        public override IEnumerable<byte> Bytes
        {
            get { yield return Instructions[(Code, Mode)]; }
        }
    }
}