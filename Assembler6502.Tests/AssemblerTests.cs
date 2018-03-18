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

        [Test]
        public void CommentsInSourceCode()
        {
            string[] sourceCode =
            {
                "; program to swap two memory locations",
                "",
                "LDA $0380 ; load memory address 896 into A",
                "LDX $0381 ; load memory address 897 into X",
                "",
                "STA $0381 ; store A into 897",
                "STX $0380 ; store X into 896",
                "",
                "RTS ; return from subroutine"
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
        public void CodeWithAddressLabels()
        {
            string[] sourceCode =
            {
                "; print 'Y' if memeory address content is non-zero, otherwise 'N'",
                "       LDA $0380  ; load memory address into A",
                "       BEQ *LABEL ; check if it was 0",
                "       LDA #$59   ; load 'Y' into A",
                "       JMP END    ; got to end",
                "LABEL: LDA #$4E   ; load 'N' into A",
                "END:   JSR $FFD2  ; write the character to the screen",
                "       RTS"
            };

            var binary = Assembler.Assemble(sourceCode, 0x033C);

            byte[] expectedBinary =
            {
                0x3C, 0x03,
                0xAD, 0x80, 0x03, // 033C
                0xF0, 0x05, // 033F
                0xA9, 0x59, // 0341
                0x4C, 0x48, 0x03, // 0343
                0xA9, 0x4E, // 0346 (LABEL)
                0x20, 0xD2, 0xFF, // 0348 (END)
                0x60 // 034B
            };

            Assert.AreEqual(expectedBinary, binary);
        }
    }
}