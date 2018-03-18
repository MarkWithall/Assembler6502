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
    }
}
