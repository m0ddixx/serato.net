using System.Runtime.InteropServices;

namespace Serato.Net.Structs
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 0)]
    public struct Session
    {
        public Chunk[] Chunks;
    }
}