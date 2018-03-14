﻿using System.Collections.Generic;
using static Assembler6502.AddressingMode;
using static Assembler6502.OpCode;


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
                yield return Instructions[(Code, Mode)];
                if (SingleByteAddressModes.Contains(Mode))
                    yield return (byte) Address;
                else if (TwoByteAddressModes.Contains(Mode))
                {
                    yield return (byte) Address;
                    yield return (byte) (Address >> 8);
                }
            }
        }

        private static readonly ISet<AddressingMode> SingleByteAddressModes = new HashSet<AddressingMode>
        {
            Immediate,
            Relative,
            ZeroPage
        };

        private static readonly ISet<AddressingMode> TwoByteAddressModes = new HashSet<AddressingMode>
        {
            Absolute,
            AbsoluteXIndexed,
            AbsoluteYIndexed
        };

        private static readonly IDictionary<(OpCode, AddressingMode), byte> Instructions =
            new Dictionary<(OpCode, AddressingMode), byte>
            {
                {(BRK, Implicit), 0},
                {(ORA, XIndexedIndirect), 1},
                {(ORA, ZeroPage), 5},
                {(ASL, ZeroPage), 6},
                {(PHP, Implicit), 8},
                {(ORA, Immediate), 9},
                {(ASL, Accumulator), 10},
                {(ORA, Absolute), 13},
                {(ASL, Absolute), 14},
                {(BPL, Relative), 16},
                {(ORA, IndirectYIndexed), 17},
                {(ORA, ZeroPageXIndexed), 21},
                {(ASL, ZeroPageXIndexed), 22},
                {(CLC, Implicit), 24},
                {(ORA, AbsoluteYIndexed), 25},
                {(ORA, AbsoluteXIndexed), 29},
                {(ASL, AbsoluteXIndexed), 30},
                {(JSR, Absolute), 32},
                {(AND, XIndexedIndirect), 33},
                {(BIT, ZeroPage), 36},
                {(AND, ZeroPage), 37},
                {(ROL, ZeroPage), 38},
                {(PLP, Implicit), 40},
                {(AND, Immediate), 41},
                {(ROL, Accumulator), 42},
                {(BIT, Absolute), 44},
                {(AND, Absolute), 45},
                {(ROL, Absolute), 46},
                {(BMI, Relative), 48},
                {(AND, IndirectYIndexed), 49},
                {(AND, ZeroPageXIndexed), 53},
                {(ROL, ZeroPageXIndexed), 54},
                {(SEC, Implicit), 56},
                {(AND, AbsoluteYIndexed), 57},
                {(AND, AbsoluteXIndexed), 61},
                {(ROL, AbsoluteXIndexed), 62},
                {(RTI, Implicit), 64},
                {(EOR, XIndexedIndirect), 65},
                {(EOR, ZeroPage), 69},
                {(LSR, ZeroPage), 70},
                {(PHA, Implicit), 72},
                {(EOR, Immediate), 73},
                {(LSR, Accumulator), 74},
                {(JMP, Absolute), 76},
                {(EOR, Absolute), 77},
                {(LSR, Absolute), 78},
                {(BVC, Relative), 80},
                {(EOR, IndirectYIndexed), 81},
                {(EOR, ZeroPageXIndexed), 85},
                {(LSR, ZeroPageXIndexed), 86},
                {(CLI, Implicit), 88},
                {(EOR, AbsoluteYIndexed), 89},
                {(EOR, AbsoluteXIndexed), 93},
                {(LSR, AbsoluteXIndexed), 94},
                {(RTS, Implicit), 96},
                {(ADC, XIndexedIndirect), 97},
                {(ADC, ZeroPage), 101},
                {(ROR, ZeroPage), 102},
                {(PLA, Implicit), 104},
                {(ADC, Immediate), 105},
                {(ROR, Accumulator), 106},
                {(JMP, Indirect), 108},
                {(ADC, Absolute), 109},
                {(ROR, Absolute), 110},
                {(BVS, Relative), 112},
                {(ADC, IndirectYIndexed), 113},
                {(ADC, ZeroPageXIndexed), 117},
                {(ROR, ZeroPageXIndexed), 118},
                {(SEI, Implicit), 120},
                {(ADC, AbsoluteYIndexed), 121},
                {(ADC, AbsoluteXIndexed), 125},
                {(ROR, AbsoluteXIndexed), 126},
                {(STA, XIndexedIndirect), 129},
                {(STY, ZeroPage), 132},
                {(STA, ZeroPage), 133},
                {(STX, ZeroPage), 134},
                {(DEY, Implicit), 136},
                {(TXA, Implicit), 138},
                {(STY, Absolute), 140},
                {(STA, Absolute), 141},
                {(STX, Absolute), 142},
                {(BCC, Relative), 144},
                {(STA, IndirectYIndexed), 145},
                {(STY, ZeroPageXIndexed), 148},
                {(STA, ZeroPageXIndexed), 149},
                {(STX, ZeroPageYIndexed), 150},
                {(TYA, Implicit), 152},
                {(STA, AbsoluteYIndexed), 153},
                {(TXS, Implicit), 154},
                {(STA, AbsoluteXIndexed), 157},
                {(LDY, Immediate), 160},
                {(LDA, XIndexedIndirect), 161},
                {(LDX, Immediate), 162},
                {(LDY, ZeroPage), 164},
                {(LDA, ZeroPage), 165},
                {(LDX, ZeroPage), 166},
                {(TAY, Implicit), 168},
                {(LDA, Immediate), 169},
                {(TAX, Implicit), 170},
                {(LDY, Absolute), 172},
                {(LDA, Absolute), 173},
                {(LDX, Absolute), 174},
                {(BCS, Relative), 176},
                {(LDA, IndirectYIndexed), 177},
                {(LDY, ZeroPageXIndexed), 180},
                {(LDA, ZeroPageXIndexed), 181},
                {(LDX, ZeroPageYIndexed), 182},
                {(CLV, Implicit), 184},
                {(LDA, AbsoluteYIndexed), 185},
                {(TSX, Implicit), 186},
                {(LDY, AbsoluteXIndexed), 188},
                {(LDA, AbsoluteXIndexed), 189},
                {(LDX, AbsoluteYIndexed), 190},
                {(CPY, Immediate), 192},
                {(CMP, XIndexedIndirect), 193},
                {(CPY, ZeroPage), 196},
                {(CMP, ZeroPage), 197},
                {(DEC, ZeroPage), 198},
                {(INY, Implicit), 200},
                {(CMP, Immediate), 201},
                {(DEX, Implicit), 202},
                {(CPY, Absolute), 204},
                {(CMP, Absolute), 205},
                {(DEC, Absolute), 206},
                {(BNE, Relative), 208},
                {(CMP, IndirectYIndexed), 209},
                {(CMP, ZeroPageXIndexed), 213},
                {(DEC, ZeroPageXIndexed), 214},
                {(CLD, Implicit), 216},
                {(CMP, AbsoluteYIndexed), 217},
                {(CMP, AbsoluteXIndexed), 221},
                {(DEC, AbsoluteXIndexed), 222},
                {(CPX, Immediate), 224},
                {(SBC, XIndexedIndirect), 225},
                {(CPX, ZeroPage), 228},
                {(SBC, ZeroPage), 229},
                {(INC, ZeroPage), 230},
                {(INX, Implicit), 232},
                {(SBC, Immediate), 233},
                {(NOP, Implicit), 234},
                {(CPX, Absolute), 236},
                {(SBC, Absolute), 237},
                {(INC, Absolute), 238},
                {(BEQ, Relative), 240},
                {(SBC, IndirectYIndexed), 241},
                {(SBC, ZeroPageXIndexed), 245},
                {(INC, ZeroPageXIndexed), 246},
                {(SED, Implicit), 248},
                {(SBC, AbsoluteYIndexed), 249},
                {(SBC, AbsoluteXIndexed), 253},
                {(INC, AbsoluteXIndexed), 254}
            };
    }
}