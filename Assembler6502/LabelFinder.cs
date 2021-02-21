using Assembler6502.InstructionTypes;

namespace Assembler6502
{
    public interface ILabelFinder
    {
        bool HasLabel(string label);
        ushort AbsoluteAddressFor(string label);
        ushort RelativeAddressFor(string label, Instruction relativeTo);
    }
}