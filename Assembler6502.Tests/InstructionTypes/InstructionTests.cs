using System.Linq;
using Assembler6502.InstructionTypes;
using NSubstitute;
using NUnit.Framework;
using static Assembler6502.AddressingMode;
using static Assembler6502.OpCode;

namespace Assembler6502.Tests.InstructionTypes
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
            var instruciton = Instruction(code, mode, $"${address:X4}");
            CollectionAssert.AreEqual(expectedBytes, instruciton.Bytes.ToArray());
        }

        [Test]
        public void InstructionWithAbsoluteLabelGivesCorrectBytes()
        {
            var labelFinder = Substitute.For<ILabelFinder>();
            labelFinder.AbsoluteAddressFor("LABEL").Returns<ushort>(0x1342);

            var instruction = Instruction(JMP, Absolute, "LABEL", labelFinder: labelFinder);

            byte[] expectedBytes = {0x4C, 0x42, 0x13};

            CollectionAssert.AreEqual(expectedBytes, instruction.Bytes.ToArray());
        }

        [Test]
        public void InstructionWithRelativeLabelGivesCorrectBytes()
        {
            var labelFinder = Substitute.For<ILabelFinder>();
            labelFinder.RelativeAddressFor("LABEL", Arg.Any<Instruction>()).Returns<ushort>(0x42);

            var instruction = Instruction(BEQ, Relative, "LABEL", labelFinder: labelFinder);

            byte[] expectedBytes = {0xF0, 0x42};

            CollectionAssert.AreEqual(expectedBytes, instruction.Bytes.ToArray());
        }

        [Test]
        public void UnknownOpCodeThrowsWhenRequestingBytes()
        {
            var instruction = Instruction(OpCode.Unknown, Implicit);
            Assert.That(() => instruction.Bytes.ToArray(), Throws.InvalidOperationException);
        }

        [Test]
        public void UnknownAddressingModeThrowsWhenRequestingBytes()
        {
            var instruction = Instruction(BRK, AddressingMode.Unknown);
            Assert.That(() => instruction.Bytes.ToArray(), Throws.InvalidOperationException);
        }

        [TestCase(BRK, Implicit, (ushort)1)]
        [TestCase(ASL, Accumulator, (ushort)1)]
        [TestCase(LDA, Immediate, (ushort)2)]
        [TestCase(BNE, Relative, (ushort)2)]
        [TestCase(EOR, ZeroPage, (ushort)2)]
        [TestCase(ROR, ZeroPageXIndexed, (ushort)2)]
        [TestCase(LDX, ZeroPageYIndexed, (ushort)2)]
        [TestCase(ORA, XIndexedIndirect, (ushort)2)]
        [TestCase(CMP, IndirectYIndexed, (ushort)2)]
        [TestCase(STA, Absolute, (ushort)3)]
        [TestCase(AND, AbsoluteXIndexed, (ushort)3)]
        [TestCase(LDX, AbsoluteYIndexed, (ushort)3)]
        [TestCase(JMP, Indirect, (ushort)3)]
        public void InstructionLength(OpCode code, AddressingMode mode, ushort expectedLength)
        {
            var instruction = Instruction(code, mode);
            Assert.That(instruction.Length, Is.EqualTo(expectedLength));
        }

        [TestCase(LDA, Absolute, "$1342", true)]
        [TestCase(BEQ, Absolute, "$1342", false)]
        public void IsValid(OpCode code, AddressingMode mode, string addressString, bool expectedIsValid)
        {
            var instruction = Instruction(code, mode, addressString);
            Assert.That(instruction.IsValid, Is.EqualTo(expectedIsValid));
        }

        [TestCase(LDA, Absolute, "$1342", 13, null)]
        [TestCase(BEQ, Absolute, "$1342", 13, "Error (line 13) - invalid op code/addressing mode combination.")]
        [TestCase(BEQ, Absolute, "$1342", 42, "Error (line 42) - invalid op code/addressing mode combination.")]
        [TestCase(STA, ZeroPage, "$1342", 42, "Error (line 42) - single byte address must be less than 256.")]
        [TestCase(STA, Absolute, "$10000", 42, "Error (line 42) - two byte address must be less than 65536.")]
        public void ErrorMessage(OpCode code, AddressingMode mode, string addressString, int lineNumber, string expecetedError)
        {
            var instruction = Instruction(code, mode, addressString, lineNumber: lineNumber);
            Assert.That(instruction.ErrorMessage, Is.EqualTo(expecetedError));
        }

        [Test]
        public void UnknownLabelErrorMessage()
        {
            var labelFinder = Substitute.For<ILabelFinder>();
            var instruction = Instruction(LDA, Absolute, "LABEL", lineNumber: 13, labelFinder: labelFinder);
            Assert.That(instruction.ErrorMessage, Is.EqualTo("Error (line 13) - unknown address label 'LABEL'."));
        }

        [TestCase((ushort)0x0080, "Error (line 13) - address label 'LABEL' is greater than 127 bytes away.")]
        [TestCase((ushort)0xFF7F, "Error (line 13) - address label 'LABEL' is greater than -128 bytes away.")]
        public void RelativeAddressToFarErrorMessage(ushort relativeAddress, string expectedError)
        {
            var labelFinder = Substitute.For<ILabelFinder>();
            labelFinder.HasLabel(Arg.Any<string>()).Returns(true);
            labelFinder.RelativeAddressFor(Arg.Any<string>(), Arg.Any<Instruction>()).Returns(relativeAddress);
            var instruction = Instruction(BNE, Relative, "LABEL", lineNumber: 13, labelFinder: labelFinder);
            Assert.That(instruction.ErrorMessage, Is.EqualTo(expectedError));
        }

        [Test]
        public void UnknownOpCodeErrorMessage()
        {
            var instruction = Instruction(OpCode.Unknown, Absolute, lineNumber: 13);
            Assert.That(instruction.ErrorMessage, Is.EqualTo("Error (line 13) - unknown op code."));
        }

        [Test]
        public void UnknownAddressingModeErrorMessage()
        {
            var instruction = Instruction(LDA, AddressingMode.Unknown, lineNumber: 42);
            Assert.That(instruction.ErrorMessage, Is.EqualTo("Error (line 42) - unknown addressing mode."));
        }

        private static Instruction Instruction(
            OpCode code,
            AddressingMode mode,
            string? addressString = null,
            string? label = null,
            int lineNumber = 0,
            ILabelFinder labelFinder = default!)
        {
            return new InstructionFactory(labelFinder).Create(code, mode, addressString, lineNumber, label);
        }
    }
}