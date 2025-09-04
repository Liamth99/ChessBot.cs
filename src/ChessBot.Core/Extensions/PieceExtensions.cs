using ChessBot.Core.Enums;

namespace ChessBot.Core.Extensions;

public static class PieceExtensions
{
    public static bool ContainsAnyType(this Piece piece, Piece targetPiece) 
        => (piece & targetPiece) > 0;
    
    public static bool IsType(this Piece piece, Piece targetPiece) 
        => (piece & targetPiece) == targetPiece;

    public static Piece ToColor(this Piece piece)
        => piece & (Piece.White | Piece.Black);

    public static Piece ToggleColor(this Piece piece)
        => piece ^ (Piece.White | Piece.Black);
    
    public static char ToPieceCharacter(this Piece piece)
    {
        bool isWhite = piece.ToColor() == Piece.White;
        
        piece &= ~Piece.Black;
        piece &= ~Piece.White;

        char value;
        
        switch (piece)
        {
            case Piece.None:
                value = ' ';
                break;
            
            case Piece.Pawn:
                value = 'p';
                break;
            
            case Piece.Bishop:
                value = 'b';
                break;
            
            case Piece.Rook:
                value = 'r';
                break;
            
            case Piece.Knight:
                value = 'n';
                break;
            
            case Piece.Queen:
                value = 'q';
                break;
            
            case Piece.King:
                value = 'k';
                break;
                
            default:
                throw new InvalidOperationException($"Cannot convert '{piece}' to a character.");
        }

        return isWhite ? char.ToUpperInvariant(value) : value;
    }
}