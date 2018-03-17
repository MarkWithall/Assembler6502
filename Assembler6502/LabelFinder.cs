using System;

namespace Assembler6502
{
    public interface LabelFinder
    {
        ushort AbsoluteAddressFor(string label);
        ushort RelativeAddressFor(string label, Instruction relativeTo);
	}
}