using System.Numerics;

namespace ChessBot.Tests;

public partial class PerftTests
{
    [Theory]
    [MemberData(nameof(DefaultDivideData))]
    // [MemberData(nameof(BugIn_DefaultDivideData))]
    [MemberData(nameof(Position2))]
    // [MemberData(nameof(BugIn_Position2))]
    [MemberData(nameof(Position3))]
    // [MemberData(nameof(BugsIn_Position3))]
    public void PerftDivideTest(string fen, int depth, Dictionary<Move, BigInteger> expectedMoves)
    {
        var board = new Board(BoardUtils.GenerateFromFenString(fen));

        var validMoves = board.LegalMoves.FriendlyMoves.ToArray();

        validMoves.Length.ShouldBe(expectedMoves.Count);

        foreach (var pair in expectedMoves)
        {
            board.LegalMoves.FriendlyMoves.ShouldContain(pair.Key, $"Move {pair.Key.DisplayString} failed");

            var newBoard = board.Clone();
            newBoard.MakeMove(pair.Key);

            Perft(newBoard, depth - 1).ShouldBe(pair.Value, $"Position: {newBoard.DebugString}\n\tMove: {pair.Key.DisplayString} failed");
        }
    }

    private static BigInteger Perft(Board board, int depth)
    {
        var result = new BigInteger();

        if (depth == 0)
            return 1;

        foreach (var move in board.LegalMoves.FriendlyMoves)
        {
            var newBoard = board.Clone();
            newBoard.MakeMove(move);

            var childResult = Perft(newBoard, depth - 1);

            result += childResult;
        }

        return result;
    }
}