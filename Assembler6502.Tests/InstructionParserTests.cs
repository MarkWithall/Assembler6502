﻿using NUnit.Framework;
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
    }
}
