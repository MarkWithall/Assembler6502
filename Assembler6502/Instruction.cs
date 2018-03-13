﻿using System.Collections.Generic;
using System.Linq;

namespace Assembler6502
{
    public class Instruction
    {
        public OpCode Code { get; set; }
        public AddressingMode Mode { get; set; }
        public ushort Address { get; set; }

        public IEnumerable<byte> Bytes
        {
            get
            {
                yield return 0x00;
            }
        }
    }
}