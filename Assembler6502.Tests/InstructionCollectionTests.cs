using NUnit.Framework;
using static Assembler6502.AddressingMode;
using static Assembler6502.OpCode;

namespace Assembler6502.Tests
{
    [TestFixture]
    public class InstructionCollectionTests
    {
        [Test]
        public void AbsoluteAddressOfLabelOnFirstInstruction()
        {
            var collection = new InstructionCollection(0x033C)
            {
                new Instruction {Code = LDA, Mode = Absolute, AddressString = "$0380", Label = "START"}
            };
            var address = collection.AbsoluteAddressFor("START");
            Assert.That(address, Is.EqualTo(0x033C));
        }

        [Test]
        public void AbsoluteAddressOfLabelOnLaterInstruction()
        {
            var collection = new InstructionCollection(0x033C)
            {
                new Instruction {Code = LDA, Mode = Absolute, AddressString = "$0380"},
                new Instruction {Code = BEQ, Mode = Relative, AddressString = "LABEL"},
                new Instruction {Code = LDA, Mode = Immediate, AddressString = "$59"},
                new Instruction {Code = JMP, Mode = Absolute, AddressString = "END"},
                new Instruction {Code = LDA, Mode = Immediate, AddressString = "$4E", Label = "LABEL"},
                new Instruction {Code = JSR, Mode = Absolute, AddressString = "$FFD2", Label = "END"},
                new Instruction {Code = RTS, Mode = Implicit}
            };
            var address = collection.AbsoluteAddressFor("END");
            Assert.That(address, Is.EqualTo(0x0348));
        }
    }
}
