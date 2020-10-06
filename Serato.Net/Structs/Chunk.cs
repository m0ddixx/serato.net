using System.Runtime.InteropServices;

namespace Serato.Net.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Chunk
    {
        public ChunkHeader Header;
        [MarshalAs(UnmanagedType.ByValArray)]
        public byte[] Data;
    }
}