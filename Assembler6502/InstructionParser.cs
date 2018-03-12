namespace Assembler6502
{
    public static class InstructionParser
    {
        public static Instruction Parse(string instruction)
        {
            return new Instruction
            {
                Code = OpCode.BRK,
                Mode = AddressingMode.Implicit
            };
        }
    }
}
