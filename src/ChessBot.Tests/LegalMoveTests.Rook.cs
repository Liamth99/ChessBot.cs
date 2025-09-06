namespace ChessBot.Tests;

public partial class LegalMoveTests
{
    [Fact]
    public void GeneratedMoves_Rook_AreValid()
    {
        var board = new Board();

        board[18] = Piece.White | Piece.Rook;

        board.GenerateLegalMoves();
        
        board.LegalMoves.FriendlyMoves.OrderBy(x => x.TargetSquare).ShouldBe(
        [
            new (18, 02), new (18, 10), new (18, 16), new (18, 17), new (18, 19), new (18, 20), new (18, 21), 
            new (18, 22), new (18, 23), new (18, 26), new (18, 34), new (18, 42), new (18, 50), new (18, 58),
        ]);
    }

    [Fact]
    public void GeneratedMoves_Rook_AreValidWithEnemyBlock()
    {
        var board = new Board();

        board[18] = Piece.White | Piece.Rook;
        
        board[10] = Piece.Black | Piece.Rook;
        board[16] = Piece.Black | Piece.Rook;
        board[20] = Piece.Black | Piece.Rook;
        board[34] = Piece.Black | Piece.Rook;

        board.GenerateLegalMoves();
        
        board.LegalMoves.FriendlyMoves.OrderBy(x => x.TargetSquare).ShouldBe(
        [
            new (18, 10), new (18, 16), new (18, 17), new (18, 19),
            new (18, 20), new (18, 26), new (18, 34),
        ]);
    }
}