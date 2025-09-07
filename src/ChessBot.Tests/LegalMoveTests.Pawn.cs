namespace ChessBot.Tests;

public partial class LegalMoveTests
{
    [Fact]
    public void GeneratedMoves_Pawn_White_AreValid()
    {
        var board = new Board();

        // Place white pawns: one on starting rank (can move 2 squares), one advanced (1 square only), one on 7th rank
        board[09] = Piece.White | Piece.Pawn;
        board[19] = Piece.White | Piece.Pawn;
        board[61] = Piece.White | Piece.Pawn;
        
        board.GenerateLegalMoves();
        
        board.LegalMoves.FriendlyMoves.OrderBy(x => x.StartSquare).ThenBy(x => x.TargetSquare)
            .ShouldBe([new(09, 17), new(09, 25), new(19, 27)]);
    }
    
    [Fact]
    public void GeneratedMoves_Pawn_Black_AreValidPromotion()
    {
        var board = new Board();

        // Black pawn one square away from promotion (1st rank)
        board[11] = Piece.Black | Piece.Pawn;
        
        // Switch to black's turn
        board.MakeMove(new Move());
        
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
    public void GeneratedMoves_Pawn_White_AreValidPromotion()
    {
        var board = new Board();

        // White pawn one square away from promotion (8th rank)
        board[51] = Piece.White | Piece.Pawn;
        
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
    public void GeneratedMoves_Pawn_White_DiagonalCapture()
    {
        var board = new Board();

        // White pawn with enemy pieces on both diagonals
        board[18] = Piece.White | Piece.Pawn;
        board[25] = Piece.Black | Piece.Pawn;
        board[27] = Piece.Black | Piece.Knight;
        
        board.GenerateLegalMoves();
        
        // Should be able to move forward and capture diagonally
        board.LegalMoves.FriendlyMoves.Where(m => m.StartSquare == 18)
            .OrderBy(x => x.TargetSquare)
            .ShouldBe([new(18, 25), new(18, 26), new(18, 27)]);
    }

    [Fact]
    public void GeneratedMoves_Pawn_Black_DiagonalCapture()
    {
        var board = new Board();

        // Black pawn with white pieces on both diagonals
        board[42] = Piece.Black | Piece.Pawn;
        board[33] = Piece.White | Piece.Bishop;
        board[35] = Piece.White | Piece.Rook;
        
        board.MakeMove(new Move());
        
        board.LegalMoves.FriendlyMoves.Where(m => m.StartSquare == 42)
            .OrderBy(x => x.TargetSquare)
            .ShouldBe([new(42, 33), new(42, 34), new(42, 35)]);
    }

    [Fact]
    public void GeneratedMoves_Pawn_White_RightDiagonalCaptureWithPromotion()
    {
        var board = new Board();

        // White pawn on 7th rank with black piece on promotion diagonal
        board[50] = Piece.White | Piece.Pawn;
        board[59] = Piece.Black | Piece.Queen;
        
        board.GenerateLegalMoves();
        
        var promotionCaptures = board.LegalMoves.FriendlyMoves.Where(m => m.StartSquare == 50 && m.TargetSquare == 59)
            .OrderBy(x => x.Promotion);
        
        promotionCaptures.ShouldBe([
            new(50, 59, PromotionFlag.Queen), 
            new(50, 59, PromotionFlag.Rook),
            new(50, 59, PromotionFlag.Bishop), 
            new(50, 59, PromotionFlag.Knight)
        ]);
    }

    [Fact]
    public void GeneratedMoves_Pawn_Black_RightDiagonalCaptureWithPromotion()
    {
        var board = new Board();

        // Black pawn on 2nd rank with white piece on promotion diagonal
        board[10] = Piece.Black | Piece.Pawn;
        board[01] = Piece.White | Piece.Rook;
        
        board.MakeMove(new Move());
        
        var promotionCaptures = board.LegalMoves.FriendlyMoves.Where(m => m.StartSquare == 10 && m.TargetSquare == 01)
            .OrderBy(x => x.Promotion);
        
        promotionCaptures.ShouldBe([
            new(10, 01, PromotionFlag.Queen), 
            new(10, 01, PromotionFlag.Rook),
            new(10, 01, PromotionFlag.Bishop), 
            new(10, 01, PromotionFlag.Knight)
        ]);
    }
    [Fact]
    public void GeneratedMoves_Pawn_White_SingleDiagonalCapture()
    {
        var board = new Board();

        // White pawn with enemy on one diagonal, own piece blocking the other
        board[18] = Piece.White | Piece.Pawn;
        board[27] = Piece.Black | Piece.Knight;
        board[25] = Piece.White | Piece.Bishop; // Blocks capture
        
        board.GenerateLegalMoves();
        
        board.LegalMoves.FriendlyMoves.Where(m => m.StartSquare == 18)
            .OrderBy(x => x.TargetSquare)
            .ShouldBe([new(18, 26), new(18, 27)]);
    }

    [Fact]
    public void GeneratedMoves_Pawn_White_LeftDiagonalCaptureWithPromotion()
    {
        var board = new Board();

        board[49] = Piece.White | Piece.Pawn;
        board[56] = Piece.Black | Piece.Queen;
        
        board.GenerateLegalMoves();
        
        var promotionCaptures = board.LegalMoves.FriendlyMoves.Where(m => m.StartSquare == 49 && m.TargetSquare == 56)
            .OrderBy(x => x.Promotion);
        
        promotionCaptures.ShouldBe([
            new(49, 56, PromotionFlag.Queen), 
            new(49, 56, PromotionFlag.Rook),
            new(49, 56, PromotionFlag.Bishop), 
            new(49, 56, PromotionFlag.Knight)
        ]);
    }

    [Fact]
    public void GeneratedMoves_Pawn_Black_LeftDiagonalCaptureWithPromotion()
    {
        var board = new Board();

        board[09] = Piece.Black | Piece.Pawn;
        board[00] = Piece.White | Piece.Rook;
        
        board.MakeMove(new Move());
        
        var promotionCaptures = board.LegalMoves.FriendlyMoves.Where(m => m.StartSquare == 09 && m.TargetSquare == 00)
            .OrderBy(x => x.Promotion);
        
        promotionCaptures.ShouldBe([
            new(09, 00, PromotionFlag.Queen), 
            new(09, 00, PromotionFlag.Rook),
            new(09, 00, PromotionFlag.Bishop), 
            new(09, 00, PromotionFlag.Knight)
        ]);
    }
    
    [Fact]
    public void GeneratedMoves_Pawn_White_EnPassantAttack_Right()
    {
        var board = new Board();
        
        // Set up en passant scenario: White pawn on e5, Black pawn moves from f7 to f5
        board[36] = Piece.White | Piece.Pawn; // e5 (rank 4, file 4)
        board[53] = Piece.Black | Piece.Pawn; // f7
        
        // Make a null move to switch to black
        board.MakeMove(new Move());
        
        // Black pawn moves f7 to f5 (two squares), creating en passant opportunity
        board.MakeMove(new Move(53, 37)); // f7 to f5
        
        board.LegalMoves.FriendlyMoves.Where(m => m.StartSquare == 36)
            .OrderBy(x => x.TargetSquare)
            .ShouldBe([new(36, 44), new(36, 45)]);
    }

    [Fact]
    public void GeneratedMoves_Pawn_White_EnPassantAttack_Left()
    {
        var board = new Board();
        
        // Set up en passant scenario: White pawn on e5, Black pawn moves from d7 to d5
        board[36] = Piece.White | Piece.Pawn; // e5 (rank 4, file 4)
        board[51] = Piece.Black | Piece.Pawn; // d7
        
        // Make a null move to switch to black
        board.MakeMove(new Move());
        
        // Black pawn moves d7 to d5 (two squares), creating en passant opportunity
        board.MakeMove(new Move(51, 35)); // d7 to d5
        
        board.LegalMoves.FriendlyMoves.Where(m => m.StartSquare == 36)
            .OrderBy(x => x.TargetSquare)
            .ShouldBe([new(36, 43), new(36, 44)]);
    }

    [Fact]
    public void GeneratedMoves_Pawn_Black_EnPassantAttack_Right()
    {
        var board = new Board();
        
        // Set up en passant scenario: Black pawn on e4, White pawn moves from f2 to f4
        board[28] = Piece.Black | Piece.Pawn; // e4 (rank 3, file 4)
        board[13] = Piece.White | Piece.Pawn; // f2
        
        // White pawn moves f2 to f4 (two squares), creating en passant opportunity
        board.MakeMove(new Move(13, 29)); // f2 to f4
        
        board.LegalMoves.FriendlyMoves.Where(m => m.StartSquare == 28)
            .OrderBy(x => x.TargetSquare)
            .ShouldBe([new(28, 20), new(28, 21)]);
    }

    [Fact]
    public void GeneratedMoves_Pawn_Black_EnPassantAttack_Left()
    {
        var board = new Board();
        
        // Set up en passant scenario: Black pawn on e4, White pawn moves from d2 to d4
        board[28] = Piece.Black | Piece.Pawn; // e4 (rank 3, file 4)
        board[11] = Piece.White | Piece.Pawn; // d2
        
        // White pawn moves d2 to d4 (two squares), creating en passant opportunity
        board.MakeMove(new Move(11, 27)); // d2 to d4
        
        board.LegalMoves.FriendlyMoves.Where(m => m.StartSquare == 28)
            .OrderBy(x => x.TargetSquare)
            .ShouldBe([new(28, 19), new(28, 20)]);
    }

    [Fact]
    public void GeneratedMoves_Pawn_EnPassantAttack_NotAvailableAfterOtherMove()
    {
        var board = new Board();
        
        // Set up en passant scenario - verify it expires after one turn
        board[36] = Piece.White | Piece.Pawn; // e5
        board[53] = Piece.Black | Piece.Pawn; // f7
        board[60] = Piece.Black | Piece.King; // e8
        
        // Make a null move to switch to black
        board.MakeMove(new Move());
        
        // Black pawn moves f7 to f5, creating en passant opportunity
        board.MakeMove(new Move(53, 37)); // f7 to f5
        
        // White makes a different move
        board.MakeMove(new Move(36, 44)); // e5 to e6
        
        // Black makes any move
        board.MakeMove(new Move(60, 61)); // King move
        
        // En passant should no longer be available
        board.EnPassantBits.ShouldBe(0);
    }

    [Fact]
    public void GeneratedMoves_Pawn_EnPassantAttack_AFile()
    {
        var board = new Board();
        
        // Test en passant on a-file (leftmost) - edge case testing
        board[32] = Piece.White | Piece.Pawn; // a5
        board[49] = Piece.Black | Piece.Pawn; // b7
        
        // Make a null move to switch to black
        board.MakeMove(new Move());
        
        // Black pawn moves b7 to b5
        board.MakeMove(new Move(49, 33)); // b7 to b5
        
        board.LegalMoves.FriendlyMoves.Where(m => m.StartSquare == 32)
            .OrderBy(x => x.TargetSquare)
            .ShouldBe([new(32, 40), new(32, 41)]);
    }

    [Fact]
    public void GeneratedMoves_Pawn_EnPassantAttack_HFile()
    {
        var board = new Board();
        
        // Test en passant on h-file (rightmost) - edge case testing
        board[39] = Piece.White | Piece.Pawn; // h5
        board[54] = Piece.Black | Piece.Pawn; // g7
        
        // Make a null move to switch to black
        board.MakeMove(new Move());
        
        // Black pawn moves g7 to g5
        board.MakeMove(new Move(54, 38)); // g7 to g5
        
        board.LegalMoves.FriendlyMoves.Where(m => m.StartSquare == 39)
            .OrderBy(x => x.TargetSquare)
            .ShouldBe([new(39, 46), new(39, 47)]);
    }

    [Fact]
    public void GeneratedMoves_Pawn_EnPassantBits_SetCorrectly()
    {
        var board = new Board();
        
        board[11] = Piece.White | Piece.Pawn; // d2
        
        // White pawn moves d2 to d4 (two squares)
        board.MakeMove(new Move(11, 27)); // d2 to d4
        
        // En passant bits should be set for d3 (square 19) - the square the pawn "passed over"
        (board.EnPassantBits & (0b1L << 19)).ShouldBe(0b1L << 19);
    }

    [Fact]
    public void GeneratedMoves_Pawn_EnPassantBits_ClearedAfterMove()
    {
        var board = new Board();
        
        board[11] = Piece.White | Piece.Pawn; // d2
        board[60] = Piece.Black | Piece.King; // e8
        
        // White pawn moves d2 to d4, setting en passant bits
        board.MakeMove(new Move(11, 27)); // d2 to d4
        
        // Verify en passant bits are set
        board.EnPassantBits.ShouldBe(0b1L << 19);
        
        // Black makes any move
        board.MakeMove(new Move(60, 61)); // King move
        
        // En passant bits should be cleared after opponent's turn
        board.EnPassantBits.ShouldBe(0);
    }
}