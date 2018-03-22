using Assembler6502.InstructionTypes;
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

        public Instruction Create(OpCode code, AddressingMode mode, string addressString, int lineNumber, string label)
        {
            switch ((Code: code, Mode: mode))
            {
                case var i when i.Code == OpCode.Unknown || i.Mode == AddressingMode.Unknown:
                    return new UnknownInstruction
                    {
                        Code = code,
                        Mode = mode,
                        AddressString = addressString,
                        LineNumber = lineNumber,
                        Label = label
                    };
                case var i when SingleByteAddressModes.Contains(i.Mode):
                    return new SingleByteAddressInstruction(_labelFinder)
                    {
                        Code = code,
                        Mode = mode,
                        AddressString = addressString,
                        LineNumber = lineNumber,
                        Label = label
                    };
                case var i when TwoByteAddressModes.Contains(i.Mode):
                    return new TwoByteAddressInstruction(_labelFinder)
                    {
                        Code = code,
                        Mode = mode,
                        AddressString = addressString,
                        LineNumber = lineNumber,
                        Label = label
                    };
                default:
                    return new NoAddressInstruction
                    {
                        Code = code,
                        Mode = mode,
                        LineNumber = lineNumber,
                        Label = label
                    };
            }
        }
    }
}