using System;
using System.Collections.Generic;

namespace Assembler6502.InstructionTypes
{
    public sealed class UnknownInstruction : Instruction
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

        public override string ErrorMessage
        {
            get
            {
                if (Code == OpCode.Unknown)
                    return Error("unknown op code");
                if (Mode == AddressingMode.Unknown)
                    return Error("unknown addressing mode");
                throw new InvalidOperationException("Unknown error!");
            }
        }
    }
}