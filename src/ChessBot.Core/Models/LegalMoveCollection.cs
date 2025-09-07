namespace ChessBot.Core.Models;

public class LegalMoveCollection
{
    private readonly Board Board;
    
    public IEnumerable<Move> FriendlyMoves => _friendlyMoves;
    public IEnumerable<Move> EnemyMoves => _enemyMoves;

    // Chose the max based on https://chess.stackexchange.com/questions/46727/maximum-number-of-pseudolegal-moves-in-position-that-might-not-be-reachable-from
    private readonly List<Move> _friendlyMoves = new (256);
    private readonly List<Move> _enemyMoves = new (256);

    public ulong WhiteAttackBits { get; private set; }
    public ulong BlackAttackBits { get; private set; }

    public LegalMoveCollection(Board board)
    {
        Board = board;
    }
    
    public void Add(Move move)
    {
        Piece piece = Board[move.StartSquare];
        
        if(piece.IsType(Board.ColorToMove))
            _friendlyMoves.Add(move);
        else
            _enemyMoves.Add(move);

        if (piece.ToColor() is Piece.White)
            WhiteAttackBits |= move.TargetSquare.ToIntBit();
        else
            BlackAttackBits |= move.TargetSquare.ToIntBit();
    }

    public void AddRange(IEnumerable<Move> moves)
    {
        foreach (var move in moves)
        {
            Add(move);
        }
    }

    public void Clear()
    {
        _friendlyMoves.Clear();
        _enemyMoves.Clear();

        BlackAttackBits = 0;
        WhiteAttackBits = 0;
    }
}