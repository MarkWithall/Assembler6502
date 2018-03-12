using NUnit.Framework;
using static Assembler6502.OpCode;
using static Assembler6502.AddressingMode;

namespace Assembler6502.Tests
{
    [TestFixture]
    public class InstructionParserTests
    {
        [TestCase("brk", BRK, Implicit)]
        [TestCase("rts", RTS, Implicit)]
        [TestCase("asl a", ASL, Accumulator)]
        public void ParseInstruction(string instructionString, OpCode expectedOpCode, AddressingMode expectedAddressingMode)
        {
            var instruction = InstructionParser.Parse(instructionString);
            Assert.Multiple(() =>
            {
                Assert.That(instruction.Code, Is.EqualTo(expectedOpCode));
                Assert.That(instruction.Mode, Is.EqualTo(expectedAddressingMode));
            });
        }
    }
}
