using System;
using System.Collections.Generic;

namespace Assembler6502.InstructionTypes
{
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
}