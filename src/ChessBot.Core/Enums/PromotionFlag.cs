namespace ChessBot.Core.Enums;

[Flags]
public enum PromotionFlag : uint
{
    None,
    Bishop = 2,
    Rook   = 4,
    Knight = 8,
    Queen  = 16,
}