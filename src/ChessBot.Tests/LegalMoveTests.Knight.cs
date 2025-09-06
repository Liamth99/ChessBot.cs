namespace ChessBot.Tests;

public partial class LegalMoveTests
{
    [Fact]
    public void GeneratedMoves_Knight_AreValid()
    {
        var board = new Board();

        board[35] = Piece.White | Piece.Knight;

        board.GenerateLegalMoves();
        
        board.LegalMoves.FriendlyMoves.OrderBy(x => x.TargetSquare).ShouldBe(
            [
                new(35, 18), new(35, 20), new(35, 25), new(35, 29), 
                new(35, 41), new(35, 45), new(35, 50), new(35, 52)
            ]);
    }
    
    [Fact]
    public void GeneratedMoves_Knight_AreValidCutoffOuterRight()
    {
        var board = new Board();
        
        board[46] = Piece.White | Piece.Knight;
        board[22] = Piece.White | Piece.Knight;

        board.GenerateLegalMoves();
        
        board.LegalMoves.FriendlyMoves.OrderBy(x => x.TargetSquare).ShouldBe(
            [
                new(22, 05), new(22, 07), new(22, 12), new(22, 28), new(46, 29), new(46, 31),
                new(46, 36), new(22, 37), new(22, 39), new(46, 52), new(46, 61), new(46, 63)
            ]);
    }
    
    [Fact]
    public void GeneratedMoves_Knight_AreValidCutoffOuterAndInnerRight()
    {
        var board = new Board();
        
        board[47] = Piece.White | Piece.Knight;
        board[23] = Piece.White | Piece.Knight;

        board.GenerateLegalMoves();
        
        board.LegalMoves.FriendlyMoves.OrderBy(x => x.TargetSquare).ShouldBe(
            [
                new(23, 06), new(23, 13), new(23, 29), new(47, 30),
                new(47, 37), new(23, 38), new(47, 53), new(47, 62)
            ]);
    }
    
    [Fact]
    public void GeneratedMoves_Knight_AreValidCutoffOuterLeft()
    {
        var board = new Board();
        
        board[41] = Piece.White | Piece.Knight;
        board[17] = Piece.White | Piece.Knight;

        board.GenerateLegalMoves();
        
        board.LegalMoves.FriendlyMoves.OrderBy(x => x.TargetSquare).ShouldBe(
            [
                new(17, 00), new(17, 02), new(17, 11), new(41, 24), new(41, 26), new(17, 27),
                new(17, 32), new(17, 34), new(41, 35), new(41, 51), new(41, 56), new(41, 58)

            ]);
    }
    
    [Fact]
    public void GeneratedMoves_Knight_AreValidCutoffOuterAndInnerLeft()
    {
        var board = new Board();
        
        board[40] = Piece.White | Piece.Knight;
        board[16] = Piece.White | Piece.Knight;

        board.GenerateLegalMoves();
        
        board.LegalMoves.FriendlyMoves.OrderBy(x => x.TargetSquare).ShouldBe(
            [
                new(16, 01), new(16, 10), new(40, 25), new(16, 26),
                new(16, 33), new(40, 34), new(40, 50), new(40, 57)
            ]);
    }
    
    [Fact]
    public void GeneratedMoves_Knight_AreValidCutoffOuterTop()
    {
        var board = new Board();
        
        board[50] = Piece.White | Piece.Knight;
        board[53] = Piece.White | Piece.Knight;

        board.GenerateLegalMoves();
        
        board.LegalMoves.FriendlyMoves.OrderBy(x => x.TargetSquare).ShouldBe(
            [
                new(50, 33), new(50, 35), new(53, 36), new(53, 38), new(50, 40), new(53, 43), 
                new(50, 44), new(53, 47), new(50, 56), new(53, 59), new(50, 60), new(53, 63)
            ]);
    }
    
    [Fact]
    public void GeneratedMoves_Knight_AreValidCutoffOuterAndInnerTop()
    {
        var board = new Board();
        
        board[58] = Piece.White | Piece.Knight;
        board[61] = Piece.White | Piece.Knight;

        board.GenerateLegalMoves();
        
        board.LegalMoves.FriendlyMoves.OrderBy(x => x.TargetSquare).ShouldBe(
            [
                new(58, 41), new(58, 43), new(61, 44), new(61, 46), 
                new(58, 48), new(61, 51), new(58, 52), new(61, 55)
            ]);
    }
    
    
}