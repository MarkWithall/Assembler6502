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
    }
}
