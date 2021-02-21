using System;
using System.IO;
using System.Linq;
using Assembler6502;

if (args.Length != 2)
{
    Console.WriteLine("No input/output file paths specified!");
    return 2;
}

var inputFilePath = args[0];
var outputFilePath = args[1];
ushort startingAddress = 0x033C;
var sourceCode = File.ReadAllLines(inputFilePath);
var (binary, errors) = Assembler.Assemble(sourceCode, startingAddress);
if (errors.Any())
{
    foreach (var error in errors)
        Console.WriteLine(error);
    return 2;
}

try
{
    using var stream = new FileStream(outputFilePath, FileMode.Create);
    using var writer = new BinaryWriter(stream);
    writer.Write(binary);
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
    return 2;
}

return 0;