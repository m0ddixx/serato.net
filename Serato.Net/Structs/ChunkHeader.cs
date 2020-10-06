using System;
using System.Runtime.InteropServices;

namespace Serato.Net.Structs
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 0)]
    public struct ChunkHeader
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        public string Identifier;
        [MarshalAs(UnmanagedType.U4, SizeConst = 4)]
        public int Length;
    }
}