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
            switch ((Code: code, Mode: mode))
            {
                case var i when i.Code == OpCode.Unknown || i.Mode == AddressingMode.Unknown:
                    return new UnknownInstruction
                    {
                        Code = code,
                        Mode = mode,
                        AddressString = addressString,
                        Label = label
                    };
                case var i when SingleByteAddressModes.Contains(i.Mode):
                    return new SingleByteAddressInstruction(_labelFinder)
                    {
                        Code = code,
                        Mode = mode,
                        AddressString = addressString,
                        Label = label
                    };
                case var i when TwoByteAddressModes.Contains(i.Mode):
                    return new TwoByteAddressInstruction(_labelFinder)
                    {
                        Code = code,
                        Mode = mode,
                        AddressString = addressString,
                        Label = label
                    };
                default:
                    return new NoAddressInstruction
                    {
                        Code = code,
                        Mode = mode,
                        Label = label
                    };
            }
        }
    }
}