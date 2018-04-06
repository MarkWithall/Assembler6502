using System;
using System.Collections.ObjectModel;
using Assembler6502.InstructionTypes;

namespace Assembler6502
{
    public class InstructionCollection : Collection<Instruction>, LabelFinder
    {
        private readonly ushort _startingAddress;

        public InstructionCollection(ushort startingAddress)
        {
            _startingAddress = startingAddress;
        }

        public bool HasLabel(string label)
        {
            foreach (var instruction in Items)
            {
                if (instruction.Label == label)
                    return true;
            }
            return false;
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
            ushort offset = 0;
            var start = IndexOf(relativeTo) + 1;
            for (var i = start; i < Count; i++)
            {
                var instruction = Items[i];
                if (instruction.Label == label)
                    return offset;
                offset += instruction.Length;
            }
            offset = 0;
            start = IndexOf(relativeTo);
            for (var i = start; i >= 0; i--)
            {
                var instruction = Items[i];
                offset -= instruction.Length;
                if (instruction.Label == label)
                    return offset;
            }
            throw new ArgumentException($"Label '{label}' does not exist");
        }
    }
}
