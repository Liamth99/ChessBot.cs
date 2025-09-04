namespace ChessBot.Core.Models;

/// <summary>
/// First Byte:  Unused <br/>
/// Second Byte: Promotion Flag<br/>
/// Third Byte:  StartSquare<br/>
/// Last Byte:   TargetSquare<br/>
/// </summary>
public readonly struct Move : IEquatable<Move>
{
    public Move(byte startSquare, byte targetSquare, PromotionFlag promotion = PromotionFlag.None)
    {
        _moveData = (int)promotion << 16 | startSquare << 8 | targetSquare;
    }
    
    public Move(int startSquare, int targetSquare, PromotionFlag promotion = PromotionFlag.None)
    {
        _moveData = (int)promotion << 16 | (byte)startSquare << 8 | (byte)targetSquare;
    }

    private readonly int _moveData;
    
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
        return _moveData;
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