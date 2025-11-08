namespace ChessBot.Tests;

public partial class BoardTests
{
    [Fact]
    public void Stalemate_Classic_KingCorner_BoxedByKingAndQueen()
    {
        // Black to move, not in check, but has no legal moves.
        var fen = "7k/8/5KQ1/8/8/8/8/8 b - - 0 1";
        var settings = BoardUtils.GenerateFromFenString(fen);
        var board = new Board(settings);

        board.IsCheck.ShouldBeFalse();
        board.IsMate.ShouldBeFalse();
        board.IsDraw.ShouldBeTrue();
    }

    [Fact]
    public void Stalemate_KingBlocked_ByKingAndPawn()
    {
        // Another stalemate motif:
        var fen = "1k6/1P6/1K6/8/8/8/8/8 b - - 0 1";
        var settings = BoardUtils.GenerateFromFenString(fen);
        var board = new Board(settings);

        board.IsCheck.ShouldBeFalse();
        board.IsMate.ShouldBeFalse();
        board.IsDraw.ShouldBeTrue();
    }

    [Fact]
    public void Pinned_Rook_OnFile_CannotMoveSideways()
    {
        // Setup:
        // White king e1 (4), white rook e2 (12), black rook e8 (60).
        // Rook on e2 is pinned along the e-file; sideways moves must be illegal.
        var board = new Board();
        board[4] = Piece.White | Piece.King; // e1
        board[12] = Piece.White | Piece.Rook; // e2
        board[60] = Piece.Black | Piece.Rook; // e8

        board.GenerateLegalMoves();

        // Illegal: e2 -> d2 (sideways while pinned)
        var illegalSideways = new Move(12, 11);
        board.LegalMoves.FriendlyMoves.ShouldNotContain(illegalSideways);

        // Legal: e2 -> e3 (still blocking the e-file)
        var legalAlongFile = new Move(12, 20);
        board.LegalMoves.FriendlyMoves.ShouldContain(legalAlongFile);

        // Legal: e2 -> e8 (capturing the attacker)
        var legalCaptureAttacker = new Move(12, 60);
        board.LegalMoves.FriendlyMoves.ShouldContain(legalCaptureAttacker);
    }

    [Fact]
    public void Pinned_Knight_OnFile_HasNoLegalMoves()
    {
        // Setup:
        // White king e1 (4), white knight e2 (12), black rook e8 (60).
        // Knight is pinned; any knight move would expose the king.
        var board = new Board();
        board[4] = Piece.White | Piece.King; // e1
        board[12] = Piece.White | Piece.Knight; // e2
        board[60] = Piece.Black | Piece.Rook; // e8

        board.GenerateLegalMoves();

        var knightMoves = board.LegalMoves.FriendlyMoves.Where(m => m.StartSquare == 12).ToList();
        knightMoves.Count.ShouldBe(0);
    }

    [Fact]
    public void Pinned_Pawn_EnPassant_Capture_IsIllegal_IfItExposesKing()
    {
        // Classic EP pin motif:
        // White king e1 (4), black rook e8 (60) along the open e-file.
        // White pawn e5 (36), black pawn d7 (51) will double-push to d5 (35) enabling EP (d6 = 43).
        // EP move e5xd6 EP would remove the e5 pawn from the e-file and expose the white king to the rook on e8.
        // Therefore, the EP capture must be excluded from legal moves.

        var board = new Board(BoardUtils.GenerateFromFenString("4r3/3p4/8/4P3/8/8/8/4K3 b - - 0 1"));

        // make the double push d7 -> d5
        board.MakeMove(new Move(51, 35));

        // EP capture from e5 to d6 is (36 -> 43). This should be disallowed due to pin.
        var illegalEpCapture = new Move(36, 43);

        board.LegalMoves.FriendlyMoves.ShouldNotContain(illegalEpCapture);

        // Normal forward move e5 -> e6 (36 -> 44) should still be legal.
        var legalForward = new Move(36, 44);
        board.LegalMoves.FriendlyMoves.ShouldContain(legalForward);
    }
}