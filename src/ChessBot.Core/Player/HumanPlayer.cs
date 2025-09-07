namespace ChessBot.Core.Player;

public class HumanPlayer : IPlayer
{
    public Move GetNextMove(Board board)
    {
        var move = new Move();

        while (!board.LegalMoves.FriendlyMoves.Contains(move))
        {
            var input = Console.ReadLine();

            ArgumentException.ThrowIfNullOrWhiteSpace(input);

            if (input.Equals("fen", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine(BoardUtils.GenerateFenString(board));
                continue;
            }

            var positions = input.Split('\u0020');

            var startIndex = BoardUtils.GetIndexByPosition(positions[0]);
            var endIndex   = BoardUtils.GetIndexByPosition(positions[1]);

            move = new Move(startIndex, endIndex);
        }

        return move;
    }
}