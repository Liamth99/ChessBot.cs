namespace ChessBot.Core.Models;

public static class ByteExtensions
{
    public static byte Rank(this byte position)
        => (byte)(position / 8);

    public static byte File(this byte position)
        => (byte)(position % 8);

    public static ulong ToIntBit(this byte position)
        => 1UL << position;
}