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
            return new Instruction(_labelFinder)
            {
                Code = code,
                Mode = mode,
                AddressString = addressString,
                Label = label
            };
        }
    }
}