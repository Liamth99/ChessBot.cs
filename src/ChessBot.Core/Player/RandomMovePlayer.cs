namespace ChessBot.Core.Player;

public class RandomMovePlayer : IPlayer
{
    private Random _random = new Random();
    
    public Move GetNextMove(Board board)
    {
        return board.LegalMoves.FriendlyMoves[_random.Next(0, board.LegalMoves.FriendlyMoves.Count - 1)];
    }
}