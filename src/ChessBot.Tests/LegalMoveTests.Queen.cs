namespace ChessBot.Tests;

public partial class LegalMoveTests
{
    [Fact]
    public void GeneratedMoves_Queen_AreValid()
    {
        var board = new Board();

        board[27] = Piece.White | Piece.Queen;

        board.GenerateLegalMoves();
        
        board.LegalMoves.FriendlyMoves.OrderBy(x => x.TargetSquare).ShouldBe(
        [
            new(27, 00), new(27, 03), new(27, 06), new(27, 09), new(27, 11), new(27, 13), new(27, 18), new(27, 19), new(27, 20), 
            new(27, 24), new(27, 25), new(27, 26), new(27, 28), new(27, 29), new(27, 30), new(27, 31), new(27, 34), new(27, 35), 
            new(27, 36), new(27, 41), new(27, 43), new(27, 45), new(27, 48), new(27, 51), new(27, 54), new(27, 59), new(27, 63)
        ]);
    }

    [Fact]
    public void GeneratedMoves_Queen_AreValidWithEnemyBlock()
    {
        var board = new Board();

        board[27] = Piece.White | Piece.Queen;
        
        board[09] = Piece.Black | Piece.Rook;
        board[13] = Piece.Black | Piece.Rook;
        board[45] = Piece.Black | Piece.Rook;
        board[41] = Piece.Black | Piece.Rook;
        
        board[25] = Piece.Black | Piece.Rook;
        board[43] = Piece.Black | Piece.Rook;
        board[29] = Piece.Black | Piece.Rook;
        board[11] = Piece.Black | Piece.Rook;

        board.GenerateLegalMoves();
        
        board.LegalMoves.FriendlyMoves.OrderBy(x => x.TargetSquare).ShouldBe(
        [
            new(27, 09), new(27, 11), new(27, 13), new(27, 18), new(27, 19), new(27, 20), new(27, 25), new(27, 26), 
            new(27, 28), new(27, 29), new(27, 34), new(27, 35), new(27, 36), new(27, 41), new(27, 43), new(27, 45)
        ]);
    }
}