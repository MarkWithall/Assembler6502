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

    public static class Compiler
    {
        public static byte[] Compile(string[] sourceCode, UInt16 startingAddress)
        {
            byte[] program =
            {
                0x3C, 0x03, // staring memory location (828)
                0xAD, 0x80, 0x03, // LDA $0380 ; (896)
                0xAE, 0x81, 0x03, // LDX $0381 ; (897)
                0x8D, 0x81, 0x03, // STA $0381
                0x8E, 0x80, 0x03, // STX $0380
                0x60, // RTS
            };
            return program;
        }
    }
}
