using static Assembler6502.InstructionInformation;

namespace Assembler6502
{
    public class InstructionFactory
    {
        private readonly LabelFinder _labelFinder;

        public InstructionFactory(LabelFinder labelFinder)
        {
            _labelFinder = labelFinder;
        }

        public Instruction Create(OpCode code, AddressingMode mode, string addressString, string label)
        {
            switch (mode)
            {
                case var m when SingleByteAddressModes.Contains(m):
                    return new SingleByteAddressInstruction(_labelFinder)
                    {
                        Code = code,
                        Mode = mode,
                        AddressString = addressString,
                        Label = label
                    };
                case var m when TwoByteAddressModes.Contains(m):
                    return new TwoByteAddressInstruction(_labelFinder)
                    {
                        Code = code,
                        Mode = mode,
                        AddressString = addressString,
                        Label = label
                    };
                default:
                    return new NoAddressInstruction(null)
                    {
                        Code = code,
                        Mode = mode,
                        Label = label
                    };
            }
        }
    }
}