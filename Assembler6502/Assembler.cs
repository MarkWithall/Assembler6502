﻿using System.Collections.Generic;
using System.Linq;

namespace Assembler6502
{
    public static class Assembler
    {
        public static byte[] Assemble(string[] sourceCode, ushort startingAddress)
        {
            var bytes = new List<byte> {(byte) startingAddress, (byte) (startingAddress >> 8)};

            foreach (var line in sourceCode.Select(StripComments).Where(l => l != string.Empty))
            {
                var instruction = InstructionParser.Parse(line);
                bytes.AddRange(instruction.Bytes);
            }

            return bytes.ToArray();
        }

        private static string StripComments(string line)
        {
            return line.Split(';')[0].Trim();
        }
    }
}