using System.Linq;
using NSubstitute;
using NUnit.Framework;
using static Assembler6502.AddressingMode;
using static Assembler6502.OpCode;

namespace Assembler6502.Tests
{
    [TestFixture]
    public class InstructionTests
    {
        [TestCase(BRK, Implicit, (ushort) 0x0000, new byte[] {0x00})]
        [TestCase(RTS, Implicit, (ushort) 0x0000, new byte[] {0x60})]
        [TestCase(ASL, Accumulator, (ushort) 0x0000, new byte[] {0x0A})]
        [TestCase(LDA, Immediate, (ushort) 0x0000, new byte[] {0xA9, 0x00})]
        [TestCase(LDA, Immediate, (ushort) 0x0001, new byte[] {0xA9, 0x01})]
        [TestCase(BNE, Relative, (ushort) 0x0002, new byte[] {0xD0, 0x02})]
        [TestCase(STA, Absolute, (ushort) 0x0104, new byte[] {0x8D, 0x04, 0x01})]
        [TestCase(AND, AbsoluteXIndexed, (ushort) 0x0105, new byte[] {0x3D, 0x05, 0x01})]
        [TestCase(LDX, AbsoluteYIndexed, (ushort) 0x0106, new byte[] {0xBE, 0x06, 0x01})]
        [TestCase(EOR, ZeroPage, (ushort) 0x0007, new byte[] {0x45, 0x07})]
        [TestCase(ROR, ZeroPageXIndexed, (ushort) 0x0008, new byte[] {0x76, 0x08})]
        [TestCase(LDX, ZeroPageYIndexed, (ushort) 0x0009, new byte[] {0xB6, 0x09})]
        [TestCase(JMP, Indirect, (ushort) 0x010A, new byte[] {0x6C, 0x0A, 0x01})]
        [TestCase(ORA, XIndexedIndirect, (ushort) 0x000C, new byte[] {0x01, 0x0C})]
        [TestCase(CMP, IndirectYIndexed, (ushort) 0x000D, new byte[] {0xD1, 0x0D})]
        public void Bytes(OpCode code, AddressingMode mode, ushort address, byte[] expectedBytes)
        {
            var instruciton = new Instruction
            {
                Code = code,
                Mode = mode,
                AddressString = $"${address:X4}"
            };
            CollectionAssert.AreEqual(expectedBytes, instruciton.Bytes.ToArray());
        }

        [Test]
        public void InstructionWithAbsoluteLabelGivesCorrectBytes()
        {
            var labelFinder = Substitute.For<LabelFinder>();
            labelFinder.AbsoluteAddressFor("LABEL").Returns<ushort>(0x1342);

            var instruction = new Instruction(labelFinder)
            {
                Code = JMP,
                Mode = Absolute,
                AddressString = "LABEL"
            };

            byte[] expectedBytes = {0x4C, 0x42, 0x13};

            CollectionAssert.AreEqual(expectedBytes, instruction.Bytes.ToArray());
        }

        [Test]
        public void InstructionWithRelativeLabelGivesCorrectBytes()
        {
            var labelFinder = Substitute.For<LabelFinder>();
            labelFinder.RelativeAddressFor("LABEL", Arg.Any<Instruction>()).Returns<ushort>(0x42);

            var instruction = new Instruction(labelFinder)
            {
                Code = BEQ,
                Mode = Relative,
                AddressString = "LABEL"
            };

            byte[] expectedBytes = {0xF0, 0x42};

            CollectionAssert.AreEqual(expectedBytes, instruction.Bytes.ToArray());
        }

        [Test]
        public void UnknownOpCodeThrowsWhenRequestingBytes()
        {
            var instruction = new Instruction {Code = OpCode.Unknown, Mode = Implicit, AddressString = null};
            Assert.That(() => instruction.Bytes.ToArray(), Throws.InvalidOperationException);
        }

        [Test]
        public void UnknownAddressingModeThrowsWhenRequestingBytes()
        {
            var instruction = new Instruction {Code = BRK, Mode = AddressingMode.Unknown, AddressString = null};
            Assert.That(() => instruction.Bytes.ToArray(), Throws.InvalidOperationException);
        }

        [TestCase(BRK, Implicit, 1)]
        [TestCase(ASL, Accumulator, 1)]
        [TestCase(LDA, Immediate, 2)]
        [TestCase(BNE, Relative, 2)]
        [TestCase(EOR, ZeroPage, 2)]
        [TestCase(ROR, ZeroPageXIndexed, 2)]
        [TestCase(LDX, ZeroPageYIndexed, 2)]
        [TestCase(ORA, XIndexedIndirect, 2)]
        [TestCase(CMP, IndirectYIndexed, 2)]
        [TestCase(STA, Absolute, 3)]
        [TestCase(AND, AbsoluteXIndexed, 3)]
        [TestCase(LDX, AbsoluteYIndexed, 3)]
        [TestCase(JMP, Indirect, 3)]
        public void InstructionLength(OpCode code, AddressingMode mode, int expectedLength)
        {
            var instruction = new Instruction { Code = code, Mode = mode, AddressString = null };
            Assert.That(instruction.Length, Is.EqualTo(expectedLength));
        }
    }
}