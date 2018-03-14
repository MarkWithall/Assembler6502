using System;
using System.IO;

namespace Assembler6502
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("No input/output file paths specified!");
                return 2;
            }

            var inputFilePath = args[0];
            var outputFilePath = args[1];

            var sourceCode = File.ReadAllLines(inputFilePath);

            var binary = Assembler.Assemble(sourceCode, 0x033C);

            try
            {
                using (var stream = new FileStream(outputFilePath, FileMode.Create))
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(binary);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return 2;
            }

            return 0;
        }
    }
}