using System.Collections.Generic;
using static Assembler6502.InstructionInformation;

namespace Assembler6502.InstructionTypes
{
    internal sealed class TwoByteAddressInstruction : Instruction
    {
        public TwoByteAddressInstruction(OpCode code, ILabelFinder labelFinder) : base(code, labelFinder)
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