using System.Linq;

namespace Assembler6502
{
    public static class Assembler
    {
        public static byte[] Assemble(string[] sourceCode, ushort startingAddress)
        {
            var instructions = new InstructionCollection(startingAddress);
            var parser = new InstructionParser(instructions);
            foreach (var instruction in sourceCode.Select(StripComments).Where(l => l != string.Empty).Select(parser.Parse))
                instructions.Add(instruction);
            
            byte[] addressBytes = {(byte) startingAddress, (byte) (startingAddress >> 8)};

            return addressBytes.Concat(instructions.SelectMany(i => i.Bytes)).ToArray();
        }

        private static string StripComments(string line)
        {
            return line.Split(';')[0].Trim();
        }
    }
}