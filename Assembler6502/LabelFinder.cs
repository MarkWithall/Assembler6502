using Assembler6502.InstructionTypes;

namespace Assembler6502
{
    internal interface ILabelFinder
    {
        bool HasLabel(string label);
        ushort AbsoluteAddressFor(string label);
        ushort RelativeAddressFor(string label, Instruction relativeTo);
    }
}