namespace Assembler6502
{
    public interface LabelFinder
    {
        ushort AbsoluteAddressFor(string label);
    }
}