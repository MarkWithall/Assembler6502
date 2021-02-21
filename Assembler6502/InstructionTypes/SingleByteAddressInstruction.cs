using System.Collections.Generic;
using static Assembler6502.InstructionInformation;

namespace Assembler6502.InstructionTypes
{
    public sealed class SingleByteAddressInstruction : Instruction
    {
        public SingleByteAddressInstruction(ILabelFinder labelFinder) : base(labelFinder)
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
}