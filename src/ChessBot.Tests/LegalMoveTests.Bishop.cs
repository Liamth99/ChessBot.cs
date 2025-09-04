namespace ChessBot.Tests;

public partial class LegalMoveTests
{
    [Fact]
    public void GeneratedMoves_Bishop_AreValid()
    {
        var board = new Board();

        board[27] = Piece.White | Piece.Bishop;

        board.GenerateLegalMoves();
        
        board.LegalMoves.OrderBy(x => x.TargetSquare).ShouldBe(
        [
            new (27, 00), new (27, 06), new (27, 09), new (27, 13), new (27, 18), new (27, 20), new (27, 34), 
            new (27, 36), new (27, 41), new (27, 45), new (27, 48), new (27, 54), new (27, 63)
        ]);
    }

    [Fact]
    public void GeneratedMoves_Bishop_AreValidWithEnemyBlock()
    {
        var board = new Board();

        board[27] = Piece.White | Piece.Bishop;
        
        board[09] = Piece.Black | Piece.Rook;
        board[13] = Piece.Black | Piece.Rook;
        board[45] = Piece.Black | Piece.Rook;
        board[41] = Piece.Black | Piece.Rook;

        board.GenerateLegalMoves();
        
        board.LegalMoves.OrderBy(x => x.TargetSquare).ShouldBe(
        [
            new (27, 09), new (27, 13), new (27, 18), new (27, 20), new (27, 34), new (27, 36), new (27, 41), new (27, 45),
        ]);
    }
}