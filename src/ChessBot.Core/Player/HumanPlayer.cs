namespace ChessBot.Core.Player;

public class HumanPlayer : IPlayer
{
    public Move GetNextMove(Board board)
    {
        
        board.GenerateLegalMoves();

        var move = new Move();

        while (!board.LegalMoves.Contains(move))
        {
            Console.WriteLine("Enter move in format of 'e2 e4'");
            var input = Console.ReadLine();

            ArgumentException.ThrowIfNullOrWhiteSpace(input);

            var positions = input.Split('\u0020');

            var startIndex = BoardUtils.GetIndexByPosition(positions[0]);
            var endIndex   = BoardUtils.GetIndexByPosition(positions[1]);

            move = new Move(startIndex, endIndex);
        }

        return move;
    }
}