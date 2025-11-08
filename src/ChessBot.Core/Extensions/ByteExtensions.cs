namespace ChessBot.Core.Extensions;

public static class ByteExtensions
{
    public static byte Rank(this byte position)
        => (byte)(position / 8);

    public static byte File(this byte position)
        => (byte)(position % 8);

    public static ulong ToIntBit(this byte position)
        => 1UL << position;

    public static string GetPositionByIndex(this byte index)
    {
        if (index > 63)
            throw new ArgumentOutOfRangeException(nameof(index), "Index must be between 0 and 63 inclusive.");

        int rank = index % 8;

        int file = index / 8;

        char rankChar = (char)('a' + rank);

        char fileChar = (char)('1' + file);

        return $"{rankChar}{fileChar}";
    }
}