namespace ChessBot.Core.Player;

public class RandomMovePlayer : IPlayer
{
    private static Random _random = new Random();
    
    public Move GetNextMove(Board board)
    {
        return board.LegalMoves.FriendlyMoves.ElementAt(_random.Next(0, board.LegalMoves.FriendlyMoves.Count() - 1));
    }
}