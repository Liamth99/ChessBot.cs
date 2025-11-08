using System.Diagnostics;

namespace ChessBot.Core.Models;

/// <summary>
/// First Byte:  Castling information (the location of the rook)<br/>
/// Second Byte: Promotion Flag<br/>
/// Third Byte:  StartSquare<br/>
/// Last Byte:   TargetSquare<br/>
/// </summary>
[DebuggerDisplay("{DisplayString}")]
public readonly struct Move : IEquatable<Move>
{
    private string DisplayString => $"({_moveData:x8}) s{StartSquare}-t{TargetSquare}-p{Promotion.ToString()}-c{CastlingSquare}";

    public Move(byte startSquare, byte targetSquare, PromotionFlag promotion = PromotionFlag.None, byte castlingSquare = 0)
    {
        _moveData = (uint)(castlingSquare << 24 | (int)promotion << 16 | startSquare << 8 | targetSquare);
    }
    
    public Move(int startSquare, int targetSquare, PromotionFlag promotion = PromotionFlag.None, int castlingSquare = 0)
    {
        _moveData = (uint)(castlingSquare << 24 | (int)promotion << 16 | startSquare << 8 | (byte)targetSquare);
    }

    public Move(string startSquare, string targetSquare, PromotionFlag promotion = PromotionFlag.None, string? castlingSquare = null)
    {
        _moveData = (uint)((castlingSquare is null ? 0 : castlingSquare.GetIndexByPosition() << 24) |
                           (uint)promotion << 16 |
                           startSquare.GetIndexByPosition() << 8 |
                           targetSquare.GetIndexByPosition());
    }

    public Move(uint moveData)
    {
        _moveData = moveData;
    }

    public static explicit operator uint(Move move) => move._moveData;

    private readonly uint _moveData;

    public byte CastlingSquare => (byte)(_moveData >> 24);
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