namespace ChessBot.Core.Player;

public interface IPlayer
{
    Move GetNextMove(Board board);
}