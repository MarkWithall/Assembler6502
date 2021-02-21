using Assembler6502.InstructionTypes;
using NUnit.Framework;
using static Assembler6502.AddressingMode;
using static Assembler6502.OpCode;

namespace Assembler6502.Tests
{
    [TestFixture]
    public class InstructionCollectionTests
    {
        private InstructionCollection _collection = default!;

        [SetUp]
        public void Setup()
        {
            _collection = new InstructionCollection(0x033C)
            {
                Instruction(LDA, Absolute, "$0380", "START"),
                Instruction(BEQ, Relative, "LABEL"),
                Instruction(LDA, Immediate, "$59"),
                Instruction(JMP, Absolute, "END"),
                Instruction(LDA, Immediate, "$4E", "LABEL"),
                Instruction(JSR, Absolute, "$FFD2", "END"),
                Instruction(RTS, Implicit)
            };
        }

        [TestCase("LABEL", true)]
        [TestCase("UNKNOWN", false)]
        public void HasLabel(string label, bool expectedResult)
        {
            Assert.That(_collection.HasLabel(label), Is.EqualTo(expectedResult));
        }

        [Test]
        public void AbsoluteAddressOfLabelOnFirstInstruction()
        {
            var address = _collection.AbsoluteAddressFor("START");
            Assert.That(address, Is.EqualTo(0x033C));
        }

        [Test]
        public void AbsoluteAddressOfLabelOnLaterInstruction()
        {
            var address = _collection.AbsoluteAddressFor("END");
            Assert.That(address, Is.EqualTo(0x0348));
        }

        [Test]
        public void AbsoluteAddressOfUnknownLabelThrows()
        {
            Assert.That(() => _collection.AbsoluteAddressFor("UNKNOWN"), Throws.ArgumentException);
        }

        [Test]
        public void RelativeAddressOfLabelOnNextInstruction()
        {
            var address = _collection.RelativeAddressFor("LABEL", _collection[3]);
            Assert.That(address, Is.EqualTo(0x00));
        }

        [Test]
        public void RelativeAddressOfLabelOnLaterInstruction()
        {
            var address = _collection.RelativeAddressFor("LABEL", _collection[1]);
            Assert.That(address, Is.EqualTo(0x05));
        }

        [Test]
        public void RelativeAddressOfLabelOnSameInstruction()
        {
            var address = _collection.RelativeAddressFor("LABEL", _collection[4]);
            Assert.That((byte)address, Is.EqualTo(0xFE));
        }

        [Test]
        public void RelativeAddressOfLabelOnEarlierInstruction()
        {
            var address = _collection.RelativeAddressFor("LABEL", _collection[5]);
            Assert.That((byte)address, Is.EqualTo(0xFB));
        }

        [Test]
        public void RelativeAddressOfUnknownLabelThrows()
        {
            Assert.That(() => _collection.RelativeAddressFor("UNKNOWN", _collection[1]), Throws.ArgumentException);
        }

        private static Instruction Instruction(OpCode code, AddressingMode mode, string? addressString = null, string? label = null)
        {
            return new InstructionFactory(default!).Create(code, mode, addressString, 0, label);
        }
    }
}
