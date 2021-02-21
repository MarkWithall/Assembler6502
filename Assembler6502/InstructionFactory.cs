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
            Instruction instruction;
            switch (Code: code, Mode: mode)
            {
                case var i when i.Code == OpCode.Unknown || i.Mode == AddressingMode.Unknown:
                    instruction = new UnknownInstruction();
                    break;
                case var i when SingleByteAddressModes.Contains(i.Mode):
                    instruction = new SingleByteAddressInstruction(_labelFinder);
                    break;
                case var i when TwoByteAddressModes.Contains(i.Mode):
                    instruction = new TwoByteAddressInstruction(_labelFinder);
                    break;
                default:
                    instruction = new NoAddressInstruction();
                    break;
            }

            instruction.Code = code;
            instruction.Mode = mode;
            instruction.AddressString = addressString;
            instruction.LineNumber = lineNumber;
            instruction.Label = label;

            return instruction;
        }
    }
}