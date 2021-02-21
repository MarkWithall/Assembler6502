using NUnit.Framework;
using static Assembler6502.OpCode;
using static Assembler6502.AddressingMode;
using Assembler6502.InstructionTypes;

namespace Assembler6502.Tests
{
    [TestFixture]
    public class InstructionParserTests
    {
        [TestCase("brk", BRK, Implicit, (ushort) 0x0000)]
        [TestCase("rts", RTS, Implicit, (ushort) 0x0000)]
        [TestCase("asl a", ASL, Accumulator, (ushort) 0x0000)]
        [TestCase("lda #$00", LDA, Immediate, (ushort) 0x0000)]
        [TestCase("lda #$01", LDA, Immediate, (ushort) 0x0001)]
        [TestCase("bne *$02", BNE, Relative, (ushort) 0x0002)]
        [TestCase("bne *$03", BNE, Relative, (ushort) 0x0003)]
        [TestCase("sta $0104", STA, Absolute, (ushort) 0x0104)]
        [TestCase("and $0105,X", AND, AbsoluteXIndexed, (ushort) 0x0105)]
        [TestCase("ldx $0106,Y", LDX, AbsoluteYIndexed, (ushort) 0x0106)]
        [TestCase("eor <$07", EOR, ZeroPage, (ushort) 0x0007)]
        [TestCase("eor <$0B", EOR, ZeroPage, (ushort) 0x000B)]
        [TestCase("ror <$08,X", ROR, ZeroPageXIndexed, (ushort) 0x0008)]
        [TestCase("ldx <$09,Y", LDX, ZeroPageYIndexed, (ushort) 0x0009)]
        [TestCase("jmp ($010A)", JMP, Indirect, (ushort) 0x010A)]
        [TestCase("ora ($0C,X)", ORA, XIndexedIndirect, (ushort) 0x000C)]
        [TestCase("cmp ($0D),Y", CMP, IndirectYIndexed, (ushort) 0x000D)]
        [TestCase("xxx $4242", OpCode.Unknown, Absolute, (ushort) 0x4242)]
        [TestCase("xxx ,,,,", OpCode.Unknown, AddressingMode.Unknown, (ushort) 0x0000)]
        [TestCase(" cmp  ($42),Y ", CMP, IndirectYIndexed, (ushort) 0x0042)]
        [TestCase("\tasl\ta\t", ASL, Accumulator, (ushort) 0x0000)]
        public void ParseInstruction(
            string instructionString,
            OpCode expectedOpCode,
            AddressingMode expectedAddressingMode,
            ushort expectedAddress)
        {
            var instruction = Parse(instructionString);
            Assert.Multiple(() =>
            {
                Assert.That(instruction.Code, Is.EqualTo(expectedOpCode));
                Assert.That(instruction.Mode, Is.EqualTo(expectedAddressingMode));
                Assert.That(instruction.Address, Is.EqualTo(expectedAddress));
                Assert.That(instruction.Label, Is.Null);
            });
        }

        [TestCase("BEQ *LABEL", "LABEL")]
        [TestCase("JMP LABEL", "LABEL")]
        [TestCase("BNE *LABEL_1", "LABEL_1")]
        public void ParseInstructionWithLabel(string instructionString, string expectedAddressString)
        {
            var instruction = Parse(instructionString);
            Assert.That(instruction.AddressString, Is.EqualTo(expectedAddressString));
        }

        [Test]
        public void LabelledInstruction()
        {
            var instruction = Parse("LABEL: LDA #$4E");
            Assert.Multiple(() =>
            {
                Assert.That(instruction.Code, Is.EqualTo(LDA));
                Assert.That(instruction.Mode, Is.EqualTo(Immediate));
                Assert.That(instruction.AddressString, Is.EqualTo("$4E"));
                Assert.That(instruction.Label, Is.EqualTo("LABEL"));
            });
        }

        private static Instruction Parse(string instructionString)
        {
            return new InstructionParser(default!).Parse(instructionString, 0);
        }
    }
}