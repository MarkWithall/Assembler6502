using System;
using Assembler6502.InstructionTypes;

namespace Assembler6502
{
    public interface LabelFinder
    {
        ushort AbsoluteAddressFor(string label);
        ushort RelativeAddressFor(string label, Instruction relativeTo);
	}
}