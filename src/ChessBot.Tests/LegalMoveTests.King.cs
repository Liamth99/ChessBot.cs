namespace ChessBot.Tests;

public partial class LegalMoveTests
{
    [Fact]
    public void GeneratedMoves_King_AreValid()
    {
        var board = new Board();
        board[35] = Piece.White | Piece.King;

        board.GenerateLegalMoves();
        
        board.LegalMoves.FriendlyMoves.OrderBy(x => x.TargetSquare).ShouldBe(
        [
            new(35, 26), new(35, 27), new(35, 28),
            new(35, 34), new(35, 36),
            new(35, 42), new(35, 43), new(35, 44)
        ]);
    }

    [Fact]
    public void GeneratedMoves_King_BlockedByFriendly()
    {
        var board = new Board();
        board[35] = Piece.White | Piece.King;
        board[34] = Piece.White | Piece.Pawn;
        board[36] = Piece.White | Piece.Pawn;
        board[43] = Piece.White | Piece.Pawn;

        board.GenerateLegalMoves();
        
        board.LegalMoves.FriendlyMoves.Where(x => x.StartSquare is 35).OrderBy(x => x.TargetSquare).ShouldBe(
        [
            new(35, 26), new(35, 27), new(35, 28),
            new(35, 42), new(35, 44)
        ]);
    }

    [Fact]
    public void GeneratedMoves_King_AtTopEdge()
    {
        var board = new Board();
        board[60] = Piece.White | Piece.King;

        board.GenerateLegalMoves();
        
        board.LegalMoves.FriendlyMoves.OrderBy(x => x.TargetSquare).ShouldBe(
        [
            new(60, 51), new(60, 52), new(60, 53),
            new(60, 59), new(60, 61)
        ]);
    }

    [Fact]
    public void GeneratedMoves_King_AtBottomEdge()
    {
        var board = new Board();
        board[4] = Piece.White | Piece.King;

        board.GenerateLegalMoves();
        
        board.LegalMoves.FriendlyMoves.OrderBy(x => x.TargetSquare).ShouldBe(
        [
            new(4, 3), new(4, 5),
            new(4, 11), new(4, 12), new(4, 13)
        ]);
    }

    [Fact]
    public void GeneratedMoves_King_AtLeftEdge()
    {
        var board = new Board();
        board[32] = Piece.White | Piece.King;

        board.GenerateLegalMoves();
        
        board.LegalMoves.FriendlyMoves.OrderBy(x => x.TargetSquare).ShouldBe(
        [
            new(32, 24), new(32, 25),
            new(32, 33),
            new(32, 40), new(32, 41)
        ]);
    }

    [Fact]
    public void GeneratedMoves_King_AtRightEdge()
    {
        var board = new Board();
        board[39] = Piece.White | Piece.King;

        board.GenerateLegalMoves();
        
        board.LegalMoves.FriendlyMoves.OrderBy(x => x.TargetSquare).ShouldBe(
        [
            new(39, 30), new(39, 31),
            new(39, 38),
            new(39, 46), new(39, 47)
        ]);
    }
}