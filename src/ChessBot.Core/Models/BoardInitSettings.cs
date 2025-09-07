namespace ChessBot.Core.Models;

public class BoardInitSettings
{
    public required Piece[] Squares { get; set; }
    public required Piece ColorToMove { get; set; }
    public required ulong EnPassantBits { get; set; }
    public required ulong ValidCastleBits { get; set; }
    public required int HalfMoveClock { get; set; }
    public required int FullMoveCount { get; set; }

    public static BoardInitSettings Empty => new()
    {
        Squares = new Piece[64],
        ColorToMove = Piece.White,
        EnPassantBits = 0,
        ValidCastleBits = 0,
        HalfMoveClock = 0,
        FullMoveCount = 1
    };
}