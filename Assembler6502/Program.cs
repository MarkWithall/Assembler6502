using System;
using System.IO;

namespace Assembler6502
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("No output file path specified!");
                return 2;
            }

            var filePath = args[0];

            // Swap 2 bytes in memory
            string[] sourceCode =
            {
                "LDA $0380",
                "LDX $0381",
                "STA $0381",
                "STX $0380",
                "RTS"
            };

            var program = Compiler.Compile(sourceCode, 0x033C);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(program);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return 2;
            }

            return 0;
        }
    }
}
