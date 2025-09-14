namespace ChessBot.Tests;

public partial class LegalMoveTests
{

    [Fact]
    public void MakeMove_Castling_WhiteKingSide_RookAndKingPositions()
    {
        // Position: White king on e1, rook on h1, white to move, K castling right
        var board = new Board(BoardUtils.GenerateFromFenString("4k3/8/8/8/8/8/8/4K2R w K - 0 1"));

        // Ensure the castle move exists
        board.LegalMoves.FriendlyMoves.ShouldContain(new Move(4, 6, PromotionFlag.None, 07));

        // Execute e1 -> g1 with castlingSquare set to h1 (7)
        var castle = new Move(4, 6, PromotionFlag.None, 7);
        board.MakeMove(castle);

        board[4].ShouldBe(Piece.None); // e1
        board[7].ShouldBe(Piece.None); // h1
        board[6].ShouldBe(Piece.White | Piece.King); // g1
        board[5].ShouldBe(Piece.White | Piece.Rook); // f1
    }
    
    [Fact]
    public void Castling_WhiteKingSide_BlockedByFriendlyPiece_OnF1()
    {
        // White king e1, rook h1, white bishop on f1 blocks path; white to move with K right
        var board = new Board(BoardUtils.GenerateFromFenString("4k3/8/8/8/8/8/8/4KB1R w K - 0 1"));

        board.LegalMoves.FriendlyMoves.ShouldNotContain(new Move(4, 6, PromotionFlag.None, 07));
    }

    [Fact]
    public void Castling_WhiteQueenSide_BlockedByFriendlyPiece_OnD1()
    {
        // White king e1, rook a1, white bishop on d1 blocks path; white to move with Q right
        var board = new Board(BoardUtils.GenerateFromFenString("4k3/8/8/8/8/8/8/R2BK3 w Q - 0 1"));

        board.LegalMoves.FriendlyMoves.ShouldNotContain(new Move(4, 2, PromotionFlag.None, 00));
    }

    [Fact]
    public void Castling_WhiteKingSide_BlockedByAttack_OnF1()
    {
        // Black bishop on a6 attacks f1; path square f1 attacked blocks castling
        var board = new Board(BoardUtils.GenerateFromFenString("4k3/8/b7/8/8/8/8/4K2R w K - 0 1"));

        board.LegalMoves.FriendlyMoves.ShouldNotContain(new Move(4, 6, PromotionFlag.None, 07));
    }

    [Fact]
    public void Castling_WhiteQueenSide_BlockedByAttack_OnD1()
    {
        // Black rook on d8 attacks d1; path square d1 attacked blocks castling
        var board = new Board(BoardUtils.GenerateFromFenString("3rk3/8/8/8/8/8/8/R3K3 w Q - 0 1"));

        board.LegalMoves.FriendlyMoves.ShouldNotContain(new Move(4, 2, PromotionFlag.None, 00));
    }

    [Fact]
    public void Castling_BlackKingSide_BlockedByFriendlyPiece_OnF8()
    {
        // Black king e8, rook h8, black knight on f8 blocks path; black to move with k right
        var board = new Board(BoardUtils.GenerateFromFenString("4kn1r/8/8/8/8/8/8/4K3 b k - 0 1"));

        board.LegalMoves.FriendlyMoves.ShouldNotContain(new Move(60, 62, PromotionFlag.None, 63));
    }

    [Fact]
    public void Castling_BlackQueenSide_BlockedByFriendlyPiece_OnD8()
    {
        // Black king e8, rook a8, black bishop on d8 blocks path; black to move with q right
        var board = new Board(BoardUtils.GenerateFromFenString("r2bk3/8/8/8/8/8/8/4K3 b q - 0 1"));

        board.LegalMoves.FriendlyMoves.ShouldNotContain(new Move(60, 58, PromotionFlag.None, 56));
    }

    [Fact]
    public void Castling_BlackKingSide_BlockedByAttack_OnF8()
    {
        // White bishop on a3 attacks f8; path square f8 attacked blocks castling
        var board = new Board(BoardUtils.GenerateFromFenString("4k2r/8/8/8/8/B7/8/4K3 b k - 0 1"));

        board.LegalMoves.FriendlyMoves.ShouldNotContain(new Move(60, 62, PromotionFlag.None, 63));
    }

    [Fact]
    public void Castling_BlackQueenSide_BlockedByAttack_OnD8()
    {
        // White rook on d1 attacks d8; path square d8 attacked blocks castling
        var board = new Board(BoardUtils.GenerateFromFenString("r3k3/8/8/8/8/8/8/3RK3 b q - 0 1"));

        board.LegalMoves.FriendlyMoves.ShouldNotContain(new Move(60, 58, PromotionFlag.None, 56));
    }

    // ... existing code ...
    [Fact]
    public void Castling_WhiteQueenSide_MoveExists()
    {
        // Position: White king on e1, rook on a1, path clear; white to move with Q right
        var board = new Board(BoardUtils.GenerateFromFenString("4k3/8/8/8/8/8/8/R3K3 w Q - 0 1"));

        // Ensure the castle move exists: e1 -> c1 with rook from a1
        board.LegalMoves.FriendlyMoves.ShouldContain(new Move(4, 2, PromotionFlag.None, 0));
    }

    [Fact]
    public void Castling_BlackKingSide_MoveExists()
    {
        // Position: Black king on e8, rook on h8, path clear; black to move with k right
        var board = new Board(BoardUtils.GenerateFromFenString("4k2r/8/8/8/8/8/8/4K3 b k - 0 1"));

        // Ensure the castle move exists: e8 -> g8 with rook from h8
        board.LegalMoves.FriendlyMoves.ShouldContain(new Move(60, 62, PromotionFlag.None, 63));
    }

    [Fact]
    public void Castling_BlackQueenSide_MoveExists()
    {
        // Position: Black king on e8, rook on a8, path clear; black to move with q right
        var board = new Board(BoardUtils.GenerateFromFenString("r3k3/8/8/8/8/8/8/4K3 b q - 0 1"));

        // Ensure the castle move exists: e8 -> c8 with rook from a8
        board.LegalMoves.FriendlyMoves.ShouldContain(new Move(60, 58, PromotionFlag.None, 56));
    }

    [Fact]
    public void Castling_White_BothSides_MovesExist()
    {
        // Position: White king on e1, rooks on a1/h1, paths clear; white to move with KQ rights
        var board = new Board(BoardUtils.GenerateFromFenString("4k3/8/8/8/8/8/8/R3K2R w KQ - 0 1"));

        // Ensure both castle moves exist: e1 -> g1 and e1 -> c1
        board.LegalMoves.FriendlyMoves.ShouldContain(new Move(4, 6, PromotionFlag.None, 7));
        board.LegalMoves.FriendlyMoves.ShouldContain(new Move(4, 2, PromotionFlag.None, 0));
    }

    [Fact]
    public void Castling_Black_BothSides_MovesExist()
    {
        // Position: Black king on e8, rooks on a8/h8, paths clear; black to move with kq rights
        var board = new Board(BoardUtils.GenerateFromFenString("r3k2r/8/8/8/8/8/8/4K3 b kq - 0 1"));

        // Ensure both castle moves exist: e8 -> g8 and e8 -> c8
        board.LegalMoves.FriendlyMoves.ShouldContain(new Move(60, 62, PromotionFlag.None, 63));
        board.LegalMoves.FriendlyMoves.ShouldContain(new Move(60, 58, PromotionFlag.None, 56));
    }
}