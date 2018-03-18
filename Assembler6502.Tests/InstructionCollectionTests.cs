using NUnit.Framework;
using static Assembler6502.AddressingMode;
using static Assembler6502.OpCode;

namespace Assembler6502.Tests
{
    [TestFixture]
    public class InstructionCollectionTests
    {
        private InstructionCollection _collection;
        
        [SetUp]
        public void Setup()
        {
            _collection = new InstructionCollection(0x033C)
            {
                new Instruction {Code = LDA, Mode = Absolute, AddressString = "$0380", Label = "START"},
                new Instruction {Code = BEQ, Mode = Relative, AddressString = "LABEL"},
                new Instruction {Code = LDA, Mode = Immediate, AddressString = "$59"},
                new Instruction {Code = JMP, Mode = Absolute, AddressString = "END"},
                new Instruction {Code = LDA, Mode = Immediate, AddressString = "$4E", Label = "LABEL"},
                new Instruction {Code = JSR, Mode = Absolute, AddressString = "$FFD2", Label = "END"},
                new Instruction {Code = RTS, Mode = Implicit}
            };
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
        public void RelativeAddressOfUnknownLabelThrows()
        {
            Assert.That(() => _collection.RelativeAddressFor("UNKNOWN", _collection[1]), Throws.ArgumentException);
        }
    }
}
