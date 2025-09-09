namespace ChessBot.Core.Player;

public class HumanPlayer : IPlayer
{
    public Move GetNextMove(Board board)
    {
        Move? move = null;

        while (true)
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
            PromotionFlag promFlag = PromotionFlag.None;

            if (positions.Length is 3)
                promFlag = Enum.Parse<PromotionFlag>(positions[2]);


            move = board.LegalMoves.FriendlyMoves.FirstOrDefault(x =>
                x.StartSquare == startIndex && x.TargetSquare == endIndex && x.Promotion == promFlag);

            if (move != new Move())
                return move.Value;
        }
    }
}