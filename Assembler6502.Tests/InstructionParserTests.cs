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
        [TestCase("lda #$42", LDA, Immediate)]
        [TestCase("bne *$42", BNE, Relative)]
        [TestCase("sta $4242", STA, Absolute)]
        [TestCase("and $4242,X", AND, AbsoluteXIndexed)]
        [TestCase("ldx $4242,Y", LDX, AbsoluteYIndexed)]
        [TestCase("eor $42", EOR, ZeroPage)]
        [TestCase("ror $42,X", ROR, ZeroPageXIndexed)]
        [TestCase("ldx $42,Y", LDX, ZeroPageYIndexed)]
        [TestCase("jmp ($4242)", JMP, Indirect)]
        [TestCase("ora ($42,X)", ORA, XIndexedIndirect)]
        [TestCase("cmp ($42),Y", CMP, IndirectYIndexed)]
        [TestCase("xxx $4242", OpCode.Unknown, Absolute)]
        [TestCase("xxx ,,,,", OpCode.Unknown, AddressingMode.Unknown)]
        [TestCase(" cmp  ($42),Y ", CMP, IndirectYIndexed)]
        [TestCase("\tasl\ta\t", ASL, Accumulator)]
        public void ParseInstruction(string instructionString, OpCode expectedOpCode, AddressingMode expectedAddressingMode)
        {
            var instruction = InstructionParser.Parse(instructionString);
            Assert.Multiple(() =>
            {
                Assert.That(instruction.Code, Is.EqualTo(expectedOpCode));
                Assert.That(instruction.Mode, Is.EqualTo(expectedAddressingMode));
            });
        }

        [TestCase("lda #$00", (ushort) 0x0000)]
        [TestCase("lda #$01", (ushort) 0x0001)]
        [TestCase("bne *$02", (ushort) 0x0002)]
        [TestCase("bne *$03", (ushort) 0x0003)]
        [TestCase("sta $0004", (ushort) 0x0004)]
        [TestCase("and $0005,X", (ushort) 0x0005)]
        [TestCase("ldx $0006,Y", (ushort) 0x0006)]
        [TestCase("eor $07", (ushort) 0x0007)]
        [TestCase("eor $0B", (ushort) 0x000B)]
        [TestCase("ror $08,X", (ushort) 0x0008)]
        [TestCase("ldx $09,Y", (ushort) 0x0009)]
        [TestCase("jmp ($000A)", (ushort) 0x000A)]
        public void ParseInstructionForAddress(string instructionString, ushort expectedAddress)
        {
            var instruction = InstructionParser.Parse(instructionString);
            Assert.AreEqual(expectedAddress, instruction.Address);
        }
    }
}