using System.Linq;
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
                Address = address
            };
            CollectionAssert.AreEqual(expectedBytes, instruciton.Bytes.ToArray());
        }

        [Test]
        public void UnknownOpCodeThrowsWhenRequestingBytes()
        {
            var instruction = new Instruction {Code = OpCode.Unknown, Mode = Implicit, Address = 0x0000};
            Assert.That(() => instruction.Bytes.ToArray(), Throws.InvalidOperationException);
        }
    }
}