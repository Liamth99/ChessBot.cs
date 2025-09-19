namespace ChessBot.Tests;

public partial class LegalMoveTests
{
    [Fact]
    public void Pawn_White_AreValid()
    {
        var board = new Board(BoardUtils.GenerateFromFenString("5P2/8/8/8/8/3P4/1P6/8 w - - 0 1"));

        // Place white pawns: one on starting rank (can move 2 squares), one advanced (1 square only), one on 7th rank
        // ... existing code ...
        
        board.GenerateLegalMoves();
        
        board.LegalMoves.FriendlyMoves.OrderBy(x => x.StartSquare).ThenBy(x => x.TargetSquare)
            .ShouldBe([new(09, 17), new(09, 25), new(19, 27)]);
    }
    
    [Fact]
    public void Pawn_Black_AreValidPromotion()
    {
        var board = new Board(BoardUtils.GenerateFromFenString("8/8/8/8/8/8/3p4/8 b - - 0 1"));
        
        // Should generate all 4 promotion options
        board.LegalMoves.FriendlyMoves.OrderBy(x => x.StartSquare).ThenBy(x => x.TargetSquare)
            .ShouldBe(
                    [
                        new(11, 03, PromotionFlag.Queen), new(11, 03, PromotionFlag.Rook), 
                        new(11, 03, PromotionFlag.Bishop), new(11, 03, PromotionFlag.Knight)
                    ]
                );
    }
    
    [Fact]
    public void Pawn_White_AreValidPromotion()
    {
        var board = new Board(BoardUtils.GenerateFromFenString("8/3P4/8/8/8/8/8/8 w - - 0 1"));
        
        board.GenerateLegalMoves();
        
        board.LegalMoves.FriendlyMoves.OrderBy(x => x.StartSquare).ThenBy(x => x.TargetSquare)
            .ShouldBe(
                    [
                        new(51, 59, PromotionFlag.Queen), new(51, 59, PromotionFlag.Rook), 
                        new(51, 59, PromotionFlag.Bishop), new(51, 59, PromotionFlag.Knight)
                    ]
                );
    }

    [Fact]
    public void Pawn_White_DiagonalCapture()
    {
        var board = new Board(BoardUtils.GenerateFromFenString("8/8/8/8/1p1n4/2P5/8/8 w - - 0 1"));
        
        board.GenerateLegalMoves();
        
        // Should be able to move forward and capture diagonally
        board.LegalMoves.FriendlyMoves.Where(m => m.StartSquare == 18)
            .OrderBy(x => x.TargetSquare)
            .ShouldBe([new(18, 25), new(18, 26), new(18, 27)]);
    }

    [Fact]
    public void Pawn_Black_DiagonalCapture()
    {
        var board = new Board(BoardUtils.GenerateFromFenString("8/8/2p5/1B1R4/8/8/8/8 b - - 0 1"));
        
        board.LegalMoves.FriendlyMoves.Where(m => m.StartSquare == 42)
            .OrderBy(x => x.TargetSquare)
            .ShouldBe([new(42, 33), new(42, 34), new(42, 35)]);
    }

    [Fact]
    public void Pawn_White_SingleDiagonalCapture()
    {
        var board = new Board(BoardUtils.GenerateFromFenString("8/8/8/8/1B1n4/2P5/8/8 w - - 0 1"));
        
        board.GenerateLegalMoves();
        
        board.LegalMoves.FriendlyMoves.Where(m => m.StartSquare == 18)
            .OrderBy(x => x.TargetSquare)
            .ShouldBe([new(18, 26), new(18, 27)]);
    }

    [Fact]
    public void Pawn_White_EnPassantAttack_Left()
    {
        var board = new Board(BoardUtils.GenerateFromFenString("8/3p4/8/4P3/8/8/8/8 b - - 0 1"));
        
        // Black pawn moves d7 to d5 (two squares), creating en passant opportunity
        board.MakeMove(new Move(51, 35)); // d7 to d5
        
        board.LegalMoves.FriendlyMoves.Where(m => m.StartSquare == 36)
            .OrderBy(x => x.TargetSquare)
            .ShouldBe([new(36, 43), new(36, 44)]);
    }

    [Fact]
    public void Pawn_Black_EnPassantAttack_Right()
    {
        var board = new Board(BoardUtils.GenerateFromFenString("8/8/8/8/4p3/8/5P2/8 w - - 0 1"));
        
        // Set up en passant scenario: Black pawn on e4, White pawn moves from f2 to f4
        // ... existing code ...
        
        // White pawn moves f2 to f4 (two squares), creating en passant opportunity
        board.MakeMove(new Move(13, 29)); // f2 to f4
        
        board.LegalMoves.FriendlyMoves.Where(m => m.StartSquare == 28)
            .OrderBy(x => x.TargetSquare)
            .ShouldBe([new(28, 20), new(28, 21)]);
    }

    [Fact]
    public void Pawn_Black_EnPassantAttack_Left()
    {
        var board = new Board(BoardUtils.GenerateFromFenString("8/8/8/8/4p3/8/3P4/8 w - - 0 1"));
        
        // White pawn moves d2 to d4 (two squares), creating en passant opportunity
        board.MakeMove(new Move(11, 27)); // d2 to d4
        
        board.LegalMoves.FriendlyMoves.Where(m => m.StartSquare == 28)
            .OrderBy(x => x.TargetSquare)
            .ShouldBe([new(28, 19), new(28, 20)]);
    }

    [Fact]
    public void Pawn_EnPassantAttack_NotAvailableAfterOtherMove()
    {
        var board = new Board(BoardUtils.GenerateFromFenString("4k3/5p2/8/4P3/8/8/8/8 b - - 0 1"));
        
        // Black pawn moves f7 to f5, creating en passant opportunity
        board.MakeMove(new Move(53, 37)); // f7 to f5
        
        // White makes a different move
        board.MakeMove(new Move(36, 44)); // e5 to e6
        
        // Black makes any move
        board.MakeMove(new Move(60, 61)); // King move
        
        // En passant should no longer be available
        board.EnPassantBits.ShouldBe(0u);
    }

    [Fact]
    public void Pawn_EnPassantAttack_AFile()
    {
        var board = new Board(BoardUtils.GenerateFromFenString("8/1p6/8/P7/8/8/8/8 b - - 0 1"));
        
        // Black pawn moves b7 to b5
        board.MakeMove(new Move(49, 33)); // b7 to b5
        
        board.LegalMoves.FriendlyMoves.Where(m => m.StartSquare == 32)
            .OrderBy(x => x.TargetSquare)
            .ShouldBe([new(32, 40), new(32, 41)]);
    }

    [Fact]
    public void Pawn_EnPassantAttack_HFile()
    {
        var board = new Board(BoardUtils.GenerateFromFenString("8/6p1/8/7P/8/8/8/8 b - - 0 1"));
        
        // Black pawn moves g7 to g5
        board.MakeMove(new Move(54, 38)); // g7 to g5
        
        board.LegalMoves.FriendlyMoves.Where(m => m.StartSquare == 39)
            .OrderBy(x => x.TargetSquare)
            .ShouldBe([new(39, 46), new(39, 47)]);
    }

    [Fact]
    public void Pawn_EnPassantBits_SetCorrectly()
    {
        var board = new Board(BoardUtils.GenerateFromFenString("8/8/8/8/8/8/3P4/8 w - - 0 1"));
        
        // White pawn moves d2 to d4 (two squares)
        board.MakeMove(new Move(11, 27)); // d2 to d4
        
        // En passant bits should be set for d3 (square 19) - the square the pawn "passed over"
        (board.EnPassantBits & (0b1L << 19)).ShouldBe(0b1UL << 19);
    }

    [Fact]
    public void Pawn_EnPassantBits_ClearedAfterMove()
    {
        var board = new Board(BoardUtils.GenerateFromFenString("4k3/8/8/8/8/8/3P4/8 w - - 0 1"));
        
        // White pawn moves d2 to d4, setting en passant bits
        board.MakeMove(new Move(11, 27)); // d2 to d4
        
        // Verify en passant bits are set
        board.EnPassantBits.ShouldBe(0b1UL << 19);
        
        // Black makes any move
        board.MakeMove(new Move(60, 61)); // King move
        
        // En passant bits should be cleared after opponent's turn
        board.EnPassantBits.ShouldBe(0u);
    }

    [Fact]
    public void Pawn_EnPassantBug_ThisTookWayToLongToFind()
    {
        var board = new Board(BoardUtils.GenerateFromFenString("8/8/8/8/P7/7p/8/8 b - a3 0 1"));
        
        board.LegalMoves.FriendlyMoves.ShouldBe([new (23, 15)]);
    }
}