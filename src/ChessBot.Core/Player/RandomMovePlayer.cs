namespace ChessBot.Core.Player;

public class RandomMovePlayer : IPlayer
{
    private Random _random = new Random();
    
    public Move GetNextMove(Board board)
    {
        
        board.GenerateLegalMoves();

        return board.LegalMoves[_random.Next(0, board.LegalMoves.Count - 1)];
    }
}