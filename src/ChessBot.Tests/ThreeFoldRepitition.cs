namespace ChessBot.Tests;

public class ThreeFoldRepitition
{
    [Fact]
    public void ThreeFoldRepetition_SamePositionThreeTimes_DrawDetected()
    {
        var board = new Board(BoardUtils.GenerateFromFenString());

        board.MakeMove(new Move(6, 21));
        board.MakeMove(new Move(62, 45));

        board.MakeMove(new Move(21, 6));
        board.MakeMove(new Move(45, 62));

        board.MakeMove(new Move(6, 21));
        board.MakeMove(new Move(62, 45));

        board.MakeMove(new Move(21, 6));
        board.MakeMove(new Move(45, 62));

        board.MakeMove(new Move(6, 21));
        board.MakeMove(new Move(62, 45));


        Assert.True(board.IsDraw);
    }

    [Fact]
    public void ThreeFoldRepetition_TwiceButNotThree_NotDraw()
    {
        var board = new Board(BoardUtils.GenerateFromFenString());

        board.MakeMove(new Move(6, 21));
        board.MakeMove(new Move(62, 45));

        board.MakeMove(new Move(21, 6));
        board.MakeMove(new Move(45, 62));

        board.MakeMove(new Move(6, 21));
        board.MakeMove(new Move(62, 45));

        Assert.False(board.IsDraw);
    }

    [Fact]
    public void ThreeFoldRepetition_ComplexPositionWithPawnMove_BreaksRepetition()
    {
        // Threefold repetition broken by a different move (pawn push)
        var board = new Board(BoardUtils.GenerateFromFenString());

        // First cycle
        board.MakeMove(new Move(6, 21));
        board.MakeMove(new Move(62, 45));
        board.MakeMove(new Move(21, 6));
        board.MakeMove(new Move(45, 62));

        // Second cycle
        board.MakeMove(new Move(6, 21));
        board.MakeMove(new Move(62, 45));
        board.MakeMove(new Move(21, 6));
        board.MakeMove(new Move(45, 62));

        // Different move - push pawn instead of repeating
        board.MakeMove(new Move(12, 28));

        Assert.False(board.IsDraw);
    }
}