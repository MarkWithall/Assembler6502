using System.Collections.ObjectModel;

namespace Assembler6502
{
    public class InstructionCollection : Collection<Instruction>, LabelFinder
    {
        private readonly ushort _startingAddress;

        public InstructionCollection(ushort startingAddress)
        {
            _startingAddress = startingAddress;
        }

        public ushort AbsoluteAddressFor(string label)
        {
            return _startingAddress;
        }

        public ushort RelativeAddressFor(string label, Instruction relativeTo)
        {
            throw new System.NotImplementedException();
        }
    }
}
