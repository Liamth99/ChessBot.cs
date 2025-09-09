using System.Diagnostics;

namespace ChessBot.Core.Models;

/// <summary>
/// First Byte:  Castling information (the location of the rook)<br/>
/// Second Byte: Promotion Flag<br/>
/// Third Byte:  StartSquare<br/>
/// Last Byte:   TargetSquare<br/>
/// </summary>
[DebuggerDisplay("{StartSquare}-{TargetSquare}-{Promotion.ToString()}-{CastlingSquare}")]
public readonly struct Move : IEquatable<Move>
{
    public Move(byte startSquare, byte targetSquare, PromotionFlag promotion = PromotionFlag.None, byte castlingSquare = 0)
    {
        _moveData = (uint)(castlingSquare << 24 | (int)promotion << 16 | startSquare << 8 | targetSquare);
    }
    
    public Move(int startSquare, int targetSquare, PromotionFlag promotion = PromotionFlag.None, int castlingSquare = 0)
    {
        _moveData = (uint)(castlingSquare << 24 | (int)promotion << 16 | startSquare << 8 | (byte)targetSquare);
    }

    private readonly uint _moveData;

    public Byte CastlingSquare => (byte)(_moveData >> 24);
    public PromotionFlag Promotion => (PromotionFlag)(_moveData >> 16);
    public byte StartSquare => (byte)(_moveData >> 8);
    public byte TargetSquare => (byte)_moveData;

    public bool Equals(Move other)
    {
        return StartSquare == other.StartSquare && TargetSquare == other.TargetSquare;
    }

    public override bool Equals(object? obj)
    {
        return obj is Move other && Equals(other);
    }

    public override int GetHashCode()
    {
        return (int)_moveData;
    }

    public static bool operator ==(Move left, Move right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Move left, Move right)
    {
        return !left.Equals(right);
    }
}