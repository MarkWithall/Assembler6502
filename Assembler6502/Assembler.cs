using System.Linq;

namespace Assembler6502
{
    public static class Assembler
    {
        public static (byte[] binary, string[] errors) Assemble(string[] sourceCode, ushort startingAddress)
        {
            var instructions = new InstructionCollection(startingAddress);
            var parser = new InstructionParser(instructions);
            var ins = sourceCode
                .Select((l, i) => (Line: StripComments(l), LineNumber: i + 1))
                .Where(l => l.Line != string.Empty)
                .Select(l => parser.Parse(l.Line, l.LineNumber));
            instructions.AddRange(ins);

            var errors = instructions.ErrorMessages.ToArray();
            if (errors.Any())
                return (null, errors);

            byte[] addressBytes = {(byte) startingAddress, (byte) (startingAddress >> 8)};

            return (addressBytes.Concat(instructions.Bytes).ToArray(), null);
        }

        private static string StripComments(string line)
        {
            return line.Split(';')[0].Trim();
        }
    }
}