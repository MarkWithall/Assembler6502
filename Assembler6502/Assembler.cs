using System.Linq;

namespace Assembler6502
{
    public static class Assembler
    {
        public static byte[] Assemble(string[] sourceCode, ushort startingAddress)
        {
            byte[] addressBytes = {(byte) startingAddress, (byte) (startingAddress >> 8)};

            var sourceBytes = sourceCode
                .Select(StripComments)
                .Where(l => l != string.Empty)
                .Select(InstructionParser.Parse)
                .SelectMany(i => i.Bytes);

            return addressBytes.Concat(sourceBytes).ToArray();
        }

        private static string StripComments(string line)
        {
            return line.Split(';')[0].Trim();
        }
    }
}