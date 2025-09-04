namespace ChessBot.Core.Enums;

[Flags]
public enum Piece : byte
{
    None   = 0,
    Pawn   = 1,
    Bishop = 2,
    Rook   = 4,
    Knight = 8,
    Queen  = 16,
    King   = 32,
    
    White = 64,
    Black = 128,
    
    SlidingPiece = Bishop | Rook | Queen
}