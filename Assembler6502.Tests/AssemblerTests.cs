using NUnit.Framework;

namespace Assembler6502.Tests
{
    [TestFixture]
    public class CompilerTests
    {
        [Test]
        public void SimpleTest()
        {
            string[] sourceCode =
            {
                "LDA $0380",
                "LDX $0381",
                "STA $0381",
                "STX $0380",
                "RTS"
            };

            var binary = Assembler.Assemble(sourceCode, 0x033C);

            byte[] expectedBinary =
            {
                0x3C, 0x03, // staring memory location (828)
                0xAD, 0x80, 0x03, // LDA $0380 ; (896)
                0xAE, 0x81, 0x03, // LDX $0381 ; (897)
                0x8D, 0x81, 0x03, // STA $0381
                0x8E, 0x80, 0x03, // STX $0380
                0x60 // RTS
            };

            Assert.AreEqual(expectedBinary, binary);
        }

        [Test]
        public void ExtraWhiteSpaceInSourceCode()
        {
            string[] sourceCode =
            {
                "",
                "LDA $0380",
                "LDX $0381",
                "\t",
                "STA $0381",
                "STX $0380",
                "RTS"
            };

            var binary = Assembler.Assemble(sourceCode, 0x033C);

            byte[] expectedBinary =
            {
                0x3C, 0x03, // staring memory location (828)
                0xAD, 0x80, 0x03, // LDA $0380 ; (896)
                0xAE, 0x81, 0x03, // LDX $0381 ; (897)
                0x8D, 0x81, 0x03, // STA $0381
                0x8E, 0x80, 0x03, // STX $0380
                0x60 // RTS
            };

            Assert.AreEqual(expectedBinary, binary);
        }
    }
}