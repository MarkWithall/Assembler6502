using System;
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
            var address = _startingAddress;
            foreach (var instruction in Items)
            {
                if (instruction.Label == label)
                    return address;
                address += instruction.Length;
            }
            throw new ArgumentException($"Label '{label}' does not exist");
        }

        public ushort RelativeAddressFor(string label, Instruction relativeTo)
        {
            return 0x00;
        }
    }
}
